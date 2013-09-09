intrinsic dynamic class Array
{
	static var CASEINSENSITIVE:Number;
	static var DESCENDING:Number;
	static var UNIQUESORT:Number;
	static var RETURNINDEXEDARRAY:Number;
	static var NUMERIC:Number;

	var length:Number;

	function push(value : Object):Number;
	function pop():Object;
	function concat(value:Object):Array;
	function shift():Object;
	function unshift(value:Object):Number;
	function slice(startIndex:Number, endIndex:Number):Array;
	function join(delimiter:String):String;
	function splice(startIndex:Number, deleteCount:Number, value:Object):Array;
	function toString():String;
	function sort(compare : Object, options: Number):Array;
	function reverse():Void;
	function sortOn(key:Object, options: Object):Array;
}


