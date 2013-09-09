intrinsic class XML extends XMLNode
{
	var contentType:String;
	var docTypeDecl:String;
	var ignoreWhite:Boolean;
	var loaded:Boolean;
	var status:Number;
	var xmlDecl:String;
	
	function XML(text:String);
	function createElement(name:String):XMLNode;
	function createTextNode(value:String):XMLNode;
	function parseXML(value:String):Void;
	function getBytesLoaded():Number;
	function getBytesTotal():Number;
	function load(url:String):Boolean;
	function send(url:String,target:String,method:String):Boolean;
	function sendAndLoad(url:String, resultXML):Void;
	function onLoad(success:Boolean):Void;
	function onData(src:String):Void;
	function addRequestHeader(header:Object, headerValue:String):Void;
}

