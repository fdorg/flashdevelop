package generatortest;
class ExtractLocaleVariable {
	static function main() {
		var newVar:String = new Test<String>("test").get();
		newVar.toString();
	}
}

@:generic
class Test<T> {
	var value:T;
	
	public function new(value:T) this.value = value;
	
	public function get():T return value;
}