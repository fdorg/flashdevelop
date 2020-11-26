package;
class BeforeAssignStatementToVar_ParameterizedFunction_case1 {
	static function foo<T>(v:T):Array<T> return [v];
	var stringValue = "string";
	public function new() {
		foo(stringValue);$(EntryPoint)
	}
}