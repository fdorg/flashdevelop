package;
class CalltipDef_issue2356_2_super {
	function new(v1:Int, v2:Int) {}
}
class CalltipDef_issue2356_2 extends CalltipDef_issue2356_2_super {
	function new() {
		super(1, $(EntryPoint));
	}
}