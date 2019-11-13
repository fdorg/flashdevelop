package;
class Issue1814 {
	public static function main() {
		var v = try ~/\s*}\s*$/.match("}") catch(e:Dynamic) false;
		trace(v);
	}

}