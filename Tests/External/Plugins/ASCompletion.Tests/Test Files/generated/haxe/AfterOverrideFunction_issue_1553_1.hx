package;
import haxe.Timer;
class Foo {
	public function new() {}
	public function foo(c:haxe.Timer->Void) {}
}

class Bar extends Foo {
	public override function foo(c:haxe.Timer->Void) {
		super.foo(c);
	}
}