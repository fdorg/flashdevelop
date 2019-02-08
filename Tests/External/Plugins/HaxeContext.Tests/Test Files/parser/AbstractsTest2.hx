package;

@:forwardStatic
@:forward(length, get, set)
abstract FixedArray<T>(haxe.ds.Vector<T>) {
	public inline function new(length) this = new haxe.ds.Vector<T>(length);
}