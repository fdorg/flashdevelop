package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		for(i in 0...newVar()) {
			trace(i);
		}
	}
	
	function newVar():Void {
		1
	}
}