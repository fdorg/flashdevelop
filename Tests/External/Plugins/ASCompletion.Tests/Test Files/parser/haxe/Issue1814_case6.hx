package;
class Issue1814 {
	function test() {
		var rx = ~/\bclass\b[^{]*{\s*\n/;
		return "string" + ~/\s*}\s*$/.replace(rx.matchedRight(), '\n');
	}
	
	
}