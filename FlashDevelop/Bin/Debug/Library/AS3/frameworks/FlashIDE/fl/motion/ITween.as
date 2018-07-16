package fl.motion
{
	/**
	 * The ITween interface defines the application programming interface (API) that interpolation * classes implement in order to work with the fl.motion classes. * The SimpleEase, CustomEase, BezierEase, and FunctionEase classes implement the ITween interface.  * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Ease, Copy Motion as ActionScript     * @see ../../motionXSD.html Motion XML Elements
	 */
	public interface ITween
	{
		/**
		 * The name of the animation property to target.     * <p>The default value is <code>""</code> (empty quotes), which targets all properties.     * The other acceptable values are <code>"x"</code>, <code>"y"</code>, <code>"position"</code> (which targets both x and y),     * <code>"scaleX"</code>, <code>"scaleY"</code>, <code>"scale"</code> (which targets both scaleX and scaleY),     * <code>"skewX"</code>, <code>"skewY"</code>, <code>"rotation"</code>  (which targets both scaleX and scaleY), <code>"color"</code>, and <code>"filters"</code>.</p>     *      * @default ""     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword ITween, Copy Motion as ActionScript
		 */
		public function get target () : String;
		/**
		 * @private (setter)
		 */
		public function set target (value:String) : void;

		/**
		 * Calculates an interpolated value for a numerical property of animation.     * The function signature matches that of the easing functions in the fl.motion.easing package.     * Various ITween classes will produce different styles of interpolation for the same inputs.     *     * @param time The time value, which must be between <code>0</code> and <code>duration</code>, inclusive.     * You can choose any unit (for example, frames, seconds, milliseconds),      * but your choice must match the <code>duration</code> unit.	 *     * @param begin The value of the animation property at the start of the tween, when time is <code>0</code>.     *     * @param change The change in the value of the animation property over the course of the tween.      * This value can be positive or negative. For example, if an object rotates from 90 to 60 degrees, the <code>change</code> is <code>-30</code>.     *     * @param duration The length of time for the tween. This value must be greater than zero.     * You can choose any unit (for example, frames, seconds, milliseconds),      * but your choice must match the <code>time</code> unit.     *     * @return The interpolated value at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword ITween, Copy Motion as ActionScript           * @see fl.motion.easing
		 */
		public function getValue (time:Number, begin:Number, change:Number, duration:Number) : Number;
	}
}
