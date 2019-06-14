package;
import haxe.Constraints.Function;
public class Issue1645_2 {
	public function new() {
		f1(f3());
	}
	
	function f3():Function {
		
	}
	
	function f1(f:Function) {}
}