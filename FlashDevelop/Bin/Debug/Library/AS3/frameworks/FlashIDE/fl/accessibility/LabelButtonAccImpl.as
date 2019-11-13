package fl.accessibility
{
	import flash.accessibility.Accessibility;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.ui.Keyboard;
	import fl.controls.LabelButton;
	import fl.core.UIComponent;
	import fl.events.ComponentEvent;

	/**
	 *  The LabelButtonAccImpl class, also called the LabelButton Accessibility Implementation class,	 *  is used to make a LabelButton component accessible. This class enables communication	 *  between a LabelButton component and a screen reader. Screen readers are used to translate	 *  screen content into synthesized speech or braille for visually impaired users.	 *      * <p>The LabelButtonAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * <p>A LabelButton reports the role <code>ROLE_SYSTEM_PUSHBUTTON</code> (0x2B) to a screen 	 * reader.</p>     *	 * <p>A LabelButton reports the following states to a screen reader:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>     *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_PRESSED</code> (0x00000008)</li>     *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>     *	 * <p>A LabelButton dispatches the following events to a screen reader:</p>	 * <ul>	 *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>     *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>	 * </ul>     *     * @see fl.controls.LabelButton LabelButton     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class LabelButtonAccImpl extends AccImpl
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing LabelButtonAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to LabelButton class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_PRESSED : uint = 0x00000008;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_NAMECHANGE : uint = 0x800C;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_STATECHANGE : uint = 0x800A;

		/**
		 *  @private		 *	Array of events that we should listen for from the master component.         *         *  @return          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function get eventsToHandle () : Array;

		/**
		 *  @private         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of LabelButton with the LabelButtonAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a LabelButton component.		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a new LabelButtonAccImpl instance for the specified LabelButton component.		 *		 *  @param component The LabelButton instance that this LabelButtonAccImpl instance         *         makes accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function LabelButtonAccImpl (component:UIComponent);
		/**
		 *  @private		 *  IAccessible method for returning the state of the LabelButton.		 *  States are predefined for all the components in MSAA.		 *  Values are assigned to each state.		 *  Depending upon the button being pressed or released,		 *  a value is returned.		 *		 *  @param childID The child id.		 *         *  @return State.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accState (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for returning the default action		 *  of the LabelButton, which is Press.		 *		 *  @param childID The child id		 *         *  @return DefaultAction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accDefaultAction (childID:uint) : String;
		/**
		 *  @private		 *  IAccessible method for performing the default action		 *  associated with LabelButton, which is Press.		 *         *  @param childID The child id.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function accDoDefaultAction (childID:uint) : void;
		/**
		 *  @private		 *  method for returning the name of the LabelButton		 *  which is spoken out by the screen reader		 *  The LabelButton should return the label inside as the name of the LabelButton.		 *  The name returned here would take precedence over the name		 *  specified in the Accessibility panel.		 *		 *  @param childID The child id.		 *         *  @return Name.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
		/**
		 *  @private		 *  Override the generic event handler.		 *  All AccImpl must implement this to listen for events from its master component. 		 *          *  @param event The event object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function eventHandler (event:Event) : void;
	}
}
