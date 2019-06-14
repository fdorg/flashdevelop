package;
public class GetExpressionType_ParameterizedFunction_issue2499_2 {
	public static function foo<T>(v:Dynamic<T>):T {
		foo(String).$(EntryPoint)
	}
}