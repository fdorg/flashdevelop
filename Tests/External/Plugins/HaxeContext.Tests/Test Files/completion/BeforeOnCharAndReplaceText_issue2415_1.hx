package;
class Issue2415_1 {
	static function main() {
		function test(v:Void->String):String {
			return v();
		}
		test(function() {
			return "test";
		}).$(EntryPoint)
	}
}