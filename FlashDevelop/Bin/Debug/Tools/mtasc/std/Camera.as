intrinsic class Camera
{
	static var names:Array;
	static function get(index:Number):Camera;

	var nativeModes:Array;
	var keyFrameInterval:Number;
	var bandwidth:Number;
	var motionLevel:Number;
	var motionTimeOut:Number;
	var quality:Number;
	var loopback:Boolean;
	var width:Number;
	var height:Number;
	var fps:Number;
	var activityLevel:Number;
	var muted:Boolean;
	var currentFps:Number;
	var name:String;
	var index:Number;

	function setKeyFrameInterval(keyFrameInterval:Number):Void;
	function setLoopback(compress:Boolean):Void;
	function setMode(width:Number,height:Number,fps:Number,favorArea:Boolean):Void;
	function setMotionLevel(motionLevel:Number,timeOut:Number):Void;
	function setQuality(bandwidth:Number,quality:Number):Void;

	function onActivity(active:Boolean):Void;
	function onStatus(infoObject:Object):Void;
}


