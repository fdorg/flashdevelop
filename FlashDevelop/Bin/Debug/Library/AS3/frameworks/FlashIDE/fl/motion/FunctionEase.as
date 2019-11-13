package fl.motion
{
	import flash.utils.*;

	/**
	 * The FunctionEase class allows custom interpolation functions to be used with * the fl.motion framework in place of other interpolations like SimpleEase and CustomEase.  * The fl.motion framework includes several easing functions in the fl.motion.easing package. *    * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Ease, Copy Motion as ActionScript     * @includeExample examples\FunctionEaseExample.as -noswf  * @see ../../motionXSD.html Motion XML Elements  * @see fl.motion.easing
	 */
	public class FunctionEase implements ITween
	{
		/**
		 * @private
		 */
		private var _functionName : String;
		/**
		 * A reference to a function with a <code>(t, b, c, d)</code> signature like     * the methods in the fl.motion.easing classes.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Easing, Copy Motion as ActionScript         * @see fl.motion.easing
		 */
		public var easingFunction : Function;
		/**
		 * An optional array of values to be passed to the easing function as additional arguments.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Easing, Copy Motion as ActionScript
		 */
		public var parameters : Array;
		/**
		 * @private
		 */
		private var _target : String;

		/**
		 * The fully qualified name of an easing function, such as <code>fl.motion.easing.Bounce.easeOut()</code>.     * The function must be a method of a class (Bounce, Cubic, Elastic, another class).     * If Flash Player cannot find the class, an exception is thrown.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Easing, Copy Motion as ActionScript         * @see fl.motion.easing
		 */
		public function get functionName () : String;
		/**
		 * @private (setter)
		 */
		public function set functionName (newName:String) : void;
		/**
		 * The name of the animation property to target.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Easing, Copy Motion as ActionScript         * @see fl.motion.ITween#target
		 */
		public function get target () : String;
		/**
		 * @private (setter)
		 */
		public function set target (value:String) : void;

		/**
		 * Constructor for FunctionEase instances.     *     * @param xml An optional E4X XML instance.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Easing, Copy Motion as ActionScript       * @see ../../motionXSD.html Motion XML Elements
		 */
		function FunctionEase (xml:XML = null);
		/**
		 * @private
		 */
		private function parseXML (xml:XML = null) : FunctionEase;
		/**
		 * Calculates an interpolated value for a numerical property of animation,     * using the specified easing function.      * If the <code>parameters</code> array has been set beforehand,     * those values will be passed to the easing function in addition to the      * time, begin, change, and duration values.      *     * @param time The time value, which must lie between <code>0</code> and <code>duration</code>, inclusive.     * You can choose any unit (for example, frames, seconds, milliseconds),      * but your choice must match the <code>duration</code> unit.	 *     * @param begin The value of the animation property at the start of the tween, when time is 0.     *     * @param change The change in the value of the animation property over the course of the tween.      * The value can be positive or negative. For example, if an object rotates from 90 to 60 degrees, the <code>change</code> is -30.     *     * @param duration The length of time for the tween. Must be greater than zero.     * You can choose any unit (for example, frames, seconds, milliseconds),      * but your choice must match the <code>time</code> unit.     *     * @return The interpolated value at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Easing, Copy Motion as ActionScript
		 */
		public function getValue (time:Number, begin:Number, change:Number, duration:Number) : Number;
	}
}
