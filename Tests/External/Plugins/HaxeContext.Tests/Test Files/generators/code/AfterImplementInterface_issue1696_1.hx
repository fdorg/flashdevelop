package;
import Type.ValueType;
import haxe.Timer;
import haxe.ds.Vector;
class Main implements IFoo {
	public function new() {
	}
	
	
	/* INTERFACE IFoo */
	
	public function foo():haxe.ds.Vector<haxe.Timer->Type.ValueType> {
		return null;
	}
}

interface IFoo {
	function foo():haxe.ds.Vector<haxe.Timer->Type.ValueType>;
}