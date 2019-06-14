package;
class Main {
	public function new() {
        var f:(Void->Int)->Int = foo;
	}

	function foo(f:Void->Int):Int {return f();}
}