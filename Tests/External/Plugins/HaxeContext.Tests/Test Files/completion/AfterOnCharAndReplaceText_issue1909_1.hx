package;
class Issue1909_1 {
	public function new(v:AIssue1909_1) {
		v.test
	}
}

abstract AIssue1909_1<T>(Array<T>) {
	public function new() this = [];
	public function test() {}
}