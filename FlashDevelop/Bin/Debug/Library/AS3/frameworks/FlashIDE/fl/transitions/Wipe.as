﻿package fl.transitions
{
	import flash.display.MovieClip;
	import flash.geom.*;

	/**
	 * The Wipe class reveals or hides the movie clip object by using an animated mask of a shape that moves  * horizontally. This effect requires the * following parameter: * <ul><li><code>startPoint</code>: An integer indicating a starting position; the range * is 1 to 9: Top Left:<code>1</code>; Top Center:<code>2</code>, Top Right:<code>3</code>; Left Center:<code>4</code>; Center:<code>5</code>; Right Center:<code>6</code>; Bottom Left:<code>7</code>; Bottom Center:<code>8</code>, Bottom Right:<code>9</code>.</li></ul> * <p>For example, the following code uses the Wipe transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *      * TransitionManager.start(img1_mc, {type:Wipe, direction:Transition.IN, duration:2, easing:None.easeNone, startPoint:1});  * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Wipe, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Wipe extends Transition
	{
		/**
		 * @private
		 */
		protected var _mask : MovieClip;
		/**
		 * @private
		 */
		protected var _innerMask : MovieClip;
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
		public function get type () : Class;

		/**
		 * @private
		 */
		function Wipe (content:MovieClip, transParams:Object, manager:TransitionManager);
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
		 * @private
		 */
		protected function _render (p:Number) : void;
		/**
		 * @private
		 */
		protected function _drawSlant (mc:MovieClip, p:Number) : void;
	}
}
