package fl.managers
{
	import flash.display.Sprite;
	import flash.display.MovieClip;
	import flash.display.Stage;
	import flash.display.SimpleButton;
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.display.InteractiveObject;
	import flash.text.TextField;
	import flash.text.TextFieldType;
	import flash.ui.Keyboard;
	import flash.events.Event;
	import flash.events.FocusEvent;
	import flash.events.MouseEvent;
	import flash.events.KeyboardEvent;
	import fl.controls.Button;
	import fl.controls.TextInput;
	import fl.core.UIComponent;
	import flash.system.Capabilities;
	import flash.utils.*;

	/**
	 *  The FocusManager class manages focus for a set of components that are navigated by mouse 	 *  or keyboard as a <em>tab loop</em>. 	 *	 *  <p>A tab loop is typically navigated by using the Tab key; focus moves between the components 	 *  in the tab loop in a circular pattern from the first component that has focus, to the last, 	 *  and then back again to the first. A tab loop includes all the components and tab-enabled	 *  components in a container. An application can contain numerous tab loops.</p>	 *  	 *  <p>A FocusManager instance is responsible for a single tab loop; an application uses	 *  a different FocusManager instance to manage each tab loop that it contains, although	 *  a main application is always associated with at least one FocusManager instance. An	 *  application may require additional FocusManager instances if it includes a popup window,	 *  for example, that separately contains one or more tab loops of components.</p> 	 * 	 *  <p>All components that can be managed by a FocusManager instance must implement the	 *  fl.managers.IFocusManagerComponent interface. Objects for which Flash Player	 *  manages focus are not required to implement the IFocusManagerComponent interface.</p>  	 *	 *  <p>The FocusManager class also manages how the default button is implemented. A default button 	 *  dispatches a <code>click</code> event when the Enter key is pressed on a form, 	 *  depending on where focus is at the time.  The default button does not dispatch the	 *  <code>click</code> event if a text area has focus or if a value is being edited in	 *  a component, for example, in a ComboBox or NumericStepper component.</p>	 * 	 * @see IFocusManager	 * @see IFocusManagerComponent     *     * @includeExample examples/FocusManagerExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class FocusManager implements IFocusManager
	{
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _form : DisplayObjectContainer;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var focusableObjects : Dictionary;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var focusableCandidates : Array;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var activated : Boolean;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var calculateCandidates : Boolean;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var lastFocus : InteractiveObject;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _showFocusIndicator : Boolean;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var lastAction : String;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var defButton : Button;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _defaultButton : Button;
		/**
		 * @private          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _defaultButtonEnabled : Boolean;

		/**
		 * Gets or sets the current default button.		 *		 * <p>The default button is the button on a form that dispatches a		 * <code>click</code> event when the Enter key is pressed,         * depending on where focus is at the time.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get defaultButton () : Button;
		/**
		 *  @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set defaultButton (value:Button) : void;
		/**
		 *  @copy fl.managers.IFocusManager#defaultButtonEnabled         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get defaultButtonEnabled () : Boolean;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set defaultButtonEnabled (value:Boolean) : void;
		/**
		 *  Gets the next unique tab index to use in this tab loop.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get nextTabIndex () : int;
		/**
		 *  Gets or sets a value that indicates whether a component that has focus should be marked with a visual indicator of focus.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get showFocusIndicator () : Boolean;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set showFocusIndicator (value:Boolean) : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get form () : DisplayObjectContainer;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set form (value:DisplayObjectContainer) : void;

		/**
		 *  Creates a new FocusManager instance.		 *		 *  <p>A focus manager manages focus within the children of a 		 *  DisplayObjectContainer object.</p>		 *		 *  @param container A DisplayObjectContainer that hosts the focus manager,          *  or <code>stage</code>.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function FocusManager (container:DisplayObjectContainer);
		/**
		 *  @private         *  Listen for children being added and see if they are focus candidates.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function addedHandler (event:Event) : void;
		/**
		 *  @private         *  Listen for children being removed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function removedHandler (event:Event) : void;
		/**
		 *  @private         *  Do a tree walk and add all children you can find.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function addFocusables (o:DisplayObject, skipTopLevel:Boolean = false) : void;
		/**
		 *  @private         *  Removes the DisplayObject and all its children.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function removeFocusables (o:DisplayObject) : void;
		/**
		 *  @private         *  Checks if the DisplayObject is visible for the tab loop.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function isTabVisible (o:DisplayObject) : Boolean;
		/**
		 *  @private         *  Checks if the DisplayObject is a valid candidate for the tab loop.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function isValidFocusCandidate (o:DisplayObject, groupName:String) : Boolean;
		/**
		 *  @private		 *  Checks if the DisplayObject is enabled and visible, or         *  a selectable or input TextField         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function isEnabledAndVisible (o:DisplayObject) : Boolean;
		/**
		 *  @private         *  Add or remove object if tabbing properties change.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function tabEnabledChangeHandler (event:Event) : void;
		/**
		 *  @private         *  Called when a focusable object's <code>tabIndex</code> property changes.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function tabIndexChangeHandler (event:Event) : void;
		/**
		 *  @private         *  Add or remove object if tabbing properties change.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function tabChildrenChangeHandler (event:Event) : void;
		/**
		 *  Activates the FocusManager instance.		 *		 *  <p>The FocusManager instance adds event handlers that allow it to monitor         *  focus-related keyboard and mouse activity.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function activate () : void;
		/**
		 *  Deactivates the FocusManager.		 *		 *  <p>The FocusManager removes the event handlers that allow it to monitor         *  focus-related keyboard and mouse activity.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function deactivate () : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function focusInHandler (event:FocusEvent) : void;
		/**
		 *  @private         *  Useful for debugging.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function focusOutHandler (event:FocusEvent) : void;
		/**
		 *  @private         *  Restore focus to the element that had it last.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function activateHandler (event:Event) : void;
		/**
		 *  @private         *  Useful for debugging.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function deactivateHandler (event:Event) : void;
		/**
		 *  @private		 *  This gets called when mouse clicks on a focusable object.         *  We block Flash Player behavior.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function mouseFocusChangeHandler (event:FocusEvent) : void;
		/**
		 *  @private         *  This function is called when the tab key is hit.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function keyFocusChangeHandler (event:FocusEvent) : void;
		/**
		 *  @private         *  Watch for the Enter key.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 *  @private		 *  This gets called when the focus changes due to a mouse click.		 *         *  <p><strong>Note:</strong> If focus is moving to a TextField, it is not         *  necessary to call <code>setFocus()</code> on it because the player handles it;         *  calling <code>setFocus()</code> on a TextField that has scrollable text		 *  causes the text to auto-scroll to the end, making the         *  mouse click set the insertion point in the wrong place.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function mouseDownHandler (event:MouseEvent) : void;
		/**
		 *  @private 		 *  Call this method to make the system         *  think the Enter key was pressed and the default button was clicked.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function sendDefaultButtonEvent () : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function setFocusToNextObject (event:FocusEvent) : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function hasFocusableObjects () : Boolean;
		/**
		 *  Retrieves the interactive object that would receive focus		 *  if the user pressed the Tab key to navigate to the next object.		 *  This method retrieves the object that currently has focus		 *  if there are no other valid objects in the application.		 *		 *  @param backward If this parameter is set to <code>true</code>, 		 *  focus moves in a backward direction, causing this method to retrieve		 *  the object that would receive focus next if the Shift+Tab key combination		 *  were pressed. 		 *		 *  @return The next component to receive focus.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getNextFocusManagerComponent (backward:Boolean = false) : InteractiveObject;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getIndexOfFocusedObject (o:DisplayObject) : int;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getIndexOfNextObject (i:int, shiftKey:Boolean, bSearchAll:Boolean, groupName:String) : int;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function sortFocusableObjects () : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function sortFocusableObjectsTabIndex () : void;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function sortByDepth (aa:InteractiveObject, bb:InteractiveObject) : Number;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getChildIndex (parent:DisplayObjectContainer, child:DisplayObject) : int;
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function sortByTabIndex (a:InteractiveObject, b:InteractiveObject) : int;
		/**
		 *  Gets the interactive object that currently has focus.		 *  Adobe recommends calling this method instead of using the Stage object 		 *  because this method indicates which component has focus.		 *  The Stage might return a subcomponent in that component.		 *         *  @return The interactive object that currently has focus.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getFocus () : InteractiveObject;
		/**
		 *  Sets focus on an IFocusManagerComponent component. This method does		 *  not check for component visibility, enabled state, or other conditions.		 *		 *  @param component An object that can receive focus.		 * 		 *  @internal Do you guys have a code snippet/test case/sample you could give us for this? (rberry(at)adobe.com)         *  Adobe: [LM] {StyleManager.setFocus(myBtn);}         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFocus (component:InteractiveObject) : void;
		/**
		 *  Sets the <code>showFocusIndicator</code> value to <code>true</code>         *  and draws the visual focus indicator on the object with focus, if any.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function showFocus () : void;
		/**
		 *  Sets the <code>showFocusIndicator</code> value to <code>false</code>         *  and removes the visual focus indicator from the object that has focus, if any.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function hideFocus () : void;
		/**
		 *  Retrieves the interactive object that contains the given object, if any.		 *  The player can set focus to a subcomponent of a Flash component;		 *  this method determines which interactive object has focus from		 *  the component perspective.		 *		 *  @param component An object that can have player-level focus.		 *		 *  @return The object containing the <code>component</code> or, if one is		 *  not found, the <code>component</code> itself.		 * 		 * @internal Description needs explanation of relationship between interactive object (param and return type) and		 * FocusManagerComponent, which the function claims to be finding. Do you guys have a code snippet/test 		 * case/sample you could give us for this? (rberry(at)adobe.com)         * Metaliq: Anybody?		 * Adobe: [LM] Changed description to InteractiveObject.  This was changed to InteractiveObject because the FocusManager supports native flash objects too.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function findFocusManagerComponent (component:InteractiveObject) : InteractiveObject;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function getTopLevelFocusTarget (o:InteractiveObject) : InteractiveObject;
	}
}
