intrinsic class TextField.StyleSheet
{
	function getStyle(name:String):Object;
	function setStyle(name:String,style:Object):Void;
	function clear():Void;
	function getStyleNames():Array;
	function transform(style:Object):TextFormat;
	function parseCSS(cssText:String):Boolean;
	function parse(cssText:String):Boolean;
	function load(url:String):Boolean;
	function onLoad(success:Boolean):Void;
}
