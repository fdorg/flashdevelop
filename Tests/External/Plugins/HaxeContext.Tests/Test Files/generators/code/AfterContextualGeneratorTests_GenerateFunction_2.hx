package;
class EFoo {
	public function new() {
		foo([
			[1, 2, 3, 4],
			"12345"
				.charAt(Math.random() * 5)
				.split("")
				.concat(["1", "2", "/", "\\"])
				.toString(),
			(v:String)
		], 1, "123", {x:Int, y:Int});
	}
	
	function foo(array:Array<T>, float:Float, string:String, dynamicValue:Dynamic) {
		
	}
}