package;
public class GetExpressionType_ParameterizedFunction_issue2499_4 {
	public static function foo<T:{}>(v:Class<T>):T {
		foo(String).$(EntryPoint)
	}
}