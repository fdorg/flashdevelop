package org.flashdevelop.test.as3.generator.extractlocalvariable {
	import flash.display.Sprite;

	public class ExtractLocalVariable extends Sprite {
		public function ExtractLocalVariable() {
			var name:String = getChildByName($(EntryPoint)"child"$(ExitPoint)).name;
		}
	}
}