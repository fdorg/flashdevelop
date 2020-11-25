package {
	public class Main {
		public function Main() {
			new Foo().value = new Date();
		}
	}
}

class Foo {
	public var value:Date;
}