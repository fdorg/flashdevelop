dynamic intrinsic class Object
{
	function watch(name:String, callback:Function, userData:Object):Boolean;
	function unwatch(name:String):Boolean;
	function addProperty(name:String, getter:Function, setter:Function):Boolean;
	function hasOwnProperty(name:String):Boolean;
	function isPropertyEnumerable(name:String):Boolean;
	function isPrototypeOf(theClass:Object):Boolean;	
	function toString():String;
	function valueOf():Object;

	var __proto__:Object;
	var constructor : Function;

	static function registerClass(name:String, theClass:Function):Boolean;
	static var prototype:Object;

}

