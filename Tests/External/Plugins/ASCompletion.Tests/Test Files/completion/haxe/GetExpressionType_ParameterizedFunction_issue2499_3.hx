package;
public class GetExpressionType_ParameterizedFunction_issue2499_3 {
	public static function foo<T>(v:Class<T>):T {
		foo(String).$(EntryPoint)
	}
}