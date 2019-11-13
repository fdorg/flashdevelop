package;
class GenerateGetterSetter {
	
	function get_foo():String {
		return foo;
	}
	
	function set_foo(value:String):String {
		return foo = value;
	}
	
	var foo(get, set):String;
	
}