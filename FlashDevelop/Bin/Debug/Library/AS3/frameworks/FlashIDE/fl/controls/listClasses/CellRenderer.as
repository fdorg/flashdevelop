package fl.controls.listClasses
{
	import fl.controls.ButtonLabelPlacement;
	import fl.controls.listClasses.ListData;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.LabelButton;
	import fl.core.UIComponent;
	import flash.events.Event;
	import flash.events.MouseEvent;

	/**
	 * @copy fl.controls.LabelButton#style:upSkin     *     * @default CellRenderer_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:downSkin     *     * @default CellRenderer_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:overSkin     *     *  @default CellRenderer_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="overSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:disabledSkin     *     *  @default CellRenderer_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedDisabledSkin     *     *  @default CellRenderer_selectedDisabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDisabledSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedUpSkin     *     *  @default CellRenderer_selectedUpSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedUpSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedDownSkin     *     *  @default CellRenderer_selectedDownSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDownSkin", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedOverSkin     *     *  @default CellRenderer_selectedOverSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedOverSkin", type="Class")] 
	/**
	 *  @copy fl.core.UIComponent#style:textFormat	 *     *  @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textFormat", type="flash.text.TextFormat")] 
	/**
	 *  @copy fl.core.UIComponent#style:disabledTextFormat     *     *  @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledTextFormat", type="flash.text.TextFormat")] 
	/**
	 *  @copy fl.controls.LabelButton#style:textPadding     *     *  @default 5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 

	/**
	 * The CellRenderer class defines methods and properties for  	 * list-based components to use to manipulate and display custom 	 * cell content in each of their rows. A customized cell can contain	 * text, an existing component such as a CheckBox, or any class that 	 * you create. The list-based components that use this class include 	 * the List, DataGrid, TileList, and ComboBox components.     *     * @see ICellRenderer     *     * @includeExample examples/CellRendererExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class CellRenderer extends LabelButton implements ICellRenderer
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
		 * @copy fl.controls.listClasses.ICellRenderer#selected         *         * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selected (value:Boolean) : void;

		/**
		 * Creates a new CellRenderer instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function CellRenderer ();
		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Specifies the dimensions at which the data should be rendered. 		 * These dimensions affect both the data and the cell that contains it; 		 * the cell renderer uses them to ensure that the data fits the cell and 		 * does not bleed into adjacent cells. 		 *         * @param width The width of the object, in pixels.		 *         * @param height The height of the object, in pixels.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSize (width:Number, height:Number) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function toggleSelected (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
	}
}
