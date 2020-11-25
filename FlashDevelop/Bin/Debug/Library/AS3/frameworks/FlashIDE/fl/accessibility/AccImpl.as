package fl.accessibility
{
	import flash.accessibility.Accessibility;
	import flash.accessibility.AccessibilityImplementation;
	import flash.accessibility.AccessibilityProperties;
	import flash.events.Event;
	import fl.core.UIComponent;
	import flash.display.MovieClip;
	import flash.text.TextField;

	/**
	 *  The AccImpl class, also called the Accessibility Implementation class, is the base class 	 *  for the implementation of accessibility in components. This class enables communication between	 *  a component and a screen reader. Screen readers are used to translate	 *  screen content into synthesized speech or braille for visually impaired users. 	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class AccImpl extends AccessibilityImplementation
	{
		/**
		 *  @private         *  Default state for all the components.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_NORMAL : uint = 0x00000000;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_FOCUSABLE : uint = 0x00100000;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_FOCUSED : uint = 0x00000004;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_UNAVAILABLE : uint = 0x00000001;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_NAMECHANGE : uint = 0x800C;
		/**
		 *  @private (protected)		 *  A reference to the UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var master : UIComponent;
		/**
		 *  @private (protected)		 *  Accessibility Role of the component being made accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var role : uint;

		/**
		 *  @private (protected)         *  All subclasses must override this property by returning an array         *  of strings that contains the events for which to listen.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function get eventsToHandle () : Array;

		/**
		 *  @private		 *  All subclasses must implement this function.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 * Enables accessibility for a component.		 * This method is required for the compiler to activate		 * the accessibility classes for a component.		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *         *  Creates a new Accessibility Implementation instance for the specified component.		 *		 *  @param component The UIComponent instance that this AccImpl instance         *  makes accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function AccImpl (component:UIComponent);
		/**
		 *  @private		 *  Returns the system role for the component.		 *		 *  @param childID The child id.		 *         *  @return Role associated with the component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accRole (childID:uint) : uint;
		/**
		 *  @private		 *  Returns the name of the component.		 *		 *  @param childID The child id.		 *         *  @return Name of the component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accName (childID:uint) : String;
		/**
		 *  @private (protected)		 *  Returns the name of the accessible component. All subclasses must          *  implement this instead of implementing get_accName.		 * 		 *  @param childID The child id.		 *          *  @return Name.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
		/**
		 *  @private (protected)         *  Returns the state of the accessible component.		 * 		 *  @param childID The child id.		 *          *  @return State.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getState (childID:uint) : uint;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getStatusName () : String;
		/**
		 *  @private (protected)         *  Handles events from the master component.		 *  All AccImpl subclasses must implement this		 *  to listen for events from its master component. 		 *          *  @param event The event object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function eventHandler (event:Event) : void;
	}
}
