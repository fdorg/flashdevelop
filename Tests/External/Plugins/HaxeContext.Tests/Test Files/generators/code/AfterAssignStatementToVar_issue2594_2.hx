package;
private class AssignStatementToVarIssue2594_2 {
	public function new() {
		var v = 1 + 2 > 0 || true ? "1" : "0";
		var v1:String = v;
	}
}