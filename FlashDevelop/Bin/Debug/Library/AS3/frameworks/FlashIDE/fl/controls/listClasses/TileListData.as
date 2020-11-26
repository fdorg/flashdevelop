package fl.controls.listClasses
{
	import fl.controls.listClasses.ListData;
	import fl.core.UIComponent;

	/**
	 * TileListData is a messenger class that holds information relevant to a specific      * cell in the list-based TileListData component. This information includes the label 	 * and image source that are associated with the cell; whether or not the cell is selected; 	 * and the position of the cell in the list by row and column. 	 *	 * <p>A new TileListData component is created for a cell renderer 	 * each time it is invalidated.</p>	 *	 * @see fl.controls.listClasses.ListData ListData     *	 * @includeExample examples/TileListDataExample.as	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class TileListData extends ListData
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _source : Object;

		/**
		 * Gets or sets an absolute or relative URL that identifies the 		 * location of the SWF or image file to load, the class name 		 * of a movie clip in the library, or a reference to a display 		 * object.  The TileListData does not load the source, it only		 * passes the value of the source on to the ImageCell.		 * 		 * <p>Valid image file formats include GIF, PNG, and JPEG.</p>         *         * @default null         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : Object;

		/**
		 * Creates a new instance of the TileListData class as specified by its 		 * parameters. The TileListData class inherits the properties of the ListData		 * class, and adds a source parameter for the path to the image that is		 * associated with the cell.         *         * @param label The label to be displayed in this cell.         * @param icon The icon to be displayed in this cell.		 * @param source The path or class that is associated with the content to be displayed in the cell.         * @param owner The component that owns this cell.         * @param index The index of the item in the data provider.         * @param row The row in which this item is being displayed. In a List or          *        DataGrid, this corresponds to the index. In a TileList it may be          *        different than the index.         * @param col The column in which this item is being displayed. In a List         *        this will always be equal to 0.		 *          * @see fl.controls.listClasses.ListData ListData         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function TileListData (label:String, icon:Object, source:Object, owner:UIComponent, index:uint, row:uint, col:uint = 0);
	}
}
