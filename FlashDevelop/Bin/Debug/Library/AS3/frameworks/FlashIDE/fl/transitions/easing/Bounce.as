package fl.transitions.easing
{
	/**
	 * The Bounce class defines three easing functions to implement  * bouncing motion with ActionScript animation, similar to a ball * falling and bouncing on a floor with several decaying rebounds. * * @see fl.transitions.TransitionManager  * * @playerversion Flash 9.0 * @langversion 3.0 * @keyword Ease, Transition
	 */
	public class Bounce
	{
		/**
		 * The <code>easeOut()</code> method starts the bounce motion fast      * and then decelerates motion as it executes.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Bounce.easeOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeOut (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeIn()</code> method starts the bounce motion slowly      * and then accelerates motion as it executes.           *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Bounce.easeIn.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeIn (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeInOut()</code> method combines the motion     * of the <code>easeIn()</code> and <code>easeOut()</code> methods     * to start the bounce motion slowly, accelerate motion, then decelerate.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Bounce.easeInOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeInOut (t:Number, b:Number, c:Number, d:Number) : Number;
	}
}
