package;
public class Main {
	function foo(v:EFoo):EFoo {
		return Bar;
	}

	var v = switch(foo()) {
		case Foo(v): 
		case Bar:
	}
}

enum EFoo {
	Foo(v:EFoo);
	Bar;
}