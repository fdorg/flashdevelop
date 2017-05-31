package {
}

class Foo {
	protected function foo():String {
	}
}

class Bar extends Foo {
	protected override function foo():String {
		return super.foo();
	}
}