package;
class CalltipDef_issue2364_10 {
	static function main() {
		foo(1, (2 + ((3 +$(EntryPoint) 1) * 2)));
	}
	static function foo(v1, v2) {}
}