package;
class Foo {
	public function new() {
		var a:Array<Dynamic> = [];
		var b = a;
		var v = b;
		for(it in v) {
			var i:Dynamic = it;
		}
	}
}