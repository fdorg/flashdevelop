package;
private class AssignStatementToVarIssue2825_1 {
	public function new() {
		foo();$(EntryPoint)
	}
	function foo() {
		return foo();
	}
}