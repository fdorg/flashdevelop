package;
class Foo2381_1 {
	@:isVar public var foo(get, set):String->String;
	
	function get_foo():String->String {
		return foo;
	}

	function set_foo(value:String->String):String->String {
		return foo = value;
	}

	function checkLength(length:Int):Bool {
		return length <= 0x7FFFFFFF;
	}
}  