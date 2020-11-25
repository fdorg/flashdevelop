package;
class Issue2203_2 {
	static function foo<T>(v:T):T return v;
	static function main() {
		var a:Array<String->String>;
		foo(a);
		var a:Array<Array<String->String>>;
		foo(a);$(EntryPoint)
	}
}