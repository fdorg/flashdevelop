package;
public class Main {
	public function new() {
		foo({x:10, y:10}, test);
	}
	
	function test(parameter0:String, parameter1:Array<Int->Int>):Void {
		
	}
	
	function foo(point:{x:Int, y:Int}, v:String->Array<Int->Int>->Void) {}
}