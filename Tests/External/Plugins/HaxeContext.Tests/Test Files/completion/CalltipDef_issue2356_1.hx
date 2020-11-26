package;
class CalltipDef_issue2356_1_super {
	function new(v1:Int) {}
}
class CalltipDef_issue2356_1 extends CalltipDef_issue2356_1_super {
	function new() {
		super($(EntryPoint));
	}
}