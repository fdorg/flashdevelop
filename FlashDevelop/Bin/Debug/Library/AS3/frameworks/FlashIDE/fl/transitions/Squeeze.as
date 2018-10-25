package fl.transitions
{
	import flash.display.MovieClip;

	/**
	 * The Squeeze class scales the movie clip object horizontally or vertically. This effect requires the * following parameters: * <ul><li><code>dimension</code>: An integer that indicates whether the Squeeze effect should be horizontal (0) or vertical (1). </li></ul> * <p>For example, the following code uses the Squeeze transition for the movie clip  * instance <code>img1_mc</code>:</p> * <listing> * import fl.transitions.~~; * import fl.transitions.easing.~~; *     * TransitionManager.start(img1_mc, {type:Squeeze, direction:Transition.IN, duration:2, easing:Elastic.easeOut, dimension:1}); * </listing> * @playerversion Flash 9 * @langversion 3.0 * @keyword Squeeze, Transitions * @see fl.transitions.TransitionManager
	 */
	public class Squeeze extends Transition
	{
		/**
		 * @private
		 */
		protected var _scaleProp : String;
		/**
		 * @private
		 */
		protected var _scaleFinal : Number;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Squeeze (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}
