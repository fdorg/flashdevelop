package somepackage;

class AClass
{
	static function main()
	{
		switch ("Haxe")
		{
			case "a":
				trace("a"); trace("another statement");
			case "b":
			{
				trace("b");
			}
		}

		if (true 
			&& false)
			trace("Inconsistent system");
		
		var a="";
		var b = "";
		
		if(true)
		{ trace("strange code"); }
		else if (true) {
			
		}
		else
			trace("test");
			
		trace ("random trace");
		
		#if example
		trace("an example");
		#end
	}
	
	function aFunction(arg1 : String,arg2 : Int) {
		
	}
	
}