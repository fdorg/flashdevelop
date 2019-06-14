package;
import Type.ValueType;
import haxe.Timer;
import haxe.ds.Vector;
class Foo {
	public function new() {}
	@:isVar public var foo(set):haxe.ds.Vector<haxe.Timer->Type.ValueType>;
	function get_foo() return 1;
	function set_foo(value) return foo = value;
}


class Bar extends Foo {
	//@:setter(foo)
	override function set_foo(value:haxe.ds.Vector<haxe.Timer->Type.ValueType>):haxe.ds.Vector<haxe.Timer->Type.ValueType> {
		return super.set_foo(value);
	}
}