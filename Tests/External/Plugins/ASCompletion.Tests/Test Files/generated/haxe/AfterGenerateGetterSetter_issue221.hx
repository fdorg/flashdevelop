package;
class GenerateGetterSetter_issue221 {
	var _foo(get, set):String;
	
	function get__foo():String {
		return _foo;
	}
	
	function set__foo(value:String):String {
		return _foo = value;
	}
}