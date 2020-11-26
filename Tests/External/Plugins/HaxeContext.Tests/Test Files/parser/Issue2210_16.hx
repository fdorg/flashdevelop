package;
class Issue2210_1 {
	function foo1(v:Bool):Int return switch(v) {
		case true: 1;
		case false: 2;
	}
	function foo2(v:Bool):Int return switch(v) {
		case true: 1;
		case false: 2;
	}
}