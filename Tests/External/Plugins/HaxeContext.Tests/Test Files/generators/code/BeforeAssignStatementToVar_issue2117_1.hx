package;
class Foo {
	public function new() {
		var it : Ints = [1,2,3,4];
		for (it in it) {
			it;$(EntryPoint)
		}
	}
}

@:forward
private abstract Ints(Array<Int>) from Array<Int> to Array<Int> {
	public function new() this = [];
}