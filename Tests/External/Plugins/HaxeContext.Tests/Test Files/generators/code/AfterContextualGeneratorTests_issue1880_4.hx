package;
class Issue1880_4 {
	public function new() {
		new Foo(~/regex/);
	}
}

class Foo {
	public function new(eReg:EReg) {
	}
}