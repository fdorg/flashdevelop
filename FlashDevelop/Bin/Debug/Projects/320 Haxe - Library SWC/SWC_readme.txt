SWC notes:

1. In order to use a Haxe-compiled SWC in AS3, you will first need to initialize the Haxe system before doing anything else, by calling:

haxe.initSwc(mc);

Example:
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

2. There are issues with SWC written in Haxe version 4p5 or above. But SWC should work with Haxe 3.4.7.