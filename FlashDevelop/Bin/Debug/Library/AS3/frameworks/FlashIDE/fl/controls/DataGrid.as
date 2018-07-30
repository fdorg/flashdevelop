package fl.controls
{
	import fl.controls.dataGridClasses.DataGridColumn;
	import fl.controls.dataGridClasses.HeaderRenderer;
	import fl.controls.listClasses.CellRenderer;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.listClasses.ListData;
	import fl.controls.ScrollPolicy;
	import fl.controls.SelectableList;
	import fl.controls.TextInput;
	import fl.core.UIComponent;
	import fl.core.InvalidationType;
	import fl.data.DataProvider;
	import fl.events.ScrollEvent;
	import fl.events.ListEvent;
	import fl.events.DataGridEvent;
	import fl.events.DataGridEventReason;
	import fl.events.DataChangeType;
	import fl.events.DataChangeEvent;
	import fl.managers.IFocusManager;
	import fl.managers.IFocusManagerComponent;
	import flash.display.Sprite;
	import flash.display.Graphics;
	import flash.display.InteractiveObject;
	import flash.display.DisplayObjectContainer;
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.FocusEvent;
	import flash.ui.Keyboard;
	import flash.ui.Mouse;
	import flash.display.DisplayObject;
	import flash.utils.Dictionary;
	import flash.utils.describeType;
	import flash.geom.Point;
	import flash.system.IME;

	/**
	 * Dispatched after the user clicks a header cell.     *     * @includeExample examples/DataGrid.headerRelease.1.as -noswf     *     * @eventType fl.events.DataGridEvent.HEADER_RELEASE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="headerRelease", type="fl.events.DataGridEvent")] 
	/**
	 * Dispatched after a user expands a column horizontally.     *     * @includeExample examples/DataGrid.columnStretch.1.as -noswf     *     * @eventType fl.events.DataGridEvent.COLUMN_STRETCH     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="columnStretch", type="fl.events.DataGridEvent")] 
	/**
	 * Dispatched after a user prepares to edit an item, for example,	 * by releasing the mouse button over the item.     *     * @eventType fl.events.DataGridEvent.ITEM_EDIT_BEGINNING     *     * @see #event:itemEditBegin     * @see #event:itemEditEnd     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemEditBeginning", type="fl.events.DataGridEvent")] 
	/**
	 *  Dispatched after the <code>editedItemPosition</code> property is set	 *  and the item can be edited.     *     *  @eventType fl.events.DataGridEvent.ITEM_EDIT_BEGIN     *     * @see #event:itemEditBeginning     * @see #event:itemEditEnd     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemEditBegin", type="fl.events.DataGridEvent")] 
	/**
	 *  Dispatched when an item editing session ends for any reason.     *     *  @eventType fl.events.DataGridEvent.ITEM_EDIT_END     *     * @see #event:itemEditBegin     * @see #event:itemEditBeginning     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemEditEnd", type="fl.events.DataGridEvent")] 
	/**
	 *  Dispatched after an item receives focus.     *     *  @eventType fl.events.DataGridEvent.ITEM_FOCUS_IN     *     * @see #event:itemFocusOut     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemFocusIn", type="fl.events.DataGridEvent")] 
	/**
	 *  Dispatched after an item loses focus.     *     *  @eventType fl.events.DataGridEvent.ITEM_FOCUS_OUT     *     * @see #event:itemFocusIn     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemFocusOut", type="fl.events.DataGridEvent")] 
	/**
	 * The name of the class that provides the cursor that is used when 	 * the mouse is between two column headers and the <code>resizableColumns</code> 	 * property is set to <code>true</code>.     *     * @default DataGrid_columnStretchCursorSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="columnStretchCursorSkin", type="Class")] 
	/**
	 * The name of the class that provides the divider that appears	 * between columns.     *     * @default DataGrid_columnDividerSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="columnDividerSkin", type="Class")] 
	/**
	 * The name of the class that provides the background for each column header.     *     * @default HeaderRenderer_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerUpSkin", type="Class")] 
	/**
	 * The name of the class that provides the background for each column header	 * when the mouse is over it.     *     * @default HeaderRenderer_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerOverSkin", type="Class")] 
	/**
	 * The name of the class that provides the background for each column header	 * when the mouse is down.     *     * @default HeaderRenderer_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerDownSkin", type="Class")] 
	/**
	 * The name of the class that provides the background for each column header	 * when the component is disabled.     *     * @default HeaderRenderer_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerDisabledSkin", type="Class")] 
	/**
	 * The name of the class that provides the sort arrow when the sorted	 * column is in descending order.     *     * @default HeaderSortArrow_descIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerSortArrowDescSkin", type="Class")] 
	/**
	 * The name of the class that provides the sort arrow when the sorted	 * column is in ascending order.     *     * @default HeaderSortArrow_ascIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerSortArrowAscSkin", type="Class")] 
	/**
	 * The format to be applied to the text contained in each column header.     *     * @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerTextFormat", type="flash.text.TextFormat")] 
	/**
	 * The format to be applied to the text contained in each column header 	 * when the component is disabled.     *     * @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerDisabledTextFormat", type="flash.text.TextFormat")] 
	/**
	 * The padding that separates the column header border from the column header 	 * text, in pixels.     *     * @default 5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerTextPadding", type="Number", format="Length")] 
	/**
	 * The name of the class that provides each column header.     *     * @default fl.controls.dataGridClasses.HeaderRenderer     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="headerRenderer", type="Class")] 

	/**
	 * The DataGrid class is a list-based component that provides a grid of	 * rows and columns. You can specify an optional header row at the top 	 * of the component that shows all the property names. Each row consists 	 * of one or more columns, each of which represents a property that belongs	 * to the specified data object. The DataGrid component is used to view data; 	 * it is not intended to be used as a layout tool like an HTML table.	 *	 * <p>A DataGrid component is well suited for the display of objects that contain	 * multiple properties. The data that a DataGrid component displays can	 * be contained in a DataProvider object or as an array of objects. The columns of a DataGrid	 * component can be represented by a list of DataGridColumn objects, 	 * each of which contains information that is specific to the column.</p>	 *	 * <p>The DataGrid component provides the following features:</p>     * <ul>	 *     <li>Columns of different widths or identical fixed widths</li>	 * 	   <li>Columns that the user can resize at run time</li>	 * 	   <li>Columns that the user can reorder at run time by using ActionScript</li>	 * 	   <li>Optional customizable column headers</li>	 * 	   <li>Support for custom item renderers to display data other than text	 *         in any column</li>	 *     <li>Support for sorting data by clicking on the column that contains it</li>     * </ul>	 *	 * <p>The DataGrid component is composed of subcomponents including ScrollBar,	 * HeaderRenderer, CellRenderer, DataGridCellEditor, and ColumnDivider components, all of which 	 * can be skinned during authoring or at run time.</p>	 *	 * <p>The DataGrid component uses the following classes that can be found in the dataGridClasses package:</p>     * <ul>	 *     <li>DataGridColumn: Describes a column in a DataGrid component. Contains the indexes, 	 *         widths, and other properties of the column. Does not contain cell data.</li>	 *     <li>HeaderRenderer: Displays the column header for the current DataGrid column. Contains	 *         the label and other properties of the column header.</li>	 *     <li>DataGridCellEditor: Manages the editing of the data for each cell.</li>     * </ul>	 *     * @includeExample examples/DataGridExample.as     *     * @see fl.controls.dataGridClasses.DataGridCellEditor DataGridCellEditor     * @see fl.controls.dataGridClasses.DataGridColumn DataGridColumn     * @see fl.controls.dataGridClasses.HeaderRenderer HeaderRenderer     * @see fl.controls.listClasses.CellRenderer CellRenderer     * @see fl.events.DataGridEvent DataGridEvent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class DataGrid extends SelectableList implements IFocusManagerComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _rowHeight : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _headerHeight : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _showHeaders : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _columns : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _minColumnWidth : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var header : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var headerMask : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var headerSortArrow : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _cellRenderer : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _headerRenderer : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _labelFunction : Function;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var visibleColumns : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var displayableColumns : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var columnsInvalid : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var minColumnWidthInvalid : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var activeCellRenderersMap : Dictionary;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var availableCellRenderersMap : Dictionary;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var dragHandlesMap : Dictionary;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var columnStretchIndex : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var columnStretchStartX : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var columnStretchStartWidth : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var columnStretchCursor : Sprite;
		/**
		 *  @private (protected)         *  The index of the column being sorted.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _sortIndex : int;
		/**
		 *  @private (protected)         *  The index of the last column being sorted on.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var lastSortIndex : int;
		/**
		 *  @private (protected)         *  The direction of the current sort.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _sortDescending : Boolean;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _editedItemPosition : Object;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var editedItemPositionChanged : Boolean;
		/**
		 *  @private (protected)         *  <code>undefined</code> means we've processed it, <code>null</code> means don't put up an editor         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var proposedEditedItemPosition : *;
		/**
		 *  @private (protected)         *  Last known position of item editor instance         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var actualRowIndex : int;
		/**
		 *  @private (protected)         *  Last known position of item editor instance         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var actualColIndex : int;
		/**
		 *  @private (protected)         *  Whether the mouse button is pressed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var isPressed : Boolean;
		/**
		 *  @private (protected)         *  True if we want to block editing on mouseUp.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var losingFocus : Boolean;
		/**
		 *  @private (protected)         *  Stores the user set headerheight (we modify header height when dg is resized down)          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var maxHeaderHeight : Number;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var currentHoveredRow : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected static const HEADER_STYLES : Object;
		/**
		 *  @private		 *  Creates the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var createAccessibilityImplementation : Function;
		/**
		 *  Indicates whether or not the user can edit items in the data provider.		 *  A value of <code>true</code> indicates that the user can edit items in the		 *  data provider; a value of <code>false</code> indicates that the user cannot.		 *		 *  <p>If this value is <code>true</code>, the item renderers in the component 		 *  are editable. The user can click on an item renderer to open an editor.</p>		 *		 *  <p>You can turn off editing for individual columns of the DataGrid component  		 *  by using the <code>DataGridColumn.editable</code> property, or by handling 		 *  the <code>itemEditBeginning</code> and <code>itemEditBegin</code> events.</p>		 *         *  @default false         *         * @see #event:itemEditBegin         * @see #event:itemEditBeginning         * @see fl.controls.dataGridClasses.DataGridColumn#editable DataGridColumn.editable         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var editable : Boolean;
		/**
		 *  Indicates whether the user can change the size of the         *  columns. A value of <code>true</code> indicates that the user can		 *  change the column size; a value of <code>false</code> indicates that 		 *  column size is fixed.		 *		 *  <p>If this value is <code>true</code>, the user can stretch or shrink 		 *  the columns of the DataGrid component by dragging the grid lines between 		 *  the header cells. Additionally, if this value is <code>true</code>,		 *  the user can change the size of the columns unless the <code>resizeable</code> 		 *  properties of individual columns are set to <code>false</code>.</p>		 *         *  @default true         *         * @includeExample examples/DataGrid.resizableColumns.1.as -noswf         *         * @see fl.controls.dataGridClasses.DataGridColumn#resizable DataGridColumn.resizable         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var resizableColumns : Boolean;
		/**
		 * Indicates whether the user can sort the items in the data provider 		 * by clicking on a column header cell. If this value is <code>true</code>, 		 * the user can sort the data provider items by clicking on a column header cell;		 * if this value is <code>false</code>, the user cannot.		 * 		 * <p>If this value is <code>true</code>, to prevent an individual column		 * from responding to a user mouse click on a header cell, set the 		 * <code>sortable</code> property of that column to <code>false</code>.</p>		 *		 * <p>The sort field of a column is either the <code>dataField</code> or 		 * <code>sortCompareFunction</code> property of the DataGridColumn component.		 * If the user clicks a column more than one time, the sort operation		 * alternates between ascending and descending order.</p>		 * 		 * <p>If both this property and the <code>sortable</code> property of a		 * column are set to <code>true</code>, the DataGrid component dispatches		 * a <code>headerRelease</code> event after the user releases the mouse		 * button of the column header cell. If a call is not made to the <code>preventDefault()</code>		 * method from a handler method of the <code>headerRelease</code> event, 		 * the DataGrid component performs a sort based on the values of the <code>dataField</code>  		 * or <code>sortCompareFunction</code> properties.</p>		 *         * @default true         *         * @includeExample examples/DataGrid.sortableColumns.1.as -noswf         *         * @see fl.controls.dataGridClasses.DataGridColumn#sortable DataGridColumn.sortable         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var sortableColumns : Boolean;
		/**
		 *  A reference to the currently active instance of the item editor,		 *  if one exists.		 *		 *  <p>To access the item editor instance and the new item value when an		 *  item is being edited, use the <code>itemEditorInstance</code>		 *  property. The <code>itemEditorInstance</code> property is not valid 		 *  until after the event listener for the <code>itemEditBegin</code> 		 *  event executes. For this reason, the <code>itemEditorInstance</code> property 		 *  is typically accessed from the event listener for the <code>itemEditEnd</code> 		 *  event.</p>		 *		 *  <p>The <code>DataGridColumn.itemEditor</code> property defines the		 *  class of the item editor, and therefore, the data type of the         *  item editor instance.</p>         *         * @includeExample examples/DataGrid.itemEditorInstance.1.as -noswf         *         * @see #event:itemEditBegin         * @see #event:itemEditEnd         * @see fl.controls.dataGridClasses.DataGridColumn#itemEditor DataGridColumn.itemEditor         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var itemEditorInstance : Object;

		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set dataProvider (dataSource:DataProvider) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the 		 * horizontal scroll bar is always on. The following list describes		 * the valid values:		 *		 * <ul>		 *   <li><code>ScrollPolicy.ON</code>: The scroll bar is always on.</li>		 *   <li><code>ScrollPolicy.OFF</code>: The scroll bar is always off.</li>		 *   <li><code>ScrollPolicy.AUTO</code>: The state of the scroll bar changes 		 *        based on the parameters that are passed to the <code>setScrollBarProperties()</code> 		 *        method.</li>		 * </ul> 		 * 		 * <p><strong>Note:</strong> If the combined width of the visible columns in the DataGrid 		 * component is smaller than the available width of the DataGrid component, the columns may not expand to fill		 * the available space of the DataGrid component, depending on the value of the 		 * <code>horizontalScrollPolicy</code> property. The following list describes		 * these values and their effects:</p>		 *		 * <ul>		 *   <li><code>ScrollPolicy.ON</code>: The horizontal scroll bar is disabled. The columns do not expand		 *         to fill the available space of the DataGrid component.</li>		 *   <li><code>ScrollPolicy.AUTO</code>: The horizontal scroll bar is not visible. The columns do not expand		 *         to fill the available space of the DataGrid component.</li>		 * </ul>		 *		 * @default ScrollPolicy.OFF         *         * @see fl.containers.BaseScrollPane#verticalScrollPolicy BaseScrollPane.verticalScrollPolicy         * @see ScrollPolicy         *		 * @includeExample examples/DataGrid.horizontalScrollPolicy.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalScrollPolicy () : String;
		/**
		 * @private (protected)		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalScrollPolicy (policy:String) : void;
		/**
		 * Gets or sets an array of DataGridColumn objects, one for each column that can be displayed.		 * If not explicitly set, the DataGrid component examines the first item in the		 * data provider, locates its properties, and then displays those properties		 * in alphabetic order.         *		 * <p>You can make changes to the columns and to their order in this DataGridColumn 		 * array. After the changes are made, however, you must explicitly assign the 		 * changed array to the <code>columns</code> property. If an explicit assignment		 * is not made, the set of columns that was used before will continue to be used.</p>		 *         * @default []         *         * @includeExample examples/DataGrid.columns.1.as -noswf         * @includeExample examples/DataGrid.columns.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get columns () : Array;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set columns (value:Array) : void;
		/**
		 * Gets or sets the minimum width of a DataGrid column, in pixels. 		 * If this value is set to <code>NaN</code>, the minimum column		 * width can be individually set for each column of the DataGrid component.		 *         * @default NaN         *         * @includeExample examples/DataGrid.minColumnWidth.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get minColumnWidth () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set minColumnWidth (value:Number) : void;
		/**
		 * Gets or sets a function that determines which fields of each 		 * item to use for the label text.         *         * @default null         *         * @includeExample examples/DataGrid.labelFunction.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelFunction (value:Function) : void;
		/**
		 * Gets or sets the number of rows that are at least partially visible in the         * list.         *         * @includeExample examples/DataGrid.rowCount.1.as -noswf         *         * @see SelectableList#length SelectableList.length         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowCount () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowCount (value:uint) : void;
		/**
		 * Gets or sets the height of each row in the DataGrid component, in pixels.         *         * @default 20         *         * @includeExample examples/DataGrid.rowHeight.1.as -noswf         *         * @see #headerHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowHeight () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowHeight (value:Number) : void;
		/**
		 * Gets or sets the height of the DataGrid header, in pixels.         *         * @default 25         *         * @includeExample examples/DataGrid.headerHeight.1.as -noswf         *         * @see #rowHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get headerHeight () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set headerHeight (value:Number) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the DataGrid component shows column headers. 		 * A value of <code>true</code> indicates that the DataGrid component shows column headers; a value 		 * of <code>false</code> indicates that it does not.         *         * @default true         *         * @includeExample examples/DataGrid.showHeaders.1.as -noswf		 *		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get showHeaders () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set showHeaders (value:Boolean) : void;
		/**
		 * Gets the index of the column to be sorted.         *         * @default -1         *         * @see #sortDescending         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get sortIndex () : int;
		/**
		 * Gets the order in which a column is sorted when 		 * the user clicks its header. A value of <code>true</code> 		 * indicates that the column is sorted in descending order; a 		 * value of <code>false</code> indicates that the column is 		 * sorted in ascending order. 		 *		 * <p>The <code>sortDescending</code> property does not affect		 * how the sort method completes the sort operation. By default, 		 * the sort operation involves a case-sensitive string sort. 		 * To change this behavior, modify the <code>sortOptions</code> 		 * and <code>sortCompareFunction</code> properties of the DataGridColumn		 * class.</p>		 *		 * <p><strong>Note:</strong> If you query this property from an event 		 * listener for the <code>headerRelease</code> event, the property value 		 * identifies the sort order for the previous sort operation. This 		 * is because the next sort has not yet occurred.</p>		 *		 *          *		 * @default false         *         * @includeExample examples/DataGrid.sortDescending.1.as -noswf         *         * @see fl.controls.dataGridClasses.DataGridColumn#sortOptions DataGridColumn.sortOptions         * @see fl.controls.dataGridClasses.DataGridColumn#sortCompareFunction DataGridColumn.sortCompareFunction		 * @see Array#sort() Array.sort()         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get sortDescending () : Boolean;
		/**
		 * @copy fl.controls.TextArea#imeMode         *         * @see flash.system.IMEConversionMode IMEConversionMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get imeMode () : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set imeMode (value:String) : void;
		/**
		 * Gets a reference to the item renderer in the DataGrid component whose item is currently being  		 * edited. If no item is being edited, this property contains a value of <code>null</code>.		 *		 * <p>You can obtain the current value of the item that is being edited by using the		 * <code>editedItemRenderer.data</code> property from an event listener for the  		 * <code>itemEditBegin</code> event or the <code>itemEditEnd</code> event.</p>		 *		 * <p>This is a read-only property. To set a custom item editor, use the <code>itemEditor</code>  		 * property of the class that represents the relevant column.</p>         *		 * @see fl.controls.dataGridClasses.DataGridColumn#itemEditor DataGridColumn.itemEditor         *         * @includeExample examples/DataGrid.editedItemPosition.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get editedItemRenderer () : ICellRenderer;
		/**
		 * Gets or sets the column and row index of the item renderer for 		 * the data provider item that is being edited. If no item is being		 * edited, this property is <code>null</code>.		 *		 * <p>This object has two fields:</p>		 *		 * <ul>		 *     <li><code>columnIndex</code>: The zero-based column index of the current item</li>		 *     <li><code>rowIndex</code>: The zero-based row index of the current item</li>		 * </ul>		 * 		 * <p>For example: <code>{ columnIndex:2, rowIndex:3 }</code></p>		 *		 * <p>Setting this property scrolls the item into view and dispatches the 		 * <code>itemEditBegin</code> event to open an item editor on the specified 		 * item renderer.</p>		 *         * @default null         *         * @includeExample examples/DataGrid.editedItemPosition.1.as -noswf         *         * @see #event:itemEditBegin         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get editedItemPosition () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set editedItemPosition (value:Object) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new DataGrid component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function DataGrid ();
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSize (w:Number, h:Number) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateAvailableHeight () : Number;
		/**
		 * Adds a column to the end of the <code>columns</code> array.		 *		 * @param column A String or a DataGridColumn object.		 *         * @return The DataGridColumn object that was added.         *         * @see #addColumnAt()         *         * @includeExample examples/DataGrid.addColumn.2.as -noswf         * @includeExample examples/DataGrid.addColumn.3.as -noswf         * @includeExample examples/DataGrid.addColumn.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addColumn (column:*) : DataGridColumn;
		/**
		 * Inserts a column at the specified index in the <code>columns</code> array.		 *		 * @param column The string or DataGridColumn object that represents the column to be inserted.		 * @param index The array index that identifies the location at which the column is to be inserted.         *         * @return The DataGridColumn object that was inserted into the array of columns.         *         * @see #addColumn()         *         * @includeExample examples/DataGrid.addColumn.1.as -noswf		          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addColumnAt (column:*, index:uint) : DataGridColumn;
		/**
		 * Removes the column that is located at the specified index of the <code>columns</code> array.		 *		 * @param index The index of the column to be removed.         *         * @return The DataGridColumn object that was removed. This method returns <code>null</code> 		 * if a column is not found at the specified index.         *         * @see #removeAllColumns()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeColumnAt (index:uint) : DataGridColumn;
		/**
		 * Removes all columns from the DataGrid component.         *         * @see #removeColumnAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeAllColumns () : void;
		/**
		 * Retrieves the column that is located at the specified index of the <code>columns</code> array.		 *		 * @param index The index of the column to be retrieved, or <code>null</code>          *        if a column is not found.         *         * @return The DataGridColumn object that was found at the specified index.         *         * @see #getColumnIndex()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getColumnAt (index:uint) : DataGridColumn;
		/**
		 * Retrieves the index of the column of the specified name,		 * or -1 if no match is found.		 *		 * @param name The data field of the column to be located.		 *         * @return The index of the location at which the column of the 		 *         specified name is found.         *         * @see #getColumnAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getColumnIndex (name:String) : int;
		/**
		 * Retrieves the number of columns in the DataGrid component.		 *         * @return The number of columns contained in the DataGrid component.         *         * @includeExample examples/DataGrid.columns.2.as -noswf         * @includeExample examples/DataGrid.columns.3.as -noswf         *         * @see #rowCount         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getColumnCount () : uint;
		/**
		 * Resets the widths of the visible columns to the same size.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function spaceColumnsEqually () : void;
		/**
		 * Edits a given field or property in the DataGrid component.         *         * @param index The index of the data provider item to be edited.         *         * @param dataField The name of the field or property in the data provider item to be edited.         *         * @param data The new data value.		 *         * @throws RangeError The specified index is less than 0 or greater than or equal to the 		 *         length of the data provider.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function editField (index:uint, dataField:String, data:Object) : void;
		/**
		 * The DataGrid component has multiple cells for any given item, so the <code>itemToCellRenderer</code>		 * method always returns <code>null</code>.         *         * @param item The item in the data provider.         *         * @return <code>null</code>.         *		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToCellRenderer (item:Object) : ICellRenderer;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawList () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateRendererStyles () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function removeCellRenderers () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function removeCellRenderersByColumn (col:DataGridColumn) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleCellRendererMouseEvent (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function isHovered (renderer:ICellRenderer) : Boolean;
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
		public function columnItemToLabel (columnIndex:uint, item:Object) : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateColumnSizes () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateContentWidth () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleHeaderRendererClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function resizeColumn (columnIndex:int, w:Number) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function sortByColumn (index:int) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function createColumnsFromDataProvider () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getVisibleColumnIndex (column:DataGridColumn) : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleHeaderResizeOver (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleHeaderResizeOut (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleHeaderResizeDown (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleHeaderResizeMove (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleHeaderResizeUp (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function showColumnStretchCursor (show:Boolean = true) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function positionColumnStretchCursor (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setEditedItemPosition (coord:Object) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function commitEditedItemPosition (coord:Object) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function itemEditorItemEditBeginningHandler (event:DataGridEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function itemEditorItemEditBeginHandler (event:DataGridEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function itemEditorItemEditEndHandler (event:DataGridEvent) : void;
		/**
		 *  @private (protected)         *  When we get focus, focus an item renderer.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusInHandler (event:FocusEvent) : void;
		/**
		 *  @private (protected)         *  When we lose focus, close the editor.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusOutHandler (event:FocusEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function editorMouseDownHandler (event:MouseEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function editorKeyDownHandler (event:KeyboardEvent) : void;
		/**
		 *  @private (protected)		 *  Determines the next item renderer to navigate to using the Tab key.		 *  If the item renderer to be focused falls out of range (the end or beginning         *  of the grid) then move focus outside the grid.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function findNextItemRenderer (shiftKey:Boolean) : Boolean;
		/**
		 *  @private (protected)         *  Find the next item renderer down from the currently edited item renderer, and focus it.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function findNextEnterItemRenderer (event:KeyboardEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function mouseFocusChangeHandler (event:MouseEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyFocusChangeHandler (event:FocusEvent) : void;
		/**
		 *  @private         *  Hides the itemEditorInstance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function itemEditorFocusOutHandler (event:FocusEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function deactivateHandler (event:Event) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function mouseDownHandler (event:MouseEvent) : void;
		/**
		 *  @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function mouseUpHandler (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleCellRendererClick (event:MouseEvent) : void;
		/**
		 *  Uses the editor specified by the <code>itemEditor</code> property to 		 *  create an item editor for the item renderer at the column and row index 		 *  identified by the <code>editedItemPosition</code> property.		 *		 *  <p>This method sets the editor instance as the <code>itemEditorInstance</code> 		 *  property.</p>		 *		 *  <p>You can call this method from the event listener for the <code>itemEditBegin</code> 		 *  event. To create an editor from other code, set the <code>editedItemPosition</code> 		 *  property to generate the <code>itemEditBegin</code> event.</p>		 *		 *  @param colIndex The column index of the item to be edited in the data provider.         *  @param rowIndex The row index of the item to be edited in the data provider.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function createItemEditor (colIndex:uint, rowIndex:uint) : void;
		/**
		 *  Closes an item editor that is currently open on an item renderer. This method is 		 *  typically called from the event listener for the <code>itemEditEnd</code> event, 		 *  after a call is made to the <code>preventDefault()</code> method to prevent the 		 *  default event listener from executing.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function destroyItemEditor () : void;
		/**
		 *  @private (protected)		 *  This method is called after the user finishes editing an item.         *  It dispatches the <code>itemEditEnd</code> event to start the process		 *  of copying the edited data from         *  the <code>itemEditorInstance</code> to the data provider and hiding the <code>itemEditorInstance</code>.         *  returns <code>true</code> if nobody called the <code>preventDefault()</code> method.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function endEdit (reason:String) : Boolean;
		/**
		 * Get the instance of a cell renderer at the specified position         * in the DataGrid.         *         * <p><strong>Note:</strong> This method returns <code>null</code>         * for positions that are not visible (i.e. scrolled out of the         * view).</p>         *         * @param row A row index.         * @param column A column index.         *         * @return The ICellRenderer object at the specified position, or         * <code>null</code> if no cell renderer exists at that position.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getCellRendererAt (row:uint, column:uint) : ICellRenderer;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function itemRendererContains (renderer:Object, object:DisplayObject) : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleDataChange (event:DataChangeEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)		 *  Moves the selection in a horizontal direction in response		 *  to the user selecting items using the left-arrow or right-arrow		 *  keys and modifiers such as  the Shift and Ctrl keys.		 *		 *  <p>Not implemented in List because the default list		 *  is single column and therefore doesn't scroll horizontally.</p>		 *		 *  @param code The key that was pressed (e.g. Keyboard.LEFT)		 *  @param shiftKey <code>true</code> if the shift key was held down when		 *  the keyboard key was pressed.		 *  @param ctrlKey <code>true</code> if the ctrl key was held down when         *  the keyboard key was pressed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionHorizontally (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)		 *  Moves the selection in a vertical direction in response		 *  to the user selecting items using the up-arrow or down-arrow		 *  Keys and modifiers such as the Shift and Ctrl keys.		 *		 *  @param code The key that was pressed (e.g. Keyboard.DOWN)		 *  @param shiftKey <code>true</code> if the shift key was held down when		 *  the keyboard key was pressed.		 *  @param ctrlKey <code>true</code> if the ctrl key was held down when         *  the keyboard key was pressed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionVertically (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @copy fl.controls.SelectableList#scrollToIndex()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function scrollToIndex (newCaretIndex:int) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function scrollToPosition (rowIndex:int, columnIndex:int) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function doKeySelection (newCaretIndex:int, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
	}
}
