package;
import haxe.ds.Vector;
class Issue2358_2 {
	function foo(v:Dynamic) {
		(v:Null<Vector<Int>>).$(EntryPoint)
	}
}