package;
class Issue2804_1 {
	function foo() {
		var v:AInt;
		switch(v) {
			case First$(EntryPoint):
			case _:
		}
	}
}
@:enum private abstract AInt(Int) {
	var Zero = 0;
	var First = 1;
}