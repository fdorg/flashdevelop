package;
class Issue3027_1 {
	function foo() {
		var programdata = env["ProgramData"];
		var runtimes = '$programdata/$app/cache/runtimes$(EntryPoint)
		return true;
	}
}