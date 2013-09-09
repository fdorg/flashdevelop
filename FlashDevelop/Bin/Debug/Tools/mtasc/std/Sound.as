intrinsic class Sound
{
	var duration:Number;
	var id3:Object;
	var ID3:Object;
	var position:Number;
	
	function Sound(target:Object);
	
	function onLoad(success:Boolean):Void;
	function onSoundComplete():Void;
	function onID3():Void;
	
	function getPan():Number;
	function getTransform():Object;
	function getVolume():Number;
	function setPan(value:Number):Void;
	function setTransform(transformObject:Object):Void;
	function setVolume(value:Number):Void;
	function stop(linkageID:String):Void;
	function attachSound(id:String):Void;
	function start(secondOffset:Number, loops:Number):Void;
	function getDuration():Number;
	function setDuration(value:Number):Void;
	function getPosition():Number;
	function setPosition(value:Number):Void;
	function loadSound(url:String, isStreaming:Boolean):Void;
	function getBytesLoaded():Number;
	function getBytesTotal():Number;
}


