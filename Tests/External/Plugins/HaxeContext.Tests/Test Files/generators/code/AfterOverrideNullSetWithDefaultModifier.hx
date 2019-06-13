package;
class Foo {
	public function new() {}
	public var foo(null, set):Int;
	function set_foo(value:Int):Int {
		return foo = value;
	}
}


class Bar extends Foo {
	//@:setter(foo)
	private override function set_foo(value:Int):Int {
		return super.set_foo(value);
	}
}