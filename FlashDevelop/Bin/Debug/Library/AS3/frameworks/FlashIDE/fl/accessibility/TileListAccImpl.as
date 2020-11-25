package fl.accessibility
{
	import flash.accessibility.Accessibility;
	import flash.events.Event;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.TileList;
	import fl.core.UIComponent;

	/**
	 *  The TileListAccImpl class, also called the Tile List Accessibility Implementation class, is	 *  used to make a TileList component accessible.  This class enables communication 	 *  between a TileList component and a screen reader. Screen readers are used to translate      *  screen content into synthesized speech or braille for visually impaired users. 	 * 	 * <p>The TileListAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * <p>A TileList reports the role <code>ROLE_SYSTEM_LIST</code> (0x21) to a screen 	 * reader. Items of a TileList report the role <code>ROLE_SYSTEM_LISTITEM</code> (0x22).</p>     *	 * <p>A TileList reports the following states to a screen reader:</p>	 * <ul>	 *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>     *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>	 * 	 * <p>Additionally, items of a TileList report the following states:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_SELECTED</code> (0x00000002)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_INVISIBLE</code> (0x00008000)</li>     *     <li><code>STATE_SYSTEM_OFFSCREEN</code> (0x00010000)</li>     *     <li><code>STATE_SYSTEM_SELECTABLE</code> (0x00200000)</li>	 * </ul>     *	 * <p>A TileList dispatches the following events to a screen reader:</p>	 * <ul>     *     <li><code>EVENT_OBJECT_FOCUS</code> (0x8005)</li>     *     <li><code>EVENT_OBJECT_SELECTION</code> (0x8006)</li>     *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>     *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>	 * </ul>     *     * @see fl.controls.TileList TileList     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class TileListAccImpl extends SelectableListAccImpl
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing TileListAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to the TileList class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;

		/**
		 *  @private         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of the TileList class with the TileListAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a TileList component. This method is required for          *  the compiler to activate the accessibility classes for a component.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private		 * Creates a new TileList Accessibility Implementation.		 *		 * @param master The UIComponent instance that this AccImpl instance         *        makes accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function TileListAccImpl (master:UIComponent);
		/**
		 *  @private		 *  IAccessible method for returning the value of the ListItem/TileList		 *  which is spoken out by the screen reader		 *  The TileList should return the name of the currently selected item		 *  with m of n string as value when focus moves to TileList.		 * 		 *  @param childID The child id		 *         *  @return String Value         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accValue (childID:uint) : String;
		/**
		 *  @private		 *  This method retrieves the name of the ListItem/TileList		 *  component that is spoken out by the screen reader.		 *  The ListItem should return the label as the name		 *  with m of n string and TileList should return the name		 *  that is specified in the Accessibility Panel.		 *		 *  @param childID The child id		 *         *  @return String Name          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
	}
}
