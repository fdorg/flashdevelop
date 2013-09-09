intrinsic class flash.filters.BlurFilter extends flash.filters.BitmapFilter {

	var quality : Number;
	var blurX : Number;
	var blurY : Number;

	function BlurFilter( bx : Number, by : Number, qual : Number );
	function clone() : BlurFilter;

}
