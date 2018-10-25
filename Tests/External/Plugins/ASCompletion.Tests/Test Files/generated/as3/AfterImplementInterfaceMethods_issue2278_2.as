package {
	public class Main implements IFoo {
		public function Main() {
		}
		
		
		/* INTERFACE IFoo */
		
		public function set foo(value:Function/*(v:*):int*/):void {
			_foo = value;
		}
	}
}

interface IFoo {
	function set foo(v:Function/*(v:*):int*/):void;
}