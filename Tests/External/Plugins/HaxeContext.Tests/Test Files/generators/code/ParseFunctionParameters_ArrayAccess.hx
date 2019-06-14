package;
class ParseFunctionParameters_ArrayAccess {
	public function new() {
		var a:Array<Array<String>> = [["0"], ["1"]];
		foo$(EntryPoint)(a[0][0].length);
	}
}