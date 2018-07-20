// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections;
using System.Text;

namespace JavaCompatibleClasses
{
    public class LinkedHashMap
        : IDictionary
    {
        #region LinkNode

        public class LinkNode
        {
            public LinkNode m_Previous;
            public LinkNode m_Next;

            public Object m_Key;
            public Object m_Value;

            public LinkNode(Object key, Object value)
            {
                m_Previous = null;
                m_Next = null;
                m_Key = key;
                m_Value = value;
            }

            public void Remove()
            {
                if (m_Previous != null)
                {
                    m_Previous.m_Next = m_Next;
                }

                if (m_Next != null)
                {
                    m_Next.m_Previous = m_Previous;
                }

                m_Previous = null;
                m_Next = null;
            }
        }

        #endregion

        #region KeyCollection

        public class KeyCollection : ICollection
        {
            private IDictionary m_Dictionary;

            public KeyCollection(IDictionary dictionary)
            {
                m_Dictionary = dictionary;
            }

            public int Count
            {
                get
                {
                    return m_Dictionary.Count;
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            public Object SyncRoot
            {
                get
                {
                    return this;
                }
            }

            public void CopyTo(Array array, int index)
            {
                foreach (DictionaryEntry entry in m_Dictionary)
                {
                    ((Object[])array)[index++] = entry.Key;
                }
            }

            public Object[] ToArray(Object[] array)
            {
                CopyTo(array, 0);
                return array;
            }

            public IEnumerator GetEnumerator()
            {
                return new KeyEnumerator(m_Dictionary.GetEnumerator());
            }

            public struct KeyEnumerator : IEnumerator
            {
                private IDictionaryEnumerator m_Enumerator;

                public KeyEnumerator(IDictionaryEnumerator enumerator)
                {
                    m_Enumerator = enumerator;
                }

                public Object Current
                {
                    get
                    {
                        return m_Enumerator.Entry.Key;
                    }
                }

                public bool MoveNext()
                {
                    return m_Enumerator.MoveNext();
                }

                public void Reset()
                {
                    m_Enumerator.Reset();
                }
            }
        }

        #endregion

        #region ValueCollection

        public class ValueCollection : ICollection
        {
            private IDictionary m_Dictionary;

            public ValueCollection(IDictionary dictionary)
            {
                m_Dictionary = dictionary;
            }

            public void CopyTo(Object[] array, int index)
            {
                foreach (DictionaryEntry entry in m_Dictionary)
                {
                    array[index++] = entry.Value;
                }
            }

            public int Count
            {
	            get
                {
                    return m_Dictionary.Count;
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            public Object SyncRoot
            {
                get
                {
                    return this;
                }
            }

            public void CopyTo(Array array, int index)
            {
                foreach (DictionaryEntry entry in m_Dictionary)
                {
                    ((Object[])array)[index++] = entry.Value;
                }
            }

            public Object[] ToArray(Object[] array)
            {
                CopyTo(array, 0);
                return array;
            }

            public IEnumerator  GetEnumerator()
            {
                return new ValueEnumerator(m_Dictionary.GetEnumerator());
            }

            public struct ValueEnumerator : IEnumerator
            {
                private IDictionaryEnumerator m_Enumerator;

                public ValueEnumerator(IDictionaryEnumerator enumerator)
                {
                    m_Enumerator = enumerator;
                }

                public Object Current
                {
                    get
                    {
                        return m_Enumerator.Entry.Value;
                    }
                }

                public bool MoveNext()
                {
                    return m_Enumerator.MoveNext();
                }

                public void Reset()
                {
                    m_Enumerator.Reset();
                }
            }
        }

        #endregion

        private LinkNode m_Head;
        private LinkNode m_Tail;
        private Hashtable m_Hashtable;

        public LinkedHashMap()
        {
            m_Head = null;
            m_Tail = null;
            m_Hashtable = new Hashtable();
        }

        public LinkedHashMap(int capacity)
        {
            m_Head = null;
            m_Tail = null;
            m_Hashtable = new Hashtable(capacity);
        }

        #region IDictionary Properties

        public int Count
        {
            get
            {
                return m_Hashtable.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public Object this[Object key]
        {
            get
            {
                LinkNode node = (LinkNode)m_Hashtable[key];

                if (node != null)
                {
                    return node.m_Value;
                }

                return null;
            }

            set
            {
                LinkNode node = (LinkNode)m_Hashtable[key];

                if (node != null)
                {
                    node.m_Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public ICollection Keys
        {
            get
            {
                return new KeyCollection(this);
            }
        }

        public Object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public ICollection Values
        {
            get
            {
                return new ValueCollection(this);
            }
        }

        #endregion

        #region IDictionary Methods

        public void Add(Object key, Object value)
        {
            if (m_Hashtable.ContainsKey(key))
            {
                return;
            }

            LinkNode node = new LinkNode(key, value);

            if (m_Head == null)
            {
                m_Head = node;
            }
            else
            {
                m_Tail.m_Next = node;
            }

            m_Tail = node;

            m_Hashtable.Add(key, node);
        }

        public void Clear()
        {
            for (LinkNode node = m_Head; node != null; )
            {
                LinkNode nextNode = node.m_Next;

                node.m_Previous = null;
                node.m_Next = null;
                node.m_Key = null;
                node.m_Value = null;

                node = nextNode;
            }

            m_Head = null;
            m_Tail = null;

            m_Hashtable.Clear();
        }

        public bool Contains(Object key)
        {
            return m_Hashtable.Contains(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Remove(Object key)
        {
            LinkNode node = (LinkNode)m_Hashtable[key];

            if (node != null)
            {
                if (m_Head == node)
                {
                    m_Head = node.m_Next;
                }

                if (m_Tail == node)
                {
                    m_Tail = node.m_Previous;
                }

                node.Remove();

                m_Hashtable.Remove(key);
            }
        }

        #endregion

        #region ICollection Methods

        public void CopyTo(Array array, int index)
        {
            foreach (DictionaryEntry entry in this)
            {
                ((Object[])array)[index++] = entry;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Enumerator

        public struct Enumerator : IDictionaryEnumerator
        {
            private LinkedHashMap m_Map;
            private LinkNode m_CurrentNode;

            public Enumerator(LinkedHashMap map)
            {
                m_Map = map;
                m_CurrentNode = null;
            }

            public Object Current
            {
                get
                {
                    return Entry;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(m_CurrentNode.m_Key, m_CurrentNode.m_Value);
                }
            }

            public Object Key
            {
                get
                {
                    return m_CurrentNode.m_Key;
                }
            }

            public Object Value
            {
                get
                {
                    return m_CurrentNode.m_Value;
                }
            }

            public Boolean MoveNext()
            {
                if (m_CurrentNode == m_Map.m_Tail)
                {
                    return false;
                }

                if (m_CurrentNode == null)
                {
                    m_CurrentNode = m_Map.m_Head;

                    if (m_CurrentNode == null)
                    {
                        return false;
                    }
                }
                else
                {
                    m_CurrentNode = m_CurrentNode.m_Next;
                }

                return true;
            }

            public void Reset()
            {
                m_CurrentNode = null;
            }
        }

        #endregion
    }
}
