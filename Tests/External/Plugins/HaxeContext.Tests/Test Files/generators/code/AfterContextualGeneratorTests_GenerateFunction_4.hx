package;
class EFoo {
	public function new() {
		foo(this.bar().x);
	}
	
	function foo(dynamicValue:Dynamic):Void {
		
	}
	function bar() {
		return {x:10};
	}
}