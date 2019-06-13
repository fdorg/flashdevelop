package generatortest {
	public class ImplementTest{
		
		public function publicMember():void
		{
		}
		
		
		/* INTERFACE ITest */
		
		public function get getter():String {
			return _getter;
		}
		
		public function set setter(value:String):void {
			_setter = value;
		}
		
		public function testMethod():Number {
			
		}
		
		public function testMethodArgs(arg:Number, arg2:Boolean):int {
			
		}
		
		private function privateMember():String
		{
		}
	}
}