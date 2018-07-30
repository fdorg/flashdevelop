package fl.transitions
{
	import flash.display.MovieClip;
	import flash.geom.*;

	/**
	 * Makes the movie clip object appear or disappear like a photographic flash. * This effect requires no additional parameters. * <p>For example, the following code uses the Photo transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *   * TransitionManager.start (img1_mc, {type:Photo, direction:Transition.IN, duration:1, easing:None.easeNone}); * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Photo, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Photo extends Transition
	{
		/**
		 * @private
		 */
		protected var _alphaFinal : Number;
		/**
		 * @private
		 */
		protected var _colorControl : ColorTransform;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Photo (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}
