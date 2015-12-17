package
{
	/// The Vector class lets you access and manipulate a vector - an array whose elements all have the same data type.
	public class Vector.<T>
	{
		/// [FP10] The range of valid indices available in the Vector.
		public function get length () : int;
		public function set length (value:int) : void;

		/// [FP10] Indicates whether the length property of the Vector can be changed.
		public function get fixed () : Boolean;
		public function set fixed (value:int) void;

		/// [FP10] Creates a Vector with the specified base type.
		public function Vector (length:uint = 0, fixed:Boolean = false);

		/// [FP10] Concatenates the elements specified in the parameters.
		public function concat (...args) : Vector.<T>;

		/// [FP10] Executes a test function on each item in the Vector until an item is reached that returns false for the specified function.
		public function every (callback:Function, thisObject:Object = null) : Boolean;

		/// [FP10] Executes a test function on each item in the Vector and returns a new Vector containing all items that return true for the specified function.
		public function filter (callback:Function, thisObject:Object = null) : Vector.<T>;

		/// [FP10] Executes a function on each item in the Vector.
		public function forEach (callback:Function, thisObject:Object = null) : void;

		/// [FP10] Searches for an item in the Vector and returns the index position of the item.
		public function indexOf (searchElement:T, fromIndex:int = 0) : int;
		
		/// [FP19] Insert a single element into the Vector.
		public function insertAt (index:int, element:T) : void;

		/// [FP10] Converts the elements in the Vector to strings.
		public function join (sep:String = ",") : String;

		/// [FP10] Searches for an item in the Vector, working backward from the specified index position, and returns the index position of the matching item.
		public function lastIndexOf (searchElement:T, fromIndex:int = 0x7fffffff) : int;

		/// [FP10] Executes a function on each item in the Vector, and returns a new Vector of items corresponding to the results of calling the function on each item in this Vector.
		public function map (callback:Function, thisObject:Object = null) : Vector.<T>;

		/// [FP10] Removes the last element from the Vector and returns that element.
		public function pop () : T;

		/// [FP10] Adds one or more elements to the end of the Vector and returns the new length of the Vector.
		public function push (...args) : uint;

		/// [FP19] Remove a single element from the Vector.
		public function removeAt (index:int) : T;

		/// [FP10] Reverses the order of the elements in the Vector.
		public function reverse () : Vector.<T>;

		/// [FP10] Removes the first element from the Vector and returns that element.
		public function shift () : T;

		/// [FP10] Returns a new Vector that consists of a range of elements from the original Vector.
		public function slice (startIndex:int = 0, endIndex:int = 16777215) : Vector.<T>;

		/// [FP10] Executes a test function on each item in the Vector until an item is reached that returns true.
		public function some (callback:Function, thisObject:Object = null) : Boolean;

		/// [FP10] Sorts the elements in the Vector.
		public function sort (compareFunction:Function) : Vector.<T>;

		/// [FP10] Adds elements to and removes elements from the Vector.
		public function splice (startIndex:int, deleteCount:uint, ...items) : Vector.<T>;

		/// [FP10] Returns a string that represents the elements in the Vector.
		public function toString () : String;

		/// [FP10] Returns a string that represents the elements in the specified Vector.
		public function toLocaleString () : String;

		/// [FP10] Adds one or more elements to the beginning of the Vector and returns the new length of the Vector.
		public function unshift (...args) : uint;

	}

}

