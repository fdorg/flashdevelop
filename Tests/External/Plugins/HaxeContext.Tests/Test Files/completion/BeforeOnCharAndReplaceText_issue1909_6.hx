package;
class Issue1909_6 {
	public function new() {
		AIssue1909_6.$(EntryPoint)
	}
}

private class CustomType {
	public static function a() {}
}

@:forwardStatics(a)
abstract AIssue1909_6(CustomType) {
	public function new() this = "";
	public static function test() {}
}