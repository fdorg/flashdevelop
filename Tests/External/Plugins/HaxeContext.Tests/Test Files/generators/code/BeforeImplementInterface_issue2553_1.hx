package;
interface IInterfaceIssue2553_1 {
	function test1<T>(v:T):T;
	function test2<T>(v:T):T;
}

class Issue2553_1 implements IInterfaceIssue$(EntryPoint)2553_1 {
	function test1<T>(v:T):T return null;
}