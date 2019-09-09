// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Util
{
	
	/// <summary> implement a sparse mapping from int -> Object.  Iterators
	/// will traverse from lowest to highest.  put() is O(1) if the key
	/// is higher than any existing key; O(logN) if the key already exists,
	/// and O(N) otherwise.  get() is an O(logN) binary search.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class IntMap
	{
        public class IntMapEntry // : System.Collections.DictionaryEntry
        {
            public IntMapEntry(IntMap map, int index)
            {
                m_Index = index;
                m_IntMap = map;
            }
            private IntMap m_IntMap;
            private int m_Index;
            virtual public System.Object Key
            {
                get
                {
                    return (System.Int32)m_IntMap.m_Keys[m_Index];
                }

            }
            virtual public System.Object Value
            {
                get
                {
                    return m_IntMap.m_Values[m_Index];
                }

                set
                {
                    System.Object old = m_IntMap.m_Values[m_Index];
                    m_IntMap.m_Values[m_Index] = value;
                }
            }
        }

        private class IntMapIterator : System.Collections.IEnumerator
        {
            private IntMap m_IntMap;
            private int m_Index = -1;

            public IntMapIterator(IntMap intMap)
            {
                m_IntMap = intMap;
            }

            public virtual System.Object Current
            {
                get
                {
                    if (m_Index == -1 || m_Index >= m_IntMap.size())
                    {
                        throw new System.InvalidOperationException();
                    }
                    return new IntMapEntry(m_IntMap, m_Index);
                }

            }
            public virtual bool MoveNext()
            {
                if (m_Index >= m_IntMap.size())
                {
                    throw new System.InvalidOperationException();
                }

                m_Index++;

                return m_Index < m_IntMap.size();
            }

            public virtual void remove()
            {
                m_IntMap.remove(m_Index);
            }

            virtual public void Reset()
            {
                m_Index = -1;
            }
        }

        internal int[] m_Keys;
        internal Object[] m_Values;
		private int m_Size;
		
		public IntMap() : this(10)
		{
		}
		
		public IntMap(int capacity)
		{
            m_Keys = new int[capacity];
            m_Values = new System.Object[capacity];
            m_Size = 0;
		}
		
		public virtual int capacity()
		{
            return m_Keys.Length;
		}
		
		private int find(int k)
		{
			int lo = 0;
            int hi = m_Size - 1;
			
			while (lo <= hi)
			{
				int i = (lo + hi) / 2;
                int m = m_Keys[i];
				if (k > m)
					lo = i + 1;
				else if (k < m)
					hi = i - 1;
				else
					return i; // key found
			}
			return - (lo + 1); // key not found, low is the insertion point
		}
		
		public virtual System.Object remove(int k)
		{
			System.Object old = null;
			int i = find(k);
			if (i >= 0)
			{
                old = m_Values[i];
                Array.Copy(m_Keys, i + 1, m_Keys, i, m_Size - i - 1);
                Array.Copy(m_Values, i + 1, m_Values, i, m_Size - i - 1);
                m_Size--;
			}
			return old;
		}
		
		public virtual void  clear()
		{
            m_Size = 0;
		}
		
		public virtual System.Object put(int k, System.Object v)
		{
            if (m_Size == 0 || k > m_Keys[m_Size - 1])
			{
                if (m_Size == m_Keys.Length)
					grow();
                m_Keys[m_Size] = k;
                m_Values[m_Size] = v;
                m_Size++;
				return null;
			}
			else
			{
				int i = find(k);
				if (i >= 0)
				{
                    System.Object old = m_Values[i];
                    m_Values[i] = v;
					return old;
				}
				else
				{
					i = - i - 1; // recover the insertion point
                    if (m_Size == m_Keys.Length)
						grow();
                    Array.Copy(m_Keys, i, m_Keys, i + 1, m_Size - i);
                    Array.Copy(m_Values, i, m_Values, i + 1, m_Size - i);
                    m_Keys[i] = k;
                    m_Values[i] = v;
                    m_Size++;
					return null;
				}
			}
		}
		
		private void  grow()
		{
            int[] newkeys = new int[m_Size * 2];
            Array.Copy(m_Keys, 0, newkeys, 0, m_Size);
            m_Keys = newkeys;

            System.Object[] newvalues = new System.Object[m_Size * 2];
            Array.Copy(m_Values, 0, newvalues, 0, m_Size);
            m_Values = newvalues;
		}
		
		public virtual System.Object get_Renamed(int k)
		{
			int i = find(k);
            return i >= 0 ? m_Values[i] : null;
		}
		
		public virtual bool contains(int k)
		{
			return find(k) >= 0;
		}
		
		/// <summary> A bit of an aberration from an academic point of view,
		/// but since this is an ordered Map, why not!
		/// 
		/// </summary>
		/// <returns> the element immediately following element k.
		/// </returns>
		public virtual System.Object getNextAdjacent(int k)
		{
			int i = find(k);
            return ((i >= 0) && (i + 1 < m_Size)) ? m_Values[i + 1] : null;
		}
		
		public virtual System.Collections.IEnumerator GetEnumerator()
		{
            return new IntMapIterator(this);
		}
		
		public virtual int size()
		{
            return m_Size;
		}
		
		/// <param name="ar">must be of size size().
		/// </param>
		public virtual System.Object[] valuesToArray(System.Object[] ar)
		{
            Array.Copy(m_Values, 0, ar, 0, m_Size);
			return ar;
		}
		
		public virtual int[] keySetToArray()
		{
            int[] ar = new int[m_Size];
            Array.Copy(m_Keys, 0, ar, 0, m_Size);
			return ar;
		}
	}
}
