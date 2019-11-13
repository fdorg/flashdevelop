package;
import haxe.Timer;
class Foo {
	public function new() {}
	public function foo(v:Array<haxe.Timer->String>) {}
}

class Bar extends Foo {
	public override function foo(v:Array<haxe.Timer->String>) {
		super.foo(v);
	}
}