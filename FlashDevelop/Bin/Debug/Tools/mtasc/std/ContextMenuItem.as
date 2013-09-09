intrinsic dynamic class ContextMenuItem
{
	var caption:String;
	var separatorBefore:Boolean;
	var enabled:Boolean;
	var visible:Boolean;
	function ContextMenuItem(caption:String, 
			callbackFunction:Function, 
			separatorBefore:Boolean, 
			enabled:Boolean, 
			visible:Boolean);
	function copy():ContextMenuItem;
	function onSelect():Void;
}
