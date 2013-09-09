intrinsic class flash.filters.DisplacementMapFilter extends flash.filters.BitmapFilter {

	var alpha : Number;
	var color : Number;
	var mode : String;
	var scaleX : Number;
	var scaleY : Number;
	var componentX : Number;
	var componentY : Number;
	var mapPoint : flash.geom.Point;
	var mapBitmap : flash.display.BitmapData;

	function DisplacementMapFilter(mapBitmap : flash.display.BitmapData, mapPoint : flash.geom.Point, componentX : Number, componentY : Number, scaleX : Number, scaleY : Number, mode : String, color : Number, alpha : Number);
	function clone() : DisplacementMapFilter;

}