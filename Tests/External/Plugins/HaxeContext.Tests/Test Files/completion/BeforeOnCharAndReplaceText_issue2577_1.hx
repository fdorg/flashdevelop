package;
class Issue2577_1 {
	macro static function foo() {
		return macro {
			$$(EntryPoint)
		}
	}
}