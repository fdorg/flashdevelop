package {
	public class Main {}
}

class Foo {
	function get foo():String { return ""; }
	function set foo(v:String):void {}
}

class Bar extends Foo {
	internal override function get foo():String {
		return super.foo;
	}
	
	internal override function set foo(value:String):void {
		super.foo = value;
	}
}