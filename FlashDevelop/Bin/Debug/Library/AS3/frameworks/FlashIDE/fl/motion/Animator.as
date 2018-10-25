package fl.motion
{
	import flash.display.DisplayObject;
	import flash.geom.Matrix;
	import flash.geom.Point;

	/**
	 * The Animator class applies an XML description of a motion tween to a display object. * The properties and methods of the Animator class control the playback of the motion, * and Flash Player broadcasts events in response to changes in the motion's status. * If there isn't any three-dimensional content, the Copy Motion as ActionScript command in * Flash CS4 uses the Animator class. For three-dimensional content, the Animator3D class is used, instead, which shares the  * same base class as the Animator class but is specifically for three-dimensional content. * You can then edit the ActionScript using the application programming interface * (API) or construct your own custom animation. * <p>If you plan to call methods of the Animator class within a function, declare the Animator  * instance outside of the function so the scope of the object is not restricted to the  * function itself. If you declare the instance within a function, Flash Player deletes the  * Animator instance at the end of the function as part of Flash Player's routine "garbage collection" * and the target object will not animate.</p> *  * @internal <p><strong>Note:</strong> If you're not using Flash CS4 to compile your SWF file, you need the * fl.motion classes in your classpath at compile time to apply the motion to the display object.</p> * * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Animator, Copy Motion as ActionScript * @see ../../motionXSD.html Motion XML Elements
	 */
	public class Animator extends AnimatorBase
	{
		/**
		 * @private (setter)
		 */
		public function set motion (value:MotionBase) : void;

		/**
		 * @private
		 */
		protected function setTargetState () : void;
		/**
		 * @private
		 */
		protected function setTimeClassic (newTime:int, thisMotion:MotionBase, curKeyframe:KeyframeBase) : Boolean;
		/**
		 * Creates an Animator object to apply the XML-based motion tween description to a display object.     *     * @param xml An E4X object containing an XML-based motion tween description.     *     * @param target The display object using the motion tween.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword AnimatorBase     * @see ../../motionXSD.html Motion XML Elements
		 */
		function Animator (xml:XML = null, target:DisplayObject = null);
		/**
		 * Creates an Animator object from a string of XML.      * This method is an alternative to using the Animator constructor, which accepts an E4X object instead.     *     * @param xmlString A string of XML describing the motion tween.     *     * @param target The display object using the motion tween.     *     * @return An Animator instance that applies the specified <code>xmlString</code> to the specified <code>target</code>.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword createFromXMLString, Animator     * @see ../../motionXSD.html Motion XML Elements
		 */
		public static function fromXMLString (xmlString:String, target:DisplayObject = null) : Animator;
	}
}
