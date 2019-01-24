package;
class Issue2632_1 {
	public function new() {
		f(() -> {  })
	}
	function f(v:Void->Void) {}
}