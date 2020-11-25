package;
import Type.ValueType;
import haxe.Timer;
class Foo {
	public function new() {}
	public function foo(v:Array<haxe.Timer->Type.ValueType->String>) {}
}

class Bar extends Foo {
	public override function foo(v:Array<haxe.Timer->Type.ValueType->String>) {
		super.foo(v);
	}
}