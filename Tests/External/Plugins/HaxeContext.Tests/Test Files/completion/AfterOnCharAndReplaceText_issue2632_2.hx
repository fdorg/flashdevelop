package;
class Issue2632_2 {
	public function new() {
		f(null, () -> {  })
	}
	function f(v1:String, v2:Void->Void) {}
}