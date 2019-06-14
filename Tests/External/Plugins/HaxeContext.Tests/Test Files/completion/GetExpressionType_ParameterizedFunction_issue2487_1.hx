package;
public class GetExpressionType_ParameterizedFunction_issue2487_1 {
	public static function foo<T>(v:T):Array<T> {
		foo("")[0].$(EntryPoint)
	}
}