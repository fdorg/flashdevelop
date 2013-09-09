intrinsic class SharedObject
{
	static function getLocal(name:String,localPath:String):SharedObject;
	static function getRemote(name:String,remotePath:String,persistence:Object):SharedObject;
	static function deleteAll(url:String);
	static function getDiskUsage(url:String);

	function connect(myConnection:NetConnection):Boolean;
	function send(handlerName:String):Void;
	function flush(minDiskSpace:Number):Object;
	function close():Void;
	function getSize():Number;
	function setFps(updatesPerSecond:Number):Boolean;

	function onStatus(infoObject:Object):Void;
	function onSync(objArray:Array):Void;

	function clear() : Void;

	var data:Object;

	// Flash Lite 2.x
	static function GetMaxSize():Number;

}


