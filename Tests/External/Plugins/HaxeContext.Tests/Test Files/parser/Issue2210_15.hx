package;
class Issue2210_15 extends Bar {
	function foo(i:Int):Int trace(i)
	override function bar() {}
}
class Bar {
	public function foo():Int {}
}