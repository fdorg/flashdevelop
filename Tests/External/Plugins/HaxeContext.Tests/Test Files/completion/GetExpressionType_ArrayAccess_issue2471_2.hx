package;
public class GetExpressionType_ArrayAccess_issue2471_2 {
	public function foo() {
		var a:Array<String->String>;
		a[0].$(EntryPoint)
	}
}