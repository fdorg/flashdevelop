intrinsic class flash.filters.BevelFilter extends flash.filters.BitmapFilter {

	var type : String;
	var blurX : Number;
	var blurY : Number;
	var knockout : Boolean;
	var strength : Number;
	var quality : Number;
	var shadowAlpha : Number;
	var shadowColor : Number;
	var highlightAlpha : Number;
	var highlightColor : Number;
	var angle : Number;
	var distance : Number;

	function BevelFilter(distance : Number, angle : Number, highlightColor : Number, highlightAlpha : Number, shadowColor : Number, shadowAlpha : Number, blurX : Number, blurY : Number, strength : Number, quality : Number, type : String, knockout : Boolean);
	function clone() : BevelFilter;

}
