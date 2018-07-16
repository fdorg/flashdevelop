package fl.transitions
{
	import flash.display.MovieClip;
	import flash.geom.*;

	/**
	 * The PixelDissolve class reveals reveals the movie clip object by using randomly appearing or disappearing rectangles * in a checkerboard pattern.  This effect requires the following parameters: * <ul><li><code>xSections</code>: An integer that indicates the number of masking  * rectangle sections along the horizontal axis. The recommended range is 1 to 50.</li> * <li><code>ySections</code>: An integer that indicates the number of masking rectangle * sections along the vertical axis. The recommended range is 1 to 50.</li></ul> * <p>For example, the following code uses the PixelDissolve transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *    * TransitionManager.start(img1_mc, {type:PixelDissolve, direction:Transition.IN, duration:2, easing:Regular.easeIn, xSections:10, ySections:10});  * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword PixelDissolve, Transitions * @see fl.transitions.TransitionManager
	 */
	public class PixelDissolve extends Transition
	{
		/**
		 * @private
		 */
		protected var _xSections : Number;
		/**
		 * @private
		 */
		protected var _ySections : Number;
		/**
		 * @private
		 */
		protected var _numSections : uint;
		/**
		 * @private
		 */
		protected var _indices : Array;
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
		public function get type () : Class;

		/**
		 * @private
		 */
		function PixelDissolve (content:MovieClip, transParams:Object, manager:TransitionManager);
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
		protected function _shuffleArray (a:Array) : void;
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}
