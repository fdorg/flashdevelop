package;
class Foo {
	public function new() {}
	public function foo(c:{v:Type.ValueType, t:{t:haxe.Timer}}) {}
}

class Bar extends Foo {
	override $(EntryPoint)foo
}