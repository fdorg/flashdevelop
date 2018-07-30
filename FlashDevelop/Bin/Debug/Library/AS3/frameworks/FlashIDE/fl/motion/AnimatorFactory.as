package fl.motion
{
	import flash.display.DisplayObject;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import flash.utils.Dictionary;

	/**
	 * The AnimatorFactory class provides ActionScript-based support to associate one Motion object with multiple     * display objects.      * <p>Use the AnimatorFactory constructor to create an AnimatorFactory instance. Then,     * use the methods inherited from the      * AnimatorFactoryBase class to associate the desired properties with display objects.</p>     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator     * @see fl.motion.AnimatorFactoryBase     * @see fl.motion.Motion     * @see fl.motion.MotionBase
	 */
	public class AnimatorFactory extends AnimatorFactoryBase
	{
		/**
		 * Creates an AnimatorFactory instance you can use to assign the properties of     * a MotionBase object to display objects.      * @param motion The MotionBase object containing the desired motion properties.     * .     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator     * @see fl.motion.AnimatorFactoryBase     * @see fl.motion.Motion     * @see fl.motion.MotionBase
		 */
		public function AnimatorFactory (motion:MotionBase);
		/**
		 * @private
		 */
		protected function getNewAnimator () : AnimatorBase;
	}
}
