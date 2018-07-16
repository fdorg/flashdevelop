package fl.transitions
{
	import flash.events.EventDispatcher;
	import flash.display.*;
	import flash.geom.*;
	import flash.events.Event;

	/**
	 * The Transition class is the base class for all transition classes. You do not use or access this class directly. It allows transition-based classes to share some common behaviors and properties that are accessed by an instance of the TransitionManager class.     * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition     * @see fl.transitions.TransitionManager * @see fl.transitions.Tween * @see fl.transitions.easing
	 */
	public class Transition extends EventDispatcher
	{
		/**
		 * Constant for the <code>direction</code> property that determines the type of easing. * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition
		 */
		public static const IN : uint = 0;
		/**
		 * Constant for the direction property that determines the type of easing. * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition
		 */
		public static const OUT : uint = 1;
		/**
		 * @private
		 */
		public var ID : int;
		/**
		 * @private
		 */
		protected var _content : MovieClip;
		/**
		 * @private
		 */
		protected var _manager : TransitionManager;
		/**
		 * @private
		 */
		protected var _direction : uint;
		/**
		 * @private
		 */
		protected var _duration : Number;
		/**
		 * @private
		 */
		protected var _easing : Function;
		/**
		 * @private
		 */
		protected var _progress : Number;
		/**
		 * @private
		 */
		protected var _innerBounds : Rectangle;
		/**
		 * @private
		 */
		protected var _outerBounds : Rectangle;
		/**
		 * @private
		 */
		protected var _width : Number;
		/**
		 * @private
		 */
		protected var _height : Number;
		/**
		 * @private
		 */
		protected var _twn : Tween;

		/**
		 * @private
		 */
		public function get type () : Class;
		/**
		 * @private
		 */
		public function set manager (mgr:TransitionManager) : void;
		/**
		 * @private
		 */
		public function get manager () : TransitionManager;
		/**
		 * @private
		 */
		public function set content (c:MovieClip) : void;
		/**
		 * @private
		 */
		public function get content () : MovieClip;
		/**
		 * Determines the easing direction for the Tween instance. Use one of the constants from * the Transition class: <code>Transition.IN</code> or <code>Transition.OUT</code>. * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition
		 */
		public function set direction (direction:Number) : void;
		public function get direction () : Number;
		/**
		 * Determines the length of time for the Tween instance.  * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition
		 */
		public function set duration (d:Number) : void;
		public function get duration () : Number;
		/**
		 * Sets the tweening effect for the animation. Use one of the effects in the fl.transitions or * fl.transitions.easing packages.  * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition
		 */
		public function set easing (e:Function) : void;
		public function get easing () : Function;
		/**
		 * @private
		 */
		public function set progress (p:Number) : void;
		/**
		 * @private
		 */
		public function get progress () : Number;

		/**
		 * @private
		 */
		function Transition (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		public function start () : void;
		/**
		 * @private
		 */
		public function stop () : void;
		/**
		 * @private remove any movie clips, masks, etc. created by this transition
		 */
		public function cleanUp () : void;
		/**
		 * @private
		 */
		public function drawBox (mc:MovieClip, x:Number, y:Number, w:Number, h:Number) : void;
		/**
		 * @private
		 */
		public function drawCircle (mc:MovieClip, x:Number, y:Number, r:Number) : void;
		/**
		 * @private abstract method - to be overridden in subclasses
		 */
		protected function _render (p:Number) : void;
		/**
		 * @private
		 */
		private function _resetTween () : void;
		/**
		 * @private
		 */
		private function _noEase (t:Number, b:Number, c:Number, d:Number) : Number;
		/**
		 * @private event that comes from an instance of fl.transitions.Tween
		 */
		public function onMotionFinished (src:Object) : void;
	}
}
