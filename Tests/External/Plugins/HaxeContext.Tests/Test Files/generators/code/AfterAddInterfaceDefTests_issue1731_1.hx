package;
import haxe.ds.Vector;
import haxe.Timer;
class Main implements IFoo {
	public function foo(c:haxe.Timer->haxe.ds.Vector<Int>) { }
}

interface IFoo {
	
	function foo(c:haxe.Timer->haxe.ds.Vector<Int>):Void;
}