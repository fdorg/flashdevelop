﻿package {
	public class Main {
		public function Main() {
			foo(test());
		}
		
		private function test():Number {
			return NaN;
		}
		
		private function foo(i:Number):void {}
	}
}