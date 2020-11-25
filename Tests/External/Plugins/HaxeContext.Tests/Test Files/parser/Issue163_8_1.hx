package;
class Bar extends Foo {
	public override function foo():Int {
		return super.foo();
	}
}
class Foo {
	public function foo():Int {}
}