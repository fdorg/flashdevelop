package;
class Issue589_13 {
	function foo(v:AType) {
		switch(v) {
			case One, Two, $(EntryPoint)
		}
	}
}

@:enum abstract AType(Int) {
	var One = 1;
	var Two = 2;
}