package;
class CalltipDef_issue2364_4 {
	static function main() {
		foo((1 +$(EntryPoint) 1), 2);
	}
	static function foo(v1, v2) {}
}