// intrinsic
intrinsic class BuiltInArray
{

	/**
	*  the length property of the array that returns the number of elements in array (readonly)
	**/
	function get Length () : int;
	
	/**
	 * copy the array elements into another fixed-size array
	 * @param	dest	Destination array
	 * @param	index	Copy array content starting at 'index'
	 */
	function CopyTo(dest:BuiltInArray, index:int = 0);

}
