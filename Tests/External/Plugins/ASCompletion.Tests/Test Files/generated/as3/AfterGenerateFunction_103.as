package {
	public class Main {
		public function Main() {
			foo(test());
		}
		
		private function test():int {
			return 0;
		}
		
		private function foo(i:int):void {}
	}
}