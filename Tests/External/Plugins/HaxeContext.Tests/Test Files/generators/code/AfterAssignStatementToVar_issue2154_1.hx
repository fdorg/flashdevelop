package;
import haxe.macro.Expr;
class Foo {
	macro public static function dispose(v:haxe.macro.Expr):haxe.macro.Expr {
		var v1:Expr = $v;
	}
}