package;
using Bar;
class Foo {
	public function new {
		new Foo().$(EntryPoint)
	}
	function bar1() {}
	function bar2() {}
}

class Bar {
	public static function bar1(v:Foo) {}
	public static inline function bar2(v:Foo) {}
	public static var property(default, default):Int = 1;
}