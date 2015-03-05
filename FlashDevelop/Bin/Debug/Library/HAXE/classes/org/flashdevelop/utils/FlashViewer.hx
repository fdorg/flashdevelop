package org.flashdevelop.utils;

import haxe.Log;
import haxe.PosInfos;
import flash.Lib;

/**
* Connects a flash movie from the ActiveX component to the FlashDevelop program.
* @author Mika Palmu
* @author Franco Ponticelli (haXe version)
* @version 1.1
*/
class FlashViewer 
{
	/**
	* Public properties of the class.
	*/
	public static var limit:Int = 1000;

	/**
	* Private properties of the class.
	*/
	private static var counter:Int = 0;
	private static var aborted:Bool = false;

	/**
	* Sends a trace message to the ActiveX component.
	*/
	public static function trace(value:Dynamic, level:Int = 1):Void
	{
		counter++;
		if (counter > limit && !aborted) 
		{
			aborted = true;
			var msg:String = "FlashViewer aborted. You have reached the limit of maximum messages.";
			Lib.fscommand("trace", "3:" + msg);
		}
		if (!aborted) Lib.fscommand("trace", level + ":" + Std.string(value));
	}

	/**
	* Sends a trace message to the ActiveX component, MTASC style.
	*/
	public static function mtrace(value:Dynamic, method:String, path:String, line:Int):Void
	{
		var fixed:String = path.split("/").join("\\");
		var formatted:String = fixed + ":" + line + ":" + value;
		FlashViewer.trace(formatted, TraceLevel.DEBUG);
	}

	/**
	* Sends a trace message to the ActiveX component, AS3 style.
	*/
	public static function atrace(?arg:String, ?args:Array<String>):Void
	{
		if (args == null) args = [];
		if (arg != null) args.insert(0, arg);
		var result:String = args.join(",");
		FlashViewer.trace(result, TraceLevel.DEBUG);
	}

	/**
	* Redirects the standard trace function of haxe to use FlashConnect
	*/
	public static function redirect():Void
	{
		Log.trace = function(v:Dynamic, ?infos:PosInfos):Void
		{
			if (infos == null) FlashConnect.trace(Std.string(v));
			else FlashConnect.trace(infos.fileName + ":" + infos.lineNumber + ": " + Std.string(v));
		};
	}
	
}
