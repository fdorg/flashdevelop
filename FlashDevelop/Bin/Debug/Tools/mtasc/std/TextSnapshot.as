intrinsic class TextSnapshot
{
	function findText(startIndex:Number, textToFind:String, caseSensitive:Boolean):Number;
	function getCount():Number;
	function getSelected(start:Number, end:Number):Boolean;
	function getSelectedText(includeLineEndings:Boolean):String;
	function getText(start:Number, end:Number, includeLineEndings:Boolean):String;
	function hitTestTextNearPos(x:Number, y:Number, closeDist:Number):Number;
	function setSelectColor(color:Number):Void;
	function setSelected(start:Number, end:Number, select:Boolean):Void;

	function getTextRunInfo(beginIndex:Number, endIndex:Number):Array;
}


