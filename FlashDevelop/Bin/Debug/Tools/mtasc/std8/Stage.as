intrinsic class Stage
{
	static var width:Number;
	static var height:Number;
	static var scaleMode:String;
	static var align:String;
	static var showMenu:Boolean;
	static function addListener(listener:Object):Void;
	static function removeListener(listener:Object):Void;
	
	static var displayState : String;
	static var fullScreenSourceRect : flash.geom.Rectangle;	
	static function onFullScreen( full : Boolean ) : Void;	
}


