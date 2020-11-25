package test {
	public class ASCompleteTest {}
}

class A {
	public function set foo(v:Function/*(v1:*):int*/):void {
	}
}

class B extends A {
	override public function set foo(value:Function/*(v1:*):int*/):void {
		super.foo = value;
	}
}