package;
class Foo {
	public function new() {}
	public function foo(c:haxe.Timer->({v:Type.ValueType, s:String}->Void)) {}
}

class Bar extends Foo {
	override $(EntryPoint)foo
}