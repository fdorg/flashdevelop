package;
class Foo {
	public function new() {}
	function foo():String {}
}

class Bar extends Foo {
	override function foo():String {
		return super.foo();
	}
}