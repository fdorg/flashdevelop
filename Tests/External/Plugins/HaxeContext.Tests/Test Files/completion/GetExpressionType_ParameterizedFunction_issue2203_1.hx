package;
public class GetExpressionType_ParameterizedFunction_issue2203_1 {
	public static function foo<T>(v:T):T {
		foo("string").$(EntryPoint)
	}
}