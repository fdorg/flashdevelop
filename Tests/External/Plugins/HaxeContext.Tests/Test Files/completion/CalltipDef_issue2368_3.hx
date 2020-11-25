package;
class CalltipDef_issue2364_3 {
	static function main() {
		foo((1 + 1)$(EntryPoint), 2);
	}
	static function foo(v1, v2) {}
}