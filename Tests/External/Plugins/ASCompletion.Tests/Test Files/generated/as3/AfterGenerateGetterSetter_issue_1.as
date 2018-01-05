package {
	public class GenerateGetterSetter {
		
		public function get foo():String {
			return _foo
		}
		
		public function set foo(value:String):void {
			_foo = value;
		}
		
		public function get bar():* {
			return null;
		}
		
		private var _foo:String;
	}
}