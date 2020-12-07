package;
private class AssignStatementToVarIssue2725_1 {
	public function new() {
		var v = null;
		var v = null == v;
		var v1:Bool = v;
	}
}