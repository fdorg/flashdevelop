package;
class Issue2632_1 {
	public function new() {
		f(function():Void {  })
	}
	function f(v:Void->Void) {}
}