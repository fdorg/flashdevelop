package;
class Issue1814 {
	function test() {
		var rx = ~/\bclass\b[^{]*{\s*\n/;
		return rx.match(text) ? ~/\s*}\s*$/.replace(rx.matchedRight(), '\n') : 'Error\n';
	}
	
	
	
}