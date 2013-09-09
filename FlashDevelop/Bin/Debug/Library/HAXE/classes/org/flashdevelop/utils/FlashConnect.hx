package org.flashdevelop.utils;

import flash.events.DataEvent;
import flash.events.Event;
import flash.events.IOErrorEvent;
import flash.events.SecurityErrorEvent;
import flash.net.XMLSocket;
import flash.xml.XMLDocument;
import flash.xml.XMLNode;
import flash.xml.XMLNodeType;
import haxe.Log;
import haxe.PosInfos;

/**
* Connects a flash movie thru XmlSocket to the FlashDevelop program.
* @author Mika Palmu
* @author Franco Ponticelli (haXe version)
* @version 3.1
*/
class FlashConnect {
   /**
   * Public properties of the class.
   */
   public static var status = 0;
   public static var limit = 1000;
   public static var host = "localhost";
   public static var port = 1978;

   /**
   * Private properties of the class.
   */
   private static var socket : XMLSocket;
   private static var messages : Array<XMLNode>;
   private static var interval : Int;
   private static var counter : Int;

   /**
   * Event callbacks of the class.
   */
   public static var onConnection : Void -> Void;
   public static var onReturnData : String -> Void;

   /**
   * Adds a custom message to the message stack.
   */
   public static function send(message : XMLNode) {
      if (messages == null) initialize();
      messages.push(message);
   }

   /**
   * Adds a trace command to the message stack.
   */
   public static function trace(value : Dynamic, ?level : Int)   {
      var msgNode = createMsgNode(Std.string(value), level);
      FlashConnect.send(msgNode);
   }

   /**
   * Adds a trace command to the message stack, AS3 style.
   */
   public static function atrace(?arg : String, ?args : Array < String > ) {
      if (args == null)
         args = [];
      if (arg != null)
         args.insert(0, arg);
      var result = args.join(",");
      var message = createMsgNode(result, TraceLevel.DEBUG);
      FlashConnect.send(message);
   }

   /**
   * Adds a trace command to the message stack, MTASC style.
   */
   public static function mtrace(value : Dynamic, method : String, path : String, line : Int) {
      var fixed = path.split("/").join("\\");
      var formatted = fixed + ":" + line + ":" + Std.string(value);
      FlashConnect.trace(formatted, TraceLevel.DEBUG);
   }

   /**
   * Opens the xml socket connection to the target port and host.
   */
   private static function initialize() {
      counter = 0;
      messages = new Array();
      socket = new XMLSocket();
      socket.addEventListener(DataEvent.DATA, onData);
      socket.addEventListener(Event.CONNECT, onConnect);
      socket.addEventListener(IOErrorEvent.IO_ERROR, onIOError);
      socket.addEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
      interval = untyped __global__["flash.utils.setInterval"](sendStack, 50);
      socket.connect(host, port);
   }
   private static function onData(event : DataEvent) {
      FlashConnect.status = 1;
      if (FlashConnect.onReturnData != null) {
         FlashConnect.onReturnData(event.data);
      }
   }
   private static function onConnect(event : Event) {
      FlashConnect.status = 1;
      if (FlashConnect.onConnection != null) {
         FlashConnect.onConnection();
      }
   }
   private static function onIOError(event : IOErrorEvent) {
      FlashConnect.status = -1;
      if (FlashConnect.onConnection != null)
      {
         FlashConnect.onConnection();
      }
   }
   private static function onSecurityError(event : SecurityErrorEvent) {
      FlashConnect.status = -1;
      if (FlashConnect.onConnection != null) {
         FlashConnect.onConnection();
      }
   }

   /**
   * Creates the required xml message for the trace operation.
   */
   private static function createMsgNode(message : String, ?level : Int) : XMLNode
   {
      if (level == null)
         level = TraceLevel.DEBUG;
      var msgNode = new XMLNode(XMLNodeType.ELEMENT_NODE, null);
      var txtNode = new XMLNode(XMLNodeType.TEXT_NODE, untyped __global__["encodeURI"](message));
      msgNode.attributes.state = "" + level;
      msgNode.attributes.cmd = "trace";
      msgNode.nodeName = "message";
      msgNode.appendChild(txtNode);
      return msgNode;
   }

   /**
   * Sends all messages in message stack to FlashDevelop.
   */
   private static function sendStack() {
      if (messages.length > 0 && status == 1) {
         var message = new XMLDocument();
         var rootNode = message.createElement("flashconnect");
         while (messages.length != 0) {
            counter++;
            if (counter > limit) {
               untyped __global__["flash.utils.clearInterval"](interval);
               var msg = new String("FlashConnect aborted. You have reached the limit of maximum messages.");
               var errorNode = createMsgNode(msg, TraceLevel.ERROR);
               rootNode.appendChild(errorNode);
               break;
            } else {
               var msgNode = messages.shift();
               rootNode.appendChild(msgNode);
            }
         }
         message.appendChild(rootNode);
         socket.send(message);
      }
   }

   /**
    * Redirects the standard trace function of haxe to use FlashConnect
    */
   public static function redirect() {
      Log.trace = function(v : Dynamic, ?infos : PosInfos) {
         if(infos == null)
            FlashConnect.trace(Std.string(v));
         else
            FlashConnect.trace(infos.fileName+":"+infos.lineNumber+": " + Std.string(v));
      };
   }
}