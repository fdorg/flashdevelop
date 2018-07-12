package fl.transitions
{
	import flash.display.MovieClip;
	import flash.geom.*;

	/**
	 * The Iris class reveals the movie clip object by using an animated mask of a square shape or a  * circle shape that zooms in or out. This effect requires the * following parameters: * <ul><li><code>startPoint</code>: An integer indicating a starting position; the range * is 1 to 9: Top Left:<code>1</code>; Top Center:<code>2</code>, Top Right:<code>3</code>; Left Center:<code>4</code>; Center:<code>5</code>; Right Center:<code>6</code>; Bottom Left:<code>7</code>; Bottom Center:<code>8</code>, Bottom Right:<code>9</code>.</li> * <li><code>shape</code>: A mask shape of either <code>fl.transitions.Iris.SQUARE</code> (a square) or <code>fl.transitions.Iris.CIRCLE</code> (a circle).</li></ul> * <p>For example, the following code uses a circle-shaped animated mask transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *  * TransitionManager.start(img1_mc, {type:Iris, direction:Transition.IN, duration:2, easing:Strong.easeOut, startPoint:5, shape:Iris.CIRCLE});  * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Iris, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Iris extends Transition
	{
		/**
		 * Used to specify a square mask shape for the transition effect.     * @playerversion Flash 9     * @langversion 3.0     * @keyword Iris, Transitions
		 */
		public static const SQUARE : String = "SQUARE";
		/**
		 * Used to specify a circle mask shape for the transition effect.     * @playerversion Flash 9     * @langversion 3.0     * @keyword Iris, Transitions
		 */
		public static const CIRCLE : String = "CIRCLE";
		/**
		 * @private
		 */
		protected var _mask : MovieClip;
		/**
		 * @private
		 */
		protected var _startPoint : uint;
		/**
		 * @private
		 */
		protected var _cornerMode : Boolean;
		/**
		 * @private
		 */
		protected var _shape : String;
		/**
		 * @private
		 */
		protected var _maxDimension : Number;
		/**
		 * @private
		 */
		protected var _minDimension : Number;
		/**
		 * @private
		 */
		protected var _renderFunction : Function;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Iris (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		public function start () : void;
		/**
		 * @private
		 */
		public function cleanUp () : void;
		/**
		 * @private
		 */
		protected function _initMask () : void;
		/**
		 * @private stub--dynamically overwritten by one of the other render methods
		 */
		protected function _render (p:Number) : void;
		/**
		 * @private
		 */
		protected function _renderCircle (p:Number) : void;
		/**
		 * @private
		 */
		protected function _drawQuarterCircle (mc:MovieClip, r:Number) : void;
		/**
		 * @private
		 */
		protected function _drawHalfCircle (mc:MovieClip, r:Number) : void;
		/**
		 * @private
		 */
		protected function _renderSquareEdge (p:Number) : void;
		/**
		 * @private
		 */
		protected function _renderSquareCorner (p:Number) : void;
	}
}
