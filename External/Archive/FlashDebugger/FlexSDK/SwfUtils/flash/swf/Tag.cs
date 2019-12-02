// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
namespace Flash.Swf
{
	
	/// <summary> Base class for all player tags</summary>
	/// <author>  Clement Wong
	/// </author>
	public abstract class Tag : TagValues
	{
        private class TagIterator : System.Collections.IEnumerator
		{
            Tag m_Tag;

			public TagIterator(Tag tag)
			{
                m_Tag = tag;
			}
			public virtual System.Object Current
			{
				get
				{
                    if (bBeforeStart || bDone || m_Tag.SimpleReference == null)
                    {
                        throw new InvalidOperationException();
                    }

                    return m_Tag.SimpleReference;
                }
				
			}

            private bool bBeforeStart = true;
            private bool bDone = false;
			
			public virtual bool MoveNext()
			{
                if (bBeforeStart && m_Tag.SimpleReference != null)
                {
                    bBeforeStart = false;
                    return true;
                }

                if (m_Tag.SimpleReference == null || bDone)
                {
                    throw new InvalidOperationException();
                }

                bDone = true;

                return false;
			}
			public virtual void remove()
			{
				throw new System.NotSupportedException();
			}

			virtual public void Reset()
			{
                bBeforeStart = true;
                bDone = false;
			}
		}
		/// <summary> Find the immediate dependencies.  unlike visitDefs, it doesn't explore the entire tree.
		/// The user must do a recursive walk if they care to go beyond the first order dependencies.
		/// The default implementation provides an iterator over a single simple reference, defined
		/// by the derived class via the getSimpleReference() call.
		/// </summary>
		/// <returns> An iterator over the first order Tag dependencies.
		/// </returns>
		virtual public System.Collections.IEnumerator References
		{
			get
			{
				return new TagIterator(this);
			}
			
		}
		public int code;
		
		public Tag(int code)
		{
			this.code = code;
		}
		
		/// <summary> Subclasses implement this method to callback one of the methods in TagHandler...</summary>
		/// <param name="h">
		/// </param>
		public abstract void  visit(TagHandler h);
		
		/// <summary> many tags have zero or one reference, in which case they only need
		/// to override this method.  Tags that have two or more references
		/// should override getReferences() and provide an Iterator.
		/// </summary>
		/// <returns>
		/// </returns>
		public virtual Tag SimpleReference
		{
            get
            {
                return null;
            }
		}
		
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;
			
			if (obj is Tag)
			{
				Tag tag = (Tag) obj;
				
				if (tag.code == this.code)
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			return code;
		}
		
		public static bool equals(System.Object o1, System.Object o2)
		{
			return o1 == o2 || o1 != null && o1.Equals(o2);
		}
	}
}
