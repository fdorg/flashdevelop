intrinsic class XMLSocket
{

	function XMLSocket();

	function connect(url:String,port:Number):Boolean;
	function send(data:Object):Boolean;
	function close():Boolean;
	function onData(src:String):Void;
	function onXML(src:XML):Void;
	function onConnect(success:Boolean):Void;
	function onClose():Void;
}
