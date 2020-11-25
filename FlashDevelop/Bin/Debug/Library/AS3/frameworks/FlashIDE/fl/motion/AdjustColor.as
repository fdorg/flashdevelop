package fl.motion
{
	/**
	 * The AdjustColor class defines various color properties, such as brightness, contrast, hue, and saturation, to support the ColorMatrixFilter class.  * You can apply the AdjustColor filter to any display object,  * and also generate a flat array representing all four color properties to use with the ColorMatrixFilter class. * @playerversion Flash 9 * @langversion 3.0 * @keyword AdjustColor * @see flash.filters.ColorMatrixFilter
	 */
	public class AdjustColor
	{
		private static var s_arrayOfDeltaIndex : Array;
		private var m_brightnessMatrix : ColorMatrix;
		private var m_contrastMatrix : ColorMatrix;
		private var m_saturationMatrix : ColorMatrix;
		private var m_hueMatrix : ColorMatrix;
		private var m_finalMatrix : ColorMatrix;

		/**
		 * Sets the brightness of the AdjustColor filter. The range of valid values is <code>-100</code> to <code>100</code>.	     * @playerversion Flash 9	     * @langversion 3.0
		 */
		public function set brightness (value:Number) : void;
		/**
		 * Sets the contrast of the AdjustColor filter. The range of valid values is <code>-100</code> to <code>100</code>.	     * @playerversion Flash 9	     * @langversion 3.0
		 */
		public function set contrast (value:Number) : void;
		/**
		 * Sets the saturation of the AdjustColor filter. The range of valid values is <code>-100</code> to <code>100</code>.	     * @playerversion Flash 9	     * @langversion 3.0
		 */
		public function set saturation (value:Number) : void;
		/**
		 * Sets the hue of the AdjustColor filter. The range of valid values is <code>-180</code> to <code>180</code>.	     * @playerversion Flash 9	     * @langversion 3.0
		 */
		public function set hue (value:Number) : void;

		/**
		 * The AdjustColor class defines various color properties to support the ColorMatrixFilter.     * @playerversion Flash 9     * @langversion 3.0     * @see flash.filters.ColorMatrixFilter
		 */
		public function AdjustColor ();
		/**
		 * Verifies if all four AdjustColor properties are set. 	     * @return A Boolean value that is <code>true</code> if all four AdjustColor properties have been set, <code>false</code> otherwise.	     * @playerversion Flash 9	     * @langversion 3.0
		 */
		public function AllValuesAreSet () : Boolean;
		/**
		 * Returns the flat array of values for all four properties.	     * @return An array of 20 numerical values representing all four AdjustColor properties	     * to use with the <code>flash.filters.ColorMatrixFilter</code> class.	     * @playerversion Flash 9	     * @langversion 3.0	     * @see flash.filters.ColorMatrixFilter
		 */
		public function CalculateFinalFlatArray () : Array;
		private function CalculateFinalMatrix () : Boolean;
	}
}
