package;
class Flags {
	@:isVar var f(get, set):cs.Flags;
	
	function get_f():Flags {
		return f;
	}
	
	function set_f(value:Flags):Flags {
		return f = value;
	}
}