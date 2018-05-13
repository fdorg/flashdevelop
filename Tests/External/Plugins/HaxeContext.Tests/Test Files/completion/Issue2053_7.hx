package;
abstract AString(String) from String to String {
	public function new(v) {
		this = v;
	}
}

typedef Foo = AString;

class Bar {
	public static function main() {
		new Foo($(EntryPoint)
	}
}