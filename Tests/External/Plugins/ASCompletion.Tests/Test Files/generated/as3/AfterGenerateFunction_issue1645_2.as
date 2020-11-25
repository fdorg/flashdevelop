package {
	public class Main {
		public function Main() {
			f1(f3());
		}
		
		private function f3():Function {
			return null;
		}
		
		private function f1(f:Function):void {}
	}
}