package {
	import flash.display.Sprite;
	public class Main {
		public function Main() {
			new Foo().sprite = new Sprite();
		}
	}
}

class Foo {
	public var sprite:Sprite;
}