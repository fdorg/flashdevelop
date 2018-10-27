package;
class Issue2203_1 {
	static function foo<T>(v:T):T return v;
	static function main() {
		var a:Array<String->String>;
		foo(a);$(EntryPoint)
	}
}