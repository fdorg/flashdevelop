package fl.managers
{
	import flash.display.InteractiveObject;
	import flash.display.Sprite;
	import fl.controls.Button;

	/**
	 *  Implement the IFocusManager interface to create a custom focus manager. 	 *  A focus manager enables an application to transfer focus among components 	 *  when the user moves the mouse or presses the Tab key. 	 *	 *  @see FocusManager	 *  @see IFocusManagerComponent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public interface IFocusManager
	{
		/**
		 *  Gets or sets a reference to the default button.		 *  The default button serves as a proxy button for		 *  any component that has focus when the Enter key is pressed.		 *  The pressing of the Enter key triggers a <code>click</code>		 *  event to be dispatched on the default button  		 *  on behalf of the component that has focus. Button components do		 *  not require default buttons. When focus moves to a Button		 *  component it cannot trigger the default button; if 		 *  focus moves from a Button component to a component that is not         *  a button, the default button may be triggered again.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get defaultButton () : Button;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set defaultButton (value:Button) : void;
		/**
		 *  Gets or sets a value that indicates whether the default button		 *  is enabled. If this value is set to <code>true</code>, the focus manager		 *  monitors the Enter key and dispatches a <code>click</code> event on the 		 *  default button if the Enter key is pressed when a component that is not a		 *  Button component has focus. If this value is set to <code>false</code>, the focus manager 		 *  does not monitor the Enter key. Components that use the Enter key set 		 *  this property to <code>false</code> to prevent a <code>click</code> event		 *  from being dispatched on the default button, if one exists, when a user          *  presses the Enter key.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get defaultButtonEnabled () : Boolean;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set defaultButtonEnabled (value:Boolean) : void;
		/**
		 *  Gets the next unique tab index to use in the current tab loop. A tab loop 		 *  includes one or more components that are managed by a focus manager.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get nextTabIndex () : int;
		/**
		 *  Gets or sets a value that determines whether the user interface		 *  changes to indicate that a specific component has focus.		 *  		 *  <p>If this property is set to <code>true</code>, a component 		 *  that has focus is marked with a visual indicator. If it is set		 *  to false, a visual indicator of focus is not used.</p>		 *		 *  <p>By default, this property is set to <code>false</code> until the user 		 *  presses the Tab key; then it is set to <code>true</code>.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get showFocusIndicator () : Boolean;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set showFocusIndicator (value:Boolean) : void;

		/**
		 *  Retrieves the IFocusManagerComponent component that currently has focus.		 *  Use this method to determine which component has focus.		 *  Using the Stage object to find out which component has focus may result		 *  in the return of the subcomponent of the component that has focus.		 *         *  @return IFocusManagerComponent object that has focus.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getFocus () : InteractiveObject;
		/**
		 *  Sets focus to an IFocusManagerComponent component. This method does not 		 *  check for component visibility, enabled state, or any other conditions.		 *         *  @param o The component that is to receive focus.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFocus (o:InteractiveObject) : void;
		/**
		 *  Sets the <code>showFocusIndicator</code> property to <code>true</code>.		 *  If a component has focus, this method draws the visual focus indicator         *  on that component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function showFocus () : void;
		/**
		 *  Sets the <code>showFocusIndicator</code> property to <code>false</code>. 		 *  If a component that has focus is marked with a visual indicator         *  of focus, this method removes that indicator.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function hideFocus () : void;
		/**
		 *  Activates a focus manager.		 *          *  <p>When multiple DisplayObjectContainer objects are displayed on the screen		 *  at the same time, the system manager activates and deactivates their 		 *  FocusManager objects as focus moves from one container to the next.         *  When focus moves to a component in an DisplayObjectContainer object whose		 *  focus manager is deactivated, the system manager activates that focus manager		 *  by making a call to the <code>activate()</code> method. Only one focus manager		 *  is active at a time; before activating a focus manager the system manager		 *  uses the <code>deactivate()</code> method to deactivate an active          *  focus manager whose components have lost focus.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function activate () : void;
		/**
		 *  Deactivates a focus manager.		 *          *  <p>When multiple DisplayObjectContainer objects are displayed on the screen		 *  at the same time, the system manager activates and deactivates their 		 *  FocusManager objects as focus moves from one container to the next.         *  When focus moves to a component in an DisplayObjectContainer object whose		 *  focus manager is deactivated, the system manager activates that focus manager		 *  by making a call to the <code>activate()</code> method. Only one focus manager		 *  is active at a time; before activating a focus manager the system manager		 *  uses the <code>deactivate()</code> method to deactivate an active          *  focus manager whose components have lost focus.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function deactivate () : void;
		/**
		 *  Retrieves the IFocusManagerComponent object that contains the specified		 *  object, if there is one.		 * 		 *  <p>Flash Player can set focus on subcomponents as well as on components		 *  themselves. This method is used to find the component that either has focus		 *  or contains the subcomponent that has focus.</p>		 *		 *  @param component An object that can have Flash Player-level focus.		 *		 *  @return The IFocusManagerComponent that contains the specified object;		 *  otherwise, this method returns <code>null</code>.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function findFocusManagerComponent (component:InteractiveObject) : InteractiveObject;
		/**
		 *  Retrieves the component that receives focus next if the user 		 *  causes focus to move by using the Tab key. 		 *		 *  <p>This method can be used to detect the next component to		 *  receive focus in the tab loop if focus moves by one element in either		 *  a forward or backward direction. If the application does not contain		 *  other valid components, this method retrieves the current component		 *  that has focus.</p>		 *		 *  @param backward Indicates whether focus moves in a backward		 *  direction through the tab loop. If this value is <code>true</code>, this method		 *  returns the component that would have focus if focus were moved in a backward		 *  direction by the user pressing the Shift+Tab key combination.		 *         *  @return The component that is next to receive focus.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getNextFocusManagerComponent (backward:Boolean = false) : InteractiveObject;
	}
}
