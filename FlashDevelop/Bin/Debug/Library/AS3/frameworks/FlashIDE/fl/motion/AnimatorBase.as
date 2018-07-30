package fl.motion
{
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.EventDispatcher;
	import flash.geom.ColorTransform;
	import flash.geom.Matrix;
	import flash.geom.Point;
	import flash.display.SimpleButton;
	import flash.utils.Dictionary;
	import flash.display.MovieClip;

	/**
	 *  Dispatched when the motion finishes playing, *  either when it reaches the end, or when the motion is  *  interrupted by a call to the <code>stop()</code> or <code>end()</code> methods. * *  @eventType fl.motion.MotionEvent.MOTION_END * @playerversion Flash 9.0.28.0 * @langversion 3.0
	 */
	[Event(name="motionEnd", type="fl.motion.MotionEvent")] 
	/**
	 *  Dispatched when the motion starts playing. * *  @eventType fl.motion.MotionEvent.MOTION_START * @playerversion Flash 9.0.28.0 * @langversion 3.0
	 */
	[Event(name="motionStart", type="fl.motion.MotionEvent")] 
	/**
	 *  Dispatched when the motion has changed and the screen has been updated. * *  @eventType fl.motion.MotionEvent.MOTION_UPDATE * @playerversion Flash 9.0.28.0 * @langversion 3.0
	 */
	[Event(name="motionUpdate", type="fl.motion.MotionEvent")] 
	/**
	 *  Dispatched when the Animator's <code>time</code> value has changed,  *  but the screen has not yet been updated (i.e., the <code>motionUpdate</code> event). *  *  @eventType fl.motion.MotionEvent.TIME_CHANGE * @playerversion Flash 9.0.28.0 * @langversion 3.0
	 */
	[Event(name="timeChange", type="fl.motion.MotionEvent")] 

	/**
	 * The AnimatorBase class applies an XML description of a motion tween to a display object. * The properties and methods of the AnimatorBase class control the playback of the motion, * and Flash Player broadcasts events in response to changes in the motion's status. * The AnimatorBase class is primarily used by the Copy Motion as ActionScript command in Flash CS4. * You can then edit the ActionScript using the application programming interface * (API) or construct your own custom animation.  * The AnimatorBase class should not be used on its own. Use its subclasses: Animator or Animator3D, instead. * * <p>If you plan to call methods of the AnimatorBase class within a function, declare the AnimatorBase  * instance outside of the function so the scope of the object is not restricted to the  * function itself. If you declare the instance within a function, Flash Player deletes the  * AnimatorBase instance at the end of the function as part of Flash Player's routine "garbage collection" * and the target object will not animate.</p> *  * @internal <p><strong>Note:</strong> If you're not using Flash CS4 to compile your SWF file, you need the * fl.motion classes in your classpath at compile time to apply the motion to the display object.</p> * * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword Animator, Copy Motion as ActionScript * @see ../../motionXSD.html Motion XML Elements
	 */
	public class AnimatorBase extends EventDispatcher
	{
		/**
		 * @private
		 */
		private var _motion : MotionBase;
		/**
		 * Sets the position of the display object along the motion path. If set to <code>true</code>     * the baseline of the display object orients to the motion path; otherwise the registration     * point orients to the motion path.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword orientToPath, orientation
		 */
		public var orientToPath : Boolean;
		/**
		 * The point of reference for rotating or scaling a display object. For 2D motion, the transformation point is      * relative to the display object's bounding box. The point's coordinates must be scaled to a 1px x 1px box, where (1, 1) is the object's lower-right corner,      * and (0, 0) is the object's upper-left corner. For 3Dmotion (when the AnimatorBase instance is an Animator3D), the transformationPoint's x and y plus the transformationPointZ are     * absolute values in the target parent's coordinate space.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword transformationPoint
		 */
		public var transformationPoint : Point;
		public var transformationPointZ : int;
		/**
		 * Sets the animation to restart after it finishes.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword autoRewind, loop
		 */
		public var autoRewind : Boolean;
		/**
		 * The Matrix object that applies an overall transformation to the motion path.      * This matrix allows the path to be shifted, scaled, skewed or rotated,      * without changing the appearance of the display object.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword positionMatrix
		 */
		public var positionMatrix : Matrix;
		/**
		 *  Number of times to repeat the animation.     *  Possible values are any integer greater than or equal to <code>0</code>.     *  A value of <code>1</code> means to play the animation once.     *  A value of <code>0</code> means to play the animation indefinitely     *  until explicitly stopped (by a call to the <code>end()</code> method, for example).     *     * @default 1     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword repeatCount, repetition, loop        * @see #end()
		 */
		public var repeatCount : int;
		/**
		 * @private
		 */
		private var _isPlaying : Boolean;
		/**
		 * @private
		 */
		protected var _target : DisplayObject;
		/**
		 * @private
		 */
		private var _lastRenderedTime : int;
		/**
		 * @private
		 */
		private var _time : int;
		private var _targetParent : DisplayObjectContainer;
		private var _targetName : String;
		private var targetStateOriginal : Object;
		private var _useCurrentFrame : Boolean;
		private var _spanStart : int;
		private var _sceneName : String;
		private static var _registeredParents : Dictionary;
		private var _frameEvent : String;
		private var _targetState3D : Array;
		/**
		 * @private
		 */
		protected var _isAnimator3D : Boolean;
		/**
		 * @private
		 */
		private var playCount : int;
		/**
		 * @private
		 */
		private static var enterFrameBeacon : MovieClip;
		/**
		 * @private
		 */
		protected var targetState : Object;

		/**
		 * The object that contains the motion tween properties for the animation.      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword motion
		 */
		public function get motion () : MotionBase;
		/**
		 * @private (setter)
		 */
		public function set motion (value:MotionBase) : void;
		/**
		 * Indicates whether the animation is currently playing.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword isPlaying
		 */
		public function get isPlaying () : Boolean;
		/**
		 * The display object being animated.      * Any subclass of flash.display.DisplayObject can be used, such as a <code>MovieClip</code>, <code>Sprite</code>, or <code>Bitmap</code>.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword target     * @see flash.display.DisplayObject
		 */
		public function get target () : DisplayObject;
		/**
		 * @private (setter)
		 */
		public function set target (value:DisplayObject) : void;
		public function set initialPosition (initPos:Array) : void;
		/**
		 * A zero-based integer that indicates and controls the time in the current animation.      * At the animation's first frame the <code>time</code> value is <code>0</code>.      * If the animation has a duration of 10 frames, at the last frame the <code>time</code> value is <code>9</code>.      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword time
		 */
		public function get time () : int;
		/**
		 * @private (setter)
		 */
		public function set time (newTime:int) : void;
		/**
		 * The target parent <code>DisplayObjectContainer</code> being animated, which can be used in conjunction with <code>targetName</code>     * to retrieve the target object after it is removed and then replaced on the timeline.     * @playerversion Flash 9.0.28.0     * @langversion 3.0
		 */
		public function get targetParent () : DisplayObjectContainer;
		public function set targetParent (p:DisplayObjectContainer) : void;
		/**
		 * The name of the target object as seen by the parent <code>DisplayObjectContainer</code>.     * This can be used in conjunction with <code>targetParent</code> to retrieve the target object after it is removed and then replaced on the timeline.     * @playerversion Flash 9.0.28.0     * @langversion 3.0
		 */
		public function get targetName () : String;
		public function set targetName (n:String) : void;
		/**
		 * Indicates whether the <code>currentFrame</code> property is checked whenever a new frame is entered and	* whether the target's animation is synchronized to the frames in its parent's timeline, 	* or always advancing no matter what the parent's current frame is.	* @playerversion Flash 9.0.28.0	* @langversion 3.0
		 */
		public function get usingCurrentFrame () : Boolean;
		/**
		 * Returns the frame of the target's parent on which the animation of the target begins.	* @playerversion Flash 9.0.28.0	* @langversion 3.0
		 */
		public function get spanStart () : int;
		/**
		 * Returns the frame of the target's parent on which the animation of the target ends. 	* This value is determined using <code>spanStart</code> and the motion's <code>duration</code> property.	* @playerversion Flash 9.0.28.0	* @langversion 3.0
		 */
		public function get spanEnd () : int;
		public function set sceneName (name:String) : void;
		public function get sceneName () : String;
		private static function get hasRegisteredParents () : Boolean;
		public function get frameEvent () : String;
		public function set frameEvent (evt:String) : void;
		/**
		 * The initial orientation for the target object. All 3D rotation is absolute to the motion data.     * If you target another object that has a different starting 3D orientation, it is reset to this target state first.     * @playerversion Flash 10     * @playerversion AIR 1.5     * @langversion 3.0
		 */
		public function get targetState3D () : Array;
		public function set targetState3D (state:Array) : void;
		private function get enterFrameHandler () : Function;

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
		protected function setTimeClassic (newTime:int, thisMotion:MotionBase, curKeyframe:KeyframeBase) : Boolean;
		/**
		 * Sets the <code>currentFrame</code> property whenever a new frame is entered, and     * sets whether the target's animation is synchronized to the frames in its parent MovieClips's timeline.     * <code>spanStart</code> is the start frame of the animation in terms of the parent's timeline.      * If <code>enable</code> is <code>true</code>, then in any given enter frame event within the span of the animation,      * the <code>time</code> property is set to a frame number relative to the <code>spanStart</code> frame.     *     * <p>For example, if a 4-frame animation starts on frame 5 (<code>spanStart=5</code>),      * and you have a script on frame 5 to <code>gotoAndPlay</code> frame 8,     * then upon entering frame 8 the time property is set to <code>3</code> (skipping <code>time = 1</code>     * and <code>time = 2</code>).</p>     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @param enable The true or false value that determines whether the currentFrame property is checked.     * @param spanStart The start frame of the animation in terms of the parent MovieClip's timeline.
		 */
		public function useCurrentFrame (enable:Boolean, spanStart:int) : void;
		/**
		 * Registers the given <code>MovieClip</code> and an <code>AnimatorBase</code> instance for a child of that <code>MovieClip</code>.	* The parent MovieClip's <code>FRAME_CONSTRUCTED</code> events are processed,	* and its <code>currentFrame</code> and the AnimatorBase's <code>spanStart</code> properties 	* are used to determine the current relative frame of the animation that should be playing. 	* <p>Calling this function automatically sets the AnimatorBase's <code>useCurrentFrame</code> property to <code>true</code>,	* and its <code>spanStart</code> property using the parameter of the same name.</p>	* @playerversion Flash 9.0.28.0	* @langversion 3.0        * @param parent The parent MovieClip of the AnimatorBase instance.        * @param anim The AnimatorBase instance associated with the parent MovieClip.        * @param spanStart The start frame of the animation in terms of the parent MovieClip's timeline.        * @param repeatCount The number of times the animation should play. The default value is 0, which means the animation will loop indefinitely.        * @param useCurrentFrame Indicates whether the useCurrentFrame property is checked whenever a new frame is entered.
		 */
		public static function registerParentFrameHandler (parent:MovieClip, anim:AnimatorBase, spanStart:int, repeatCount:int = 0, useCurrentFrame:Boolean = false) : void;
		private static function parentEnterFrameHandler (evt:Event) : void;
		public static function processCurrentFrame (parent:MovieClip, anim:AnimatorBase, startEnterFrame:Boolean, playOnly:Boolean = false) : void;
		public static function registerButtonState (targetParentBtn:SimpleButton, anim:AnimatorBase, stateFrame:int) : void;
		/**
		 * Creates an AnimatorBase object to apply the XML-based motion tween description to a display object.     * If XML is null (which is the default value), then you can either supply XML directly to a Motion instance      * or you can set the arrays of property values in the Motion instance.     * @param xml An E4X object containing an XML-based motion tween description.     *     * @param target The display object using the motion tween.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword AnimatorBase     * @see ../../motionXSD.html Motion XML Elements
		 */
		function AnimatorBase (xml:XML = null, target:DisplayObject = null);
		/**
		 * Advances Flash Player to the next frame in the animation sequence.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword nextFrame
		 */
		public function nextFrame () : void;
		/**
		 *  Begins the animation. Call the <code>end()</code> method      *  before you call the <code>play()</code> method to ensure that any previous      *  instance of the animation has ended before you start a new one.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @param startTime Indicates an alternate start time (relative frame) to use. If not specified, then the default start time of 0 is used.     * @param startEnterFrame Indicates whether the event listener needs to be added to the parent in order to capture frame events.      * The value can be <code>false</code> if the parent was registered to its AnimatorBase instance via <code>registerParentFrameHandler()</code>.     * @keyword play, begin     * @see #end()
		 */
		public function play (startTime:int = -1, startEnterFrame:Boolean = true) : void;
		/**
		 *  Stops the animation and Flash Player goes immediately to the last frame in the animation sequence.      *  If the <code>autoRewind</code> property is set to <code>true</code>, Flash Player goes to the first     * frame in the animation sequence.      * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @param reset Indicates whether <code>_lastRenderedTime</code> and <code>_target</code> should be reset to their original values.      * <code>_target</code> only resets if <code>targetParent</code> and <code>targetName</code> have been supplied.     * @keyword end, stop     * @see #autoRewind
		 */
		public function end (reset:Boolean = false, stopEnterFrame:Boolean = true) : void;
		/**
		 *  Stops the animation and Flash Player goes back to the first frame in the animation sequence.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword stop, end     * @see #end()
		 */
		public function stop () : void;
		/**
		 *  Pauses the animation until you call the <code>resume()</code> method.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword pause     * @see #resume()
		 */
		public function pause () : void;
		/**
		 *  Resumes the animation after it has been paused      *  by the <code>pause()</code> method.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword resume     * @see #pause()
		 */
		public function resume () : void;
		public function startFrameEvents () : void;
		/**
		 * Sets Flash Player to the first frame of the animation.      * If the animation was playing, it continues playing from the first frame.      * If the animation was stopped, it remains stopped at the first frame.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @keyword rewind
		 */
		public function rewind () : void;
		/**
		 * @private
		 */
		private function handleLastFrame (reset:Boolean = false, stopEnterFrame:Boolean = true) : void;
		/**
		 * @private
		 */
		private function handleEnterFrame (event:Event) : void;
	}
	internal class AnimatorParent
	{
		public var parent : MovieClip;
		public var animators : Array;
		public var lastFrameHandled : int;

	}
}
