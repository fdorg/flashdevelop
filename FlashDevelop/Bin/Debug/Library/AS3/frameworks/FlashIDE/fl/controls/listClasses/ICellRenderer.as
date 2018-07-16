package fl.controls.listClasses
{
	import fl.controls.listClasses.ListData;

	/**
	 * The ICellRenderer interface provides the methods and properties that a cell renderer requires.	 * All user defined cell renderers should implement this interface. All user defined cell renderers	 * must extend either the UIComponent class or a subclass of the UIComponent class.	 *	 * @includeExample examples/ICellRendererExample.as -noswf	 * @includeExample examples/MyRenderer.as	 *     * @see CellRenderer     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public interface ICellRenderer
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set y (y:Number) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set x (x:Number) : void;
		/**
		 * Gets or sets the list properties that are applied to the cell--for example,		 * the <code>index</code> and <code>selected</code> values. These list properties		 * are automatically updated after the cell is invalidated.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get listData () : ListData;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set listData (value:ListData) : void;
		/**
		 * Gets or sets an Object that represents the data that is 		 * associated with a component. When this value is set, the 		 * component data is stored and the containing component is 		 * invalidated. The invalidated component is then automatically 		 * redrawn.		 *		 * <p>The data property represents an object containing the item		 * in the DataProvider that the cell represents.  Typically, the		 * data property contains standard properties, depending on the		 * component type. In CellRenderer in a List or ComboBox component		 * the data contains a label, icon, and data properties; a TileList: a 		 * label and a source property; a DataGrid cell contains values		 * for each column.  The data property can also contain user-specified		 * data relevant to the specific cell. Users can extend a CellRenderer		 * for a component to utilize different properties of the data 		 * in the rendering of the cell.</p>		 *		 * <p>Additionally, the <code>labelField</code>, <code>labelFunction</code>, 		 * <code>iconField</code>, <code>iconFunction</code>, <code>sourceField</code>, 		 * and <code>sourceFunction</code> elements can be used to specify which properties 		 * are used to draw the label, icon, and source respectively.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get data () : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set data (value:Object) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the		 * current cell is selected. A value of <code>true</code> indicates		 * that the current cell is selected; a value of <code>false</code> 		 * indicates that it is not.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selected (value:Boolean) : void;

		/**
		 * Sets the size of the data according to the pixel values specified by the <code>width</code>		 * and <code>height</code> parameters.		 *         * @param width The width to display the cell renderer at, in pixels.		 *         * @param height The height to display the cell renderer at, in pixels.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSize (width:Number, height:Number) : void;
		/**
		 * Sets the current cell to a specific mouse state.  This method 		 * is necessary for the DataGrid to set the mouse state on an entire         * row when the user interacts with a single cell.         *         * @param state A string that specifies a mouse state, such as "up" or "over".          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setMouseState (state:String) : void;
	}
}
