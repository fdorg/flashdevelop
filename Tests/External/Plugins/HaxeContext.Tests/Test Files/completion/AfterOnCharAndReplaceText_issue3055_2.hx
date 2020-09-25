package;
class Issue3055_2 extends PrivateClass {
	function foo() {
		this.a;
	}
}

private class PrivateClass {
	var a:String;
}