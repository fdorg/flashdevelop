intrinsic dynamic class ContextMenu
{
	var customItems:Array;
	var builtInItems:Object;

	function ContextMenu(callbackFunction:Function);	
	function copy():ContextMenu;
	function hideBuiltInItems():Void;
	function onSelect():Void;
}


