package {
	public class Issue103_1 {
		public function Main() {
			foo(test());
		}
		
		private function test():int {
			return 0;
		}
		
		private function foo(i:int):void {}
	}
}