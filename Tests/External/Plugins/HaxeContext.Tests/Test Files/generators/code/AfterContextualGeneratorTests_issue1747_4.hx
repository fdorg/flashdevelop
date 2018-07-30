package;
class Flags {
	@:isVar var f(get, set):cs.Flags;
	
	function get_f():cs.Flags {
		return f;
	}
	
	function set_f(value:cs.Flags):cs.Flags {
		return f = value;
	}
}