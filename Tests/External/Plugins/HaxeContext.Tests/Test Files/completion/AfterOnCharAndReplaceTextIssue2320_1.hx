package;
class Foo {
	function foo(? v : String = null) {}
}
class Bar extends Foo {
	override function foo(?v:String = null) {
		super.foo(v);
	}
}