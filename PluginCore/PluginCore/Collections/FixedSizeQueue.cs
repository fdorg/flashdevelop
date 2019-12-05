using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PluginCore.Collections
{
    /// <summary>
    /// Represents a first-in, first-out collection of objects with a fixed capacity.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerNonUserCode]
    [Serializable]
    public class FixedSizeQueue<T> : ICollection<T>
    {
        T[] _array;
        int _capacity;
        int _head;
        int _tail;
        int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeQueue{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="FixedSizeQueue{T}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public FixedSizeQueue(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _array = new T[capacity];
            _capacity = capacity;
            _head = 0;
            _tail = 0;
            Count = 0;
            _version = 0;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _array[(_head + index) % _capacity];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _array[(_head + index) % _capacity] = value;
            }
        }

        /// <summary>
        /// Gets or sets the total number of elements the queue can hold without discarding any elements.
        /// Setting the capacity to less than <see cref="Count"/> will discard elements in the order they were added.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (_capacity == value)
                {
                    return;
                }

                var newArray = new T[value];
                int head;
                int size;

                if (Count <= value)
                {
                    head = _head;
                    size = Count;
                }
                else
                {
                    head = (_head + Count - value) % _capacity;
                    size = value;
                }

                if (size > 0)
                {
                    if (head < _tail)
                    {
                        Array.Copy(_array, head, newArray, 0, size);
                    }
                    else
                    {
                        int length = _capacity - head;
                        Array.Copy(_array, head, newArray, 0, length);
                        Array.Copy(_array, 0, newArray, length, _tail);
                    }
                }

                _array = newArray;
                _capacity = value;
                _head = 0;
                _tail = size == value ? 0 : size;
                Count = size;

                _version++;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        void ICollection<T>.Add(T item)
        {
            if (Count == _capacity)
            {
                Capacity = Math.Max(_capacity * 2, _capacity + 4);
            }

            Enqueue(item);
        }

        /// <summary>
        /// Removes all elements from the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                if (_head < _tail)
                {
                    Array.Clear(_array, _head, Count);
                }
                else
                {
                    Array.Clear(_array, _head, _capacity - _head);
                    Array.Clear(_array, 0, _tail);
                }
            }

            _head = 0;
            _tail = 0;
            Count = 0;

            _version++;
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="FixedSizeQueue{T}"/>. The value can be <see langword="null"/> for reference types.</param>
        public bool Contains(T item)
        {
            if (Count > 0)
            {
                int index = _head;
                var equalityComparer = EqualityComparer<T>.Default;

                for (int i = Count; i > 0; i--)
                {
                    if (item is null)
                    {
                        if (_array[index] is null)
                        {
                            return true;
                        }
                    }
                    else if (_array[index] != null)
                    {
                        if (equalityComparer.Equals(_array[index], item))
                        {
                            return true;
                        }
                    }

                    index++;
                    if (index == _capacity)
                    {
                        index = 0;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the <see cref="FixedSizeQueue{T}"/> elements to an existing one-dimensional <see cref="Array"/>, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="FixedSizeQueue{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("The number of elements in the source " + nameof(FixedSizeQueue<T>) + " is greater than the available space from " + nameof(arrayIndex) + " to the end of the destination " + nameof(array));
            }

            if (Count > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, array, arrayIndex, Count);
                }
                else
                {
                    int length = _capacity - _head;
                    Array.Copy(_array, _head, array, 0, length);
                    Array.Copy(_array, 0, array, arrayIndex + length, _tail);
                }
            }
        }

        /// <summary>
        /// Removes and returns the element at the beginning of the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The " + nameof(FixedSizeQueue<T>) + " is empty.");
            }

            var obj = _array[_head];
            _array[_head] = default;
            Count--;

            _head++;
            if (_head == _capacity)
            {
                _head = 0;
            }

            _version++;
            return obj;
        }

        /// <summary>
        /// Adds an element to the end of the <see cref="FixedSizeQueue{T}"/>.
        /// When required, elements are removed in the order they were added, to ensure the size does not exceed the capacity.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="FixedSizeQueue{T}"/>. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="InvalidOperationException"/>
        public void Enqueue(T item)
        {
            if (_capacity == 0)
            {
                throw new InvalidOperationException("The " + nameof(FixedSizeQueue<T>) + " has zero capacity.");
            }

            _array[_tail] = item;

            _tail++;
            if (_tail == _capacity)
            {
                _tail = 0;
            }

            if (Count == _capacity)
            {
                _head++;
                if (_head == _capacity)
                {
                    _head = 0;
                }
            }
            else
            {
                Count++;
            }

            _version++;
        }

        /// <summary>
        /// Returns an <see cref="Enumerator"/> object that iterates through the <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns the element at the beginning of the <see cref="FixedSizeQueue{T}"/> without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The " + nameof(FixedSizeQueue<T>) + " is empty.");
            }

            return _array[_head];
        }

        /// <summary>
        /// Returns the element at the end of the <see cref="FixedSizeQueue{T}"/> without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T PeekEnd()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The " + nameof(FixedSizeQueue<T>) + " is empty.");
            }

            return _array[_tail == 0 ? _capacity - 1 : _tail - 1];
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        bool ICollection<T>.Remove(T item)
        {
            if (Count > 0)
            {
                var equalityComparer = EqualityComparer<T>.Default;

                for (int i = _tail - 1; i >= 0; i--)
                {
                    if (item is null ? _array[i] is null : _array[i] != null && equalityComparer.Equals(_array[i], item))
                    {
                        Array.Copy(_array, i + 1, _array, i, (_tail - 1) - i);
                        _array[_tail - 1] = default;
                        Count--;
                        _tail--;

                        _version++;
                        return true;
                    }

                    if (i == _head)
                    {
                        return false;
                    }
                }

                for (int i = _capacity - 1; i >= _head; i--)
                {
                    if (item is null ? _array[i] is null : _array[i] != null && equalityComparer.Equals(_array[i], item))
                    {
                        Array.Copy(_array, _head, _array, _head + 1, (i - 1) - _head);
                        _array[_head] = default;
                        Count--;
                        _head++;
                        if (_head == _capacity)
                        {
                            _head = 0;
                        }

                        _version++;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the <see cref="FixedSizeQueue{T}"/> elements to a new array.
        /// </summary>
        public T[] ToArray()
        {
            var array = new T[Count];

            if (Count > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, array, 0, Count);
                }
                else
                {
                    int length = _capacity - _head;
                    Array.Copy(_array, _head, array, 0, length);
                    Array.Copy(_array, 0, array, 0 + length, _tail);
                }
            }

            return array;
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="FixedSizeQueue{T}"/>.
        /// </summary>
        [Serializable]
        public sealed class Enumerator : IEnumerator<T>
        {
            T m_current;
            int m_index;
            readonly FixedSizeQueue<T> m_queue;
            int m_version;

            [DebuggerHidden]
            internal Enumerator(FixedSizeQueue<T> queue)
            {
                m_current = default;
                m_index = -1;
                m_queue = queue;
                m_version = m_queue._version;
            }

            /// <summary>
            /// Gets the element at the current position of the <see cref="Enumerator"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException"/>
            public T Current
            {
                [DebuggerHidden]
                get
                {
                    if (m_index < 0)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }
                    return m_current;
                }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException"/>
            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    if (m_index < 0)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }
                    return m_current;
                }
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            [DebuggerHidden]
            public void Dispose()
            {
                m_current = default;
                m_index = -2;
            }

            /// <summary>
            /// Advances the <see cref="Enumerator"/> to the next element of the <see cref="FixedSizeQueue{T}"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException"/>
            [DebuggerHidden]
            public bool MoveNext()
            {
                if (m_version != m_queue._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                switch (m_index)
                {
                    case -1:
                        if (m_queue.Count == 0)
                        {
                            m_index = -2;
                            goto case -2;
                        }
                        m_index = m_queue._head;

                        do_while:
                        m_current = m_queue._array[m_index];
                        return true;

                        yield_return:
                        m_index++;
                        if (m_index == m_queue._capacity)
                        {
                            m_index = 0;
                        }
                        if (m_index == m_queue._tail)
                        {
                            m_index = -2;
                            goto case -2;
                        }
                        goto do_while;

                    case -2:
                        return false;

                    default:
                        goto yield_return;
                }
            }

            /// <summary>
            /// Sets the <see cref="Enumerator"/> to its initial position, which is before the first element in the collection.
            /// </summary>
            [DebuggerHidden]
            public void Reset()
            {
                m_current = default;
                m_index = -1;
                m_version = m_queue._version;
            }
        }
    }
}
