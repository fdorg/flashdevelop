package;
class CalltipDef_issue2468_1<T> {
	public function new(v1:Int) {}
}
class CalltipDef_issue2468_1_subtype {
	function new() {
		new CalltipDef_issue2468_1<Foo>($(EntryPoint));
	}
}