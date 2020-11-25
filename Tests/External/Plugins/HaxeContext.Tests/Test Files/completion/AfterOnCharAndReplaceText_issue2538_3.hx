package;
class Issue2538_3 {
    static function main() {
		var v = new SubClass2538_3_1();
		@:privateAccess v.a1.a2
    }
}
class SubClass2538_3_1 {
    var a1:SubClass2538_3_2;
}
class SubClass2538_3_2 {
    var a2;
}