package test.test;

class Test
{
	public static inline var CONSTANT = 1;
	
	public var id:Int;

	private var _name:String;

	public var ro(default, null):Int;

	public var wo(null, default):Int;

	@:isVar public var x(get, set):Int;
	function get_x()
	{
		return 1;
	}
	function set_x(val)
	{
		return x = val;
	}

	public var y(get, never):Int;
	function get_y():Int return 1;
  
	public function new(?ds:Iterable<String>)
	{
	}
	
	static function bar(s:String, v:Bool):Void
	{
	}
	
	public function foo(?s:String, ?v:Bool = true):Bool
	{
		return true;
	}
	
	public function boz(?s:String, 
		?v:Bool = true)
	{
		return true;
	}
	
	public function nestedGenerics(s:Array<Array<Int>>):Void
	{
	}
}