package;
class Issue1814 {
	function test() {
		var rx = ~/\bclass\b[^{]*{\s*\n/;
		return !~/\s*}\s*$/.matched();
	}
	
	
}