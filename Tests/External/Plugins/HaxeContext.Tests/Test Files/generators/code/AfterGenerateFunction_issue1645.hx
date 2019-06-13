package;
import haxe.Constraints.Function;
public class Main {
	public function new() {
		f1(f3);
	}
	
	function f3():Void {
		
	}
	
	function f1(f:Function) {}
}