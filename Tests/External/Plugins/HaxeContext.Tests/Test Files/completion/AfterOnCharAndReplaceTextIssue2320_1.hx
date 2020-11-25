package;
class Foo2320_1 {
	function foo(? v : String = null) {}
}
class Issue2320_1 extends Foo2320_1 {
	override function foo(?v:String = null):Void {
		super.foo(v);
	}
}