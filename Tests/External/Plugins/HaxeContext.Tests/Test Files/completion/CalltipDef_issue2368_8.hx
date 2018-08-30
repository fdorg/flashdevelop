package;
class CalltipDef_issue2364_8 {
	static function main() {
		foo(1, bar_({x:3$(EntryPoint)}, 4));
	}
	static function foo(v1, v2) {}
	static function bar_(v1, v2) {}
}