// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;
using System.IO;

/// <summary>
/// Contains conversion support elements such as classes, interfaces and static methods.
/// </summary>
public class SupportClass
{
    /*******************************/
    //Provides access to a static Random class instance
    static public Random Random = new Random();

	/*******************************/
	/// <summary>
	/// Provides functionality for classes that implements the IList interface.
	/// </summary>
    public class IListSupport
    {
        /// <summary>
        /// Ensures the capacity of the list to be greater or equal than the specified.
        /// </summary>
        /// <param name="list">The list to be checked.</param>
        /// <param name="capacity">The expected capacity.</param>
        public static void EnsureCapacity(System.Collections.ArrayList list, int capacity)
        {
            if (list.Capacity < capacity) list.Capacity = 2 * list.Capacity;
            if (list.Capacity < capacity) list.Capacity = capacity;
        }
    }

	/*******************************/
	/// <summary>
	/// This class has static methods to manage collections.
	/// </summary>
    public class CollectionsSupport
    {
        /// <summary>
        /// Sorts an IList collections
        /// </summary>
        /// <param name="list">The System.Collections.IList instance that will be sorted</param>
        /// <param name="Comparator">The Comparator criteria, null to use natural comparator.</param>
        public static void Sort(System.Collections.IList list, System.Collections.IComparer Comparator)
        {
            if (((System.Collections.ArrayList)list).IsReadOnly)
                throw new NotSupportedException();

            if ((Comparator == null) || (Comparator is System.Collections.Comparer))
            {
                try
                {
                    ((System.Collections.ArrayList)list).Sort();
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidCastException(e.Message);
                }
            }
            else
            {
                try
                {
                    ((System.Collections.ArrayList)list).Sort(Comparator);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidCastException(e.Message);
                }
            }
        }
    }

	/// <summary>
	/// This class provides functionality not found in .NET collection-related interfaces.
	/// </summary>
    public class ICollectionSupport
    {
        /// <summary>
        /// Returns an array containing all the elements of the collection.
        /// </summary>
        /// <returns>The array containing all the elements of the collection.</returns>
        public static Object[] ToArray(System.Collections.ICollection c)
        {
            int index = 0;
            Object[] objects = new Object[c.Count];
            System.Collections.IEnumerator e = c.GetEnumerator();

            while (e.MoveNext())
                objects[index++] = e.Current;

            return objects;
        }

        /// <summary>
        /// Obtains an array containing all the elements of the collection.
        /// </summary>
        /// <param name="objects">The array into which the elements of the collection will be stored.</param>
        /// <returns>The array containing all the elements of the collection.</returns>
        public static Object[] ToArray(System.Collections.ICollection c, Object[] objects)
        {
            int index = 0;

            Type type = objects.GetType().GetElementType();
            Object[] objs = (Object[])Array.CreateInstance(type, c.Count);

            System.Collections.IEnumerator e = c.GetEnumerator();

            while (e.MoveNext())
                objs[index++] = e.Current;

            //If objects is smaller than c then do not return the new array in the parameter
            if (objects.Length >= c.Count)
                objs.CopyTo(objects, 0);

            return objs;
        }

        /// <summary>
        /// Adds all of the elements of the "c" collection to the "target" collection.
        /// </summary>
        /// <param name="target">Collection where the new elements will be added.</param>
        /// <param name="c">Collection whose elements will be added.</param>
        /// <returns>Returns true if at least one element was added, false otherwise.</returns>
        public static bool AddAll(System.Collections.ICollection target, System.Collections.ICollection c)
        {
            System.Collections.IEnumerator e = new System.Collections.ArrayList(c).GetEnumerator();
            bool added = false;

            //Reflection. Invoke "addAll" method for proprietary classes
            System.Reflection.MethodInfo method;
            try
            {
                method = target.GetType().GetMethod("addAll");

                if (method != null)
                    added = (bool)method.Invoke(target, new Object[] { c });
                else
                {
                    method = target.GetType().GetMethod("Add");
                    while (e.MoveNext() == true)
                    {
                        bool tempBAdded = (int)method.Invoke(target, new Object[] { e.Current }) >= 0;
                        added = added ? added : tempBAdded;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return added;
        }
    }

    /*******************************/
    /// <summary>
    /// Provides functionality not found in .NET map-related interfaces.
    /// </summary>
    public class MapSupport
    {
        /// <summary>
        /// Determines whether the SortedList contains a specific value.
        /// </summary>
        /// <param name="d">The dictionary to check for the value.</param>
        /// <param name="obj">The object to locate in the SortedList.</param>
        /// <returns>Returns true if the value is contained in the SortedList, false otherwise.</returns>
        public static bool ContainsValue(System.Collections.IDictionary d, Object obj)
        {
            bool contained = false;
            Type type = d.GetType();

            //Classes that implement the SortedList class
            if (type == Type.GetType("System.Collections.SortedList"))
            {
                contained = (bool)((System.Collections.SortedList)d).ContainsValue(obj);
            }
            //Classes that implement the Hashtable class
            else if (type == Type.GetType("System.Collections.Hashtable"))
            {
                contained = (bool)((System.Collections.Hashtable)d).ContainsValue(obj);
            }
            else
            {
                //Reflection. Invoke "containsValue" method for proprietary classes
                try
                {
                    System.Reflection.MethodInfo method = type.GetMethod("containsValue");
                    contained = (bool)method.Invoke(d, new Object[] { obj });
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return contained;
        }


        /// <summary>
        /// Determines whether the NameValueCollection contains a specific value.
        /// </summary>
        /// <param name="d">The dictionary to check for the value.</param>
        /// <param name="obj">The object to locate in the SortedList.</param>
        /// <returns>Returns true if the value is contained in the NameValueCollection, false otherwise.</returns>
        public static bool ContainsValue(System.Collections.Specialized.NameValueCollection d, Object obj)
        {
            bool contained = false;
            Type type = d.GetType();

            for (int i = 0; i < d.Count && !contained; i++)
            {
                String[] values = d.GetValues(i);
                if (values != null)
                {
                    foreach (String val in values)
                    {
                        if (val.Equals(obj))
                        {
                            contained = true;
                            break;
                        }
                    }
                }
            }
            return contained;
        }

        /// <summary>
        /// Copies all the elements of d to target.
        /// </summary>
        /// <param name="target">Collection where d elements will be copied.</param>
        /// <param name="d">Elements to copy to the target collection.</param>
        public static void PutAll(System.Collections.IDictionary target, System.Collections.IDictionary d)
        {
            if (d != null)
            {
                System.Collections.ArrayList keys = new System.Collections.ArrayList(d.Keys);
                System.Collections.ArrayList values = new System.Collections.ArrayList(d.Values);

                for (int i = 0; i < keys.Count; i++)
                    target[keys[i]] = values[i];
            }
        }

        /// <summary>
        /// Returns a portion of the list whose keys are less than the limit object parameter.
        /// </summary>
        /// <param name="l">The list where the portion will be extracted.</param>
        /// <param name="limit">The end element of the portion to extract.</param>
        /// <returns>The portion of the collection whose elements are less than the limit object parameter.</returns>
        public static System.Collections.SortedList HeadMap(System.Collections.SortedList l, Object limit)
        {
            System.Collections.Comparer comparer = System.Collections.Comparer.Default;
            System.Collections.SortedList newList = new System.Collections.SortedList();

            for (int i = 0; i < l.Count; i++)
            {
                if (comparer.Compare(l.GetKey(i), limit) >= 0)
                    break;

                newList.Add(l.GetKey(i), l[l.GetKey(i)]);
            }

            return newList;
        }

        /// <summary>
        /// Returns a portion of the list whose keys are greater that the lowerLimit parameter less than the upperLimit parameter.
        /// </summary>
        /// <param name="list">The list where the portion will be extracted.</param>
        /// <param name="lowerLimit">The start element of the portion to extract.</param>
        /// <param name="upperLimit">The end element of the portion to extract.</param>
        /// <returns>The portion of the collection.</returns>
        public static System.Collections.SortedList SubMap(System.Collections.SortedList list, Object lowerLimit, Object upperLimit)
        {
            System.Collections.Comparer comparer = System.Collections.Comparer.Default;
            System.Collections.SortedList newList = new System.Collections.SortedList();

            if (list != null)
            {
                if ((list.Count > 0) && (!(lowerLimit.Equals(upperLimit))))
                {
                    int index = 0;
                    while (comparer.Compare(list.GetKey(index), lowerLimit) < 0)
                        index++;

                    for (; index < list.Count; index++)
                    {
                        if (comparer.Compare(list.GetKey(index), upperLimit) >= 0)
                            break;

                        newList.Add(list.GetKey(index), list[list.GetKey(index)]);
                    }
                }
            }

            return newList;
        }

        /// <summary>
        /// Returns a portion of the list whose keys are greater than the limit object parameter.
        /// </summary>
        /// <param name="list">The list where the portion will be extracted.</param>
        /// <param name="limit">The start element of the portion to extract.</param>
        /// <returns>The portion of the collection whose elements are greater than the limit object parameter.</returns>
        public static System.Collections.SortedList TailMap(System.Collections.SortedList list, Object limit)
        {
            System.Collections.Comparer comparer = System.Collections.Comparer.Default;
            System.Collections.SortedList newList = new System.Collections.SortedList();

            if (list != null)
            {
                if (list.Count > 0)
                {
                    int index = 0;
                    while (comparer.Compare(list.GetKey(index), limit) < 0)
                        index++;

                    for (; index < list.Count; index++)
                        newList.Add(list.GetKey(index), list[list.GetKey(index)]);
                }
            }

            return newList;
        }
    }


    /*******************************/
    /// <summary>
    /// Represents a collection ob objects that contains no duplicate elements.
    /// </summary>	
    public interface SetSupport : System.Collections.ICollection, System.Collections.IList
    {
        /// <summary>
        /// Adds a new element to the Collection if it is not already present.
        /// </summary>
        /// <param name="obj">The object to add to the collection.</param>
        /// <returns>Returns true if the object was added to the collection, otherwise false.</returns>
        new bool Add(Object obj);

        /// <summary>
        /// Adds all the elements of the specified collection to the Set.
        /// </summary>
        /// <param name="c">Collection of objects to add.</param>
        /// <returns>true</returns>
        bool AddAll(System.Collections.ICollection c);
    }

    /*******************************/
    /// <summary>
    /// SupportClass for the HashSet class.
    /// </summary>
    [Serializable]
    public class HashSetSupport : System.Collections.ArrayList, SetSupport
    {
        public HashSetSupport()
            : base()
        {
        }

        public HashSetSupport(System.Collections.ICollection c)
        {
            this.AddAll(c);
        }

        public HashSetSupport(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Adds a new element to the ArrayList if it is not already present.
        /// </summary>		
        /// <param name="obj">Element to insert to the ArrayList.</param>
        /// <returns>Returns true if the new element was inserted, false otherwise.</returns>
        new public virtual bool Add(Object obj)
        {
            bool inserted;

            if ((inserted = this.Contains(obj)) == false)
            {
                base.Add(obj);
            }

            return !inserted;
        }

        /// <summary>
        /// Adds all the elements of the specified collection that are not present to the list.
        /// </summary>
        /// <param name="c">Collection where the new elements will be added</param>
        /// <returns>Returns true if at least one element was added, false otherwise.</returns>
        public bool AddAll(System.Collections.ICollection c)
        {
            System.Collections.IEnumerator e = new System.Collections.ArrayList(c).GetEnumerator();
            bool added = false;

            while (e.MoveNext() == true)
            {
                if (this.Add(e.Current) == true)
                    added = true;
            }

            return added;
        }

        /// <summary>
        /// Returns a copy of the HashSet instance.
        /// </summary>		
        /// <returns>Returns a shallow copy of the current HashSet.</returns>
        public override Object Clone()
        {
            return base.MemberwiseClone();
        }
    }

    /*******************************/
	/// <summary>
	/// This class manages array operations.
	/// </summary>
    public class ArraySupport
    {
        /// <summary>
        /// Compares the entire members of one array whith the other one.
        /// </summary>
        /// <param name="array1">The array to be compared.</param>
        /// <param name="array2">The array to be compared with.</param>
        /// <returns>True if both arrays are equals otherwise it returns false.</returns>
        /// <remarks>Two arrays are equal if they contains the same elements in the same order.</remarks>
        public static bool Equals(Array array1, Array array2)
        {
            bool result = false;
            if ((array1 == null) && (array2 == null))
                result = true;
            else if ((array1 != null) && (array2 != null))
            {
                if (array1.Length == array2.Length)
                {
                    int length = array1.Length;
                    result = true;
                    for (int index = 0; index < length; index++)
                    {
                        if (!(array1.GetValue(index).Equals(array2.GetValue(index))))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Fills the array with an specific value from an specific index to an specific index.
        /// </summary>
        /// <param name="array">The array to be filled.</param>
        /// <param name="fromindex">The first index to be filled.</param>
        /// <param name="toindex">The last index to be filled.</param>
        /// <param name="val">The value to fill the array with.</param>
        public static void Fill(Array array, Int32 fromindex, Int32 toindex, Object val)
        {
            Object Temp_Object = val;
            Type elementtype = array.GetType().GetElementType();
            if (elementtype != val.GetType())
                Temp_Object = Convert.ChangeType(val, elementtype);
            if (array.Length == 0)
                throw (new NullReferenceException());
            if (fromindex > toindex)
                throw (new ArgumentException());
            if ((fromindex < 0) || ((Array)array).Length < toindex)
                throw (new IndexOutOfRangeException());
            for (int index = (fromindex > 0) ? fromindex-- : fromindex; index < toindex; index++)
                array.SetValue(Temp_Object, index);
        }
    }

    /*******************************/
    /// <summary>
    /// Converts the specified collection to its string representation.
    /// </summary>
    /// <param name="c">The collection to convert to string.</param>
    /// <returns>A string representation of the specified collection.</returns>
    public static String CollectionToString(System.Collections.ICollection c)
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder();

        if (c != null)
        {

            System.Collections.ArrayList l = new System.Collections.ArrayList(c);

            bool isDictionary = (c is System.Collections.BitArray || c is System.Collections.Hashtable || c is System.Collections.IDictionary || c is System.Collections.Specialized.NameValueCollection || (l.Count > 0 && l[0] is System.Collections.DictionaryEntry));
            for (int index = 0; index < l.Count; index++)
            {
                if (l[index] == null)
                    s.Append("null");
                else if (!isDictionary)
                    s.Append(l[index]);
                else
                {
                    isDictionary = true;
                    if (c is System.Collections.Specialized.NameValueCollection)
                        s.Append(((System.Collections.Specialized.NameValueCollection)c).GetKey(index));
                    else
                        s.Append(((System.Collections.DictionaryEntry)l[index]).Key);
                    s.Append("=");
                    if (c is System.Collections.Specialized.NameValueCollection)
                        s.Append(((System.Collections.Specialized.NameValueCollection)c).GetValues(index)[0]);
                    else
                        s.Append(((System.Collections.DictionaryEntry)l[index]).Value);

                }
                if (index < l.Count - 1)
                    s.Append(", ");
            }

            if (isDictionary)
            {
                if (c is System.Collections.ArrayList)
                    isDictionary = false;
            }
            if (isDictionary)
            {
                s.Insert(0, "{");
                s.Append("}");
            }
            else
            {
                s.Insert(0, "[");
                s.Append("]");
            }
        }
        else
            s.Insert(0, "null");
        return s.ToString();
    }


	/*******************************/
	/// <summary>
	/// The class performs token processing in strings
	/// </summary>
	public class Tokenizer: System.Collections.IEnumerator
	{
		/// Position over the string
		private long currentPos = 0;

		/// Include demiliters in the results.
		private bool includeDelims = false;

		/// Char representation of the String to tokenize.
		private char[] chars = null;
			
		//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
		private string delimiters = " \t\n\r\f";		

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// </summary>
		/// <param name="source">String to tokenize</param>
		public Tokenizer(String source)
		{			
			this.chars = source.ToCharArray();
		}

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// and the specified token delimiters to use
		/// </summary>
		/// <param name="source">String to tokenize</param>
		/// <param name="delimiters">String containing the delimiters</param>
		public Tokenizer(String source, String delimiters):this(source)
		{			
			this.delimiters = delimiters;
		}


		/// <summary>
		/// Initializes a new class instance with a specified string to process, the specified token 
		/// delimiters to use, and whether the delimiters must be included in the results.
		/// </summary>
		/// <param name="source">String to tokenize</param>
		/// <param name="delimiters">String containing the delimiters</param>
		/// <param name="includeDelims">Determines if delimiters are included in the results.</param>
		public Tokenizer(String source, String delimiters, bool includeDelims):this(source,delimiters)
		{
			this.includeDelims = includeDelims;
		}	


		/// <summary>
		/// Returns the next token from the token list
		/// </summary>
		/// <returns>The string value of the token</returns>
		public String NextToken()
		{				
			return NextToken(this.delimiters);
		}

		/// <summary>
		/// Returns the next token from the source string, using the provided
		/// token delimiters
		/// </summary>
		/// <param name="delimiters">String containing the delimiters to use</param>
		/// <returns>The string value of the token</returns>
		public String NextToken(String delimiters)
		{
			//According to documentation, the usage of the received delimiters should be temporary (only for this call).
			//However, it seems it is not true, so the following line is necessary.
			this.delimiters = delimiters;

			//at the end 
			if (this.currentPos == this.chars.Length)
				throw new ArgumentOutOfRangeException();
			//if over a delimiter and delimiters must be returned
			else if (   (Array.IndexOf(delimiters.ToCharArray(),chars[this.currentPos]) != -1)
				     && this.includeDelims )                	
				return "" + this.chars[this.currentPos++];
			//need to get the token wo delimiters.
			else
				return nextToken(delimiters.ToCharArray());
		}

		//Returns the nextToken wo delimiters
		private String nextToken(char[] delimiters)
		{
			string token="";
			long pos = this.currentPos;

			//skip possible delimiters
			while (Array.IndexOf(delimiters,this.chars[currentPos]) != -1)
				//The last one is a delimiter (i.e there is no more tokens)
				if (++this.currentPos == this.chars.Length)
				{
					this.currentPos = pos;
					throw new ArgumentOutOfRangeException();
				}
			
			//getting the token
			while (Array.IndexOf(delimiters,this.chars[this.currentPos]) == -1)
			{
				token+=this.chars[this.currentPos];
				//the last one is not a delimiter
				if (++this.currentPos == this.chars.Length)
					break;
			}
			return token;
		}

				
		/// <summary>
		/// Determines if there are more tokens to return from the source string
		/// </summary>
		/// <returns>True or false, depending if there are more tokens</returns>
		public bool HasMoreTokens()
		{
			//keeping the current pos
			long pos = this.currentPos;
			
			try
			{
				this.NextToken();
			}
			catch (ArgumentOutOfRangeException)
			{				
				return false;
			}
			finally
			{
				this.currentPos = pos;
			}
			return true;
		}

		/// <summary>
		/// Remaining tokens count
		/// </summary>
		public int Count
		{
			get
			{
				//keeping the current pos
				long pos = this.currentPos;
				int i = 0;
			
				try
				{
					while (true)
					{
						this.NextToken();
						i++;
					}
				}
				catch (ArgumentOutOfRangeException)
				{				
					this.currentPos = pos;
					return i;
				}
			}
		}

		/// <summary>
		///  Performs the same action as NextToken.
		/// </summary>
		public Object Current
		{
			get
			{
				return (Object) this.NextToken();
			}		
		}		
		
		/// <summary>
		///  Performs the same action as HasMoreTokens.
		/// </summary>
		/// <returns>True or false, depending if there are more tokens</returns>
		public bool MoveNext()
		{
			return this.HasMoreTokens();
		}
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void  Reset()
		{
			;
		}			
	}
}
