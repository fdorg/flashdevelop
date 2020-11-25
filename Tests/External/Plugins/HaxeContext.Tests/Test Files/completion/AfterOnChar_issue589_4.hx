package;
class Issue589_4 {
	function foo(v:EIssue589_4) {
		switch(v) {
			case One:
			default 
		}
	}
}

@:enum abstract EIssue589_4(Int) {
	var One = 1;
	var Two = 2;
}