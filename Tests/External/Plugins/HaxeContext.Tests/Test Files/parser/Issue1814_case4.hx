package;
class Issue1814 {
	function test() {
		var rx = ~/\bclass\b[^{]*{\s*\n/;
		var result = ~/\s*}\s*$/.replace(rx.matchedRight(), '\n');
	}
	
	
	
}