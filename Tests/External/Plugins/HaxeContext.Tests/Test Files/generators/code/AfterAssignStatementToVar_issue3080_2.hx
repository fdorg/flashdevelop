package;
private class AssignStatementToVarIssue3080_2 {
	function foo() {
		var a = [];
		var v = a[0] = v;
		var v1:Dynamic = v;
	}
}