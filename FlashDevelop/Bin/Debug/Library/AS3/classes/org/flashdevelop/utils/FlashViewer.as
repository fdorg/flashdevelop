package org.flashdevelop.utils
{
	import flash.system.*;
	
	/**
	* Connects a flash movie from the ActiveX component to the FlashDevelop program.
	* @author Mika Palmu
	* @version 1.1
	*/
	public class FlashViewer
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
		public static function trace(value:Object, level:Number = 1):void
		{
			counter++;
			if (counter > limit && !aborted)
			{
				aborted = true;
				var msg:String = "FlashViewer aborted. You have reached the limit of maximum messages.";
				fscommand("trace", "3:" + msg);
			} 
			if (!aborted) fscommand("trace", level + ":" + value);
		}
		
		/**
		* Sends a trace message to the ActiveX component, MTASC style.
		*/
		public static function mtrace(value:Object, method:String, path:String, line:Number):void 
		{
			var fixed:String = path.split("/").join("\\");
			var formatted:String = fixed + ":" + line + ":" + value;
			FlashViewer.trace(formatted, TraceLevel.DEBUG);
		}
		
		/**
		* Sends a trace message to the ActiveX component, AS3 style.
		*/
		public static function atrace(...rest):void
		{
			var result:String = rest.join(",");
			FlashViewer.trace(result, TraceLevel.DEBUG);
		}
		
	}
	
}
