package;
class CalltipDef_issue2364_1 {
	static function main() {
		foo(1, 2 * (3 +$(EntryPoint) 4));
	}
	static function foo(v1, v2) {}
}