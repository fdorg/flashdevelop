package;
class Issue2771_1 {
	function foo() {
		var v$(EntryPoint) =
		switch(v) {
			case {x:1, y:2}:
				for(var it in ["1", "2"]) {
				}
			default:
				//:nop
		}
	}
}