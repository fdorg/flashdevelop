package;
class Test {
	public function new() {
		var eFoo:AType = AType.EFoo;
	}
}

@:enum abstract AType(Int) {
	var EFoo = 1;
}