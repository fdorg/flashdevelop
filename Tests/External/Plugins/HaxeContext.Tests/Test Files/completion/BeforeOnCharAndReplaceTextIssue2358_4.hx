package;
import haxe.ds.Vector;
typedef Ints1 = Vector<Int>;
typedef Ints2 = Ints1;
typedef Ints3 = Ints2;
class Issue2358_4 {
	var v:Ints3;
	function foo() {
		(v:Null<Vector<Int>>).$(EntryPoint)
	}
}