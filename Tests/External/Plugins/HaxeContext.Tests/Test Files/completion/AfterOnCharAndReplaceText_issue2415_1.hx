package;
class Issue2415 {
	static function main() {
		function test(v:Void->String):String {
			return v();
		}
		test(function() {
			return "test";
		}).charAt
	}
}