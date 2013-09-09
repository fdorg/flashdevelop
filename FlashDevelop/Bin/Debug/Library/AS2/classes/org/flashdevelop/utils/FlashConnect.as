/**
* Connects a flash movie thru XmlSocket to the FlashDevelop program.
* @author Mika Palmu
* @version 3.2
*/

import org.flashdevelop.utils.*;

class org.flashdevelop.utils.FlashConnect 
{
	/**
	* Public properties of the class.
	*/
	public static var status:Number = 0;
	public static var limit:Number = 1000;
	public static var host:String = "127.0.0.1";
	public static var port:Number = 1978;
	
	/**
	* Private properties of the class.
	*/
	private static var messages:Array;
	private static var socket:XMLSocket;
	private static var interval:Number;
	private static var counter:Number;
	
	/**
	* Event callbacks of the class.
	*/
	public static var onConnection:Function;
	public static var onReturnData:Function;
	
	/**
	* Adds a custom message to the message stack.
	*/
	public static function send(message:XMLNode):Void 
	{
		if (messages == null) initialize();
		messages.push(message);
	}
	
	/**
	* Adds a trace command to the message stack.
	*/
	public static function trace(value:Object, level:Number):Void 
	{
		var msgNode:XMLNode = createMsgNode(value.toString(), level);
		FlashConnect.send(msgNode);
	}
	
	/**
	* Adds compatibility with MTASC's tracing facilities.
	*/
	public static function mtrace(value:Object, method:String, path:String, line:Number):Void 
	{
		if (path.charAt(1) != ":") path = "~/" + path;
		var formatted:String = path + ":" + line + ":" + value;
		FlashConnect.trace(formatted, TraceLevel.DEBUG);
	}
	
	/**
	* Opens the xml socket connection to the target port and host.
	*/
	private static function initialize():Void 
	{
		counter = 0;
		messages = new Array();
		socket = new XMLSocket();
		socket.onData = function(data:String)
		{
			FlashConnect.onReturnData(data);
		}
		socket.onConnect = function(success:Boolean) 
		{
			if (success) FlashConnect.status = 1;
			else FlashConnect.status = -1;
			FlashConnect.onConnection();
		}
		interval = setInterval(sendStack, 50);
		socket.connect(host, port);
	}
	
	/**
	* Creates the required xml message for the trace operation.
	*/
	private static function createMsgNode(message:String, level:Number):XMLNode
	{
		if (isNaN(level)) level = TraceLevel.DEBUG;
		var msgNode:XMLNode = new XMLNode(1, null);
		var txtNode:XMLNode = new XMLNode(3, escape(message));
		msgNode.attributes.state = level.toString();
		msgNode.attributes.cmd = "trace";
		msgNode.nodeName = "message";
		msgNode.appendChild(txtNode);
		return msgNode;
	}

	/**
	* Sends all messages in message stack to FlashDevelop.
	*/
	private static function sendStack() 
	{
		if (messages.length > 0 && status == 1) 
		{
			var message:XML = new XML();
			var rootNode:XMLNode = message.createElement("flashconnect");
			while (messages.length != 0) 
			{
				counter++;
				if (counter > limit)
				{
					clearInterval(interval);
					var msg:String = new String("FlashConnect aborted. You have reached the limit of maximum messages.");
					var msgNode:XMLNode = createMsgNode(msg, TraceLevel.ERROR);
					rootNode.appendChild(msgNode);
					messages = new Array();
					break;
				} 
				else 
				{
					var msgNode:XMLNode = XMLNode(messages.shift());
					rootNode.appendChild(msgNode);
				}
			}
			message.appendChild(rootNode);
			socket.send(message);
			counter = 0;
		}
	}
	
}
