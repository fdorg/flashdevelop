package fl.transitions
{
	import flash.display.*;
	import flash.geom.*;

	/**
	 * The Fly class slides the movie clip object in from a specified direction. This effect requires the * following parameters: * <ul><li><code>startPoint</code>: An integer indicating a starting position; the range * is 1 to 9: Top Left:<code>1</code>; Top Center:<code>2</code>, Top Right:<code>3</code>; Left Center:<code>4</code>; Center:<code>5</code>; Right Center:<code>6</code>; Bottom Left:<code>7</code>; Bottom Center:<code>8</code>, Bottom Right:<code>9</code>.</li></ul> * <p>For example, the following code uses the Fly transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *   * TransitionManager.start(img1_mc, {type:Fly, direction:Transition.IN, duration:3, easing:Elastic.easeOut, startPoint:9});  * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Fly, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Fly extends Transition
	{
		/**
		 * @private
		 */
		public var className : String;
		/**
		 * @private
		 */
		protected var _startPoint : Number;
		/**
		 * @private
		 */
		protected var _xFinal : Number;
		/**
		 * @private
		 */
		protected var _yFinal : Number;
		/**
		 * @private
		 */
		protected var _xInitial : Number;
		/**
		 * @private
		 */
		protected var _yInitial : Number;
		/**
		 * @private
		 */
		protected var _stagePoints : Object;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Fly (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}
