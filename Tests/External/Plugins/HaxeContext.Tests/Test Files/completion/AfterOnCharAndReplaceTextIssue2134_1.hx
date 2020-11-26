package;
import flash.utils.Proxy;
import haxe.extern.Rest;
class Issue2134_1 extends Proxy {
	override public function callProperty(name:Dynamic, restArgs:haxe.extern.Rest<Dynamic>):Dynamic {
		return super.callProperty(name, restArgs);
	}
}