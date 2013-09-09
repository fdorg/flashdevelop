intrinsic class flash.filters.DropShadowFilter extends flash.filters.BitmapFilter {

	var hideObject : Boolean;
	var blurX : Number;
	var blurY : Number;
	var knockout : Boolean;
	var strength : Number;
	var inner : Boolean;
	var quality : Number;
	var alpha : Number;
	var color : Number;
	var angle : Number;
	var distance : Number;

	function DropShadowFilter(distance : Number, angle : Number, color : Number, alpha : Number, blurX : Number, blurY : Number, strength : Number, quality : Number, inner : Boolean, knockout : Boolean, hideObject : Boolean);
	function clone() : DropShadowFilter;

}
