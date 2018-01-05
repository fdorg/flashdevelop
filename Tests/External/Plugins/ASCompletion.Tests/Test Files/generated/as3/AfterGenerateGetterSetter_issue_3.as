package {
	public class GenerateGetterSetter {
		
		public function set bar(value:String):void {
		}
		
		public function get foo():String {
			return _foo;
		}
		
		public function set foo(value:String):void {
			_foo = value;
		}
		
		private var _foo:String;
	}
}