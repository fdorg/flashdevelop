package;
import flash.display.BitmapData;
import haxe.ds.Vector;
class Foo {
	public function new() {
		var v = new haxe.ds.Vector<flash.display.BitmapData>(Std.int(10));
		var v:Vector<BitmapData> = v;
	}
}