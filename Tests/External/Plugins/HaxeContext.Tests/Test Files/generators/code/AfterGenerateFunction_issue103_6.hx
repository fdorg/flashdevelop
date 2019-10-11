package;
public class Main {
	public function new() {
		foo(bar(1, 2), test);
	}
	
	function test(parameter0:String, parameter1:Array<Int->Int>):Void {
		
	}

	function bar(x:Int, y:Int):Int return 1;
	
	function foo(distance:Int, v:String->Array<Int->Int>->Void) {}
}