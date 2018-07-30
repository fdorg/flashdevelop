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

@:enum abstract EFoo(Int) {
	var Foo = 0;
	var Bar = 1;
}