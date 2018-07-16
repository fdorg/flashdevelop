package fl.accessibility
{
	import flash.events.Event;
	import flash.accessibility.Accessibility;
	import fl.events.DataGridEvent;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.SelectableList;
	import fl.controls.DataGrid;
	import fl.core.UIComponent;

	/**
	 *  The DataGridAccImpl class, also called the DataGrid Accessibility Implementation class,     *  is used to make a DataGrid component accessible.	 * 	 * <p>The DataGridAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * <p>A DataGrid reports the role <code>ROLE_SYSTEM_LIST</code> (0x21) to a screen 	 * reader. Items of a DataGrid report the role <code>ROLE_SYSTEM_LISTITEM</code> (0x22).</p>     *	 * <p>A DataGrid reports the following states to a screen reader:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>     *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>	 * 	 * <p>Additionally, items of a DataGrid report the following states:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_SELECTED</code> (0x00000002)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_INVISIBLE</code> (0x00008000)</li>     *     <li><code>STATE_SYSTEM_OFFSCREEN</code> (0x00010000)</li>     *     <li><code>STATE_SYSTEM_SELECTABLE</code> (0x00200000)</li>	 * </ul>     *	 * <p>A DataGrid dispatches the following events to a screen reader:</p>	 * <ul>     *     <li><code>EVENT_OBJECT_FOCUS</code> (0x8005)</li>     *     <li><code>EVENT_OBJECT_SELECTION</code> (0x8006)</li>     *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>     *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>	 * </ul>     *     * @see fl.controls.DataGrid DataGrid     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class DataGridAccImpl extends SelectableListAccImpl
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing DataGridAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to DataGrid class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;
		/**
		 *  @private         *  Role of listItem.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const ROLE_SYSTEM_LISTITEM : uint = 0x22;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_FOCUSED : uint = 0x00000004;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_INVISIBLE : uint = 0x00008000;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_OFFSCREEN : uint = 0x00010000;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_SELECTABLE : uint = 0x00200000;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const STATE_SYSTEM_SELECTED : uint = 0x00000002;
		/**
		 *  @private         *  Event emitted if 1 item is selected.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_FOCUS : uint = 0x8005;
		/**
		 *  @private         *  Event emitted if 1 item is selected.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static const EVENT_OBJECT_SELECTION : uint = 0x8006;

		/**
		 *  @private         *  Array of events that we should listen for from the master component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function get eventsToHandle () : Array;

		/**
		 *  @private         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of DataGrid with the DataGridAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a DataGrid component. 		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a new List Accessibility Implementation.		 *		 *  @param master The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function DataGridAccImpl (master:UIComponent);
		/**
		 *  @private		 *  Gets the role for the component.		 *         *  @param childID Children of the component         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accRole (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for returning the value of the ListItem/DataGrid		 *  which is spoken out by the screen reader		 *  The DataGrid should return the name of the currently selected item		 *  with m of n string as value when focus moves to DataGrid.		 *		 *  @param childID		 *         *  @return Name         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accValue (childID:uint) : String;
		/**
		 *  @private		 *  IAccessible method for returning the state of the grid item.		 *  States are predefined for all the components in MSAA.		 *  Values are assigned to each state.		 *  Depending upon the GridItem being Selected, Selectable, Invisible,		 *  Offscreen, a value is returned.		 *		 *  @param childID uint		 *         *  @return State uint         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accState (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for executing the Default Action.		 *         *  @param childID uint         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function accDoDefaultAction (childID:uint) : void;
		/**
		 *  @private		 *  Method to return an array of childIDs.		 *         *  @return Array         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getChildIDArray () : Array;
		/**
		 *  @private		 *  IAccessible method for returning the bounding box of the GridItem.		 *		 *  @param childID uint		 *         *  @return Location Object         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function accLocation (childID:uint) : *;
		/**
		 *  @private		 *  IAccessible method for returning the childFocus of the DataGrid.		 *		 *  @param childID uint		 *         *  @return focused childID.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accFocus () : uint;
		/**
		 *  @private		 *  method for returning the name of the DataGrid/ListItem		 *  which is spoken out by the screen reader		 *  The ListItem should return the label as the name with m of n string and		 *  DataGrid should return the name specified in the AccessibilityProperties.		 *		 *  @param childID uint		 *         *  @return Name         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
		/**
		 *  @private		 *  Override the generic event handler.		 *  All AccImpl must implement this to listen         *  for events from its master component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function eventHandler (event:Event) : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getItemAt (index:int) : Object;
	}
}
