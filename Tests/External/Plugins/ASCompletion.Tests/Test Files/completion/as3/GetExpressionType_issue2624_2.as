package {
	public class Issue2624_2 extends InternalIssue2624_2 {
		public static function foo() : void {
			super.$(EntryPoint)
		}
	}
}
class InternalIssue2624_2 {
	public var a : *;
}