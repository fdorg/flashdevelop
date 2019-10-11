package;
public class GetExpressionType_ParameterizedClass_issue2536_2<T:A2536_2> {
	var v:T;
	function foo() {
		var l = new GetExpressionType_ParameterizedClass_issue2536_2<B2536_2>();
		l.v.$(EntryPoint)
	}
}
class A2536_2 {
	public function a() {}
}
class B2536_2 extends A2536_2 {
	public function b() {}
}