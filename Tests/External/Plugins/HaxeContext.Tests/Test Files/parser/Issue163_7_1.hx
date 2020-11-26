package;
class Bar extends Foo {
	override function foo():Int {
		return super.foo();
	}
}
class Foo {
	function foo():Int {}
}