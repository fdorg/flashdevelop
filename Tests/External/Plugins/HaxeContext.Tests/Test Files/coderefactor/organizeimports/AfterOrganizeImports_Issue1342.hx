package;
import flash.display.DisplayObject;
import flash.display.DisplayObjectContainer;
import flash.display.Sprite;

import haxe.Timer;

import haxe.ds.Either;
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