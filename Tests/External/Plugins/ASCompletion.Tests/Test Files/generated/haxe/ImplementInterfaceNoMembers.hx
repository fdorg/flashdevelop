package generatortest;

class ImplementTest{
	
	/* INTERFACE ITest */
	
	public var normalVariable:Int;
	
	public var ro(default, null):Int;
	
	public var wo(null, default):Int;
	
	@:isVar public var x(get, set):Int;
	
	function get_x():Int {
		return x;
	}
	
	function set_x(value:Int):Int {
		return x = value;
	}
	
	public var y(get, never):Int;
	
	function get_y():Int {
		return y;
	}
	
	public function testMethod():Float {
		
	}
	
	public function testMethodArgs(arg:Float, arg2:Bool):Int {
		
	}
	
	function testPrivateMethod(?arg:String, ?arg2:Int = 1):Float {
		
	}
}