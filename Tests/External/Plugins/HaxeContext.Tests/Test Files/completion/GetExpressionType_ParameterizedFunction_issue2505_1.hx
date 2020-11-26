package;
public class GetExpressionType_ParameterizedFunction_issue2505_1 {
	static function foo<T:String>(v:T) {
		v.$(EntryPoint)
	}
}