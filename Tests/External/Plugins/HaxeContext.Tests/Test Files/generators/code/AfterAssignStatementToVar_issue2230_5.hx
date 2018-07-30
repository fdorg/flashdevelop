package;
class Bar {
	function new();
	var it:Ints = [1,2,3,4];
}

class Foo extends Bar {
	public function new() {
		for (it in it) {
			var i = it;
		}
		var it : Float = 1.0;
	}
}

@:forward
private abstract Ints(Array<Int>) from Array<Int> to Array<Int> {
	public function new() this = [];
}