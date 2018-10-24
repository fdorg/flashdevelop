package;
class BeforeOnCharAndReplaceText_issue589_3 {
	function foo() {
		var v = true;
		switch v {
			case true:
			case false:
		}

		var v = One;
		switch v {
			case One
		}
	}
}

@:enum abstract ABeforeOnCharAndReplaceText_issue589_3(Int) {
	var One = 1;
	var Two = 2;
}