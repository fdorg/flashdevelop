package;
import haxe.io.Input;
class Foo {
	public function new() {}
	@:isVar public var foo(get, set):haxe.io.Input;
	function get_foo():haxe.io.Input return 1;
	function set_foo(value:haxe.io.Input) return foo = value;
}

class Bar extends Foo {
	//@:getter(foo)
	override function get_foo():haxe.io.Input {
		return super.get_foo();
	}
	
	//@:setter(foo)
	override function set_foo(value:haxe.io.Input):haxe.io.Input {
		return super.set_foo(value);
	}
}