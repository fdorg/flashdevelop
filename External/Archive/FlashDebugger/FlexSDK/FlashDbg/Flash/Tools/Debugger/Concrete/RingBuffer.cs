// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace Flash.Tools.Debugger.Concrete
{
    class RingBuffer
    {
        public int Size
        {
            get
            {
                return m_Buffer.Length;
            }
        }

        public int Length
        {
            get
            {
                int length = m_IndexIn - m_IndexOut;

                if (length < 0)
                {
                    length += Size;
                }

                return length;
            }
        }

        public Boolean Overwrite
        {
            get
            {
                return m_Overwrite;
            }

            set
            {
                m_Overwrite = value;
            }
        }

        public Byte[] Data
        {
            get
            {
                if (Length > 0)
                {
                    Byte[] array = new Byte[Length];
                    CopyTo(array);
                    return array;
                }
                else
                {
                    return null;
                }
            }
        }

        private Byte[] m_Buffer;
        private int m_IndexIn;
        private int m_IndexOut;
        private Boolean m_Overwrite;

        public RingBuffer()
            : this(1024, false)
        {
        }

        public RingBuffer(int size)
            : this(size, false)
        {
        }

        public RingBuffer(Boolean overwrite)
            : this(1024, overwrite)
        {
        }

        public RingBuffer(int size, Boolean overwrite)
        {
            m_Buffer = new Byte[size];

            m_IndexIn = 0;
            m_IndexOut = 0;
            m_Overwrite = overwrite;
        }

        public void Add(Byte value)
        {
            if (((m_IndexIn + 1) % m_Buffer.Length) == m_IndexOut)
            {
                if (m_Overwrite)
                {
                    m_IndexOut = (m_IndexOut + 1) % m_Buffer.Length;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            m_Buffer[m_IndexIn] = value;
            m_IndexIn = (m_IndexIn + 1) % m_Buffer.Length;
        }

        public void Add(Array array, int offset, int count)
        {
            for (int index = 0; index < count; index++)
            {
                Add(((Byte[])array)[offset + index]);
            }
        }

        public Byte Remove()
        {
            if (m_IndexOut == m_IndexIn)
            {
                throw new InvalidOperationException();
            }

            Byte result = m_Buffer[m_IndexOut];

            m_IndexOut = (m_IndexOut + 1) % m_Buffer.Length;

            return result;
        }

        public void CopyTo(Array array)
        {
            CopyTo(0, array, 0, Length);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo(0, array, index, Length);
        }

        public void CopyTo(int index, Array array, int arrayIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (index < 0 || arrayIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (array.Rank != 1 || index >= m_Buffer.Length || arrayIndex >= array.Length ||
                count > (m_Buffer.Length - index) || count > (array.Length - arrayIndex))
            {
                throw new ArgumentException();
            }

            for (int readIndex = (m_IndexOut + index) % m_Buffer.Length; 
                 readIndex != m_IndexIn; 
                 readIndex = (readIndex + 1) % m_Buffer.Length)
            {
                ((Byte[])array)[index++] = m_Buffer[readIndex];
            }
        }
    }
}
