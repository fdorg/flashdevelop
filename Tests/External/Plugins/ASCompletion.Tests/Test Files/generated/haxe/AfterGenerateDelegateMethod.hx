package;
import haxe.ds.Vector;
import haxe.Timer;
class Main {
	public var _foo:Foo;
	
	/* DELEGATE Foo */
	
	public function foo(c:haxe.Timer->Void):Void->haxe.ds.Vector<Int> {
		return _foo.foo(c);
	}
}

class Foo {
	public function foo(c:haxe.Timer->Void):Void->haxe.ds.Vector<Int> return null;
}