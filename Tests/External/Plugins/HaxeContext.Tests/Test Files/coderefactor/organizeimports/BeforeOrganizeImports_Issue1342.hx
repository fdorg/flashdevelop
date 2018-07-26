package;
import flash.display.Sprite;
import flash.display.DisplayObject;
import flash.display.DisplayObjectContainer;
import haxe.ds.Either;
import haxe.Timer;
import haxe.ds.List;

class BeforeOrganizeImports extends Sprite {
	public function new() {
		super();
		var imports = [
			flash.display.Sprite,
			flash.display.DisplayObject,
			flash.display.DisplayObjectContainer,
			haxe.ds.Either,
			haxe.Timer,
			haxe.ds.List,
		];
	}
}