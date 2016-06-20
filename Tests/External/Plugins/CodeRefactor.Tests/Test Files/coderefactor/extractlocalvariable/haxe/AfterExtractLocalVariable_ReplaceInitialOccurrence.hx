package org.flashdevelop.test.haxe.generator.extractlocalvariable {
	import flash.display.Sprite;

	public class ExtractLocalVariable extends Sprite {
		public function extractLocalVariable() {
		    // ... some code here ...
			var alpha = getChildByName("child").alpha;
			// ... some code here ...
			var newVar = getChildByName("child");
			var name = newVar.name;
		}
	}
}