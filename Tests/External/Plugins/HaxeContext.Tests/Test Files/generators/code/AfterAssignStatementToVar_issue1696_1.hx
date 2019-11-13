package;
import haxe.ds.Vector;
class Main {
	public function new() {
        var foo:Vector<Int> = new Foo().foo();
	}
}

class Foo {
	public function new() {}
	public function foo():haxe.ds.Vector<Int> return null;
}