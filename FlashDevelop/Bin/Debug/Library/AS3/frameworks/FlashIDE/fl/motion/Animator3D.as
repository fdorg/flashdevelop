package fl.motion
{
	import flash.display.DisplayObject;
	import flash.geom.Matrix;
	import flash.geom.Matrix3D;
	import flash.geom.Vector3D;
	import __AS3__.vec.Vector;
	import flash.geom.Point;

	/**
	 * The Animator3D class applies an XML description of a three-dimensional motion tween to a display object. * The properties and methods of the Animator3D class control the playback of the motion, * and Flash Player broadcasts events in response to changes in the motion's status. * If there isn't any three-dimensional content in the motion tween, the Copy Motion as ActionScript command in * Flash CS4 uses the Animator class. For three-dimensional content, the Animator3D class is used, instead, which shares the  * same base class as the Animator class but is specifically for three-dimensional content.  * You can then edit the ActionScript using the application programming interface * (API) or construct your own custom animation. * <p>If you plan to call methods of the Animator3D class within a function, declare the Animator3D  * instance outside of the function so the scope of the object is not restricted to the  * function itself. If you declare the instance within a function, Flash Player deletes the  * Animator instance at the end of the function as part of Flash Player's routine "garbage collection" * and the target object will not animate.</p> *  * @internal <p><strong>Note:</strong> If you're not using Flash CS4 to compile your SWF file, you need the * fl.motion classes in your classpath at compile time to apply the motion to the display object.</p> * * @playerversion Flash 10 * @playerversion AIR 1.5  * @langversion 3.0 * @keyword Animator, Copy Motion as ActionScript * @see ../../motionXSD.html Motion XML Elements
	 */
	public class Animator3D extends AnimatorBase
	{
		private var _initialPosition : Vector3D;
		private var _initialMatrixOfTarget : Matrix3D;
		/**
		 * @private
		 */
		protected static const EPSILON : Number = 0.00000001;

		/**
		 * Establishes, the x-, y-, and z- coordinates of the display object.     *     * @param xml An array containing the x, y, and z coordinates of the object, in order, of the display     * object at the start of the motion.     *     * @param target The display object using the motion tween.     * @playerversion Flash 10	 * @playerversion AIR 1.5          * @langversion 3.0     * @keyword AnimatorBase     * @see ../../motionXSD.html Motion XML Elements
		 */
		public function set initialPosition (initPos:Array) : void;

		/**
		 * Creates an Animator3D object to apply the XML-based motion tween description in three dimensions to a display object.     *     * @param xml An E4X object containing an XML-based motion tween description.     *     * @param target The display object using the motion tween.     * @playerversion Flash 10	 * @playerversion AIR 1.5          * @langversion 3.0     * @keyword AnimatorBase     * @see ../../motionXSD.html Motion XML Elements
		 */
		public function Animator3D (xml:XML = null, target:DisplayObject = null);
		/**
		 * @private
		 */
		protected function setTargetState () : void;
		/**
		 * @private
		 */
		protected function setTime3D (newTime:int, thisMotion:MotionBase) : Boolean;
		/**
		 * @private
		 */
		private function getScaleSkewMatrix (thisMotion:MotionBase, newTime:int, positionX:Number, positionY:Number) : Matrix;
		/**
		 * @private
		 */
		protected static function getSign (n:Number) : int;
		protected static function convertMatrixToMatrix3D (mat2D:Matrix) : Matrix3D;
	}
}
