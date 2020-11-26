package test {
	public class ASCompleteTest {}
}

class A {
	public function get foo():Function/*(v1:*):int*/ {
	}
}

class B extends A {
	override public function get foo():Function/*(v1:*):int*/ {
		return super.foo;
	}
}