intrinsic class Microphone
{
	static var names:Array;
	static function get(index:Number):Microphone;

	var gain:Number;
	var index:Number;
	var activityLevel:Number;
	var name:String;
	var silenceLevel:Number;
	var silenceTimeOut:Number;
	var rate:Number;
	var useEchoSuppression:Boolean;
	var muted:Boolean;
	
	function setSilenceLevel(silenceLevel:Number,timeOut:Number):Void;
	function setRate(rate:Number):Void;
	function setGain(gain:Number):Void;
	function setUseEchoSuppression(useEchoSuppression:Boolean):Void;
	
	function onActivity(active:Boolean):Void;
	function onStatus(infoObject:Object):Void;
}



