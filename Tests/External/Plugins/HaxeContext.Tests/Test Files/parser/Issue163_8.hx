package;
class Bar extends Foo {
	override function foo():Int {
		return super.foo();
	}
}
@:publicFields
class Foo {
	function foo():Int {}
}