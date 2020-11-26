package;
class Foo {
	public var it:Ints = [1,2,3,4];
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