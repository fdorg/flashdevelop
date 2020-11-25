package {
	public class Main {}
}

class Foo {
	public function get foo():String { return ""; }
	public function set foo(v:String):void {}
}

class Bar extends Foo {
	public override function get foo():String {
		return super.foo;
	}
	
	public override function set foo(value:String):void {
		super.foo = value;
	}
}