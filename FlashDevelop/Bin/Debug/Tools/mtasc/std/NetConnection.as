dynamic intrinsic class NetConnection
{
	var isConnected : Boolean;
	var uri : String;

	function connect( targetURI : String) : Boolean;
	function call( remoteMethod : String, resultObject : Object) : Void;
	function onStatus(infoObject : Object) : Void;
	function onResult(infoObject : Object) : Void;
	function addHeader():Void;
	function close() : Void;
}


