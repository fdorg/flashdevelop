package fl.motion
{
	import flash.geom.Point;

	/**
	 * A Bezier segment consists of four Point objects that define a single cubic Bezier curve. * The BezierSegment class also contains methods to find coordinate values along the curve. * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword BezierSegment, Copy Motion as ActionScript * @see ../../motionXSD.html Motion XML Elements
	 */
	public class BezierSegment
	{
		/**
		 * The first point of the Bezier curve.     * It is a node, which means it falls directly on the curve.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public var a : Point;
		/**
		 * The second point of the Bezier curve.      * It is a control point, which means the curve moves toward it,     * but usually does not pass through it.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public var b : Point;
		/**
		 * The third point of the Bezier curve.      * It is a control point, which means the curve moves toward it,     * but usually does not pass through it.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public var c : Point;
		/**
		 * The fourth point of the Bezier curve.     * It is a node, which means it falls directly on the curve.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public var d : Point;

		/**
		 * Constructor for BezierSegment instances.     *     * @param a The first point of the curve, a node.     *     * @param b The second point of the curve, a control point.     *     * @param c The third point of the curve, a control point.     *     * @param d The fourth point of the curve, a node.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript        * @see #propertyDetail property details
		 */
		function BezierSegment (a:Point, b:Point, c:Point, d:Point);
		/**
		 * Calculates the location of a two-dimensional cubic Bezier curve at a specific time.     *     * @param t The <code>time</code> or degree of progress along the curve, as a decimal value between <code>0</code> and <code>1</code>.     * <p><strong>Note:</strong> The <code>t</code> parameter does not necessarily move along the curve at a uniform speed. For example, a <code>t</code> value of <code>0.5</code> does not always produce a value halfway along the curve.</p>     *     * @return A point object containing the x and y coordinates of the Bezier curve at the specified time.      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public function getValue (t:Number) : Point;
		/**
		 * Calculates the value of a one-dimensional cubic Bezier equation at a specific time.     * By contrast, a Bezier curve is usually two-dimensional      * and uses two of these equations, one for the x coordinate and one for the y coordinate.     *     * @param t The <code>time</code> or degree of progress along the curve, as a decimal value between <code>0</code> and <code>1</code>.     * <p><strong>Note:</strong> The <code>t</code> parameter does not necessarily move along the curve at a uniform speed. For example, a <code>t</code> value of <code>0.5</code> does not always produce a value halfway along the curve.</p>     *     * @param a The first value of the Bezier equation.     *     * @param b The second value of the Bezier equation.     *     * @param c The third value of the Bezier equation.     *     * @param d The fourth value of the Bezier equation.     *     * @return The value of the Bezier equation at the specified time.      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public static function getSingleValue (t:Number, a:Number = 0, b:Number = 0, c:Number = 0, d:Number = 0) : Number;
		/**
		 * Finds the <code>y</code> value of a cubic Bezier curve at a given x coordinate.     * Some Bezier curves overlap themselves horizontally,      * resulting in more than one <code>y</code> value for a given <code>x</code> value.     * In that case, this method will return whichever value is most logical.     *      * Used by CustomEase and BezierEase interpolation.     *     * @param x An x coordinate that lies between the first and last point, inclusive.     *      * @param coefficients An optional array of number values that represent the polynomial     * coefficients for the Bezier. This array can be used to optimize performance by precalculating      * values that are the same everywhere on the curve and do not need to be recalculated for each iteration.     *      * @return The <code>y</code> value of the cubic Bezier curve at the given x coordinate.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, Copy Motion as ActionScript
		 */
		public function getYForX (x:Number, coefficients:Array = null) : Number;
		/**
		 * Calculates the coefficients for a cubic polynomial equation,     * given the values of the corresponding cubic Bezier equation.     *     * @param a The first value of the Bezier equation.     *     * @param b The second value of the Bezier equation.     *     * @param c The third value of the Bezier equation.     *     * @param d The fourth value of the Bezier equation.     *     * @return An array containing four number values,     * which are the coefficients for a cubic polynomial.     * The coefficients are ordered from the highest degree to the lowest,     * so the first number in the array would be multiplied by t^3, the second by t^2, and so on.     *      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript             * @see #getCubicRoots()
		 */
		public static function getCubicCoefficients (a:Number, b:Number, c:Number, d:Number) : Array;
		/**
		 * Finds the real solutions, if they exist, to a cubic polynomial equation of the form: at^3 + bt^2 + ct + d.     * This method is used to evaluate custom easing curves.     *     * @param a The first coefficient of the cubic equation, which is multiplied by the cubed variable (t^3).     *     * @param b The second coefficient of the cubic equation, which is multiplied by the squared variable (t^2).     *     * @param c The third coefficient of the cubic equation, which is multiplied by the linear variable (t).     *     * @param d The fourth coefficient of the cubic equation, which is the constant.     *     * @return An array of number values, indicating the real roots of the equation.      * There may be no roots, or as many as three.      * Imaginary or complex roots are ignored.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public static function getCubicRoots (a:Number = 0, b:Number = 0, c:Number = 0, d:Number = 0) : Array;
		/**
		 * Finds the real solutions, if they exist, to a quadratic equation of the form: at^2 + bt + c.     *     * @param a The first coefficient of the quadratic equation, which is multiplied by the squared variable (t^2).     *     * @param b The second coefficient of the quadratic equation, which is multiplied by the linear variable (t).     *     * @param c The third coefficient of the quadratic equation, which is the constant.     *     * @return An array of number values, indicating the real roots of the equation.      * There may be no roots, or as many as two.      * Imaginary or complex roots are ignored.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Bezier curve, node, Copy Motion as ActionScript
		 */
		public static function getQuadraticRoots (a:Number, b:Number, c:Number) : Array;
	}
}
