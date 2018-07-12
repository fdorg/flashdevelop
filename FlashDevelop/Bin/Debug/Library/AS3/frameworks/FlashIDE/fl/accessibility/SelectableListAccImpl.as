package fl.accessibility
{
	import flash.events.Event;
	import flash.accessibility.Accessibility;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.SelectableList;
	import fl.core.UIComponent;

	/**
	 *  The SelectableListAccImpl class, also called the SelectableList Accessibility Implementation class,     *  is used to make a SelectableList component accessible.     *     * @see fl.controls.SelectableList SelectableList     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SelectableListAccImpl extends AccImpl
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing SelectableListAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to SelectableList class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
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
		 *  @private         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of SelectableList with the SelectableListAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a SelectableList component.		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a new SelectableList Accessibility Implementation.		 *		 *  @param master The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SelectableListAccImpl (master:UIComponent);
		/**
		 *  @private		 *  Gets the role for the component.		 *		 *  @param childID The child id.		 *          *  @return Role.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accRole (childID:uint) : uint;
		/**
		 *  @private		 *  must be overridden in SelectableListAccImpl subclasses.		 *		 *  @param childID The child id.         *  @return Value.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accValue (childID:uint) : String;
		/**
		 *  @private		 *  IAccessible method for returning the state of the ListItem.		 *  States are predefined for all the components in MSAA.		 *  Values are assigned to each state.		 *  Depending upon the listItem being Selected, Selectable,		 *  Invisible, Offscreen, a value is returned.		 *		 *  @param childID The child id.		 *         *  @return State.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accState (childID:uint) : uint;
		/**
		 *  @private		 *  IAccessible method for returning the Default Action.		 *		 *  @param childID The child id.		 *         *  @return DefaultAction.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accDefaultAction (childID:uint) : String;
		/**
		 *  @private		 *  IAccessible method for executing the Default Action.		 *         *  @param childID The child id.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function accDoDefaultAction (childID:uint) : void;
		/**
		 *  @private		 *  Method to return an array of childIDs.		 *         *  @return Array child ids         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getChildIDArray () : Array;
		/**
		 *  @private		 *  IAccessible method for returning the bounding box of the ListItem.		 *		 *  @param childID The child id		 *         *  @return Object Location          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function accLocation (childID:uint) : *;
		/**
		 *  @private		 *  IAccessible method for returning the child Selections in the List.		 *		 *  @param childID The child id		 *         *  @return Array child ids.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accSelection () : Array;
		/**
		 *  @private		 *  IAccessible method for returning the childFocus of the List.		 *		 *  @param childID The child id		 *         *  @return uint focused child id.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get_accFocus () : uint;
		/**
		 *  @private		 *  IAccessible method for selecting an item.		 *         *  @param childID The child id         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function accSelect (selFlag:uint, childID:uint) : void;
		/**
		 *  @private		 *  must be overridden in SelectableListAccImpl subclasses		 *		 *  @param childID The child id.         *  @return Name.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getName (childID:uint) : String;
		/**
		 *  @private		 *  Override the generic event handler.		 *  All AccImpl must implement this		 *  to listen for events from its master component. 		 *          *  @param event The event object         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function eventHandler (event:Event) : void;
	}
}
