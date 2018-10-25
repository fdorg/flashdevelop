package fl.transitions.easing
{
	/**
	 * The None class defines easing functions to implement  * nonaccelerated motion with ActionScript animations.  * Its methods all produce the same effect, a constant motion. * The various names, <code>easeIn</code>, <code>easeOut</code> and so on * are provided in the interest of polymorphism. * The None class is identical to the fl.motion.easing.Linear class in functionality. *  * @playerversion Flash 9.0 * @langversion 3.0 * @keyword Ease, Transition     * @see fl.transitions.TransitionManager
	 */
	public class None
	{
		/**
		 * The <code>easeNone()</code> method defines a constant motion,      * with no acceleration.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeNone (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeIn()</code> method defines a constant motion,      * with no acceleration.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/None.easeIn.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeIn (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeOut()</code> method defines a constant motion,      * with no acceleration.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/None.easeOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeOut (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * The <code>easeInOut()</code> method defines a constant motion,      * with no acceleration.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/None.easeInOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeInOut (t:Number, b:Number, c:Number, d:Number) : Number;
	}
}
