package test.test;

interface Test
{
	var testVar:String;
	function test(?arg:Array<Dynamic>):Int;
	function test2(arg:Bool):Void;
	private function testPrivate():Int;
	var testProperty(get, set):Float;
}