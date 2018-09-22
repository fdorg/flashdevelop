package;
import haxe.ds.Vector;
class Issue2358_3 {
	var v:Dynamic<Int>;
	function foo() {
		(v:Null<Vector<Int>>).$(EntryPoint)
	}
}