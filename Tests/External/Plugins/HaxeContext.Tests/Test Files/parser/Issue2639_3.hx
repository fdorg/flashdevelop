package;
import haxe.extern.Rest;
import haxe.macro.Expr;
import haxe.macro.Context;
import haxe.macro.Compiler;
using StringTools;

class Issue2639_1 {
	#if display
	public static var addFormat():StringBuf->String->Rest<Void->
	#else
	public static macro function addFormat(buf:ExprOf<StringBuf>, _fmt:ExprOf<String>, rest:Array<Expr>) {
		var cp = Context.currentPos();
		var fmt:String = switch (_fmt.expr) {
			case EConst(CString(s)): s;
			default: throw Context.error("Expected a constant string for format", cp);
		};
		var block:Array<Expr> = [];
		var index = 0;
		var pos = 0;
		var start = 0;
		var len = fmt.length;
		inline function flush(p:Int) {
			block.push(macro @:pos(cp) $buf.add($v{fmt.substr(start, num)}));
		}
		while (pos < len) {
			if (fmt.fastCodeAt(pos++) != "%".code) continue;
			var inc = true;
			var arg = rest[index++];
			flush(pos - 1);
			switch (fmt.fastCodeAt(pos++)) {
				case "s".code: block.push(macro @:pos(cp) StringBufTools.addString($buf, $arg));
				case "d".code: block.push(macro @:pos(cp) StringBufTools.addInt($buf, $arg));
				case "c".code: block.push(macro @:pos(cp) $buf.addChar($arg));
				case "%".code: block.push(macro @:pos(cp) $buf.addChar("%".code));
				default: throw Context.error("Unknown format %"
					+ String.fromCharCode(fmt.fastCodeAt(pos - 1)), cp);
			}
			start = pos;
		}
		if (index < rest.length) throw Context.error("Unused arguments", cp);
		flush(len);
		return { pos: cp, expr: EBlock(block) };
	}
	#end
}