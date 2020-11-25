package;
import haxe.Timer;
import Type.ValueType;
class Foo {
	public function new() {}
	public function foo(c:{v:Type.ValueType, t:haxe.Timer}) {}
}

class Bar extends Foo {
	public override function foo(c:{v:Type.ValueType, t:haxe.Timer}) {
		super.foo(c);
	}
}