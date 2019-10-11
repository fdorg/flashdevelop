package;
import Type.ValueType;
import haxe.Timer;
class Foo {
	public function new() {}
	public function foo(c:haxe.Timer->(Type.ValueType->Void)) {}
}

class Bar extends Foo {
	public override function foo(c:haxe.Timer->(Type.ValueType->Void)) {
		super.foo(c);
	}
}