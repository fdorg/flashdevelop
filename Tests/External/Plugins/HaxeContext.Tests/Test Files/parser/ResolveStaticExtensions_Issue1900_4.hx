package;
using Bar;
class Foo {
	public function new {
		new Foo().$(EntryPoint)
	}
}

class Bar {
	public static function bar1(v:Foo) {}
	public static inline function bar2(v:Foo) {}
	public static var property(default, default):Int = 1;
}