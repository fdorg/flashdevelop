package;
public class GetExpressionType_ParameterizedClass_issue2536_3<T:A2536_3> {
	var v:T;
	function foo() {
		var l = new GetExpressionType_ParameterizedClass_issue2536_3<B2536_3>();
		trace(l.v);
		v.$(EntryPoint)
	}
}
class A2536_3 {
	public function a() {}
}
class B2536_3 extends A2536_3 {
	public function b() {}
}