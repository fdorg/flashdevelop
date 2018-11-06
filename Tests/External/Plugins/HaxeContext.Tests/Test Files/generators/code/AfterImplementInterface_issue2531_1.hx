package;
interface IInterfaceIssue2531_1<T> {
	function test(v:T):T;
}

class Issue2531_1 implements IInterfaceIssue2531_1<String> {
	
	/* INTERFACE IInterfaceIssue2531_1<T> */
	
	public function test(v:String):String {
		return null;
	}
}