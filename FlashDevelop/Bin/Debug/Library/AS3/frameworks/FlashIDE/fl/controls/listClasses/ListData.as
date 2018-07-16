package fl.controls.listClasses
{
	import fl.core.UIComponent;

	/**
	 * ListData is a messenger class that holds information relevant to a specific      * cell in a list-based component. This information includes the label and icon that are	 * associated with the cell; whether or not the cell is selected; and the position of 	 * the cell in the list by row and column. 	 *	 * <p>A new ListData component is created for a cell renderer 	 * each time it is invalidated.</p>     *	 * @includeExample examples/ListDataExample.as	*	 * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ListData
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _icon : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _label : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _owner : UIComponent;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _index : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _row : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _column : uint;

		/**
		 * The label to be displayed in the cell.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get label () : String;
		/**
		 * A class that represents the icon for the item in the List component,          * computed from the List class method.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] I think this could be more clear. What is "computed" from the 		 *                   List class method? Also not sure I understand what method the		 *                   "List class method" is. Is what is "computed" an array location		 *                   for the specified item?
		 */
		public function get icon () : Object;
		/**
		 * A reference to the List object that owns this item.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get owner () : UIComponent;
		/**
		 * The index of the item in the data provider.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get index () : uint;
		/**
		 * The row in which the data item is displayed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get row () : uint;
		/**
		 * The column in which the data item is displayed. In a list,          * this value is always 0.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get column () : uint;

		/**
		 * Creates a new instance of the ListData class as specified by its parameters.          *         * @param label The label to be displayed in this cell.         *         * @param icon The icon to be displayed in this cell.         *         * @param owner The component that owns this cell.         *         * @param index The index of the item in the data provider.         *         * @param row The row in which this item is being displayed. In a List or          *        DataGrid, this value corresponds to the index. In a TileList, this		 *        value may be different than the index.         *         * @param col The column in which this item is being displayed. In a List,          *        this value is always 0.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ListData (label:String, icon:Object, owner:UIComponent, index:uint, row:uint, col:uint = 0);
	}
}
