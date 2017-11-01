package;
class Main {
	public function new() {
		var someVar = Std.string(1);
		var v = StringTools.l$(EntryPoint)pad('-${someVar}', "0", 20);
	}
}