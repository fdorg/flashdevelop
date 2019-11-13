package;
class Issue2838_1 {
	@:isVar var foo(get, set):Int->Void;
	
	function get_foo():Int->Void {
		return foo;
	}
	
	function set_foo(value:Int->Void):Int->Void {
		return foo = value;
	}
}