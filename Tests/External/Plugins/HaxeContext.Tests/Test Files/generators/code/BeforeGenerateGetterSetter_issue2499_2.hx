package;
class Issue2499_2 {
	static function test<T:{}>(v:Class<T>):T return v;
	public function new() {
		test(String);$(EntryPoint)
	}
}