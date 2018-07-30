package fl.motion.easing
{
	/**
	 *  The Quartic class defines three easing functions to implement  *  motion with ActionScript animation. The acceleration of motion for a Quartic easing *  equation is greater than for a Cubic easing equation. * * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Ease, Copy Motion as ActionScript     * @see ../../../motionXSD.html Motion XML Elements  * @see fl.motion.FunctionEase
	 */
	public class Quartic
	{
		/**
		 *  The <code>easeIn()</code> method starts motion from zero velocity      *  and then accelerates motion as it executes.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeIn (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 *  The <code>easeOut()</code> method starts motion fast      *  and then decelerates motion to a zero velocity as it executes.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeOut (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 *  The <code>easeInOut()</code> method combines the motion     *  of the <code>easeIn()</code> and <code>easeOut()</code> methods	 *  to start the motion from a zero velocity, accelerate motion, 	 *  then decelerate to a zero velocity.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeInOut (t:Number, b:Number, c:Number, d:Number) : Number;
	}
}
