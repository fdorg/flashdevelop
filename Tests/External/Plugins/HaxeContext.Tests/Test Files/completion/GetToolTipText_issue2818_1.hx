package;
class Issue2818_1 {
	function foo() {
		var v:AInt;
		var r = v +$(EntryPoint) 1;
	}
}
private abstract AInt(Int) {
	@:op(A+B)
	public static inline function plus(a:AType, b:Int):AType {
		return a + 1;
	}
}