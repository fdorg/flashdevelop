package fl.accessibility
{
	import flash.accessibility.Accessibility;
	import flash.events.Event;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.List;
	import fl.core.UIComponent;

	/**
	 * The ListAccImpl class, also called the List Accessiblity Implementation class, 	 * is used to make a List component accessible.	 * 	 * <p>The ListAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * <p>A List reports the role <code>ROLE_SYSTEM_LIST</code> (0x21) to a screen 	 * reader. Items of a List report the role <code>ROLE_SYSTEM_LISTITEM</code> (0x22).</p>     *	 * <p>A List reports the following states to a screen reader:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>     *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>	 * 	 * <p>Additionally, items of a List report the following states:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_SELECTED</code> (0x00000002)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_INVISIBLE</code> (0x00008000)</li>     *     <li><code>STATE_SYSTEM_OFFSCREEN</code> (0x00010000)</li>     *     <li><code>STATE_SYSTEM_SELECTABLE</code> (0x00200000)</li>	 * </ul>     *	 * <p>A List dispatches the following events to a screen reader:</p>	 * <ul>     *     <li><code>EVENT_OBJECT_FOCUS</code> (0x8005)</li>     *     <li><code>EVENT_OBJECT_SELECTION</code> (0x8006)</li>     *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>	 *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>	 * </ul>     *     * @see fl.controls.List List     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ListAccImpl extends SelectableListAccImpl
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing ListAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to List class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;

		/**
		 *  @private         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of List with the ListAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a List component.		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a new List Accessibility Implementation.		 *		 *  @param master The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ListAccImpl (master:UIComponent);
		/**
		 *  @private		 *  IAccessible method for returning the value of the ListItem/List		 *  which is spoken out by the screen reader		 *  The List should return the name of the currently selected item		 *  with m of n string as value when focus moves to List.		 * 		 *  @param childID The child id		 *         *  @return Value         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accValue (childID:uint) : String;
		/**
		 *  @private		 *  method for returning the name of the ListItem/List		 *  which is spoken out by the screen reader.		 *  The ListItem should return the label as the name		 *  with m of n string and List should return the name		 *  specified in the Accessibility Panel.		 *		 *  @param childID The child id.		 *         *  @return Name.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
	}
}
