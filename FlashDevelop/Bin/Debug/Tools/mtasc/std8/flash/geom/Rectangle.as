import flash.geom.Point;

intrinsic class flash.geom.Rectangle {

	var left : Number;
	var top : Number;
	var right : Number;
	var bottom : Number;

	// OR
	var x : Number;
	var y : Number;
	var width : Number;
	var height : Number;

	// OR
	var size : Point;
	var bottomRight : Point;
	var topLeft : Point;

	function Rectangle( x : Number, y : Number, w : Number, h : Number );

	function equals( r : Object ) : Boolean;
	function union( r : Rectangle ) : Rectangle;
	function intersects( r : Rectangle ) : Boolean;
	function intersection( r : Rectangle ) : Rectangle;
	function containsRectangle( r : Rectangle ) : Boolean;
	function containsPoint( p : Point ) : Boolean;
	function contains( x : Number, y : Number ) : Boolean;
	function offsetPoint( p : Point ) : Void;
	function offset( x : Number, y : Number ) : Void;

	function inflatePoint( p : Point ) : Void;
	function inflate( x : Number, y : Number ) : Void;
	function isEmpty() : Boolean;
	function setEmpty() : Void;
	function clone() : Rectangle;

	function toString() : String;


}