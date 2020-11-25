package {
}

class Foo {
    internal function foo():String {
    }
}

class Bar extends Foo {
    internal override function foo():String {
		return super.foo();
	}
}