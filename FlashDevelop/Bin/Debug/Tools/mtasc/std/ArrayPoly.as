intrinsic class ArrayPoly extends Array
{
	function push(value : ArrayParam):Number;
	function pop():Object;
	function concat(value:Object):/*ArrayParam*/Array;
	function shift():Object;
	function unshift(value:ArrayParam):Number;
	function slice(startIndex:Number, endIndex:Number):/*ArrayParam*/Array;
	function join(delimiter:String):String;
	function splice(startIndex:Number, deleteCount:Number, value:ArrayParam):/*ArrayParam*/Array;
	function toString():String;
	function sort(compare : Object, options: Number):/*ArrayParam*/Array;
	function reverse():Void;
	function sortOn(key:Object, options: Number):/*ArrayParam*/Array;
}


