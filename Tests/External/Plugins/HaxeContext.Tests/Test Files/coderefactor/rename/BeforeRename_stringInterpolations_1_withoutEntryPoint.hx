package;
class Main {
	public function new() {
		function foo(?v:Foo) {
			var s = '${v}';
		}
	}
}

class Foo {
	public function new() {}
	public function foo() {}
}