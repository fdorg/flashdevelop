package fl.controls.dataGridClasses
{
	import fl.controls.LabelButton;
	import fl.controls.listClasses.ListData;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.TextInput;
	import fl.core.UIComponent;
	import flash.events.Event;
	import flash.events.MouseEvent;

	/**
	 *  @copy fl.core.UIComponent#style:textFormat	 *     *  @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textFormat", type="flash.text.TextFormat")] 
	/**
	 *	Name of the class to use as the skin for the background and border	 *	of the DataGridCellEditor.     *     *  @default DataGridCellEditor_skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:textPadding     *     *  @default 1     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 

	/**
	 * The DataGridCellEditor class defines the default item editor for a 	 * DataGrid control. You can override the default item editor by subclassing 	 * the DataGridCellEditor class, or by creating your own cell editor class.     *     * @see fl.controls.listClasses.ICellRenderer     *     * @includeExample examples/DataGridCellEditorExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class DataGridCellEditor extends TextInput implements ICellRenderer
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _listData : ListData;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _data : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * @copy fl.controls.listClasses.ICellRenderer#listData         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get listData () : ListData;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set listData (value:ListData) : void;
		/**
		 * @copy fl.controls.listClasses.ICellRenderer#data         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get data () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set data (value:Object) : void;
		/**
		 * Indicates whether the cell is included in the		 * indices that were selected by the owner. A value of <code>true</code> indicates		 * that the cell is included in the specified indices; a value of <code>false</code>		 * indicates that it is not. 		 * 		 * <p>Note that this value cannot be changed in the DataGrid. 		 * The DataGridCellEditor class implements the ICellRenderer interface, which specifies 		 * that this value must be defined.</p>         *          * @default false		 *		 * @see fl.controls.listClasses.ICellRenderer ICellRenderer         *		 * @internal [kenos] If the getter always returns false, shouldn't we say so? I'd like to add such a sentence to the end		 * of the first paragraph.		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selected (value:Boolean) : void;

		/**
		 * Creates a new DataGridCellEditor instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function DataGridCellEditor ();
		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * @copy fl.controls.listClasses.ICellRenderer#setMouseState()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setMouseState (state:String) : void;
	}
}
