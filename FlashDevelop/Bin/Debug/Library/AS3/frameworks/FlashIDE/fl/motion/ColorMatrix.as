package fl.motion
{
	/**
	 * The ColorMatrix class calculates and stores color matrixes based on given values.      * This class extends the DynamicMatrix class and also supports the ColorMatrixFilter class.     * @playerversion Flash 9     * @langversion 3.0     * @see fl.motion.DynamicMatrix     * @see flash.filters.ColorMatrixFilter
	 */
	public class ColorMatrix extends DynamicMatrix
	{
		/**
		 * @private
		 */
		protected static const LUMINANCER : Number = 0.3086;
		/**
		 * @private
		 */
		protected static const LUMINANCEG : Number = 0.6094;
		/**
		 * @private
		 */
		protected static const LUMINANCEB : Number = 0.0820;

		/**
		 * Calculates and stores color matrixes based on given values.     * @playerversion Flash 9     * @langversion 3.0     * @see DynamicMatrix
		 */
		public function ColorMatrix ();
		/**
		 * Calculates and stores a brightness matrix based on the given value.	     * @playerversion Flash 9	     * @langversion 3.0	     * @param value 0-255
		 */
		public function SetBrightnessMatrix (value:Number) : void;
		/**
		 * Calculates and stores a contrast matrix based on the given value.	     * @playerversion Flash 9	     * @langversion 3.0	     * @param value 0-255
		 */
		public function SetContrastMatrix (value:Number) : void;
		/**
		 * Calculates and stores a saturation matrix based on the given value.	     * @playerversion Flash 9	     * @langversion 3.0	     * @param value 0-255
		 */
		public function SetSaturationMatrix (value:Number) : void;
		/**
		 * Calculates and stores a hue matrix based on the given value.	     * @playerversion Flash 9	     * @langversion 3.0	     * @param value 0-255
		 */
		public function SetHueMatrix (angle:Number) : void;
		/**
		 * Calculates and returns a flat array of 20 numerical values representing the four matrixes set in this object.	     * @playerversion Flash 9	     * @langversion 3.0	     * @return An array of 20 items.
		 */
		public function GetFlatArray () : Array;
	}
	internal class XFormData
	{
		public var ox : Number;
		public var oy : Number;
		public var oz : Number;

	}
}
