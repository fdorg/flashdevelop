package fl.transitions.easing
{
	/**
	 * The Elastic class defines three easing functions to implement  * motion with ActionScript animation, where the motion is defined by  * an exponentially decaying sine wave.  * * @playerversion Flash 9.0 * @langversion 3.0 * @keyword Ease, Transition     * @see fl.transitions.TransitionManager
	 */
	public class Elastic
	{
		/**
		 * The <code>easeIn()</code> method starts motion slowly      * and then accelerates motion as it executes.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @param a Specifies the amplitude of the sine wave.     *     * @param p Specifies the period of the sine wave.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Elastic.easeIn.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeIn (t:Number, b:Number, c:Number, d:Number, a:Number = 0, p:Number = 0) : Number;
		/**
		 * The <code>easeOut()</code> method starts motion fast      * and then decelerates motion as it executes.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @param a Specifies the amplitude of the sine wave.     *     * @param p Specifies the period of the sine wave.     *     * @return The value of the interpolated property at the specified time.     *     * @includeExample examples/Elastic.easeOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition         * @see fl.transitions.TransitionManager
		 */
		public static function easeOut (t:Number, b:Number, c:Number, d:Number, a:Number = 0, p:Number = 0) : Number;
		/**
		 * The <code>easeInOut()</code> method combines the motion     * of the <code>easeIn()</code> and <code>easeOut()</code> methods     * to start the motion slowly, accelerate motion, then decelerate.      *     * @param t Specifies the current time, between 0 and duration inclusive.     *     * @param b Specifies the initial value of the animation property.     *     * @param c Specifies the total change in the animation property.     *     * @param d Specifies the duration of the motion.     *     * @param a Specifies the amplitude of the sine wave.     *     * @param p Specifies the period of the sine wave.     *     * @return The value of the interpolated property at the specified time.     *     * @see fl.transitions.TransitionManager       *     * @includeExample examples/Elastic.easeInOut.1.as -noswf     *     * @playerversion Flash 9.0     * @langversion 3.0     * @keyword Ease, Transition
		 */
		public static function easeInOut (t:Number, b:Number, c:Number, d:Number, a:Number = 0, p:Number = 0) : Number;
	}
}
