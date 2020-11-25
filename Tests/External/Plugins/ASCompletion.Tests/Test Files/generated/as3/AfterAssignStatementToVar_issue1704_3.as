package {
	import flash.display.Sprite;
	public class Main {
		public function Main() {
			var f:Array/*Sprite*/ = foo()
		}

		public function foo():Array/*flash.display.Sprite*/ { return [new Sprite()]; }
	}
}