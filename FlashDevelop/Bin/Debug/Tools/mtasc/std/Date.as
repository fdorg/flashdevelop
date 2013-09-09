intrinsic class Date
{
	function Date(year:Number,month:Number,date:Number,hour:Number,min:Number,sec:Number,ms:Number);

	function getFullYear():Number;
	function getYear():Number;
	function getMonth():Number;
	function getDate():Number;
	function getDay():Number;
	function getHours():Number;
	function getMinutes():Number;
	function getSeconds():Number;
	function getMilliseconds():Number;

	function getUTCFullYear():Number;
	function getUTCYear():Number;
	function getUTCMonth():Number;
	function getUTCDate():Number;
	function getUTCDay():Number;
	function getUTCHours():Number;
	function getUTCMinutes():Number;
	function getUTCSeconds():Number;
	function getUTCMilliseconds():Number;

	function setFullYear(value:Number):Void;
	function setMonth(value:Number):Void;
	function setDate(value:Number):Void;
	function setHours(value:Number):Void;
	function setMinutes(value:Number):Void;
	function setSeconds(value:Number):Void;
	function setMilliseconds(value:Number):Void;

	function setUTCFullYear(value:Number):Void;
	function setUTCMonth(value:Number):Void;
	function setUTCDate(value:Number):Void;
	function setUTCHours(value:Number):Void;
	function setUTCMinutes(value:Number):Void;
	function setUTCSeconds(value:Number):Void;
	function setUTCMilliseconds(value:Number):Void;

	function getTime():Number;
	function setTime(value:Number):Void;
	function getTimezoneOffset():Number;
	function toString():String;
	function valueOf():Number;
	function setYear(value:Number):Void;


	// Flash Lite 2.x
	function getLocaleLongDate():String;
	function getLocaleShortDate():String;
	function getLocaleTime():String;

	static function UTC(year:Number,month:Number,date:Number,
                        hour:Number,min:Number,sec:Number,ms:Number):Number;
}


