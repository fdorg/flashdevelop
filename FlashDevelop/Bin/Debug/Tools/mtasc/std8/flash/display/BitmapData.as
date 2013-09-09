import flash.geom.Rectangle;
import flash.geom.Point;

intrinsic class flash.display.BitmapData {

	static function loadBitmap( id : String ) : BitmapData;

	var width : Number;
	var height : Number;
	var rectangle : Rectangle;
	var transparent : Boolean;

	function BitmapData( width : Number, height : Number, transparent : Boolean, fillcolor : Number );

	function getPixel( x : Number, y : Number ) : Number;
	function setPixel( x : Number, y : Number, color : Number ) : Void;
	function getPixel32( x : Number, y : Number ) : Number;
	function setPixel32( x : Number, y : Number, color : Number ) : Void;

	function fillRect( r : Rectangle, color : Number ) : Void;
	function copyPixels( src : BitmapData, srcRect : Rectangle, dst : Point, alpha : BitmapData, alphaPos : Point, mergeAlpha : Boolean ) : Void;
	function applyFilter( source : BitmapData, sourceRect : Rectangle, dest : Point, filter : flash.filters.BitmapFilter ) : Number;
	function scroll( dx : Number, dy : Number ) : Void;
	function threshold( src : BitmapData , srcRect : Rectangle, dstPoint : Point, op : String, threshold : Number, color : Number, mask : Number, copy : Boolean ) : Number;
	function draw( source : Object, matrix : flash.geom.Matrix, colortrans : flash.geom.ColorTransform, blendMode : Object, clipRect : Rectangle, smooth : Boolean) : Void;
	function pixelDissolve( src : BitmapData, srcRect : Rectangle, dst : Point, seed : Number, npixels : Number, fillColor : Number ) : Number;
	function floodFill( x : Number, y : Number, color : Number ) : Void;
	function getColorBoundsRect( mask : Number, color : Number, fillColor : Boolean ) : Rectangle;
	function perlinNoise( x : Number, y : Number, num : Number, seed : Number, stitch : Boolean, noise : Boolean, channels : Number, gray : Boolean, offsets : Object ) : Void;
	function colorTransform( r : Rectangle, trans : flash.geom.ColorTransform ) : Void;
	function hitTest( firstPoint : Point, firstAlpha : Number, object : Object, secondPoint : Point, secondAlpha : Number ) : Boolean;
	function paletteMap( source : BitmapData, srcRect : Rectangle, dst : Point, reds : Array, greens, Array, blues : Array, alphas : Array ) : Void;
	function merge( src : BitmapData, srcRect : Rectangle, dst : Point, redMult : Number, greenMult : Number, blueMult : Number, alphaMult : Number ) : Void;
	function noise( seed : Number, low : Number, high : Number, channels : Number, gray : Boolean ) : Void;
	function copyChannel( source : BitmapData, sourceRect : Rectangle, dest : Point, sourceChannel : Number, destChannel : Number ) : Void;
	function clone() : BitmapData;
	function dispose() : Void;
	function generateFilterRect(sourceRect : Rectangle, filter : flash.filters.BitmapFilter ) : Rectangle;

	function compare( b : BitmapData ) : Object;
}