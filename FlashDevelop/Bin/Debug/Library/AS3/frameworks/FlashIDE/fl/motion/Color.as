package fl.motion
{
	import fl.motion.*;
	import flash.display.*;
	import flash.geom.ColorTransform;

	/**
	 * The Color class extends the Flash Player ColorTransform class, * adding the ability to control brightness and tint. * It also contains static methods for interpolating between two ColorTransform objects * or between two color numbers.  * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Color, Copy Motion as ActionScript     * @see flash.geom.ColorTransform ColorTransform class * @see ../../motionXSD.html Motion XML Elements
	 */
	public class Color extends ColorTransform
	{
		/**
		 * @private
		 */
		private var _tintColor : Number;
		/**
		 * @private
		 */
		private var _tintMultiplier : Number;

		/**
		 * The percentage of brightness, as a decimal between <code>-1</code> and <code>1</code>.      * Positive values lighten the object, and a value of <code>1</code> turns the object completely white.     * Negative values darken the object, and a value of <code>-1</code> turns the object completely black.      *      * @default 0     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword brightness, Copy Motion as ActionScript
		 */
		public function get brightness () : Number;
		/**
		 * @private (setter)
		 */
		public function set brightness (value:Number) : void;
		/**
		 * The tinting color value in the 0xRRGGBB format.     *      *      * @default 0x000000 (black)     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword tint, Copy Motion as ActionScript
		 */
		public function get tintColor () : uint;
		/**
		 * @private (setter)
		 */
		public function set tintColor (value:uint) : void;
		/**
		 * The percentage to apply the tint color, as a decimal value between <code>0</code> and <code>1</code>.     * When <code>tintMultiplier = 0</code>, the target object is its original color and no tint color is visible.     * When <code>tintMultiplier = 1</code>, the target object is completely tinted and none of its original color is visible.     * @default 0     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword tint, Copy Motion as ActionScript
		 */
		public function get tintMultiplier () : Number;
		/**
		 * @private (setter)
		 */
		public function set tintMultiplier (value:Number) : void;

		/**
		 * Constructor for Color instances.     *     * @param redMultiplier The percentage to apply the color, as a decimal value between 0 and 1.     * @param greenMultiplier The percentage to apply the color, as a decimal value between 0 and 1.     * @param blueMultiplier The percentage to apply the color, as a decimal value between 0 and 1.     * @param alphaMultiplier A decimal value that is multiplied with the alpha transparency channel value, as a decimal value between 0 and 1.     * @param redOffset A number from -255 to 255 that is added to the red channel value after it has been multiplied by the <code>redMultiplier</code> value.      * @param greenOffset A number from -255 to 255 that is added to the green channel value after it has been multiplied by the <code>greenMultiplier</code> value.      * @param blueOffset A number from -255 to 255 that is added to the blue channel value after it has been multiplied by the <code>blueMultiplier</code> value.      * @param alphaOffset A number from -255 to 255 that is added to the alpha channel value after it has been multiplied by the <code>alphaMultiplier</code> value.     *     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Color
		 */
		function Color (redMultiplier:Number = 1.0, greenMultiplier:Number = 1.0, blueMultiplier:Number = 1.0, alphaMultiplier:Number = 1.0, redOffset:Number = 0, greenOffset:Number = 0, blueOffset:Number = 0, alphaOffset:Number = 0);
		/**
		 * Sets the tint color and amount at the same time.     *     * @param tintColor The tinting color value in the 0xRRGGBB format.     *     * @param tintMultiplier The percentage to apply the tint color, as a decimal value between <code>0</code> and <code>1</code>.     * When <code>tintMultiplier = 0</code>, the target object is its original color and no tint color is visible.     * When <code>tintMultiplier = 1</code>, the target object is completely tinted and none of its original color is visible.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword brightness, Copy Motion as ActionScript
		 */
		public function setTint (tintColor:uint, tintMultiplier:Number) : void;
		/**
		 * @private
		 */
		private function deriveTintColor () : uint;
		/**
		 * Creates a Color instance from XML.     *     * @param xml An E4X XML object containing a <code>&lt;color&gt;</code> node from Motion XML.     *     * @return A Color instance that matches the XML description.      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword color, Copy Motion as ActionScript
		 */
		public static function fromXML (xml:XML) : Color;
		/**
		 * @private
		 */
		private function parseXML (xml:XML = null) : Color;
		/**
		 * Blends smoothly from one ColorTransform object to another.     *     * @param fromColor The starting ColorTransform object.     *     * @param toColor The ending ColorTransform object.     *     * @param progress The percent of the transition as a decimal, where <code>0</code> is the start and <code>1</code> is the end.     *     * @return The interpolated ColorTransform object.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword blend, Copy Motion as ActionScript
		 */
		public static function interpolateTransform (fromColor:ColorTransform, toColor:ColorTransform, progress:Number) : ColorTransform;
		/**
		 * Blends smoothly from one color value to another.     *     * @param fromColor The starting color value, in the 0xRRGGBB or 0xAARRGGBB format.     *     * @param toColor The ending color value, in the 0xRRGGBB or 0xAARRGGBB format.     *     * @param progress The percent of the transition as a decimal, where <code>0</code> is the start and <code>1</code> is the end.     *     * @return The interpolated color value, in the 0xRRGGBB or 0xAARRGGBB format.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @refpath     * @keyword blend, Copy Motion as ActionScript
		 */
		public static function interpolateColor (fromColor:uint, toColor:uint, progress:Number) : uint;
	}
}
