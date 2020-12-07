package;
private class AssignStatementToVarIssue2725_5 {
	public function new() {
		var v = 1.0;
		var v = ++v;
		var v1:Float = v;
	}
}