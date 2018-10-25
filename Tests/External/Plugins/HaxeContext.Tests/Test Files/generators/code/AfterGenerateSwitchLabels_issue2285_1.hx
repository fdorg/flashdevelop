package;
public class Main {
	public function new(value:EFoo) {
		switch(value) {
			case Foo: 
			case Bar:
		}
	}
}

@:enum abstract EFoo(Int) {
	var Foo = 0;
	var Bar = 1;
}