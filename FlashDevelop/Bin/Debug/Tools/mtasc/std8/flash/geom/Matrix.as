import flash.geom.Point;

intrinsic class flash.geom.Matrix {

	// 3x2 affine 2D matrix
	var a : Number;
	var b : Number;
	var c : Number;
	var d : Number;
	var tx : Number;
	var ty : Number;

	function Matrix(a : Number, b : Number, c : Number, d : Number, tx : Number, ty : Number);

	function transformPoint( p : Point ) : Point;
	function deltaTransformPoint( p : Point ) : Point;
	function toString() : String;
	function scale( sx : Number, sy : Number ) : Void;
	function translate( tx : Number, ty : Number ) : Void;
	function rotate( r : Number ) : Void;
	function identity() : Void;
	function invert() : Void;
	function concat( m : Matrix ) : Void;
	function clone() : Matrix;

	function createGradientBox( width : Number, height : Number, rot : Number, tx : Number, ty : Number ) : Void;
	function createBox( scalex : Number, scaley : Number, rot : Number, tx : Number, ty : Number ) : Void;


}