package;
class BeforeAssignStatementToVar_ParameterizedFunction_issue2510_2 {
	public function new() {
		PIssue2510_2.test("string").charAt;$(EntryPoint)
	}
}

private class PIssue2510_2 {
	public static function test<T>(v:T):T {
		return v;
	}
}