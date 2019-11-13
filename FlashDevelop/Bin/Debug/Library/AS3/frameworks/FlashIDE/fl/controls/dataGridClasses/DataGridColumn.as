package fl.controls.dataGridClasses
{
	import fl.controls.DataGrid;
	import fl.controls.listClasses.ICellRenderer;
	import fl.core.InvalidationType;
	import fl.controls.dataGridClasses.DataGridCellEditor;

	/**
	 * The DataGridColumn class describes a column in a DataGrid component. There      * is one DataGridColumn object for every column that could be displayed on	 * the screen, even for columns that are currently hidden or off-screen. The 	 * data provider items that belong to a DataGrid component can contain properties 	 * that are not displayed; such properties do not require a DataGridColumn. 	 *     * <p>You can specify the kind of component that displays the data for a DataGridColumn.	 * The characteristics that can be specified include the text that appears in the	 * column header and whether the column can be edited, sorted, or resized.</p>     *     * @see fl.controls.DataGrid     *     * @includeExample examples/DataGridColumnExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class DataGridColumn
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _columnName : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _headerText : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _minWidth : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _width : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _visible : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _cellRenderer : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _headerRenderer : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _labelFunction : Function;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _sortCompareFunction : Function;
		/**
		 *  @private         *  Storage for the <code>imeMode</code> property.
		 */
		private var _imeMode : String;
		/**
		 * @private (internal)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var owner : DataGrid;
		/**
		 * @private (internal)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var colNum : Number;
		/**
		 * @private (internal)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var explicitWidth : Number;
		/**
		 * Indicates whether the user can click on the header of the current column 		 * to sort the data provider. A value of <code>true</code> indicates that		 * the column can be sorted by clicking on its header; a value of <code>false</code> 		 * indicates that it cannot be sorted by clicking on its header.         *         * @default true         *         * @includeExample examples/DataGridColumn.sortable.1.as -noswf         * @includeExample examples/DataGridColumn.sortable.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var sortable : Boolean;
		/**
		 * Indicates whether the user is allowed to change the width of the		 * column. A value of <code>true</code> indicates that the user can		 * change the column width; a value of <code>false</code> indicates that		 * the user cannot change the column width.         *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var resizable : Boolean;
		/**
		 *  Indicates whether the items in the column can be edited. A value of <code>true</code>		 *  indicates that the column items can be edited; a value of <code>false</code> indicates		 *  that they cannot be edited.		 *		 *  <p>If this property is set to <code>true</code> and the <code>editable</code>		 *  property of the DataGrid is also <code>true</code>, the items in a column are 		 *  editable and can be individually edited by clicking an item 		 *  or by navigating to the item by using the Tab and arrow keys.</p>         *         *  @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var editable : Boolean;
		/**
		 *  Indicates the class of the instances of the item editor to use for the 		 *  column, when it is editable.		 * 		 *  The type of this property can be Class, Sprite or String.		 *  If the property type is String, the string value must be a		 *  fully qualified class name.         *		 *  @default "fl.controls.dataGridClasses.DataGridCellEditor"         *         *  @includeExample examples/DataGridColumn.itemEditor.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var itemEditor : Object;
		/**
		 *  Identifies the name of the property of the item editor that contains the new		 *  data for the list item.		 *		 *  <p>For example, the default <code>itemEditor</code> is		 *  TextInput, so the default value of the <code>editorDataField</code> 		 *  property is <code>"text"</code>. This value specifies the 		 *  <code>text</code> property of the TextInput component.</p>         *         *  @default "text"         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var editorDataField : String;
		/**
		 * Identifies the name of the field or property in the data provider item         * that is associated with the column.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var dataField : String;
		/**
		 * Indicates whether the DataGridColumn is to be sorted in ascending or          * descending order. A value of <code>true</code> indicates that the          * DataGridColumn is sorted in descending order; a value of <code>false</code>          * indicates that the DataGridColum is sorted in ascending order.         *         * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var sortDescending : Boolean;
		/**
		 * One or more defined constants, identified by name or number and separated		 * by the bitwise OR (|) operator. These constants are used to specify		 * the sort operation.         *		 * @default 0         *         * @includeExample examples/DataGridColumn.sortOptions.1.as -noswf         *		 * @see Array#sort()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var sortOptions : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var forceImport : DataGridCellEditor;

		/**
		 * The class that is used to render the items in this column.		 *		 *  The type of this property can be Class, Sprite or String.		 *  If the property type is String, the String value must be a		 *  fully qualified class name.         *		 * @default null         *         * @includeExample examples/DataGridColumn.cellRenderer.1.as -noswf         *         * @see #headerRenderer DataGridColumn.headerRenderer         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get cellRenderer () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set cellRenderer (value:Object) : void;
		/**
		 * The class that is used to render the header of this column.		 *		 * <p>The type of this property can be Class, Sprite or String.		 *  If the property type is String, the string value must be a		 *  fully qualified class name.</p>         *         * @default null         *         * @see #cellRenderer DataGridColumn.cellRenderer         * @see HeaderRenderer         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get headerRenderer () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set headerRenderer (value:Object) : void;
		/**
		 * The column name to be displayed in the column header.         * By default, the DataGrid component uses the value of the <code>dataField</code>          * property as the column name.         *         * @includeExample examples/DataGridColumn.headerText.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get headerText () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set headerText (value:String) : void;
		/**
		 *  The mode of the input method editor (IME). The IME enables users to 		 *  enter text in Chinese, Japanese, and Korean. The flash.system.IMEConversionMode 		 *  class defines constants to be used as the valid values for this property.  		 *		 *  <p>If this property is <code>null</code>, the mode of the IME is 		 *  set to the value of the <code>imeMode</code> property of the DataGrid		 *  component.</p>		 *		 *  @default null		 *         *  @see flash.system.IMEConversionMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0         *		 * @internal [kenos] The IME probably allows users of a QWERTY keyboard to enter 		 * characters from the Chinese, Japanese, and Korean *character sets*. You can		 * render Japanese words in Roman letters, if you like, on a QWERTY keyboard,		 * so the existing description here is probably not quite right.
		 */
		public function get imeMode () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set imeMode (value:String) : void;
		/**
		 * A function that determines the text to be displayed in this column. By default, the		 * column displays the text for the data field that matches the column name.		 * However, a column can also be used to display the text of more than one data field, 		 * or to display content that is not in the proper format.  This can be done by         * using the <code>labelFunction</code> property to specify a callback function.		 *          * <p>If both the <code>labelFunction</code> and <code>labelField</code> properties          * are defined, the <code>labelFunction</code> takes precedence.</p>		 *         * @default null         *         * @includeExample examples/DataGridColumn.labelFunction.1.as -noswf         * @includeExample examples/DataGridColumn.sortCompareFunction.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelFunction (value:Function) : void;
		/**
		 * The minimum width of the column, in pixels.		 *		 * @default 20         *         * @includeExample examples/DataGridColumn.minWidth.1.as -noswf         *         * @see #width DataGridColumn.width         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get minWidth () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set minWidth (value:Number) : void;
		/**
		 * A callback function that is called when sorting the data in		 * the column. If this property is not specified, the data is sorted by string		 * or number, depending on the <code>sortOptions</code> property.         * When specified, the <code>sortCompareFunction</code> property allows you to create          * your own custom sorting method for the current data grid column.		 *         * @default null         *         * @see #sortOptions         *         * @includeExample examples/DataGridColumn.sortCompareFunction.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] More information here on what the callback is used to do? Sounds like		 * it supplants the sortOptions but is it used to specify any particular type of 		 * sort?
		 */
		public function get sortCompareFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set sortCompareFunction (value:Function) : void;
		/**
		 * Indicates whether the column is visible. A value of <code>true</code> indicates		 * that the column is visible; a value of <code>false</code> indicates that the column		 * is invisible.         *         * @includeExample examples/DataGridColumn.visible.1.as -noswf		 *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get visible () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set visible (value:Boolean) : void;
		/**
		 * The width of the column, in pixels.		 *         * @default 100         *         * @see #minWidth DataGridColumn.minWidth         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get width () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set width (value:Number) : void;

		/**
		 * Creates a new DataGridColumn instance.		 *          * @param columnName The column name to display in the column header. If         *        no name is specified, the <code>dataField</code> value is used.		 * 		 * @default null		 *          * @see DataGridColumn#headerText         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function DataGridColumn (columnName:String = null);
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setWidth (value:Number) : void;
		/**
		 *  Returns the string that the item renderer displays for the given data object.		 *  If the DataGridColumn or its DataGrid component has a non-null <code>labelFunction</code>		 *  property, it applies the function to the data object. Otherwise, the method extracts          *  the contents of the field that is specified by the <code>dataField</code> property, or gets the          *  string value of the data object. If the method cannot convert the parameter to a string,          *  it returns a single space.		 *		 *  @param data The Object to be rendered.		 *         *  @return Displayable string based on the specified <code>data</code> object.         *         *  @internal         *    dg.addEventListener("itemClick", itemClickHandler);         *    function itemClickHandler(evt:ListEvent):void {         *    	var col:DataGridColumn = dg.getColumnAt(evt.columnIndex);         *    	trace(col.itemToLabel(evt.item));         *    }         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToLabel (data:Object) : String;
		/**
		 *  Returns a string representation of the DataGridColumn object.		 *         *  @return "[object DataGridColumn]"         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
	}
}
