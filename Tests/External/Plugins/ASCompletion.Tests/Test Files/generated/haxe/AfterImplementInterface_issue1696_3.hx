package;
import Type.ValueType;
import haxe.Timer;
import haxe.ds.Vector;
class Main implements IBar, IFoo {
	public function new() {
	}
	
	
	/* INTERFACE IFoo */
	
	@:isVar public var foo(get, set):haxe.ds.Vector<haxe.Timer->Type.ValueType>;
	
	function get_foo():haxe.ds.Vector<haxe.Timer->Type.ValueType> {
		return foo;
	}
	
	function set_foo(value:haxe.ds.Vector<haxe.Timer->Type.ValueType>):haxe.ds.Vector<haxe.Timer->Type.ValueType> {
		return foo = value;
	}
}

interface IBar {
	function bar():Void;
}

interface IFoo {
	var foo(get, set):haxe.ds.Vector<haxe.Timer->Type.ValueType>;
}