package;
public class Issue103_15 {
	public function new() {
		foo(null);
		test();
	}
	
	function test():Void {
		
	}
	
	function foo(v:Int->String->Void) {}
}