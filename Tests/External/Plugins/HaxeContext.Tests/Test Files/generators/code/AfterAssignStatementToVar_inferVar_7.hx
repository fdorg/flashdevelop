﻿package;
class Foo {
	public function new() {
		function test(v1:Int, v2:Array<String>) return v2[v1]; 
		var a = cast test(0, ["1"]);
		var b = a;
		var v = b;
		for(it in v) {
			var i:Dynamic = it;
		}
	}
}