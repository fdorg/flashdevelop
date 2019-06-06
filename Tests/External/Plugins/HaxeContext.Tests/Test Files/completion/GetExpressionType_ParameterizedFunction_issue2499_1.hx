package;
public class GetExpressionType_ParameterizedFunction_issue2499_1 {
	public static function foo<T>(v:Null<T>):T {
		foo(String).$(EntryPoint)
	}
}