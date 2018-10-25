package;
class CalltipDef_issue2364_7 {
	static function main() {
		foo(1, bar({x:3$(EntryPoint)}, 4));
	}
	static function foo(v1, v2) {}
	static function bar(v1, v2) {}
}