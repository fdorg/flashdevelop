intrinsic class flash.filters.ConvolutionFilter extends flash.filters.BitmapFilter {

	var alpha : Number;
	var color : Number;
	var clamp : Boolean;
	var preserveAlpha : Boolean;
	var bias : Number;
	var divisor : Number;
	var matrix : Array;
	var matrixX : Number;
	var matrixY : Number;

	function ConvolutionFilter(matrixX : Number, matrixY : Number, matrix : Array, divisor : Number, bias : Number, preserveAlpha : Boolean, clamp : Boolean, color : Number, alpha : Number);
	function clone() : ConvolutionFilter;

}