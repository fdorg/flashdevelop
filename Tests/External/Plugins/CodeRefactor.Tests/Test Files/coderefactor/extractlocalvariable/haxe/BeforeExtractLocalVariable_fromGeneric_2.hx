package generatortest;
class ExtractLocaleVariable {
	static function main() {
		$(EntryPoint)new Test<String>("test")$(ExitPoint).get().toString();
	}
}

@:generic
class Test<T> {
	var value:T;
	
	public function new(value:T) this.value = value;
	
	public function get():T return value;
}