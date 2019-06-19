package;
class Issue2804_2 {
	function foo() {
		var v:AInt;
		switch(v) {
			case First$(EntryPoint):
			case _:
		}
	}
}
enum abstract AInt(Int) {
	var Zero;
	var First;
}