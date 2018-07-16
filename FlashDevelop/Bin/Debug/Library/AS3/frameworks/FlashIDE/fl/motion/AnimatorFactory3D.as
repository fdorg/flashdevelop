package fl.motion
{
	import flash.geom.Vector3D;

	/**
	 * The AnimatorFactory3D class provides ActionScript-based support to associate one Motion object containing	 * three-dimensional properties with multiple     * display objects. If the Motion does not contain three-dimensional properties, use the AnimatorFactory class.     * <p>Use the AnimatorFactory3D constructor to create an AnimatorFactory3D instance. Then,     * use the methods inherited from the      * AnimatorFactoryBase class to associate the desired properties with display objects.</p>     * @playerversion Flash 10     * @playerversion AIR 1.5          * @langversion 3.0     * @see fl.motion.Animator     * @see fl.motion.AnimatorFactoryBase     * @see fl.motion.Motion     * @see fl.motion.MotionBase
	 */
	public class AnimatorFactory3D extends AnimatorFactoryBase
	{
		/**
		 * Creates an AnimatorFactory3D instance you can use to assign the properties of     * a MotionBase object to display objects.      * @param motion The MotionBase object containing the desired three-dimensional motion properties.     * .     * @playerversion Flash 10     * @playerversion AIR 1.5           * @langversion 3.0     * @see fl.motion.Animator     * @see fl.motion.AnimatorFactoryBase     * @see fl.motion.Motion     * @see fl.motion.MotionBase
		 */
		public function AnimatorFactory3D (motion:MotionBase);
		/**
		 * @private
		 */
		protected function getNewAnimator () : AnimatorBase;
	}
}
