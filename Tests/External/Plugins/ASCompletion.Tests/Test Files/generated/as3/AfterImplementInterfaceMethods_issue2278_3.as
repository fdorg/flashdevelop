package {
	public class Main implements IFoo {
		public function Main() {
		}
		
		
		/* INTERFACE IFoo */
		
		public function foo(v1:Function/*(v:*):int*/, v2:Array/*String*/, v3:Function/*():String*/):void {
			
		}
	}
}

interface IFoo {
	function foo(v1:Function/*(v:*):int*/, v2:Array/*String*/, v3:Function/*():String*/):void;
}