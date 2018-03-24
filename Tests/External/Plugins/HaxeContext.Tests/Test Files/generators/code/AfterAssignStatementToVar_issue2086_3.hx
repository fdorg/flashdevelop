package;
private typedef Ints = Array<Int>;
class Foo {
	public function new() {
		var a : Ints = [1,2,3,4];
		for (it in a) {
			var i:Int = it;
		}
	}
}