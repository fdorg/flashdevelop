package;
class Foo {
	public function new() {
		var it : Ints = [1,2,3,4];
		for (it in it) {
			
			var i = it;
		}
	}
}

@:forward
private abstract Ints(Array<Int>) from Array<Int> to Array<Int> {
	public function new() this = [];
}