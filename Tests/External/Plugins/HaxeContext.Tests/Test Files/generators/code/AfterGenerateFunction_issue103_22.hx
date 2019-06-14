package;
public class Main {
	public function new() {
		foo(test());
	}
	
	function test():AInt {
		return 0;
	}
	
	function foo(v:AInt) {}
}

abstract AInt(Int) from Int to Int {
	public function new() this = 1;
}