package {
	public class Main {
		public function Main() {
			foo(test());
		}
		
		private function test():Object {
			return null;
		}
		
		private function foo(i:Object):void {}
	}
}