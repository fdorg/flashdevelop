package fl.motion
{
	import flash.geom.Point;

	/**
	 * The BezierEase class provides precise easing control for a motion tween between two keyframes.  * You can apply an instance of this class to all properties of a keyframe at once,  * or you can define different curves for different properties. * <p>Both this class and the CustomEase class use one or more cubic Bezier curves to define the interpolation. * However, the BezierEase class defines its coordinates slightly differently than the CustomEase class.</p> * * <p>The BezierEase class uses literal values for the y coordinates of the curve, rather than  * normalized values between <code>0</code> and <code>1</code>. * This allows you to create curves that cannot be created with custom easing. For example,  * you can create a curve where the start and end values are identical, but the curve rises and falls  * in between those values. Also, depending on the context, you may want to define the easing  * curve with literal values instead of percentages.</p> * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword BezierEase, Copy Motion as ActionScript * @see ../../motionXSD.html Motion XML Elements  * @see CustomEase CustomEase class
	 */
	public class BezierEase implements ITween
	{
		/**
		 * An ordered collection of points in the custom easing curve.      * Each item in the array is a <code>flash.geom.Point</code> instance, with <code>x</code> and <code>y</code> properties.      * <p>The x coordinate of each point represents the time coordinate of the ease, as a percentage.      * The x value is normalized to fall between <code>0</code> and <code>1</code>,      * where <code>0</code> is the beginning of the tween and <code>1</code> is the end of the tween.      * The y coordinate of each point contains the literal value of the animation property at that point      * in the ease. The y value is <i>not</i> normalized to fall between <code>0</code> and <code>1</code>.</p>       * <p>The first and last points of the curve are not included in the array,      * because the first point is locked to the starting value, defined by the current keyframe,      * and the last point is locked to the ending value, defined by the next keyframe.</p>       * @playerversion Flash 9.0.28.0      * @langversion 3.0      * @keyword points            * @see flash.geom.Point
		 */
		public var points : Array;
		/**
		 * @private
		 */
		private var firstNode : Point;
		/**
		 * @private
		 */
		private var lastNode : Point;
		/**
		 * @private
		 */
		private var _target : String;

		/**
		 * The name of the animation property to target.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword target                * @see fl.motion.ITween#target     * @default ""
		 */
		public function get target () : String;
		/**
		 * @private (setter)
		 */
		public function set target (value:String) : void;

		/**
		 * Constructor for BezierEase instances.     *     * @param xml Optional E4X XML object defining a BezierEase in Motion XML format.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword BezierEase
		 */
		function BezierEase (xml:XML = null);
		/**
		 * @private
		 */
		private function parseXML (xml:XML = null) : BezierEase;
		/**
		 * Calculates an interpolated value for a numerical property of animation,     * using a Bezier easing curve.      * The percent value is read from the BezierEase instance's <code>points</code> property,     * rather than being passed into the method.     * Using the <code>points</code> property value allows the function signature to match the ITween interface.     *     * @param time The time value, which must be between <code>0</code> and <code>duration</code>, inclusive.     * The unit can be freely chosen (for example, frames, seconds, milliseconds),      * but must match the <code>duration</code> unit.	 *     * @param begin The value of the animation property at the start of the tween, when time is <code>0</code>.     *     * @param change The change in the value of the animation property over the course of the tween.      * This value can be positive or negative. For example, if an object rotates from 90 to 60 degrees, the <code>change</code> is <code>-30</code>.     *     * @param duration The length of time for the tween. This value must be greater than zero.     * The unit can be freely chosen (for example, frames, seconds, milliseconds),      * but must match the <code>time</code> unit.     *     * @return The interpolated value at the specified time.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword BezierEase            * @see #points
		 */
		public function getValue (time:Number, begin:Number, change:Number, duration:Number) : Number;
	}
}
