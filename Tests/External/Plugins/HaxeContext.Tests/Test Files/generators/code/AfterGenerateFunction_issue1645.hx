package;
import haxe.Constraints.Function;
public class Issue1645 {
	public function new() {
		f1(f3);
	}
	
	function f3():Void {
		
	}
	
	function f1(f:Function) {}
}