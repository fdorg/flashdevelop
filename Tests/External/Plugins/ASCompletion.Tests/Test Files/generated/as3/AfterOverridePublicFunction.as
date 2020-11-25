package {
	public class Main {}
}

class Foo {
    public function foo():String {
    }
}

class Bar extends Foo {
    public override function foo():String {
		return super.foo();
	}
}