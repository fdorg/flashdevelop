package {
	public class GenerateGetterSetter {
		private var _foo:String;
		
		public function get foo():String {
			return _foo;
		}
		
		public function set foo(value:String):void {
			_foo = value;
		}
	}
}