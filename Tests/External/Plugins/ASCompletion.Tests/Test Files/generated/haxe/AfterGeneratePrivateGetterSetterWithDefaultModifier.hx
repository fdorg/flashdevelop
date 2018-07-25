package;
class GenerateGetterSetter {
	@:isVar var foo(get, set):String;
	
	private function get_foo():String {
		return foo;
	}
	
	private function set_foo(value:String):String {
		return foo = value;
	}
}