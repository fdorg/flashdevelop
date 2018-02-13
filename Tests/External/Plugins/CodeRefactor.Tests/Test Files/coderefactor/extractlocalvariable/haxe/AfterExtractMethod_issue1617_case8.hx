package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		var v = {v:newVar()};
	}
	
	function newVar():Void {
		1
	}
}