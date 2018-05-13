package;
class EFoo {
	public function new() {
		var v = "1"
			.charAt(0)
			.toLowerCase()
			.split("")
			.filter(function(it) return it != null)
			.map(function(it) return Std.parseInt(it))
			.length;
		v;$(EntryPoint)
	}
}