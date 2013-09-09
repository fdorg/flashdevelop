intrinsic class Video {
	var _alpha : Number;
	var _height : Number;
	var _name : String;
	var _parent : MovieClip;
	var _rotation : Number;
	var _visible : Boolean;
	var _width : Number;
	var _x : Number;
	var _xmouse : Number;
	var _xscale : Number;
	var _y : Number;
	var _ymouse : Number;
	var _yscale : Number;
	var deblocking : Number;
	var height : Number;
	var smoothing : Boolean;
	var width : Number;
	function attachVideo(source : Object) : Void;
	function clear() : Void;

	// Flash Lite 2.x
	function close() : Void;
	function pause() : Void;
	function play() : Boolean;
	function resume() : Void;
	function stop() : Void;
}
