package;
class Foo {
	public function new() {}
	public function foo():String {}
}

class Bar extends Foo {
	public override function foo():String {
		return super.foo();
	}
}