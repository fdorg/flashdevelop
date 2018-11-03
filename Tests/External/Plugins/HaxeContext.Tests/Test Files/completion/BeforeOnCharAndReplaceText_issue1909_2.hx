package;
class Issue1909_2 {
	public function new(v:AIssue1909_2) {
		v.$(EntryPoint)
	}
}

@:forward
abstract AIssue1909_2<T>(Array<T>) {
	public function new() this = [];
	public function test() {}
}