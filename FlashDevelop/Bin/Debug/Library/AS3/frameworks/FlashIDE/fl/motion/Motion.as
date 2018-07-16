package fl.motion
{
	import flash.filters.*;
	import flash.geom.ColorTransform;
	import flash.utils.*;

	/**
	 * The Motion class stores a keyframe animation sequence that can be applied to a visual object. * The animation data includes position, scale, rotation, skew, color, filters, and easing. * The Motion class has methods for retrieving data at specific points in time, and * interpolating values between keyframes automatically.  * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Motion, Copy Motion as ActionScript     * @see ../../motionXSD.html Motion XML Elements
	 */
	public class Motion extends MotionBase
	{
		/**
		 * An object that stores information about the context in which the motion was created,     * such as frame rate, dimensions, transformation point, and initial position, scale, rotation, and skew.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		public var source : Source;
		/**
		 * @private
		 */
		private var _keyframesCompact : Array;
		/**
		 *  @private
		 */
		private static var typeCache : Object;

		/**
		 * A compact array of keyframes, where each index is occupied by a keyframe.      * By contrast, a sparse array has empty indices (as in the <code>keyframes</code> property).      * In the compact array, no <code>null</code> values are used to fill indices between keyframes.     * However, the index of a keyframe in <code>keyframesCompact</code> likely does not match its index in the <code>keyframes</code> array.     * <p>This property is primarily used for compatibility with the Flex MXML compiler,     * which generates a compact array from the motion XML.</p>     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript      * @see #keyframes
		 */
		public function get keyframesCompact () : Array;
		/**
		 * @private (setter)
		 */
		public function set keyframesCompact (compactArray:Array) : void;

		/**
		 * Constructor for Motion instances.     * By default, one initial keyframe is created automatically, with default transform properties.     *     * @param xml Optional E4X XML object defining a Motion instance.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		function Motion (xml:XML = null);
		/**
		 * Retrieves an interpolated ColorTransform object at a specific time index in the Motion instance.     *	 * @param index The time index of a frame in the Motion instance, as an integer greater than or equal to zero.     *     * @return The interpolated ColorTransform object.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.geom.ColorTransform
		 */
		public function getColorTransform (index:int) : ColorTransform;
		/**
		 * Retrieves an interpolated array of filters at a specific time index in the Motion instance.     *	 * @param index The time index of a frame in the Motion instance, as an integer greater than or equal to zero.     *     * @return The interpolated array of filters.      * If there are no applicable filters, returns an empty array.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.filters
		 */
		public function getFilters (index:Number) : Array;
		/**
		 * @private
		 */
		protected function findTweenedValue (index:Number, tweenableName:String, curKeyframeBase:KeyframeBase, timeFromKeyframe:Number, begin:Number) : Number;
		/**
		 * @private
		 */
		private function parseXML (xml:XML) : Motion;
		/**
		 * A method needed to create a Motion instance from a string of XML.     *     * @param xmlString A string of motion XML.     *     * @return A new Motion instance.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		public static function fromXMLString (xmlString:String) : Motion;
		/**
		 * Blends filters smoothly from one array of filter objects to another.     *      * @param fromFilters The starting array of filter objects.     *     * @param toFilters The ending array of filter objects.     *     * @param progress The percent of the transition as a decimal, where <code>0</code> is the start and <code>1</code> is the end.     *      * @return The interpolated array of filter objects.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.filters
		 */
		public static function interpolateFilters (fromFilters:Array, toFilters:Array, progress:Number) : Array;
		/**
		 * Blends filters smoothly from one filter object to another.     *      * @param fromFilters The starting filter object.     *     * @param toFilters The ending filter object.     *     * @param progress The percent of the transition as a decimal, where <code>0</code> is the start and <code>1</code> is the end.     *      * @return The interpolated filter object.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.filters
		 */
		public static function interpolateFilter (fromFilter:BitmapFilter, toFilter:BitmapFilter, progress:Number) : BitmapFilter;
		/**
		 * @private
		 */
		private static function getTypeInfo (o:*) : XML;
		/**
		 * @private
		 */
		protected function getNewKeyframe (xml:XML = null) : KeyframeBase;
	}
}
