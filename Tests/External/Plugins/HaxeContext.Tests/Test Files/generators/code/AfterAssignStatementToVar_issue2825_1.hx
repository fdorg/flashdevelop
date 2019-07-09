package;
private class AssignStatementToVarIssue2825_1 {
	public function new() {
		var f:Dynamic = foo();
	}
	function foo() {
		return foo();
	}
}