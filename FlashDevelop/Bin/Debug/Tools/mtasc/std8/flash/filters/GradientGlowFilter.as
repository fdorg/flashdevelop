intrinsic class flash.filters.GradientGlowFilter extends flash.filters.BitmapFilter {

	var type : String;
	var knockout : Boolean;
	var strength : Number;
	var quality : Number;
	var blurX : Number;
	var blurY : Number;
	var ratios : Array;
	var alphas : Array;
	var colors : Array;
	var angle : Number;
	var distance : Number;

	function GradientGlowFilter(distance : Number, angle : Number, colors : Array, alphas : Array, ratios : Array, blurX : Number, blurY : Number, strength : Number, quality : Number, type : String, knockout : Boolean);
	function clone() : GradientGlowFilter;

}