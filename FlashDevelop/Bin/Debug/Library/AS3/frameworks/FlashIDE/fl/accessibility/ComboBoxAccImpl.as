package fl.accessibility
{
	import flash.accessibility.Accessibility;
	import flash.events.Event;
	import fl.controls.ComboBox;
	import fl.core.UIComponent;

	/**
	 * The ComboBoxAccImpl class, also called the ComboBox Accessibility Implementation class,     * is used to make a ComboBox component accessible.	 * 	 * <p>The ComboBoxAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * <p>A ComboBox reports the role <code>ROLE_SYSTEM_COMBOBOX</code> (0x2E) to a screen 	 * reader. Items of a ComboBox report the role <code>ROLE_SYSTEM_LISTITEM</code> (0x22).</p>     *	 * <p>A ComboBox reports the following states to a screen reader:</p>	 * <ul>	 *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>     *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_PRESSED</code> (0x00000008)</li>     *     <li><code>STATE_SYSTEM_CHECKED</code> (0x00000010)</li>     *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>	 * 	 * <p>Additionally, items of a ComboBox report the following states:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_SELECTED</code> (0x00000002)</li>     *     <li><code>STATE_SYSTEM_SELECTABLE</code> (0x00200000)</li>	 * </ul>     *	 * <p>A ComboBox dispatches the following events to a screen reader:</p>	 * <ul>     *     <li><code>EVENT_OBJECT_SELECTION</code> (0x8006)</li>     *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>     *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>     *     <li><code>EVENT_OBJECT_VALUECHANGE</code> (0x800E)</li>	 * </ul>     *     * @see fl.controls.ComboBox ComboBox     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ComboBoxAccImpl extends AccImpl
	{
		/**
		 * @private         * Static variable triggering the <code>hookAccessibility()</code> method.         * This is used for initializing ComboBoxAccImpl class to hook its         * <code>createAccessibilityImplementation()</code> method to ComboBox class          * before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const ROLE_SYSTEM_LISTITEM : uint = 0x22;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_FOCUSED : uint = 0x00000004;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_SELECTABLE : uint = 0x00200000;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_SELECTED : uint = 0x00000002;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_VALUECHANGE : uint = 0x800E;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_SELECTION : uint = 0x8006;

		/**
		 *  @private         *  Array of events that we should listen for from the master component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function get eventsToHandle () : Array;

		/**
		 * @private         * Static method for swapping the <code>createAccessibilityImplementation()</code>         * method of ComboBox with the ComboBoxAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private         *		 *  Method for creating the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a ComboBox component.		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a new ComboBox Accessibility Implementation.		 *		 *  @param master The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ComboBoxAccImpl (master:UIComponent);
		/**
		 *  @private		 *  Gets the role for the component.		 *         *  @param childID uint         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accRole (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for returning the value of the ComboBox		 *  (which would be the text of the item selected).		 *  The ComboBox should return the content of the TextField as the value.		 *		 *  @param childID uint		 *         *  @return Value string         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accValue (childID:uint) : String;
		/**
		 *  @private		 *  IAccessible method for returning the state of the ListItem		 *  (basically to remove 'not selected').		 *  States are predefined for all the components in MSAA.		 *  Values are assigned to each state.		 *  Depending upon the listItem being Selected, Selectable,		 *  Invisible, Offscreen, a value is returned.		 *		 *  @param childID 		 *         *  @return          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accState (childID:uint) : uint;
		/**
		 *  @private		 *  Method to return an array of childIDs.		 *         *  @return         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getChildIDArray () : Array;
		/**
		 *  @private		 *  Method for returning the name of the ComboBox		 *  For children items, it would add m of n string to the name.		 *  ComboBox should return the name specified in the AccessibilityProperties.		 *		 *  @param childID 		 *         *  @return          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
		/**
		 *  @private		 *  Override the generic event handler.		 *  All AccImpl must implement this to listen for events         *  from its master component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function eventHandler (event:Event) : void;
	}
}
