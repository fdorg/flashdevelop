package org.flashdevelop.test.haxe.generator.extractlocalvariable;
import flash.display.Sprite;

class ExtractLocalVariable extends Sprite {
	public function extractLocalVariable() {
		var name = getChildAt($(EntryPoint)0$(ExitPoint)).name;
	}
}