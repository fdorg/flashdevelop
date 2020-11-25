package;
class Foo {
	public function new() {
		var a : Ints = [1,2,3,4];
		cast a.length;$(EntryPoint)
	}
}

@:forward
private abstract Ints(Array<Int>) from Array<Int> to Array<Int> {
	public function new() this = [];
}