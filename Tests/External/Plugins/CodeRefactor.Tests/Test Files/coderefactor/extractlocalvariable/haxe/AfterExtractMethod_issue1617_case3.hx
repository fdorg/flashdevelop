package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		var name = newVar()/*some comment*/;
	}
	
	function newVar():Void {
		0
	}
}