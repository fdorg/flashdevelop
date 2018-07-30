package;
typedef Foo = {
	function foo():Void;
}

typedef Bar = {>Foo
	override $(EntryPoint)    
}