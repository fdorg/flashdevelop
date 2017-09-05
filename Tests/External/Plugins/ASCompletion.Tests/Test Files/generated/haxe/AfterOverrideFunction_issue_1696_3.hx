package;
import Type.ValueType;
import haxe.ds.Vector;
import haxe.Timer;
class Foo {
	public function new() {}
	public function foo(v:{a:Array<haxe.Timer>}->{a:haxe.ds.Vector<Type.ValueType>}->String) {}
}

class Bar extends Foo {
	public override function foo(v:{a:Array<haxe.Timer>}->{a:haxe.ds.Vector<Type.ValueType>}->String) {
		super.foo(v);
	}
}