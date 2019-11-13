package fl.motion
{
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import flash.utils.Dictionary;
	import flash.display.SimpleButton;

	/**
	 * The AnimatorFactoryBase class provides ActionScript-based support to display and tween multiple targeted objects with one Motion dynamically at runtime.      * AnimatorFactoryBase uses the AnimatorBase class to assign one Motion (derived from MotionBase)      * to multiple tween instances (targeted objects)      * whereas the AnimatorBase class associates a single Motion instance with a single targeted tween object.       * The AnimatorFactoryBase class should not be used on its own. Use its subclasses: AnimatorFactory or AnimatorFactory3D, instead.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator     * @see fl.motion.AnimatorFactory
	 */
	public class AnimatorFactoryBase
	{
		private var _motion : MotionBase;
		private var _animators : Dictionary;
		/**
		 * @private
		 */
		protected var _transformationPoint : Point;
		protected var _transformationPointZ : int;
		/**
		 * @private
		 */
		protected var _is3D : Boolean;
		/**
		 * @private
		 */
		protected var _sceneName : String;

		/**
		 * The <code>MotionBase</code> instance that the <code>AnimatorFactoryBase</code> instance and its target objects are associated with.	* The <code>MotionBase</code> instance stores the animated properties and their values.	* @category Property[read-only]	* @playerversion Flash 9.0.28.0	* @langversion 3.0	* @see fl.motion.Motion
		 */
		public function get motion () : MotionBase;
		/**
		 * The point of reference for rotating or scaling a display object.         * The <code>transformationPoint</code> property (or setter) is overridden in the <code>AnimatorFactory3D</code> subclass,          * In 3D, the points are not percentages like they are in 2D; they are absolute values of the original object's transformation point.         * @playerversion Flash 10         * @playerversion AIR 1.5         * @langversion 3.0
		 */
		public function set transformationPoint (p:Point) : void;
		public function set transformationPointZ (z:int) : void;
		public function set sceneName (name:String) : void;

		/**
		 * Creates an instance of the <code>AnimatorFactoryBase</code> class.         *          * @param motion The associated MotionBase instance.         * @playerversion Flash 9.0.28.0         * @langversion 3.0
		 */
		public function AnimatorFactoryBase (motion:MotionBase);
		/**
		 * Creates and returns an <code>AnimatorBase</code> instance whose target property is set to the <code>DisplayObject</code> (if applicable)	* that is the <code>targetName</code> property of the <code>targetParent</code>,	* and whose <code>Motion</code> property is stored in the <code>AnimatorFactoryBase</code> instance upon creation.	*	* @param target The display object using the motion tween.	* @param repeatCount The number of times the animation should play. The default value is 0, which means the animation will loop indefinitely.	* @param autoPlay The value (default is true) specifying whether the animation automatically begins to play.	* @param startFrame The frame on which the animation starts relative to the parent's timeline.	* If the parent's timeline is shorter than the duration of the associated Motion, 	* then startFrame indicates the number of frames after this <code>addTarget</code> call is made before the target animation begins.	* @param useCurrentFrame A flag specifying, if true, to use the parent's <code>currentFrame</code> property 	* to determine which animation frame the target object should be on. 	* @return A new AnimatorBase instance.	* @playerversion Flash 9.0.28.0	* @langversion 3.0        * @see #addTargetInfo()	* @see fl.motion.Animator	* @see fl.motion.Motion        * @see flash.display.DisplayObject
		 */
		public function addTarget (target:DisplayObject, repeatCount:int = 0, autoPlay:Boolean = true, startFrame:int = -1, useCurrentFrame:Boolean = false) : AnimatorBase;
		/**
		 * @private
		 */
		protected function getNewAnimator () : AnimatorBase;
		/**
		 * References the parent <code>DisplayObjectContainer</code> and then creates and returns an <code>AnimatorBase</code> 	* instance whose target property is set to the <code>DisplayObject</code> (if applicable) 	* that is the <code>targetName</code> property of the <code>targetParent</code>,	* and whose <code>Motion</code> property is stored in the <code>AnimatorFactoryBase</code> instance upon creation.	*	* @param targetParent The parent DisplayObjectContainer.	* @param targetName The target's instance name as seen by its parent.	* @param repeatCount The number of times the animation should play. The default value is 0, which means the animation will loop indefinitely.	* @param autoPlay The value (default is true) specifying whether the animation automatically begins to play.	* @param startFrame The frame on which the animation starts relative to the parent's timeline.	* If the parent's timeline is shorter than the duration of the associated Motion, 	* then startFrame indicates the number of frames after this <code>addTarget</code> call is made before the target animation begins.	* @param useCurrentFrame A flag specifying, if true, to use the parent's <code>currentFrame</code> property 	* to determine which animation frame the target object should be on. 	* @return A new AnimatorBase instance.	* @playerversion Flash 9.0.28.0	* @langversion 3.0        * @see #addTarget()	* @see fl.motion.Animator	* @see fl.motion.Motion        * @see flash.display.DisplayObjectContainer
		 */
		public function addTargetInfo (targetParent:DisplayObject, targetName:String, repeatCount:int = 0, autoPlay:Boolean = true, startFrame:int = -1, useCurrentFrame:Boolean = false, initialPosition:Array = null) : AnimatorBase;
	}
}
