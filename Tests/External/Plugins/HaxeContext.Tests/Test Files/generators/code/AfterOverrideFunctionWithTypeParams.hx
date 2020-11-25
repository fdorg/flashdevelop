package;
class Foo {
	public function new() {}
	public function foo<K:ISomething, V:(Iterable<String>, Measurable)>(arg1:K, arg2:V):K {}
}

class Bar extends Foo {
	public override function foo<K:ISomething,V:(Iterable<String>,Measurable)>(arg1:K, arg2:V):K {
		return super.foo(arg1, arg2);
	}
}