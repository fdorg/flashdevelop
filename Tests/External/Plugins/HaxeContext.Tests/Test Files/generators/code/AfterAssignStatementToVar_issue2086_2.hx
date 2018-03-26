package;
class Foo {
	public function new() {
		var a : Iterable<Int> = [1,2,3,4];
		for (it in a) {
			var i:Int = it;
		}
	}
}