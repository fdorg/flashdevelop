package {
	public class ParseFunctionParameters_ArrayAccess {
		public function ParseFunctionParameters_ArrayAccess() {
			var v:Vector.<Vector.<String>> = new <Vector.<String>>[new <String>[""]];
			foo$(EntryPoint)(v[0][0].length);
		}
	}
}