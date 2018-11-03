package;
class Issue1909_8 {
	public function new(v:AIssue1909_8<String>) {
		v.$(EntryPoint)
	}
}

@:forward(push, shift)
abstract AIssue1909_8<T>(Array<T>) {
	public function new() this = [];
}