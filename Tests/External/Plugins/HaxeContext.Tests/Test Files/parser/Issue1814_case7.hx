package;
class Issue1814 {
	function test() {
		var rx = ~/\bclass\b[^{]*{\s*\n/;
		return [1 => ~/\s*}\s*$/.replace(rx.matchedRight(), '\n')];
	}
	
	
}