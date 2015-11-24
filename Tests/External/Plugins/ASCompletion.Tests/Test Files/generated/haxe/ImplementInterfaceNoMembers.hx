package generatortest;

class ImplementTest{
	
	/* INTERFACE ITest */
	
	public var normalVariable():Int;
	
	public var ro(default, null):Int;

	public var wo(null, default):Int;

	@:isVar public var x(get, set):Int;
	
	get_x():Int {
		return x;
	}
	
	set_x(val:Int):Int {
		return x = val;
	}

	public var y(get, never):Int;
	
	function get_y():Int {
		return y;
	}
	
	public function testMethod():Number {
		
	}
	
	public function testMethodArgs(arg:Float, arg2:Bool):Int {
		
	}
	
	private function testPrivateMethod(?arg:String, ?arg2:Int = 1):Number {
		
	}
}