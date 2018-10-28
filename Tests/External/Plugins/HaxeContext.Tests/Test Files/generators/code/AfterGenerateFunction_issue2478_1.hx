package;
class Issue2478_1 {
	static function test() {
		var a:Array<String->String>;
		foo(a[0]());
	}
	
	static function foo(item:String) {
		
	}
}