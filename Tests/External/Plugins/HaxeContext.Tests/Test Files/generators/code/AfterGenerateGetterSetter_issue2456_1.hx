package;
class Issue2456_1 {
	@:isVar var foo(get, set) = 1;
	
	function get_foo():Int {
		return foo;
	}
	
	function set_foo(value:Int):Int {
		return foo = value;
	}
}