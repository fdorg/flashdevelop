package {
	public class Main {}
}

class Foo {
	function get foo():String { return ""; }
	function set foo(v:String):void {}
}

class Bar extends Foo {
	override $(EntryPoint)foo
}