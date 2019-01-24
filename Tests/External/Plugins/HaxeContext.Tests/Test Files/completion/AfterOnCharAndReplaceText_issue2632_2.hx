package;
class Issue2632_2 {
	public function new() {
		f(null, function():Void {  })
	}
	function f(v1:String, v2:Void->Void) {}
}