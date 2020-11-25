package;
import haxe.ds.Vector;
import Type.ValueType;
import haxe.Timer;
class Main {
	public var _foo:Foo;
	
	/* DELEGATE Foo */
	
	public var v1(get, null):haxe.Timer->(Type.ValueType->Void);
	
	private function get_v1():haxe.Timer->(Type.ValueType->Void) {
		return _foo.v1;
	}
	
	public var v2(null, set):haxe.Timer->(Type.ValueType->Void);
	
	private function set_v2(value:haxe.Timer->(Type.ValueType->Void)):haxe.Timer->(Type.ValueType->Void) {
		return _foo.v2 = value;
	}
	
	public var v3(get, set):haxe.Timer->(Type.ValueType->Void);
	
	private function get_v3():haxe.Timer->(Type.ValueType->Void) {
		return _foo.v3;
	}
	
	private function set_v3(value:haxe.Timer->(Type.ValueType->Void)):haxe.Timer->(Type.ValueType->Void) {
		return _foo.v3 = value;
	}
	
	public function foo(c:haxe.Timer->Void):Void->haxe.ds.Vector<Int> {
		return _foo.foo(c);
	}
}

class Foo {
	public var v1(default, null):haxe.Timer->(Type.ValueType->Void);
	public var v2(null, default):haxe.Timer->(Type.ValueType->Void);
	public var v3(get, set):haxe.Timer->(Type.ValueType->Void);
	public function foo(c:haxe.Timer->Void):Void->haxe.ds.Vector<Int> return null;
}