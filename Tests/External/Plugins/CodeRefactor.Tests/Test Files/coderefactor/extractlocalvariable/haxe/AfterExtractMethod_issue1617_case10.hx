package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		switch newVar() {
			case 1: trace(1);
			case v: trace(v);
		}
	}
	
	function newVar():Void {
		0
	}
}