package {
	public class Main {
		public function Main() {
			foo(test());
		}
		
		private function test():uint {
			return 0;
		}
		
		private function foo(i:uint):void {}
	}
}