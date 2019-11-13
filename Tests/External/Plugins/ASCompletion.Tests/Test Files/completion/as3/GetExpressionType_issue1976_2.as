package {
	public class Foo {
		public function foo():void {
		}
		public function self():Foo {}
		public function test():void {
			var f:Foo = this
			f.self().fo$(EntryPoint)o
		}
	}
}