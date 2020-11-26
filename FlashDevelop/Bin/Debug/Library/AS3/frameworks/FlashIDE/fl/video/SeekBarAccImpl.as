package fl.video
{
	import flash.accessibility.Accessibility;
	import flash.accessibility.AccessibilityImplementation;
	import flash.accessibility.AccessibilityProperties;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.FocusEvent;
	import flash.display.Sprite;
	import fl.video.FLVPlayback;
	import fl.video.VideoEvent;
	import flash.events.TimerEvent;
	import flash.utils.Dictionary;
	import flash.utils.setTimeout;
	import flash.utils.Timer;

	/**
	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SeekBarAccImpl extends AccessibilityImplementation
	{
		/**
		 *  @private		 *  Default state for all the components.
		 */
		private static const STATE_SYSTEM_NORMAL : uint = 0x00000000;
		/**
		 *  @private
		 */
		private static const STATE_SYSTEM_FOCUSABLE : uint = 0x00100000;
		/**
		 *  @private
		 */
		private static const STATE_SYSTEM_FOCUSED : uint = 0x00000004;
		/**
		 *  @private
		 */
		private static const STATE_SYSTEM_SELECTABLE : uint = 0x00200000;
		/**
		 *  @private
		 */
		private static const STATE_SYSTEM_SELECTED : uint = 0x00000002;
		/**
		 *  @private
		 */
		private static const STATE_SYSTEM_UNAVAILABLE : uint = 0x00000001;
		/**
		 *  @private
		 */
		private static const EVENT_OBJECT_FOCUS : uint = 0x8005;
		/**
		 *  @private
		 */
		private static const EVENT_OBJECT_VALUECHANGE : uint = 0x800E;
		/**
		 *  @private
		 */
		private static const EVENT_OBJECT_SELECTION : uint = 0x8006;
		/**
		 *  @private
		 */
		private static const EVENT_OBJECT_LOCATIONCHANGE : uint = 0x800B;
		/**
		 *  @private
		 */
		private static const ROLE_SLIDER : uint = 0x33;
		/**
		 *  @private
		 */
		private static const ROLE_SYSTEM_INDICATOR : uint = 0x27;
		/**
		 *  @private
		 */
		private static const ROLE_SYSTEM_PUSHBUTTON : uint = 0x2b;
		/**
		 *  @private (protected)		 *  A reference to the MovieClip instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var master : Sprite;
		/**
		 *  @private (protected)		 *  Accessibility Role of the MovieClip being made accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var role : uint;
		private var _vc : FLVPlayback;
		private var _cachedPercentage : Number;
		private var _timer : Timer;

		/**
		 *  @private (protected)         *  All subclasses must override this property by returning an array         *  of strings that contains the events for which to listen.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function get eventsToHandle () : Array;

		/**
		 *  @private		 *  All subclasses must implement this function.		 * 		 *  @param sprite The Sprite instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (sprite:Sprite) : void;
		/**
		 * Enables accessibility for a component.		 * This method is required for the compiler to activate		 * the accessibility classes for a component.		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         *         *  Creates a new Accessibility Implementation instance for the specified MovieClip.		 *		 *  @param sprite The Sprite instance that this AccImpl instance         *  makes accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SeekBarAccImpl (sprite:Sprite);
		/**
		 *  @private		 *  Returns the system role for the MovieClip.		 *		 *  @param childID The child id.		 *         *  @return Role associated with the MovieClip.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accRole (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for returning the value of the slider		 *  (which would be the value of the item selected).		 *  The slider should return the value of the current thumb as the value.		 *		 *  @param childID uint		 *		 *  @return Value String		 *  @review
		 */
		public function get_accValue (childID:uint) : String;
		/**
		 *  @private		 *  Returns the name of the component.		 *		 *  @param childID uint.		 *		 *  @return Name of the component.		 *		 *  @tiptext Returns the name of the component		 *  @helpid 3000
		 */
		public function get_accName (childID:uint) : String;
		/**
		 *  @private		 *  method for returning the name of the slider		 *  should return the value		 *		 *  @param childID uint		 *		 *  @return Name String		 *  @review
		 */
		protected function getName (childID:uint) : String;
		/**
		 *  @private		 *  Method to return an array of childIDs.		 *         *  @return Array child ids         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getChildIDArray () : Array;
		/**
		 *  @private		 *  IAccessible method for returning the bounding box of the component or its child element.		 *		 *  @param childID:uint		 *		 *  @return Location:Object
		 */
		public function accLocation (childID:uint) : *;
		/**
		 *  @private		 *  IAccessible method for returning the state of the Button.		 *  States are predefined for all the components in MSAA.		 *  Values are assigned to each state.		 *		 *  @param childID uint		 *		 *  @return State uint
		 */
		public function get_accState (childID:uint) : uint;
		/**
		 *  Utility method determines state of the accessible component.
		 */
		protected function getState (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for returning the Default Action.		 *		 *  @param childID The child id.		 *         *  @return DefaultAction.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accDefaultAction (childID:uint) : String;
		/**
		 *  @private		 *  IAccessible method for executing the Default Action.		 *		 *  @param childID uint
		 */
		public function accDoDefaultAction (childID:uint) : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getStatusName () : String;
		/**
		 *  @private		 *  Override the generic event handler.		 *  All AccImpl must implement this to listen		 *  for events from its master component.
		 */
		protected function eventHandler (event:Event) : void;
		/**
		 *  @private		 *  This is (kind of) a hack to get around the fact that SeekBarSlider is not 		 *  an IFocusManagerComponent. It forces focus from accessibility its thumb		 *  gets focus.
		 */
		private function focusInHandler (event:Event) : void;
		/**
		 *  @private		 *  Returns a timecode string for a given time in seconds.
		 */
		private function secondsToTime (sec:Number) : String;
		/**
		 *  @private		 *  Dispatches an accessibility event to notify MSAA of a value change.
		 */
		private function sendAccessibilityEvent (event:TimerEvent) : void;
	}
}
