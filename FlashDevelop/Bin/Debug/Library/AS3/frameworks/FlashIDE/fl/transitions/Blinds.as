package fl.transitions
{
	import flash.display.MovieClip;
	import flash.geom.*;

	/**
	 * The Blinds class reveals the movie clip object by using appearing or disappearing rectangles.  * This effect requires the following parameters: * <ul><li><code>numStrips</code>: The number of masking strips in the Blinds effect. * The recommended range is 1 to 50.</li> * <li><code>dimension</code>: An integer that indicates whether the masking strips are  * to be vertical (<code>0</code>) or horizontal (<code>1</code>).</li></ul> * <p>For example, the following code uses the Blinds transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *   * TransitionManager.start(img1_mc, {type:Blinds, direction:Transition.IN, duration:2, easing:None.easeNone, numStrips:10, dimension:0});  * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Blinds, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Blinds extends Transition
	{
		/**
		 * @private
		 */
		protected var _numStrips : uint;
		/**
		 * @private
		 */
		protected var _dimension : uint;
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
		function Blinds (content:MovieClip, transParams:Object, manager:TransitionManager);
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
	}
}
