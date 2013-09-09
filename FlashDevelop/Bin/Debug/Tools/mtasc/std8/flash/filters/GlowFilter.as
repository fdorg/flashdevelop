intrinsic class flash.filters.GlowFilter extends flash.filters.BitmapFilter {

	var blurX : Number;
	var blurY : Number;
	var knockout : Boolean;
	var strength : Number;
	var quality : Number;
	var inner : Boolean;
	var alpha : Number;
	var color : Number;

	function GlowFilter(color : Number, alpha : Number, blurX : Number, blurY : Number, strength : Number, quality : Number, inner : Boolean, knockout : Boolean)
	function clone() : GlowFilter;

}
