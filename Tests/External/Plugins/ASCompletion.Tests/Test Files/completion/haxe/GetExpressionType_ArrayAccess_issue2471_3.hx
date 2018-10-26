package;
public class GetExpressionType_ArrayAccess_issue2471_3 {
	public function foo() {
		var a:Array<Array<String->String>>;
		a[0].$(EntryPoint)
	}
}