package {
	public class Main {}
}

class Foo {
	public function get foo():String { return ""; }
	public function set foo(v:String):void {}
}

class Bar extends Foo {
	override $(EntryPoint)foo
}