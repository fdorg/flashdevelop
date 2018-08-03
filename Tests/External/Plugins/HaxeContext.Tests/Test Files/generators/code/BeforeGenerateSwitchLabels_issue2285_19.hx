package;
public class Main {
	function foo(v$(EntryPoint):EFoo):EFoo {
		return Bar;
	}
}

@:enum abstract EFoo(Int) {
	var Foo = 0;
	var Bar = 1;
}