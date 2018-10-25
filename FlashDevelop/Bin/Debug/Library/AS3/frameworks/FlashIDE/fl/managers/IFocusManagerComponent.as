package fl.managers
{
	/**
	 * The IFocusManagerComponent interface provides methods and properties	 * that give components the capability to receive focus. Components	 * must implement this interface to receive focus from the FocusManager.     *	 * <p>The UIComponent class provides a base implementation of this interface	 * but does not fully implement it because not all UIComponent objects 	 * receive focus. Components that are derived from the UIComponent	 * class must implement this interface to be capable of receiving focus.	 * To enable focus, add the statement <code>implements IFocusManagerComponent</code>	 * to the class definition of a component that is derived from the UIComponent	 * class.</p>	 *      * @see FocusManager     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public interface IFocusManagerComponent
	{
		/**
		 * Gets or sets a Boolean value that indicates whether a selected component can receive 		 * focus from the focus manager. 		 * 		 * <p>To make a component capable of receiving focus from the focus manager, 		 * set this property to <code>true</code>. To make the component incapable of receiving 		 * focus, set this property to <code>false</code>. When <code>focusEnabled</code>  		 * is set to <code>false</code>, the focus manager ignores the component over the		 * component's lifetime and does not monitor it for changes in the following 		 * properties: <code>tabEnabled</code>, <code>tabChildren</code>, and 		 * <code>mouseFocusEnabled</code>.</p>		 * 		 * <p>You can use the <code>focusEnabled</code> property to prevent the child component of 		 * a component that implements the IFocusManagerComponent interface 		 * from receiving focus from the focus manager. To do so, set this property to <code>false</code> 		 * before using the <code>addChild()</code> method to add the child component		 * to the display list. Note that if you set this property to <code>false</code> 		 * before adding the component to the display list, the focus manager will continue to 		 * ignore the component even if you set this property to <code>true</code> 		 * later on.</p>		 *          * <p><strong>Note:</strong> Even if you set this property to <code>false</code>,          * you can still set focus programmatically by using the <code>setFocus()</code>         * method.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get focusEnabled () : Boolean;
		/**
		 * @private
		 */
		public function set focusEnabled (value:Boolean) : void;
		/**
		 * Gets a Boolean value that indicates whether a component that is		 * selected by using a mouse device can receive focus.		 * 		 * <p>Set this property to <code>true</code> to give focus		 * to components that are selected by using a mouse device.		 * Set this property to <code>false</code> to prevent focus		 * from being given to components that are selected by using		 * a mouse device. If this property is set to <code>false</code>		 * when a component is selected by using a mouse device, focus		 * is transferred to the first parent component for which         * this property is set to <code>true</code>.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get mouseFocusEnabled () : Boolean;
		/**
		 * Gets a Boolean value that indicates whether pressing the Tab key can 		 * move focus to this component. A value of <code>true</code> indicates		 * that pressing the Tab key can cause focus to be moved to this component;		 * a value of <code>false</code> indicates that the Tab key cannot be		 * used to give this component focus. 		 *		 * <p>Even if this value is set to <code>false</code>, the component can still 		 * receive focus when it is selected by a mouse device or through a call to the          * <code>setFocus()</code> method.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get tabEnabled () : Boolean;
		/**
		 * Gets the order in which the component receives focus, if <code>tabEnabled</code>is set         * to <code>true</code>. The <code>tabIndex</code> property is -1 by default, meaning          * that no tab index is set for the object and that the object receives focus based on z-order.		 *          * <p>The <code>tabIndex</code> property can also be a non-negative integer. In this          * case, the objects are ordered according to their <code>tabIndex</code> properties,          * in ascending order. An object with a <code>tabIndex</code> value of 1 precedes an          * object with a <code>tabIndex</code> value of 2. If two objects have the same          * <code>tabIndex</code> value, the one that comes first in the default tab ordering          * precedes the other.</p>         *         * @default -1         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get tabIndex () : int;

		/**
		 * Sets focus for a component.		 * 		 * <p>This method is called by the focus manager when the component receives focus.         * The component may in turn set focus to an internal component.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFocus () : void;
		/**
		 * Draws a visual focus indicator.		 * 		 * <p>This method is called by the focus manager when the component receives focus. 		 * The component should draw or hide a graphic that indicates that the component has focus.</p>		 *		 * @param draw If <code>true</code>, draw the focus indicator,         * otherwise hide it.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function drawFocus (draw:Boolean) : void;
	}
}
