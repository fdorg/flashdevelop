package;
interface IInterfaceIssue2553_1 {
	function test1<T>(v:T):T;
	function test2<T>(v:T):T;
}

class Issue2553_1 implements IInterfaceIssue2553_1 {
	function test1<T>(v:T):T return null;
	
	
	/* INTERFACE IInterfaceIssue2553_1 */
	
	public function test2<T>(v:T):T {
		return null;
	}
}