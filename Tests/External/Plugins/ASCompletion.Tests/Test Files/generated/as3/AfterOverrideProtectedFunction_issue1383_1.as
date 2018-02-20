package {
}

class Foo {
	protected function foo(v:Vector.<*>):Vector.<*> {
	}
}

class Bar extends Foo {
	protected override function foo(v:Vector.<*>):Vector.<*> {
		return super.foo(v);
	}
}