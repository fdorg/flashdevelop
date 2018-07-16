package fl.motion.easing
{
	/**
	 * The Linear class defines easing functions to implement  * non-accelerated motion with ActionScript animations.  * Its methods all produce the same effect, a constant motion. * The various names <code>easeIn</code>, <code>easeOut</code> and so on, * are provided in the interest of polymorphism. *  * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Ease, Copy Motion as ActionScript     * @see ../../../motionXSD.html Motion XML Elements  * @see fl.motion.FunctionEase
	 */
	public class Linear
	{
		/**
		 *  The <code>easeNone()</code> method defines a constant motion      *  with no acceleration.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeNone (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 *  The <code>easeIn()</code> method defines a constant motion      *  with no acceleration.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeIn (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 *  The <code>easeOut()</code> method defines a constant motion      *  with no acceleration.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeOut (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 *  The <code>easeInOut()</code> method defines a constant motion      *  with no acceleration.      *     *  @param t Specifies the current time, between 0 and duration inclusive.	 *     *  @param b Specifies the initial value of the animation property.	 *     *  @param c Specifies the total change in the animation property.	 *     *  @param d Specifies the duration of the motion.     *     *  @return The value of the interpolated property at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Ease, Copy Motion as ActionScript         * @see fl.motion.FunctionEase
		 */
		public static function easeInOut (t:Number, b:Number, c:Number, d:Number) : Number;
	}
}
