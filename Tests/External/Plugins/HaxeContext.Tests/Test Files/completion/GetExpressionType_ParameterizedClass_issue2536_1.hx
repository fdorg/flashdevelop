package;
public class GetExpressionType_ParameterizedClass_issue2536_1<T:A2536_1> {
	var v:T;
	function foo() {
		v.$(EntryPoint)
	}
}
class A2536_1 {
	public function a() {}
}