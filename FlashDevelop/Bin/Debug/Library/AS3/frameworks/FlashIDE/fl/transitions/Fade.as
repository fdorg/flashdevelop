package fl.transitions
{
	import flash.display.*;

	/**
	 * The Fade class fades the movie clip object in or out. This effect requires no additional parameters. * <p>For example, the following code uses the Fade transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *   * TransitionManager.start(img1_mc, {type:Fade, direction:Transition.IN, duration:9, easing:Strong.easeOut}); * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Fade, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Fade extends Transition
	{
		/**
		 * @private
		 */
		protected var _alphaFinal : Number;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Fade (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}
