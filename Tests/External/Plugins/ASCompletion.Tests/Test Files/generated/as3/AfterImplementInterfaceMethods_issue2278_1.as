package {
	public class Main implements IFoo {
		public function Main() {
		}
		
		
		/* INTERFACE IFoo */
		
		public function get foo():Function/*(v:*):int*/ {
			return _foo;
		}
	}
}

interface IFoo {
	function get foo():Function/*(v:*):int*/;
}