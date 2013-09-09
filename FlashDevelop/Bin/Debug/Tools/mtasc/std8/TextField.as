dynamic intrinsic class TextField
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

	var autoSize:Object;
	var background:Boolean;
	var backgroundColor:Number;
	var border:Boolean;
	var borderColor:Number;
	var bottomScroll:Number;
	var condenseWhite:Boolean;
	var embedFonts:Boolean;
	var hscroll:Number;
	var html:Boolean;
	var htmlText:String;
	var length:Number;
	var maxChars:Number;
	var maxhscroll:Number;
	var maxscroll:Number;
	var multiline:Boolean;
	var password:Boolean;
	var restrict:String;
	var scroll:Number;
	var selectable:Boolean;
	var tabEnabled:Boolean;
	var tabIndex:Number;
	var text:String;
	var textColor:Number;
	var textHeight:Number;
	var textWidth:Number;
	var type:String;
	var variable:String;
	var wordWrap:Boolean;
	var mouseWheelEnabled:Boolean;

	var styleSheet:TextField.StyleSheet;

	function replaceText(beginIndex:Number,endIndex:Number,newText:String):Void;
	function replaceSel(newText:String):Void;
	function getTextFormat(beginIndex:Number,endIndex:Number):TextFormat;
	function setTextFormat():Void;
	function removeTextField():Void;
	function getNewTextFormat():TextFormat;
	function setNewTextFormat(tf:TextFormat):Void;
	function getDepth():Number;
	function addListener(listener:Object):Boolean;
	function removeListener(listener:Object):Boolean;
	static function getFontList():Array;

	function onChanged(changedField:TextField):Void;
	function onKillFocus(newFocus:Object):Void;
	function onScroller(scrolledField:TextField):Void;
	function onSetFocus(oldFocus:Object):Void;

	// Flash 8

	var antiAliasType : String;
	var filters : Array;
	var gridFitType : String;
	var sharpness : Number;
	var thickness : Number;

}
