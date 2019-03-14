SWC notes:

1. In order to use a Haxe-compiled SWC in AS3, you will first need to initialize the Haxe system before doing anything else, by calling:

haxe.initSwc(mc);

Example:

SwcMain.hx:
=8<======================
package;

class SwcMain
{
	static function main()
	{
		trace("swc::init");
	}

	static function foo()
	{
		trace("swc::foo");
	}
}
=>8======================

Main.as3:
=8<======================
package
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	public class Main extends MovieClip
	{
		public function Main()
		{
			trace("app::Main");
			haxe.initSwc(this);
			SwcMain.foo();
		}
	}
}
=>8======================

Output:
[Starting debug session with FDB]
app::Main
SwcMain.hx:12: swc::init
SwcMain.hx:17: swc::foo

2. There are issues with SWC written in Haxe version 4p5 or above. But SWC should work with Haxe 3.4.7.