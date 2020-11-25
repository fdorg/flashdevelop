package fl.controls
{
	import fl.data.DataProvider;
	import fl.controls.listClasses.CellRenderer;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.listClasses.ListData;
	import fl.controls.ScrollPolicy;
	import fl.controls.SelectableList;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.DataChangeType;
	import fl.events.DataChangeEvent;
	import fl.events.ListEvent;
	import fl.events.ScrollEvent;
	import fl.managers.IFocusManagerComponent;
	import flash.display.DisplayObject;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.ui.Keyboard;
	import flash.utils.Dictionary;
	import flash.geom.Rectangle;

	/**
	 * The List component displays list-based information and is ideally suited 	 * for the display of arrays of information.  	 *	 * <p>The List component consists of items, rows, and a data provider, which are 	 * described as follows:</p>	 * <ul>	 * <li>Item: An ActionScript object that usually contains a descriptive <code>label</code> 	 *           property and a <code>data</code> property that stores the data associated with that item. </li>	 * <li>Row: A component that is used to display the item. </li>	 * <li>Data provider: A component that models the items that the List component displays.</li>	 * </ul>	 *	 * <p>By default, the List component uses the CellRenderer class to supply the rows in 	 * which list items are displayed. You can create these rows programmatically; this 	 * is usually done by subclassing the CellRenderer class. The CellRenderer class 	 * implements the ICellRenderer interface, which provides the set of properties and 	 * methods that the List component uses to manipulate its rows and to send data and state information 	 * to each row for display. This includes information about data sizing and selection.</p>	 *	 * <p>The List component provides methods that act on its data provider--for example, the	 * <code>addItem()</code> and <code>removeItem()</code> methods. You can use these and 	 * other methods to manipulate the data of any array that exists in the same frame as 	 * a List component and then broadcast the changes to multiple views. If a List component 	 * is not provided with an external data provider, these methods automatically create an 	 * instance of a data provider and expose it through the <code>List.dataProvider</code> 	 * property. The List component renders each row by using a Sprite that implements the 	 * ICellRenderer interface. To specify this renderer, use the <code>List.cellRenderer</code> 	 * property. You can also build an Array instance or get one from a server and use it as a 	 * data model for multiple lists, combo boxes, data grids, and so on. </p> 	 *     * @includeExample examples/ListExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class List extends SelectableList implements IFocusManagerComponent
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _rowHeight : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _cellRenderer : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _labelField : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _labelFunction : Function;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _iconField : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _iconFunction : Function;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 *  @private		 *  Method for creating the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * Gets or sets the name of the field in the <code>dataProvider</code> object          * to be displayed as the label for the TextInput field and drop-down list. 		 *         * <p>By default, the component displays the <code>label</code> property 		 * of each <code>dataProvider</code> item. If the <code>dataProvider</code> 		 * items do not contain a <code>label</code> property, you can set the 		 * <code>labelField</code> property to use a different property.</p>         *         * <p><strong>Note:</strong> The <code>labelField</code> property is not used          * if the <code>labelFunction</code> property is set to a callback function.</p>         *          * @default "label"         *         * @see #labelFunction          *		 * @includeExample examples/List.labelField.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelField () : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelField (value:String) : void;
		/**
		 * Gets or sets the function to be used to obtain the label for the item.		 *         * <p>By default, the component displays the <code>label</code> property		 * for a <code>dataProvider</code> item. But some data sets may not have 		 * a <code>label</code> field or may not have a field whose value		 * can be used as a label without modification. For example, a given data 		 * set might store full names but maintain them in <code>lastName</code> and  		 * <code>firstName</code> fields. In such a case, this property could be		 * used to set a callback function that concatenates the values of the 		 * <code>lastName</code> and <code>firstName</code> fields into a full 		 * name string to be displayed.</p>		 *         * <p><strong>Note:</strong> The <code>labelField</code> property is not used          * if the <code>labelFunction</code> property is set to a callback function.</p>         *         * @default null         *		 * @includeExample examples/List.labelFunction.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelFunction () : Function;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelFunction (value:Function) : void;
		/**
		 * Gets or sets the item field that provides the icon for the item.         *         * <p><strong>Note:</strong> The <code>iconField</code> is not used          * if the <code>iconFunction</code> property is set to a callback function.</p>         *         * @default "icon"         *		 * @includeExample examples/List.iconField.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get iconField () : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set iconField (value:String) : void;
		/**
		 * Gets or sets the function to be used to obtain the icon for the item.         *         * <p><strong>Note:</strong> The <code>iconField</code> is not used          * if the <code>iconFunction</code> property is set to a callback function.</p>         *         * @default null         *		 * @includeExample examples/List.iconFunction.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get iconFunction () : Function;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set iconFunction (value:Function) : void;
		/**
		 * Gets or sets the number of rows that are at least partially visible in the          * list.         *         * @includeExample examples/SelectableList.rowCount.1.as -noswf         * @includeExample examples/List.rowCount.2.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowCount () : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowCount (value:uint) : void;
		/**
		 * Gets or sets the height of each row in the list, in pixels.         *         * @default 20         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowHeight () : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowHeight (value:Number) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new List component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function List ();
		/**
		 * @copy fl.controls.SelectableList#scrollToIndex()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function scrollToIndex (newCaretIndex:int) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateAvailableHeight () : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setHorizontalScrollPosition (value:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setVerticalScrollPosition (scroll:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawList () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)		 * Moves the selection in a horizontal direction in response		 * to the user selecting items using the left-arrow or right-arrow		 * keys and modifiers such as  the Shift and Ctrl keys.		 *		 * <p>Not implemented in List because the default list		 * is single column and therefore doesn't scroll horizontally.</p>		 *		 * @param code The key that was pressed (e.g. Keyboard.LEFT)         *		 * @param shiftKey <code>true</code> if the shift key was held down when		 *        the keyboard key was pressed.         *		 * @param ctrlKey <code>true</code> if the ctrl key was held down when         *        the keyboard key was pressed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionHorizontally (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)		 * Moves the selection in a vertical direction in response		 * to the user selecting items using the up-arrow or down-arrow		 * Keys and modifiers such as the Shift and Ctrl keys.		 *		 * @param code The key that was pressed (e.g. Keyboard.DOWN)         *		 * @param shiftKey <code>true</code> if the shift key was held down when		 *        the keyboard key was pressed.         *		 * @param ctrlKey <code>true</code> if the ctrl key was held down when         *        the keyboard key was pressed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionVertically (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function doKeySelection (newCaretIndex:int, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * Retrieves the string that the renderer displays for the given data object          * based on the <code>labelField</code> and <code>labelFunction</code> properties.         *         * <p><strong>Note:</strong> The <code>labelField</code> is not used           * if the <code>labelFunction</code> property is set to a callback function.</p>		 *		 * @param item The object to be rendered.		 *         * @return The string to be displayed based on the data.         *         * @internal <code>var label:String = myList.itemToLabel(data);</code>         *		 * @includeExample examples/List.itemToLabel.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToLabel (item:Object) : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
	}
}
