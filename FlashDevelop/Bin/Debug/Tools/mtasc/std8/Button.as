intrinsic class Button
{
	var _x:Number;
	var _y:Number;
	var _xmouse:Number;
	var _ymouse:Number;
	var _xscale:Number;
	var _yscale:Number;
	var _width:Number;
	var _height:Number;
	var _alpha:Number;
	var _visible:Boolean;
	var _target:String;
	var _rotation:Number;
	var _name:String;
	var _framesloaded:Number;
	var _droptarget:String;
	var _currentframe:Number;
	var _totalframes:Number;
	var _quality:String;
	var _focusrect:Boolean;
	var _soundbuftime:Number;
	var _url:String;
	var _parent:MovieClip;

	var useHandCursor:Boolean;
	var enabled:Boolean;
	var tabEnabled:Boolean;
	var tabIndex:Number;
	var trackAsMenu:Boolean;
	var menu:ContextMenu;

	function getDepth():Number;

	function onDragOut():Void;
	function onDragOver():Void;
	function onKillFocus(newFocus:Object):Void;
	function onPress():Void;
	function onRelease():Void;
	function onReleaseOutside():Void;
	function onRollOut():Void;
	function onRollOver():Void;
	function onSetFocus(oldFocus:Object):Void;
	function onKeyDown():Void;
	function onKeyUp():Void;

	// Flash 8

	var scale9Grid : flash.geom.Rectangle;
	var filters : Array;
	var cacheAsBitmap : Boolean;
	var blendMode : String;
}
