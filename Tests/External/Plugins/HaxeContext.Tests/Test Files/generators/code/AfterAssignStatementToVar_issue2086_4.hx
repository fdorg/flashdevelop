package;
private typedef Ints1 = Array<Int>;
private typedef Ints = Ints1;
class Foo {
	public function new() {
		var a : Ints = [1,2,3,4];
		for (it in a) {
			var i:Int = it;
		}
	}
}