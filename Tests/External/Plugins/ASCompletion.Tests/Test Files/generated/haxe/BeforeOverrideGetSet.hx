package;
class Foo {
	public function new() {}
	@:isVar public var foo(get, set):Int;
	function get_foo():Int return 1;
	function set_foo(value:Int) return foo = value;
}


class Bar extends Foo {
	override $(EntryPoint)foo
}