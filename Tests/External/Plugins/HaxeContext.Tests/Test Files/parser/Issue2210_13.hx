package;
class Issue2210_7 {
	function foo(i:Int):Int trace(i)

	macro public static inline function max<T>():ExprOf<T> {
		return macro {
			var l = ${f};
			var r = ${s};
			l > r ? l : r;
		};
	}
}