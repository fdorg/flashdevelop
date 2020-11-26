package;
class Main {
	public function new() {
		function foo(?newName:Foo) {
			var s = '${newName}';
		}
	}
}

class Foo {
	public function new() {}
	public function foo() {}
}