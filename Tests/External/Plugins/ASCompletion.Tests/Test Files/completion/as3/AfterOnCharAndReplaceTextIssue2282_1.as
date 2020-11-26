package test {
	public class ASCompleteTest {}
}

class A {
	public function foo(v:Function/*(v1:*):int*/):void {
	}
}

class B extends A {
	override public function foo(v:Function/*(v1:*):int*/):void {
		super.foo(v);
	}
}