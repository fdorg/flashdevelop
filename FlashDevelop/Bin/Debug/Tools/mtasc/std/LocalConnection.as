dynamic intrinsic class LocalConnection
{
	function LocalConnection();

	function connect(connectionName:String):Boolean;
	function send(connectionName:String, methodName:String, args:Object):Boolean;
	function close():Void;
	function domain():String;
	function allowDomain(domain:String):Boolean;
	function allowInsecureDomain(domain:String):Boolean;

	function onStatus(infoObject:Object):Void;
}


