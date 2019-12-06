package;
class Issue2939_1 {
	public function new() {
		function foo() {}
		StringTools.tri$(EntryPoint)m("", foo());
	}
}