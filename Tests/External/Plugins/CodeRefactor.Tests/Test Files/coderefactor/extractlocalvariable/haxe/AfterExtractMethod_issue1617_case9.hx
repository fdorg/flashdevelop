package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		for(i in newVar()...1)
		{
			trace(i);
		}
	}
	
	function newVar():Void {
		0
	}
}