package;
public class GetExpressionType_ParameterizedFunction_issue2203_2 {
	public static function foo<T>(i:Int, v:T, ?a, ?b):T {
		foo( 10 , true ).$(EntryPoint)
	}
}