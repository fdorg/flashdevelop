package;
class Bar {
	var v:haxe.Timer;
}

class Foo extends Bar {
	public function new() {
		var v1 = v;
	}
}