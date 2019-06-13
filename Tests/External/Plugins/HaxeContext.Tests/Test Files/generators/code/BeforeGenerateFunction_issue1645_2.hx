package;
import haxe.Constraints.Function;
public class Issue1645_2 {
	public function new() {
		f1(f3$(EntryPoint)());
	}
	
	function f1(f:Function) {}
}