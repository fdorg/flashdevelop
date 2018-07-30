package fl.controls.dataGridClasses
{
	import fl.controls.ButtonLabelPlacement;
	import fl.controls.LabelButton;
	import fl.core.UIComponent;
	import flash.events.Event;
	import flash.events.MouseEvent;

	/**
	 * @copy fl.controls.LabelButton#style:selectedDisabledSkin     *     * @default HeaderRenderer_selectedDisabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedUpSkin     *     * @default HeaderRenderer_selectedUpSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedDownSkin     *     * @default HeaderRenderer_selectedDownSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedOverSkin     *     * @default HeaderRenderer_selectedOverSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedOverSkin", type="Class")] 

	/**
	 * The HeaderRenderer class displays the column header for the current 	 * DataGrid column. This class extends the LabelButton class and adds a      * <code>column</code> property that associates the current header with its 	 * DataGrid column.     *     * @see fl.controls.DataGrid DataGrid     *	 * @includeExample examples/HeaderRendererExample.as	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class HeaderRenderer extends LabelButton
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var _column : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * The index of the column that belongs to this HeaderRenderer instance.		 * 		 * <p>You do not need to know how to get or set this property		 * because it is internal. However, if you create your own  		 * HeaderRenderer, be sure to expose it; the HeaderRenderer is used  		 * by the DataGrid to maintain a reference between the header 		 * and the related DataGridColumn.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal Adobe: [LM] Added more details.  This *could* be marked (at)private.
		 */
		public function get column () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set column (value:uint) : void;

		/**
		 * Creates a new HeaderRenderer instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function HeaderRenderer ();
		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
	}
}
