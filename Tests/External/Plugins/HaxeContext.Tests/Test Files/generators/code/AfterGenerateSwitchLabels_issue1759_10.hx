package;
public class Main {
	public function new() {
		var m:Main;
		switch(m.foo()) {
			case Foo: 
			case Bar:
		}
	}
	
	function foo(v):EFoo {
		return Bar;
	}
}

enum EFoo {
	Foo;
	Bar;
}