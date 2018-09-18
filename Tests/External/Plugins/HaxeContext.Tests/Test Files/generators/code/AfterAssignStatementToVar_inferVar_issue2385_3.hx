package;
typedef Ints1<T> = Array<T>;
typedef Ints2 = Ints1<Int>;
typedef Ints3 = Ints2;
class Foo2385_1 {
	public function new() {
		var ints:Ints3;
		var i:Int = ints[0];
	}
}