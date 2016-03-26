package org.flashdevelop.test.as3.generator.extractlocalvariable {
	import flash.display.DisplayObject;
	import flash.display.Sprite;

	public class ExtractLocalVariable extends Sprite {
		public function ExtractLocalVariable() {
			var newVar:DisplayObject = getChildByName("child");
			var name:String = newVar.name;
		}
	}
}