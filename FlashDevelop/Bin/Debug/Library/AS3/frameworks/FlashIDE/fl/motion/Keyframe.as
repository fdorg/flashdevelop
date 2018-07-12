package fl.motion
{
	import flash.display.BlendMode;
	import flash.filters.BitmapFilter;
	import flash.filters.ColorMatrixFilter;
	import flash.utils.*;

	/**
	 * The Keyframe class defines the visual state at a specific time in a motion tween. * The primary animation properties are <code>position</code>, <code>scale</code>, <code>rotation</code>, <code>skew</code>, and <code>color</code>. * A keyframe can, optionally, define one or more of these properties. * For instance, one keyframe may affect only position,  * while another keyframe at a different point in time may affect only scale. * Yet another keyframe may affect all properties at the same time. * Within a motion tween, each time index can have only one keyframe.  * A keyframe also has other properties like <code>blend mode</code>, <code>filters</code>, and <code>cacheAsBitmap</code>, * which are always available. For example, a keyframe always has a blend mode.    * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Keyframe, Copy Motion as ActionScript     * @see ../../motionXSD.html Motion XML Elements * @see fl.motion.KeyframeBase
	 */
	public class Keyframe extends KeyframeBase
	{
		/**
		 * An array that contains each tween object to be applied to the target object at a particular keyframe.     * One tween can target all animation properties (as with standard tweens on the Flash authoring tool's timeline),     * or multiple tweens can target individual properties (as with separate custom easing curves).     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Keyframe, Copy Motion as ActionScript
		 */
		public var tweens : Array;
		/**
		 * A flag that controls whether scale will be interpolated during a tween.     * If <code>false</code>, the display object will stay the same size during the tween, until the next keyframe.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Keyframe, Copy Motion as ActionScript
		 */
		public var tweenScale : Boolean;
		/**
		 * Stores the value of the Snap checkbox for motion tweens, which snaps the object to a motion guide.      * This property is used in the Copy and Paste Motion feature in Flash CS4      * but does not affect motion tweens defined using ActionScript.      * It is included here for compatibility with the Flex 2 compiler.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Keyframe, Copy Motion as ActionScript
		 */
		public var tweenSnap : Boolean;
		/**
		 * Stores the value of the Sync checkbox for motion tweens, which affects graphic symbols only.      * This property is used in the Copy and Paste Motion feature in Flash CS4      * but does not affect motion tweens defined using ActionScript.      * It is included here for compatibility with the Flex 2 compiler.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Keyframe, Copy Motion as ActionScript
		 */
		public var tweenSync : Boolean;

		public function get tweensLength () : int;

		/**
		 * Constructor for keyframe instances.     *     * @param xml Optional E4X XML object defining a keyframe in Motion XML format.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Keyframe, Copy Motion as ActionScript
		 */
		function Keyframe (xml:XML = null);
		/**
		 * @private
		 */
		private function parseXML (xml:XML = null) : KeyframeBase;
		/**
		 * @private
		 */
		private static function splitNumber (valuesString:String) : Array;
		/**
		 * @private
		 */
		private static function splitUint (valuesString:String) : Array;
		/**
		 * @private
		 */
		private static function splitInt (valuesString:String) : Array;
		/**
		 * Retrieves an ITween object for a specific animation property.     *     * @param target The name of the property being tweened.     * @see fl.motion.ITween#target     *     * @return An object that implements the ITween interface.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword Keyframe, Copy Motion as ActionScript
		 */
		public function getTween (target:String = '') : ITween;
		/**
		 * @private
		 */
		protected function hasTween () : Boolean;
	}
}
