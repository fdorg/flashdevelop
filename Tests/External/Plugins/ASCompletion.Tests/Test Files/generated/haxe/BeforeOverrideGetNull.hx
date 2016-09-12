package;
class Foo {
	public function new() {}
	public var foo(get, null):Int;
	function get_foo():Int return 1;
}


class Bar extends Foo {
	override $(EntryPoint)foo
}