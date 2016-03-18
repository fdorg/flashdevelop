package generatortest;
class ExtractLocaleVariable {
	static function main() {
		var s = new Test<String>("test").get();
		s.toString();
	}
}

@:generic
class Test<T> {
	var value:T;
	
	public function new(value:T) this.value = value;
	
	public function get():T return value;
}