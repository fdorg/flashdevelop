intrinsic class flash.geom.ColorTransform {

	var rgb : Number;
	var blueOffset : Number;
	var greenOffset : Number;
	var redOffset : Number;
	var alphaOffset : Number;
	var blueMultiplier : Number;
	var greenMultiplier : Number;
	var redMultiplier : Number;
	var alphaMultiplier : Number;

	function ColorTransform( rm : Number, gm : Number, bm : Number, am : Number, ro : Number, go : Number, bo : Number, ao : Number );
	function toString() : String;
	function concat( c : ColorTransform ) : Void;

}