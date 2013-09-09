package org.flashdevelop.utils
{	
	import flash.net.*;
	import flash.events.*;
	import flash.utils.*;
	import flash.xml.*;
	
	/**
	* Connects a flash movie thru XmlSocket to the FlashDevelop program.
	* @author Mika Palmu
	* @version 3.3
	*/
	public class FlashConnect
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
		private static var socket:XMLSocket;
		private static var messages:Array;
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
		public static function send(message:XMLNode):void 
		{
			if (messages == null) initialize();
			messages.push(message);
		}
		
		/**
		* Adds a trace command to the message stack.
		*/
		public static function trace(value:Object, level:Number = TraceLevel.DEBUG):void
		{
			var msgNode:XMLNode = createMsgNode(value.toString(), level);
			FlashConnect.send(msgNode);
		}
		
		/**
		* Adds a trace command to the message stack, AS3 style.
		*/
		public static function atrace(...rest):void
		{
			var result:String = rest.join(",");
			var message:XMLNode = createMsgNode(result, TraceLevel.DEBUG);
			FlashConnect.send(message);
		}

		/**
		* Adds a trace command to the message stack, MTASC style.
		*/
		public static function mtrace(value:Object, method:String, path:String, line:Number):void 
		{
			var fixed:String = path.split("/").join("\\");
			var formatted:String = fixed + ":" + line + ":" + value;
			FlashConnect.trace(formatted, TraceLevel.DEBUG);
		}
		
		/**
		* Send message queue immediately
		* @return Success
		*/
		public static function flush():Boolean
		{
			if (status) 
			{
				sendStack();
				return true;
			}
			else return false;
		}
		
		/**
		* Opens the xml socket connection to the target port and host.
		*/
		public static function initialize():int
		{
			if (socket) return status;
			counter = 0;
			messages = new Array();
			socket = new XMLSocket();
			socket.addEventListener(Event.CLOSE, onClose);
			socket.addEventListener(DataEvent.DATA, onData);
			socket.addEventListener(Event.CONNECT, onConnect);
			socket.addEventListener(IOErrorEvent.IO_ERROR, onIOError);
			socket.addEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
			interval = setInterval(sendStack, 50);
			socket.connect(host, port);
			return status;
		}
		private static function onData(event:DataEvent):void
		{
			FlashConnect.status = 1;
			if (FlashConnect.onReturnData != null)
			{
				FlashConnect.onReturnData(event.data);
			}
		}
		private static function onClose(event:Event):void
		{
			socket = null;
			FlashConnect.status = -1;
			if (FlashConnect.onConnection != null) 
			{
				FlashConnect.onConnection();
			}
		}
		private static function onConnect(event:Event):void
		{
			FlashConnect.status = 1;
			if (FlashConnect.onConnection != null) 
			{
				FlashConnect.onConnection();
			}
		}
		private static function onIOError(event:IOErrorEvent):void
		{
			FlashConnect.status = -1;
			if (FlashConnect.onConnection != null) 
			{
				FlashConnect.onConnection();
			}
		}
		private static function onSecurityError(event:SecurityErrorEvent):void
		{
			FlashConnect.status = -1;
			if (FlashConnect.onConnection != null) 
			{
				FlashConnect.onConnection();
			}
		}
		
		/**
		* Creates the required xml message for the trace operation.
		*/
		private static function createMsgNode(message:String, level:Number):XMLNode
		{
			if (isNaN(level)) level = TraceLevel.DEBUG;
			var msgNode:XMLNode = new XMLNode(1, null);
			var txtNode:XMLNode = new XMLNode(3, encodeURI(message));
			msgNode.attributes.state = level.toString();
			msgNode.attributes.cmd = "trace";
			msgNode.nodeName = "message";
			msgNode.appendChild(txtNode);
			return msgNode;
		}
		
		/**
		* Sends all messages in message stack to FlashDevelop.
		*/
		private static function sendStack():void
		{
			if (messages.length > 0 && status == 1)
			{
				var message:XMLDocument = new XMLDocument();
				var rootNode:XMLNode = message.createElement("flashconnect");
				while (messages.length != 0) 
				{
					counter++;
					if (counter > limit)
					{
						clearInterval(interval);
						var msg:String = new String("FlashConnect aborted. You have reached the limit of maximum messages.");
						var errorNode:XMLNode = createMsgNode(msg, TraceLevel.ERROR);
						rootNode.appendChild(errorNode);
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
				if (socket && socket.connected) socket.send(message);
				counter = 0;
			}
		}
		
	}
	
}
