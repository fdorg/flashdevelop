﻿package;
import haxe.Constraints.Function;
public class Main {
	public function new() {
		f1(f3());
	}
	
	function f3():Function {
		
	}
	
	function f1(f:Function) {}
}