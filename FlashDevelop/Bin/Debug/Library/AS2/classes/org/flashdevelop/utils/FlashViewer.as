/**
* Connects a flash movie from the ActiveX component to the FlashDevelop program.
* @author Mika Palmu
* @version 1.2
*/

class org.flashdevelop.utils.FlashViewer
{
	/**
	* Public properties of the class.
	*/
	public static var limit:Number = 1000;
	
	/**
	* Private properties of the class.
	*/
	private static var counter:Number = 0;
	private static var aborted:Boolean = false;
	
	/**
	* Sends a trace message to the ActiveX component.
	*/
	public static function trace(value:Object, level:Number)
	{
		counter++;
		if (counter > limit && !aborted)
		{
			aborted = true;
			var msg:String = new String("FlashViewer aborted. You have reached the limit of maximum messages.");
			fscommand("trace", "3:" + msg);
		} 
		if (!aborted) 
		{
			if (!isNaN(level)) fscommand("trace", level + ":" + value);
			else fscommand("trace", value.toString());
		}
	}
	
	/**
	* Adds compatibility with MTASC's tracing facilities.
	*/
	public static function mtrace(value:Object, method:String, path:String, line:Number):Void 
	{
		if (path.charAt(1) != ":") path = "~/" + path;
		var formatted:String = path + ":" + line + ":" + value;
		FlashViewer.trace(formatted);
	}
	
}
