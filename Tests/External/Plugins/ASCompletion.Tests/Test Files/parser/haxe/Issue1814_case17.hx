package;
class Issue1814 {
	public static function main() {
		switch ~/\s*}\s*$/.match("}") {
			case true: trace(1);
			case false: trace(0);
		}
	}

}