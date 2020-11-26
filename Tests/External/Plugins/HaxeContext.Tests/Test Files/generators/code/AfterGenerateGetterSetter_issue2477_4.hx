package;
class Issue2477_4 {
	@:isVar public static var foo(get, set):Int;
	
	static function get_foo():Int {
		return foo;
	}
	
	static function set_foo(value:Int):Int {
		return foo = value;
	}
}