package org.flashdevelop.test.haxe.generator.extractlocalvariable;

class ExtractLocalVariable {
	public function extractLocalVariable() {
		for(i in $(EntryPoint)0$(ExitPoint)...1)
		{
			trace(i);
		}
	}
}