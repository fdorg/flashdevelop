package fl.controls
{
	import fl.containers.BaseScrollPane;
	import fl.controls.listClasses.CellRenderer;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.listClasses.ListData;
	import fl.controls.ScrollPolicy;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.data.DataProvider;
	import fl.data.SimpleCollectionItem;
	import fl.events.DataChangeEvent;
	import fl.events.DataChangeType;
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

	/**
	 * Dispatched when the user rolls the pointer off of an item in the component.     *	 * @eventType fl.events.ListEvent.ITEM_ROLL_OUT     *     * @see #event:itemRollOver     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemRollOut", type="fl.events.ListEvent")] 
	/**
	 * Dispatched when the user rolls the pointer over an item in the component.     *     * @eventType fl.events.ListEvent.ITEM_ROLL_OVER     *     * @see #event:itemRollOut     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemRollOver", type="fl.events.ListEvent")] 
	/**
	 * Dispatched when the user rolls the pointer over the component.	 * 	 * @eventType flash.events.MouseEvent.ROLL_OVER     *     * @see #event:rollOut     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="rollOver", type="flash.events.MouseEvent")] 
	/**
	 * Dispatched when the user rolls the pointer off of the component.     *	 * @eventType flash.events.MouseEvent.ROLL_OUT     *     * @see #event:rollOver     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="rollOut", type="flash.events.MouseEvent")] 
	/**
	 * Dispatched when the user clicks an item in the component. 	 *	 * <p>The <code>click</code> event is dispatched before the value	 * of the component is changed. To identify the row and column that were clicked,	 * use the properties of the event object; do not use the <code>selectedIndex</code>      * and <code>selectedItem</code> properties.</p>     *     * @eventType fl.events.ListEvent.ITEM_CLICK     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemClick", type="fl.events.ListEvent")] 
	/**
	 * Dispatched when the user clicks an item in the component twice in     * rapid succession. Unlike the <code>click</code> event, the doubleClick event is 	 * dispatched after the <code>selectedIndex</code> of the component is 	 * changed.     *     * @eventType fl.events.ListEvent.ITEM_DOUBLE_CLICK     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemDoubleClick", type="fl.events.ListEvent")] 
	/**
	 * Dispatched when a different item is selected in the list.     *     * @eventType flash.events.Event.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="flash.events.Event")] 
	/**
	 * Dispatched when the user scrolls horizontally or vertically.     *     * @eventType fl.events.ScrollEvent.SCROLL     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="scroll", type="fl.events.ScrollEvent")] 
	/**
	 * The class that provides the skin for the background of the component.     *     * @default List_skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="skin", type="Class")] 
	/**
	 * The class that provides the cell renderer for each item in the component.     *     * @default fl.contols.listClasses.CellRenderer     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="cellRenderer", type="Class")] 
	/**
	 * The alpha value to set the list to when the <code>enabled</code> property is <code>false</code>.     *     * @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledAlpha", type="Number")] 
	/**
	 * The padding that separates the border of the list from its contents, in pixels.     *     * @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="contentPadding", type="Number", format="Length")] 

	/**
	 * The SelectableList is the base class for all list-based components--for  	 * example, the List, TileList, DataGrid, and ComboBox components. 	 * This class provides methods and properties that are used for the 	 * rendering and layout of rows, and to set scroll bar styles and data	 * providers.      *     * <p><strong>Note:</strong> This class does not create a component; 	 * it is exposed only so that it can be extended.</p>	 * 	 * @see fl.controls.DataGrid	 * @see fl.controls.List 	 * @see fl.controls.TileList     * @see fl.data.DataProvider     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SelectableList extends BaseScrollPane implements IFocusManagerComponent
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var listHolder : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var list : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _dataProvider : DataProvider;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var activeCellRenderers : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var availableCellRenderers : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var renderedItems : Dictionary;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var invalidItems : Dictionary;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _horizontalScrollPosition : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _verticalScrollPosition : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _allowMultipleSelection : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _selectable : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _selectedIndices : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var caretIndex : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var lastCaretIndex : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var preChangeItems : Array;
		/**
		 * @private
		 */
		private var collectionItemImport : SimpleCollectionItem;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var rendererStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var updatedRendererStyles : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 *  @private		 *  Creates the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets the data model of the list of items to be viewed. A data provider 		 * can be shared by multiple list-based components. Changes to the data provider 		 * are immediately available to all components that use it as a data source. 		 *  		 * @default null         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get dataProvider () : DataProvider;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set dataProvider (value:DataProvider) : void;
		/**
		 * Gets or sets the number of pixels that the list scrolls to the right when the         * <code>horizontalScrollPolicy</code> property is set to <code>ScrollPolicy.ON</code>.         *         * @see fl.containers.BaseScrollPane#horizontalScrollPosition         * @see fl.containers.BaseScrollPane#maxVerticalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxHorizontalScrollPosition () : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maxHorizontalScrollPosition (value:Number) : void;
		/**
		 * Gets the number of items in the data provider.         *         * @includeExample examples/SelectableList.length.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get length () : uint;
		/**
		 * Gets a Boolean value that indicates whether more than one list item 		 * can be selected at a time. A value of <code>true</code> indicates that 		 * multiple selections can be made at one time; a value of <code>false</code> 		 * indicates that only one item can be selected at one time.          *         * @default false         *         * @includeExample examples/List.allowMultipleSelection.1.as -noswf         *         * @see #selectable         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get allowMultipleSelection () : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set allowMultipleSelection (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the items in the list 		 * can be selected. A value of <code>true</code> indicates that the list items		 * can be selected; a value of <code>false</code> indicates that they cannot be.  		 *          * @default true         *         * @see #allowMultipleSelection         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectable () : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectable (value:Boolean) : void;
		/**
		 * Gets or sets the index of the item that is selected in a single-selection		 * list. A single-selection list is a list in which only one item can be selected 		 * at a time. 		 *		 * <p>A value of -1 indicates that no item is selected; if multiple selections		 * are made, this value is equal to the index of the item that was selected last in		 * the group of selected items.</p>		 *		 * <p>When ActionScript is used to set this property, the item at the specified index		 * replaces the current selection. When the selection is changed programmatically, 		 * a <code>change</code> event object is not dispatched. </p>         *         * @see #selectedIndices         * @see #selectedItem		 *		 * @includeExample examples/SelectableList.selectedIndex.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedIndex () : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedIndex (value:int) : void;
		/**
		 * Gets or sets an array that contains the items that were selected from		 * a multiple-selection list. 		 *		 * <p>To replace the current selection programmatically, you can make an 		 * explicit assignment to this property. You can clear the current selection 		 * by setting this property to an empty array or to a value of <code>undefined</code>. 		 * If no items are selected from the list of items, this property is 		 * <code>undefined</code>. </p>		 *		 * <p>The sequence of values in the array reflects the order in which the items		 * were selected from the multiple-selection list. For example, if you click the second 		 * item from the list, then the third item, and finally the first item, this property 		 * contains an array of values in the following sequence: <code>[1,2,0]</code>.</p>         *         * @see #allowMultipleSelection         * @see #selectedIndex         * @see #selectedItems         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedIndices () : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedIndices (value:Array) : void;
		/**
		 * Gets or sets the item that was selected from a single-selection list. For a 		 * multiple-selection list in which multiple items are selected, this property		 * contains the item that was selected last. 		 *		 * <p>If no selection is made, the value of this property is <code>null</code>.</p>         *         * @see #selectedIndex         * @see #selectedItems		 *		 * @includeExample examples/SelectableList.selectedIndex.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedItem () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedItem (value:Object) : void;
		/**
		 * Gets or sets an array that contains the objects for the  		 * items that were selected from the multiple-selection list. 		 *		 * <p>For a single-selection list, the value of this property is an 		 * array containing the one selected item. In a single-selection 		 * list, the <code>allowMultipleSelection</code> property is set to 		 * <code>false</code>.</p>         *         * @includeExample examples/SelectableList.selectedItems.1.as -noswf         *         * @see #allowMultipleSelection         * @see #selectedIndices         * @see #selectedItem         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedItems () : Array;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedItems (value:Array) : void;
		/**
		 * Gets the number of rows that are at least partially visible in the list.		 *          * <p><strong>Note:</strong> This property must be overridden in any class that extends SelectableList.</p>         *		 * @includeExample examples/SelectableList.rowCount.1.as -noswf		 *		 * @default 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowCount () : uint;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new SelectableList instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SelectableList ();
		/**
		 * Clears the currently selected item in the list and sets the <code>selectedIndex</code> property to -1.         *         * @includeExample examples/List.clearSelection.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clearSelection () : void;
		/**
		 * Retrieves the ICellRenderer for a given item object, if there is one. 		 * This method always returns <code>null</code>.         *         * @param item The item in the data provider.         *         * @return A value of <code>null</code>.         *		 * @includeExample examples/SelectableList.itemToCellRenderer.1.as -noswf		 *		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToCellRenderer (item:Object) : ICellRenderer;
		/**
		 * Appends an item to the end of the list of items. 		 *		 * <p>An item should contain <code>label</code> and <code>data</code> 		 * properties; however, items that contain other properties can also 		 * be added to the list. By default, the <code>label</code> property of 		 * an item is used to display the label of the row; the <code>data</code> 		 * property is used to store the data of the row. </p>		 *          * @param item The item to be added to the data provider.         *         * @see #addItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addItem (item:Object) : void;
		/**
		 * Inserts an item into the list at the specified index location. The indices of 		 * items at or after the specified index location are incremented by 1.         *         * @param item The item to be added to the list.         *         * @param index The index at which to add the item.		 *         * @throws RangeError The specified index is less than 0 or greater than or equal to the length of the data provider.         *         * @see #addItem()         * @see #replaceItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addItemAt (item:Object, index:uint) : void;
		/**
		 * Removes all items from the list.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeAll () : void;
		/**
		 * Retrieves the item at the specified index.         *          * @param index The index of the item to be retrieved.         *         * @return The object at the specified index location.		 *         * @throws RangeError The specified index is less than 0 or greater than or equal to the length of the data provider.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getItemAt (index:uint) : Object;
		/**
		 * Removes the specified item from the list.          *         * @param item The item to be removed.         *         * @return The item that was removed.		 *         * @throws RangeError The item could not be found.         *         * @see #removeAll()         * @see #removeItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeItem (item:Object) : Object;
		/**
		 * Removes the item at the specified index position. The indices of 		 * items after the specified index location are decremented by 1.         *         * @param index The index of the item in the data provider to be removed.         *         * @return The item that was removed.         *         * @see #removeAll()         * @see #removeItem()         * @see #replaceItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeItemAt (index:uint) : Object;
		/**
		 * Replaces the item at the specified index location with another item. 		 * This method modifies the data provider of the List component. If 		 * the data provider is shared with other components, the data that is		 * provided to those components is also updated.         *         * @param item The item to replace the item at the specified index location.         *         * @param index The index position of the item to be replaced.         *         * @return The item that was replaced.		 *         * @throws RangeError The specified index is less than 0 or greater than or equal to the length of the data provider.         *         * @see #removeItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function replaceItemAt (item:Object, index:uint) : Object;
		/**
		 * Invalidates the whole list, forcing the list items to be redrawn.         *         * @see #invalidateItem()         * @see #invalidateItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function invalidateList () : void;
		/**
		 * Invalidates a specific item renderer.		 *         * @param item The item in the data provider to invalidate.         *         * @see #invalidateItemAt()         * @see #invalidateList()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 *		 * @internal [kenos] This doesn't make sense. It says the method invalidates an item renderer,		 *                   but the param description for item says that it receives "the data provider to invalidate."		 *                   Isn't the item renderer distinct from the item? There is a similar problem in the next method.
		 */
		public function invalidateItem (item:Object) : void;
		/**
		 * Invalidates the renderer for the item at the specified index.		 *         * @param index The index of the item in the data provider to invalidate.         *         * @see #invalidateItem()         * @see #invalidateList()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 *		 * @internal [kenos] This method appears to invalid an item, but it is described as		 *                   invalidating an item renderer. What is correct?
		 */
		public function invalidateItemAt (index:uint) : void;
		/**
		 * Sorts the elements of the current data provider. This method 		 * performs a sort based on the Unicode values of the elements. ASCII is a 		 * subset of Unicode.		 * 		 * @param sortArgs The arguments against which to sort.		 *         * @return The return value depends on whether any parameters are passed to 		 *         this method. For more information, see the <code>Array.sort()</code> method. 		 *         Note that this method returns 0 when the <code>sortArgs</code> parameter          *         is set to <code>Array.UNIQUESORT</code>.         *		 * @see #sortItemsOn()         * @see Array#sort()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function sortItems (...sortArgs:Array) : *;
		/**
		 * Sorts the elements of the current data provider by one or more  		 * of its fields.		 * 		 * @param field The field on which to sort.		 * @param options Sort arguments that are used to override the default sort behavior. 		 *                Separate two or more arguments with the bitwise OR (|) operator.		 *         * @return The return value depends on whether any parameters are passed to this		 *         method. For more information, see the <code>Array.sortOn()</code> method. 		 *         Note that this method returns 0 when the <code>sortOption</code> parameter           *         is set to <code>Array.UNIQUESORT</code>.         *         * @includeExample examples/SelectableList.sortItemsOn.1.as -noswf         *		 * @see #sortItems()         * @see fl.data.DataProvider#sortOn() DataProvider.sortOn()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function sortItemsOn (field:String, options:Object = null) : *;
		/**
		 * Checks whether the specified item is selected in the list. 		 *		 * @param item The item to check.		 *         * @return This method returns <code>true</code> if the specified item is selected; 		 *         otherwise, if the specified item has a value of <code>null</code> or is not		 *         included in the list, this method returns <code>false</code>.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function isItemSelected (item:Object) : Boolean;
		/**
		 * Scrolls the list to the item at the location indicated by    		 * the current value of the <code>selectedIndex</code> property.         *         * @see #selectedIndex		 * @see #scrollToIndex()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function scrollToSelected () : void;
		/**
		 * Scrolls the list to the item at the specified index. If the index 		 * is out of range, the scroll position does not change.		 *		 * @param newCaretIndex The index location to scroll to.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function scrollToIndex (newCaretIndex:int) : void;
		/**
		 * Returns the index of the next item in the dataProvider in which		 * the label's first character matches a specified string character.		 * If the search reaches the end of the dataProvider without searching		 * all the items, it will loop back to the start.  The search does not		 * include the startIndex.		 * 		 * @param firstLetter The string character to search for		 * @param startIndex The index in the dataProvider to start at.		 *		 * @return The index of the next item in the dataProvider.		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getNextIndexAtLetter (firstLetter:String, startIndex:int = -1) : int;
		/**
		 * Retrieves the string that the renderer displays for the given data object          * based on the <code>label</code> properties of the object.  This method		 * is intended to be overwritten in sub-components.  For example, List has		 * a <code>labelField</code> and a <code>labelFunction</code> to derive the		 * label.		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToLabel (item:Object) : String;
		/**
		 * Sets a style on the renderers in the list.		 *		 * @param name The name of the style to be set.		 * @param style The value of the style to be set.         *         * @includeExample examples/SelectableList.setRendererStyle.1.as -noswf         * @includeExample examples/SelectableList.setRendererStyle.2.as -noswf         * @includeExample examples/SelectableList.setRendererStyle.3.as -noswf         * @includeExample examples/SelectableList.setRendererStyle.4.as -noswf		 *         * @see #clearRendererStyle()         * @see #getRendererStyle()         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setRendererStyle (name:String, style:Object, column:uint = 0) : void;
		/**
		 * Retrieves a style that is set on the renderers in the list.		 *		 * @param name The name of the style to be retrieved.		 * @param style The value of the style to be retrieved.         *         * @see #clearRendererStyle()         * @see #setRendererStyle()		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getRendererStyle (name:String, column:int = -1) : Object;
		/**
		 * Clears a style that is set on the renderers in the list.		 *		 * @param name The name of the style to be cleared.		 *         * @see #getRendererStyle()         * @see #setRendererStyle()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clearRendererStyle (name:String, column:int = -1) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function _invalidateList () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleDataChange (event:DataChangeEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleCellRendererMouseEvent (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleCellRendererClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleCellRendererChange (event:Event) : void;
		/**
		 * @private (protected)
		 */
		protected function handleCellRendererDoubleClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setHorizontalScrollPosition (scroll:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setVerticalScrollPosition (scroll:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateRendererStyles () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawList () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)		 *  Moves the selection in a horizontal direction in response		 *  to the user selecting items using the left-arrow or right-arrow		 *  keys and modifiers such as  the Shift and Ctrl keys.		 *		 *  <p>Not implemented in List because the default list		 *  is single column and therefore doesn't scroll horizontally.</p>		 *		 *  @param code The key that was pressed (e.g. Keyboard.LEFT)         *		 *  @param shiftKey <code>true</code> if the shift key was held down when		 *         the keyboard key was pressed.         *		 *  @param ctrlKey <code>true</code> if the ctrl key was held down when         *         the keyboard key was pressed         *
		 */
		protected function moveSelectionHorizontally (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)		 *  Moves the selection in a vertical direction in response		 *  to the user selecting items using the up-arrow or down-arrow		 *  Keys and modifiers such as the Shift and Ctrl keys.		 *		 *  @param code The key that was pressed (e.g. Keyboard.DOWN)		 *         *  @param shiftKey <code>true</code> if the shift key was held down when		 *         the keyboard key was pressed.         *		 *  @param ctrlKey <code>true</code> if the ctrl key was held down when         *         the keyboard key was pressed         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionVertically (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onPreChange (event:DataChangeEvent) : void;
	}
}
