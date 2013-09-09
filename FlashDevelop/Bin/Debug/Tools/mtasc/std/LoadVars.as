dynamic intrinsic class LoadVars
{
	var contentType:String;
	var loaded:Boolean;
	var _customHeaders:Array;

	function LoadVars();	

	function addRequestHeader(header:Object, headerValue:String):Void;
	function load(url:String):Boolean;
	function send(url:String,target:String,method:String):Boolean;
	function sendAndLoad(url:String,target,method:String):Boolean;
	function getBytesLoaded():Number;
	function getBytesTotal():Number;
	function decode(queryString:String):Void;
	function toString():String;

	function onLoad(success:Boolean):Void;
	function onData(src:String):Void;

}
