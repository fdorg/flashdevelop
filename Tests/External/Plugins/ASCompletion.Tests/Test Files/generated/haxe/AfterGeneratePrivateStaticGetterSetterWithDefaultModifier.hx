package;
class GenerateGetterSetter {
	@:isVar private static var foo(get, set):String;
	
	private static function get_foo():String {
		return foo;
	}
	
	private static function set_foo(value:String):String {
		return foo = value;
	}
}