package;
class Issue2538_2 {
    static function main() {
		var v = new SubClass2538_2();
		@:privateAccess v.a
    }
}
class SubClass2538_2 {
    var a;
	public var b;
}