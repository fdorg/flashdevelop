package;
class GenerateGetterSetter {
	var foo(get, set):String;
	
	function get_foo():String {
		return foo;
	}
	
	function set_foo(value:String):String {
		return foo = value;
	}
}