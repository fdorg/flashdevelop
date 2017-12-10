package;
class Issue1814 {
	public static function main() {
		function test():Bool ~/\s*}\s*$/.match("}");
		trace(test());
	}

}