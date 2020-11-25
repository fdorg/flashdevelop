package;
using StringTools;
class Issue2939_1 {
	public function new() {
		function foo() {}
		"".trim(foo());
	}
}