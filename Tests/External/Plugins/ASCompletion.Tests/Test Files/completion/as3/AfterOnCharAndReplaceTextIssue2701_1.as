package test {
	public class Issue2701_1 {}
}

class A {
	public function get foo():Number {
		return 0;
	}
	public function set foo(value:Number):void {
		return 0;
	}
}

class B extends A {
	override public function get foo():Number {
		return super.foo;
	}
	
	override public function set foo(value:Number):void {
		super.foo = value;
	}
}