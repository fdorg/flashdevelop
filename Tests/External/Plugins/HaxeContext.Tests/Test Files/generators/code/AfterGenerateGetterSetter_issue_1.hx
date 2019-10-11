package;
class GenerateGetterSetter {
	
	function get_foo():String {
		return foo;
	}
	
	function set_foo(value:String):String {
		return foo = value;
	}
	
	function get_bar() {
		return null;
	}
	
	var foo(get, set):String;
}