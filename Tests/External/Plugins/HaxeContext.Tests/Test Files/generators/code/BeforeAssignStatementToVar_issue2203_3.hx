package;
class Issue2203_3 {
	static function foo<T>(v:T):T return v;
	static function main() {
		var a:Array<Array<String->String>>;
		var v = foo(a);
		v;$(EntryPoint)
	}
}