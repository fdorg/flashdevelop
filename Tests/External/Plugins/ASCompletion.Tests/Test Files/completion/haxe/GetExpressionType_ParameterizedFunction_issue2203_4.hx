package;
public class GetExpressionType_ParameterizedFunction_issue2203_4 {
	public static function foo<T0, TResult, T1>(i:Int, v:TResult, ?a, ?b):TResult {
		var localVar = 1.0;
		foo( 10 , localVar ).$(EntryPoint)
	}
}