package;
class Issue1814 {
	public static function main() {
		function test():{v:Bool} ~/\s*}\s*$/.match("}") ? return {v:true} : return {v:false};
		trace(test());
	}

}