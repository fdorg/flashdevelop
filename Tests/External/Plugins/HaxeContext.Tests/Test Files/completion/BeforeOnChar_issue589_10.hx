package;
class Issue589_10 {
	function foo(v:EType) {
		switch v {
			case Node(1 | $(EntryPoint)):
			case _:
		}
	}
}

enum EType {
	Node(v);
}