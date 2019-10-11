package;
public class GetExpressionType_ParameterizedFunction_issue2505_2 {
	static function foo<T:String>():T {
		var v:T;
		v.$(EntryPoint)
		return v;
	}
}