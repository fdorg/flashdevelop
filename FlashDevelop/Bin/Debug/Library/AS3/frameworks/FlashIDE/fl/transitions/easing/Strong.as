package fl.transitions.easing
{
	/**
	 * The Strong class defines three easing functions to implement  * motion with ActionScript animation. The acceleration of motion for a Strong easing * equation is greater than for a Regular easing equation. * The Strong class is identical to the fl.motion.easing.Quintic class in functionality. * * @playerversion Flash 9.0 * @langversion 3.0 * @keyword Ease, Transition     * @see fl.transitions.TransitionManager
	 */
	public class Strong
	{
		/**
		 * The <code>easeIn()</code> method starts motion from zero velocity      * and then accelerates motion as it executes.      *     * @param t Specifies the current time, between 0 and duration inclusive.	 *     * @param b Specifies the initial value of the animation property.	 *     * @param c Specifies the total change in the animation property.	 *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Strong.easeIn.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeIn (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeOut()</code> method starts motion fast      * and then decelerates motion to a zero velocity as it executes.      *     * @param t Specifies the current time, between 0 and duration inclusive.	 *     * @param b Specifies the initial value of the animation property.	 *     * @param c Specifies the total change in the animation property.	 *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Strong.easeOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeOut (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeInOut()</code> method combines the motion     * of the <code>easeIn()</code> and <code>easeOut()</code> methods	 * to start the motion from a zero velocity, accelerate motion, 	 * then decelerate to a zero velocity.      *     * @param t Specifies the current time, between 0 and duration inclusive.	 *     * @param b Specifies the initial value of the animation property.	 *     * @param c Specifies the total change in the animation property.	 *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Strong.easeInOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeInOut (t:Number, b:Number, c:Number, d:Number) : Number;
	}
}
