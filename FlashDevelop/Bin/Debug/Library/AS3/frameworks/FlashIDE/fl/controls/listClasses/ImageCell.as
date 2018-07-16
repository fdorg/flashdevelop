package fl.controls.listClasses
{
	import fl.controls.listClasses.CellRenderer;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.listClasses.ListData;
	import fl.controls.listClasses.TileListData;
	import fl.controls.TextInput;
	import fl.containers.UILoader;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import flash.display.Graphics;
	import flash.display.Shape;
	import flash.events.IOErrorEvent;

	/**
	 * The skin that is used to indicate the selected state.	 *     * @default ImageCell_selectedSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedSkin", type="Class")] 
	/**
	 * The padding that separates the edge of the cell from the edge of the text, 	 * in pixels.	 *     * @default 3     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 
	/**
	 * The padding that separates the edge of the cell from the edge of the image, 	 * in pixels.	 *     * @default 1     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="imagePadding", type="Number", format="Length")] 
	/**
	 * The opacity of the overlay behind the cell label.	 *	 * @default 0.7	 *
	 */
	[Style(name="textOverlayAlpha", type="Number", format="Length")] 

	/**
	 * The ImageCell is the default cell renderer for the TileList     * component. An ImageCell class accepts <code>label</code> and 	 * <code>source</code> properties, and displays a thumbnail and 	 * single-line label.	 *	 * <p><strong>Note:</strong> When content is being loaded from a different 	 * domain or <em>sandbox</em>, the properties of the content may be inaccessible	 * for security reasons. For more information about how domain security 	 * affects the load process, see the Loader class.</p>     *     * @see flash.display.Loader Loader     *	 * @includeExample examples/ImageCellExample.as	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ImageCell extends CellRenderer implements ICellRenderer
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var textOverlay : Shape;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var loader : UILoader;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * Gets or sets the list properties that are applied to the cell, for example,		 * the <code>index</code> and <code>selected</code> values. These list properties		 * are automatically updated after the cell is invalidated.		 *		 * <p>Although the listData property returns an instance of ListData, in the		 * TileList cells receive an instance of <code>TileListData</code> instead, 		 * which contains a <code>source</code> property.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get listData () : ListData;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set listData (value:ListData) : void;
		/**
		 * Gets or sets an absolute or relative URL that identifies the 		 * location of the SWF or image file to load, the class name 		 * of a movie clip in the library, or a reference to a display 		 * object.		 * 		 * <p>Valid image file formats include GIF, PNG, and JPEG.</p>         *         * @default null         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : Object;
		/**
		 * @private (setter)		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set source (value:Object) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new ImageCell instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ImageCell ();
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
		 * @private (protected)
		 */
		protected function handleErrorEvent (event:IOErrorEvent) : void;
	}
}
