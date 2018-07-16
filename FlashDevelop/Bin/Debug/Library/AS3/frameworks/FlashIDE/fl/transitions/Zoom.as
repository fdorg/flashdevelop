package fl.transitions
{
	import flash.display.MovieClip;

	/**
	 * The Zoom class zooms the movie clip object in or out by scaling it in proportion. * This effect requires no additional parameters: * <p>For example, the following code uses the Zoom transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *       * TransitionManager.start(img1_mc, {type:Zoom, direction:Transition.IN, duration:2, easing:Elastic.easeOut}); * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Zoom, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Zoom extends Transition
	{
		/**
		 * @private
		 */
		protected var _scaleXFinal : Number;
		/**
		 * @private
		 */
		protected var _scaleYFinal : Number;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Zoom (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}
