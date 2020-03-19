package;
import flash.display.Proxy;
import haxe.extern.Rest;
class Bar extends Proxy {
	public override function callProperty(name:Dynamic, restArgs:haxe.extern.Rest<Dynamic>):Dynamic {
		return super.callProperty(name, restArgs);
	}
}