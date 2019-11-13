package;
class Main implements IFoo {
	public function new() {
	}
	
	
	/* INTERFACE IFoo */
	
	public var bar:Dynamic;
}

interface IFoo extends IBar {
}

interface IBar {
	var bar:Dynamic;
}