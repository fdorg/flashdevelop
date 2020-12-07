package;
private class AssignStatementToVarIssue2725_3 {
	public function new() {
		var v = 1;
		var v = 1 | v;
		var v1:Int = v;
	}
}