package generatortest;

class ImplementTest{
	
	/* INTERFACE ITest */
	
	@:isVar public var x(get, set):Int;
	
	function get_x():Int {
		return x;
	}
	
	function set_x(value:Int):Int {
		return x = value;
	}
}