package;
public class GetExpressionType_ParameterizedFunction_issue2203_3 {
	public static function foo<T0, TResult, T1>(i:Int, v:TResult, ?a, ?b):TResult {
		foo( 10 , 1.0 ).$(EntryPoint)
	}
}