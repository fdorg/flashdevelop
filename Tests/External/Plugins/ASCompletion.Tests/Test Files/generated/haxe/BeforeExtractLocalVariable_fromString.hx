package org.flashdevelop.test.haxe.generator.extractlocalvariable {
	import flash.display.Sprite;

	public class ExtractLocalVariable extends Sprite {
		public function extractLocalVariable() {
			var name = getChildByName($(EntryPoint)"child"$(ExitPoint)).name;
		}
	}
}