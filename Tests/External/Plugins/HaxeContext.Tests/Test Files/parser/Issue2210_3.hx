package;
class Issue2210_3 {
	function foo(i:Int):Int 
		return i % 2 == 0
			? 0
			: 1;
}