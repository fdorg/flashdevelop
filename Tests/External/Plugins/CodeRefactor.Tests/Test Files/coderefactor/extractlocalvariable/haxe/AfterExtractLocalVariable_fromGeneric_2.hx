package generatortest;
class ExtractLocaleVariable {
	static function main() {
		var newVar:Test<String> = new Test<String>("test");
		newVar.get().toString();
	}
}

@:generic
class Test<T> {
	var value:T;
	
	public function new(value:T) this.value = value;
	
	public function get():T return value;
}