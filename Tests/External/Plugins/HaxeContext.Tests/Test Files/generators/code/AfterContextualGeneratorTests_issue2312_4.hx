package;
class EFoo {
	var args:Dynamic;
	public function new(args = /*AbstractIssue2312_3.Value*/) {
		this.args = args;
		
	}
}

@:enum abstract AbstractIssue2312_3(String) {
	var Value = "2312_3";
}