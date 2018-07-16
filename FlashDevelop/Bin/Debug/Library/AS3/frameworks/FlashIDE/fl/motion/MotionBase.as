package fl.motion
{
	import flash.filters.*;
	import flash.geom.ColorTransform;
	import flash.utils.*;

	/**
	 * The MotionBase class stores a keyframe animation sequence that can be applied to a visual object. * The animation data includes position, scale, rotation, skew, color, filters, and easing. * The MotionBase class has methods for retrieving data at specific keyframe points. To get * interpolated values between keyframes, use the Motion class.  * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Motion, Copy Motion as ActionScript     * @see fl.motion.Motion Motion class * @see ../../motionXSD.html Motion XML Elements
	 */
	public class MotionBase
	{
		/**
		 * An array of keyframes that define the motion's behavior over time.     * This property is a sparse array, where a keyframe is placed at an index in the array     * that matches its own index. A motion object with keyframes at 0 and 5 has      * a keyframes array with a length of 6.       * Indices 0 and 5 in the array each contain a keyframe,      * and indices 1 through 4 have null values.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		public var keyframes : Array;
		/**
		 * @private
		 */
		private var _duration : int;
		private var _is3D : Boolean;
		/**
		 * Indicates whether property values for scale, skew, and rotate added in subsequent	 * calls to addPropertyArray should be made relative to the first value, or used as-is,	 * such that they override the target object's initial target transform.
		 */
		private var _overrideScale : Boolean;
		private var _overrideSkew : Boolean;
		private var _overrideRotate : Boolean;

		/**
		 * Controls the Motion instance's length of time, measured in frames.     * The duration cannot be less than the time occupied by the Motion instance's keyframes.     * @default 0     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		public function get duration () : int;
		/**
		 * @private (setter)
		 */
		public function set duration (value:int) : void;
		/**
		 * Specifies whether the motion contains 3D property changes. If <code>true</code>, the      * motion contains 3D property changes.      * @default false     * @playerversion Flash 10     * @playerversion AIR 1.5     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		public function get is3D () : Boolean;
		/**
		 * Sets flag that indicates whether the motion contains 3D properties.      * @playerversion Flash 10     * @playerversion AIR 1.5          * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		public function set is3D (enable:Boolean) : void;

		/**
		 * Constructor for MotionBase instances.     * By default, one initial keyframe is created automatically, with default transform properties.     *     * @param xml Optional E4X XML object defining a Motion instance.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript
		 */
		function MotionBase (xml:XML = null);
		public function overrideTargetTransform (scale:Boolean = true, skew:Boolean = true, rotate:Boolean = true) : void;
		/**
		 * @private
		 */
		private function indexOutOfRange (index:int) : Boolean;
		/**
		 * Retrieves the keyframe that is currently active at a specific frame in the Motion instance.	 * A frame that is not a keyframe derives its values from the keyframe that preceded it.  	 * 	 * <p>This method can also filter values by the name of a specific tweenables property.	 * You can find the currently active keyframe for <code>x</code>, which may not be	 * the same as the currently active keyframe in general.</p>	 * 	 * @param index The index of a frame in the Motion instance, as an integer greater than or equal to zero.	 *      * @param tweenableName Optional name of a tweenable's property (like <code>"x"</code> or <code>"rotation"</code>).	 * 	 * @return The closest matching keyframe at or before the supplied frame index.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript           * @see fl.motion.Tweenables
		 */
		public function getCurrentKeyframe (index:int, tweenableName:String = '') : KeyframeBase;
		/**
		 * Retrieves the next keyframe after a specific frame in the Motion instance.	 * If a frame is not a keyframe, and is in the middle of a tween, 	 * this method derives its values from both the preceding keyframe and the following keyframe.	 * 	 * <p>This method also allows you to filter by the name of a specific tweenables property     * to find the next keyframe for a property, which might not be	 * the same as the next keyframe in general.</p>	 * 	 * @param index The index of a frame in the Motion instance, as an integer greater than or equal to zero.	 *      * @param tweenableName Optional name of a tweenable's property (like <code>"x"</code> or <code>"rotation"</code>).	 * 	 * @return The closest matching keyframe after the supplied frame index.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript           * @see fl.motion.Tweenables
		 */
		public function getNextKeyframe (index:int, tweenableName:String = '') : KeyframeBase;
		/**
		 * Sets the value of a specific tweenables property at a given time index in the Motion instance.     * If a keyframe doesn't exist at the index, one is created automatically.     *	 * @param index The time index of a frame in the Motion instance, as an integer greater than zero.	 * If the index is zero, no change is made. 	 * Transformation properties are relative to the starting transformation values of the target object,	 * the values for the first frame (zero index value) are always default values and should not be changed.     *     * @param tweenableName The name of a tweenable's property as a string (like <code>"x"</code> or <code>"rotation"</code>).     *     * @param value The new value of the tweenable property.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript           * @see fl.motion.Tweenables
		 */
		public function setValue (index:int, tweenableName:String, value:Number) : void;
		/**
		 * Retrieves an interpolated ColorTransform object at a specific time index in the Motion instance.     *	 * @param index The time index of a frame in the Motion instance, as an integer greater than or equal to zero.     *     * @return The interpolated ColorTransform object.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.geom.ColorTransform
		 */
		public function getColorTransform (index:int) : ColorTransform;
		/**
		 * Returns the Matrix3D object for the specified index position of     * the frame of animation.      * @param index The zero-based index position of the frame of animation containing the 3D matrix.     * @return The Matrix3D object, or null value. This method can return a null value even if      * <code>MotionBase.is3D</code> is <code>true</code>, because other 3D motion tween property changes can be used     * without a Matrix3D object.     * @playerversion Flash 10     * @playerversion AIR 1.5          * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript      * @see flash.geom.Matrix3D
		 */
		public function getMatrix3D (index:int) : Object;
		/**
		 * Rotates the target object when data for the motion is supplied by the <code>addPropertyArray()</code> method.     * @playerversion Flash 10     * @playerversion AIR 1.5     * @langversion 3.0     * @param index The index position of the frame of animation.     * @return Indicates whether the target object will rotate using the stored property from      * <code>KeyframeBase.rotationConcat</code>.     * @keyword Motion, Copy Motion as ActionScript      * @see #addPropertyArray()     * @see fl.motion.KeyframeBase#rotationConcat
		 */
		public function useRotationConcat (index:int) : Boolean;
		/**
		 * Retrieves an interpolated array of filters at a specific time index in the Motion instance.     *	 * @param index The time index of a frame in the Motion, as an integer greater than or equal to zero.     *     * @return The interpolated array of filters.      * If there are no applicable filters, returns an empty array.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.filters
		 */
		public function getFilters (index:Number) : Array;
		/**
		 * @private
		 */
		protected function findTweenedValue (index:Number, tweenableName:String, curKeyframeBase:KeyframeBase, timeFromKeyframe:Number, begin:Number) : Number;
		/**
		 * Retrieves the value for an animation property at a point in time.     *	 * @param index The time index of a frame in the Motion instance, as an integer greater than or equal to zero.     *     * @param tweenableName The name of a tweenable's property (like <code>"x"</code> or <code>"rotation"</code>).     * @return The number value for the property specified in the <code>tweenableName</code> parameter.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see fl.motion.Tweenables
		 */
		public function getValue (index:Number, tweenableName:String) : Number;
		/**
		 * Adds a keyframe object to the Motion instance.      *     * @param newKeyframe A keyframe object with an index property already set.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see fl.motion.Keyframe
		 */
		public function addKeyframe (newKeyframe:KeyframeBase) : void;
		/**
		 * Stores an array of values in corresponding keyframes for a declared property of the Motion class.     * The order of the values in the array determines the assignment of each value to a keyframe. For each     * non-null value in the given <code>values</code> array, this method finds the keyframe      * corresponding to the value's index position in the array, or creates a new keyframe for that index     * position, and stores the property name/value pair in the keyframe.      *      * @param name The name of the Motion class property to store in each keyframe.     *     * @param values The array of values for the property specified in the <code>name</code>     * parameter. Each non-null value is assigned to a keyframe that corresponds to the value's      * order in the array.     *      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see fl.motion.Motion
		 */
		public function addPropertyArray (name:String, values:Array, startFrame:int = -1, endFrame:int = -1) : void;
		/**
		 * Initializes the filters list for the target object and copies the list of filters to each Keyframe     * instance of the Motion object.      *      * @param filterClasses An array of filter classes. Each item in the array is the fully qualified      * class name (in String form) for the filter type occupying that index.     *     * @param gradientSubarrayLengths An array of numbers containing a value for every filter that will be in the filters      * list for the motion (every class name in the <code>filterClasses</code> array). A value in the      * <code>gradientSubarrayLengths</code> array is only used if the filter class entry at the same index position in the      * <code>filterClasses</code> array is GradientGlowFilter or GradientBevelFilter.     * The corresponding value in the <code>gradientSubarrayLengths</code> array is a number that determines the length for the arrays      * that initialize the <code>colors</code>, <code>alphas</code>, and <code>ratios</code> parameters for the      * GradientGlowFilter and GradientBevelFilter constructor functions.     *     *      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript       * @see flash.filters           * @see flash.filters.GradientGlowFilter     * @see flash.filters.GradientBevelFilter
		 */
		public function initFilters (filterClasses:Array, gradientSubarrayLengths:Array, startFrame:int = -1, endFrame:int = -1) : void;
		/**
		 * Modifies a filter property in all corresponding keyframes for a Motion object. Call <code>initFilters()</code> before     * using this method. The order of the values in the array determines the assignment of each value     * to the filter property for all keyframes. For each non-null value in the specified <code>values</code>     * array, this method finds the keyframe corresponding to the value's index position in the array,     * and stores the property name/value pair for the filter in the keyframe.      *      * @param index The zero-based index position in the array of filters.     *     * @param name The name of the filter property to store in each keyframe.     *     * @param values The array of values for the property specified in the <code>name</code>     * parameter. Each non-null value is assigned to the filter in a keyframe that corresponds to      * the value's index in the array.     *      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Motion, Copy Motion as ActionScript      * @see #initFilters()     * @see flash.filters
		 */
		public function addFilterPropertyArray (index:int, name:String, values:Array, startFrame:int = -1, endFrame:int = -1) : void;
		/**
		 * @private
		 */
		protected function getNewKeyframe (xml:XML = null) : KeyframeBase;
	}
}
