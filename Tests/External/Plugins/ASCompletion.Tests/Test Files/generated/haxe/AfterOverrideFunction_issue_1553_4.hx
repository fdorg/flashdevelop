package;
import Type.ValueType;
import haxe.Timer;
class Foo {
	public function new() {}
	public function foo(c:haxe.Timer->({v:Type.ValueType, s:String}->Void)) {}
}

class Bar extends Foo {
	public override function foo(c:haxe.Timer->({v:Type.ValueType, s:String}->Void)) {
		super.foo(c);
	}
}