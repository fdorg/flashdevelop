package generatortest {
	public class FieldFromParameterTest{
		public var arg;
		public function FieldFromParameterTest(arg:String){
			super(function():void {
				test();
			});
			this.arg = arg;
			
		}
	}
}