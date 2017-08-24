package {
	public class Main {
		public function Main() {
			foo(test());
		}
		
		private function test():* {
			return undefined;
		}
		
		private function foo(i:*):void {}
	}
}