package {
	public class Main {
		public function Main() {
			foo(test());
		}
		
		private function test():Boolean {
			return false;
		}
		
		private function foo(i:Boolean):void {}
	}
}