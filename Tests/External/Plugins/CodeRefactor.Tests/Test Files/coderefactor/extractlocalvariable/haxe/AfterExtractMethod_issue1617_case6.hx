package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		var name = [
			newVar(),
			2
		];
	}
	
	function newVar():Void {
		1
	}
}