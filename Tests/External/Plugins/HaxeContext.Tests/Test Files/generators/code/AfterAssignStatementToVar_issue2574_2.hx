package;
private class AssignStatementToVarIssue2574_2 {
	public static function trace():Int {}
	public function new() {
		var t:Int = AssignStatementToVarIssue2574_2.trace();
	}
}