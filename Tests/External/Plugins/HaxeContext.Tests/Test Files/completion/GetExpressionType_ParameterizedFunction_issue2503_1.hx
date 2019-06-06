package;
public class GetExpressionType_ParameterizedFunction_issue2503_1 {
	public static function main() {
		var a:Array<String>;
		a[AType.One].$(EntryPoint)
	}
}

@:enum abstract AType(Int) {
	var One = 1;
	var Two = 2;
}