package;
public class Main {
	function foo(v:EFoo):EFoo {
		return Bar;
	}

	var v = switch(foo()) {
		case Foo: 
		case Bar:
	}
}

@:enum abstract EFoo(Int) {
	var Foo = 0;
	var Bar = 1;
}