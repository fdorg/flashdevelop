package fl.controls
{
	import fl.controls.listClasses.CellRenderer;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.listClasses.ImageCell;
	import fl.controls.listClasses.ListData;
	import fl.controls.listClasses.TileListData;
	import fl.controls.ScrollBar;
	import fl.controls.ScrollBarDirection;
	import fl.controls.ScrollPolicy;
	import fl.controls.SelectableList;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.data.DataProvider;
	import fl.data.TileListCollectionItem;
	import fl.events.DataChangeEvent;
	import fl.events.DataChangeType;
	import fl.events.ListEvent;
	import fl.events.ScrollEvent;
	import fl.managers.IFocusManagerComponent;
	import flash.display.DisplayObject;
	import flash.display.Sprite;
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.ui.Keyboard;
	import flash.utils.Dictionary;

	/**
	 * The skin to be used as the background of the TileList component.     *     * @default TileList_skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="skin", type="Class")] 
	/**
	 * The cell renderer to be used to render each item in the TileList component.     *     * @default fl.contols.listClasses.ImageCell     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="cellRenderer", type="Class")] 

	/**
	 * The TileList class provides a grid of rows and columns that is typically used	 * to format and display images in a "tiled" format. The default cell renderer for	 * this component is the ImageCell class. An ImageCell cell renderer displays a	 * thumbnail image and a single-line label. To render a list-based cell in a	 * TileList component, use the CellRenderer class.	 *	 * <p>To modify the padding that separates the cell border from the image, you	 * can globally set the <code>imagePadding</code> style, or set it on the ImageCell 	 * class. Like other cell styles, the <code>imagePadding</code> style cannot be	 * set on the TileList component instance.</p>     *     * @see fl.controls.listClasses.CellRenderer     * @see fl.controls.listClasses.ImageCell     *     * @includeExample examples/TileListExample.as	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class TileList extends SelectableList implements IFocusManagerComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _rowHeight : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _columnWidth : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _scrollDirection : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _scrollPolicy : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _cellRenderer : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var oldLength : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _labelField : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _labelFunction : Function;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _iconField : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _iconFunction : Function;
		/**
		 * @private (protected)
		 */
		protected var _sourceField : String;
		/**
		 * @private (protected)
		 */
		protected var _sourceFunction : Function;
		/**
		 * @private (protected)
		 */
		protected var __rowCount : uint;
		/**
		 * @private (protected)
		 */
		protected var __columnCount : uint;
		/**
		 * @private
		 */
		private var collectionItemImport : TileListCollectionItem;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 *  @private		 *  Method for creating the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * @copy fl.controls.SelectableList#dataProvider         *         * @includeExample examples/TileList.dataProvider.1.as -noswf         * @includeExample examples/TileList.dataProvider.2.as -noswf         * @includeExample examples/TileList.dataProvider.3.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get dataProvider () : DataProvider;
		/**
		 * @private (setter)
		 */
		public function set dataProvider (value:DataProvider) : void;
		/**
		 * Gets or sets a field in each item that contains a label for each tile.         *          * <p><strong>Note:</strong> The <code>labelField</code> is not used if 		 * the <code>labelFunction</code> property is set to a callback function.</p>		 *         * @default "label"         *         * @includeExample examples/TileList.labelField.1.as -noswf         *         * @see #labelFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelField () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelField (value:String) : void;
		/**
		 * Gets a function that indicates the fields of an item that provide the label text for a tile.         *         * <p><strong>Note:</strong> The <code>labelField</code> is not used if 		 * the <code>labelFunction</code> property is set to a callback function.</p>         *         * @default null         *         * @includeExample examples/TileList.labelFunction.1.as -noswf         *         * @see #labelField         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelFunction (value:Function) : void;
		/**
		 * Gets or sets the item field that provides the icon for the item.          *         * <p><strong>Note:</strong> The <code>iconField</code> is not used 		 * if the <code>iconFunction</code> property is set to a callback function.</p>         *         * <p>Icons can be classes or they can be symbols from the library that have a class name.</p>         *         * @default null         *         * @includeExample examples/TileList.iconField.1.as -noswf         *         * @see #iconFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get iconField () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set iconField (value:String) : void;
		/**
		 * Gets or sets the function to be used to obtain the icon for the item.          *         * <p><strong>Note:</strong> The <code>iconField</code> is not used if the 		 * <code>iconFunction</code> property is set to a callback function.</p>         *         * <p>Icons can be classes, or they can be library items that have class names.</p>         *         * @default null         *         * @includeExample examples/TileList.iconFunction.1.as -noswf         *         * @see #iconField         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get iconFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set iconFunction (value:Function) : void;
		/**
		 * Gets or sets the item field that provides the source path for a tile.         *         * <p><strong>Note:</strong> The <code>sourceField</code> is not used if the 		 * <code>sourceFunction</code> property is set to a callback function.</p>		 *         * @default "source"         *         * @includeExample examples/TileList.sourceField.1.as -noswf         *         * @see #sourceFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 *
		 */
		public function get sourceField () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set sourceField (value:String) : void;
		/**
		 * Gets or sets the function to be used to obtain the source path for a tile.         *         * <p><strong>Note:</strong> The <code>sourceField</code> is not used if the 		 * <code>sourceFunction</code> property is set to a callback function.</p>         *         * @default null         *         * @includeExample examples/TileList.sourceFunction.1.as -noswf         *         * @see #sourceField         *         * @internal [peter] Check with Metaliq that this is still accurate.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get sourceFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set sourceFunction (value:Function) : void;
		/**
		 * Gets or sets the number of rows that are at least partially visible 		 * in the list.		 *         * <p>Setting the <code>rowCount</code> property changes the height of the          * list, but the TileList component does not maintain this value. It          * is important to set the <code>rowCount</code> value <em>after</em> setting the           * <code>dataProvider</code> and <code>rowHeight</code> values. The only          * exception is if the <code>rowCount</code> is set with the Property          * inspector; in this case, the property is maintained until the component		 * is first drawn.</p>         *		 * @default 0         *         * @includeExample examples/TileList.rowCount.1.as -noswf         *         * @see #columnCount         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowCount () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowCount (value:uint) : void;
		/**
		 * Gets or sets the height that is applied to each row in the list, in pixels.         *		 * @default 50         *         * @includeExample examples/TileList.rowHeight.1.as -noswf         *          * @see #columnWidth         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowHeight () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowHeight (value:Number) : void;
		/**
		 * Gets or sets the number of columns that are at least partially visible in the 		 * list. Setting the <code>columnCount</code> property changes the width of the list,		 * but the TileList component does not maintain this value. It is important to set the 		 * <code>columnCount</code> value <em>after</em> setting the <code>dataProvider</code>		 * and <code>rowHeight</code> values. The only exception is if the <code>rowCount</code>		 * is set with the Property inspector; in this case, the property is maintained until the 		 * component is first drawn.         *		 * @default 0         *         * @includeExample examples/TileList.columnCount.1.as -noswf         *         * @see #rowCount         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get columnCount () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set columnCount (value:uint) : void;
		/**
		 * Gets or sets the width that is applied to a column in the list, in pixels.         *		 * @default 50         *         * @includeExample examples/TileList.columnWidth.1.as -noswf         *         * @see #rowHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get columnWidth () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set columnWidth (value:Number) : void;
		/**
		 * Gets the width of the content area, in pixels. This value is the component width         * minus the combined width of the <code>contentPadding</code> value and vertical scroll bar, 		 * if the vertical scroll bar is visible.         *         * @includeExample examples/TileList.innerWidth.1.as -noswf         *         * @see #innerHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get innerWidth () : Number;
		/**
		 * Gets the height of the content area, in pixels. This value is the component height		 * minus the combined height of the <code>contentPadding</code> value and horizontal		 * scroll bar height, if the horizontal scroll bar is visible.          *         * @see #innerWidth         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get innerHeight () : Number;
		/**
		 * Gets or sets a value that indicates whether the TileList component scrolls 		 * horizontally or vertically. A value of <code>ScrollBarDirection.HORIZONTAL</code>		 * indicates that the TileList component scrolls horizontally; a value of 		 * <code>ScrollBarDirection.VERTICAL</code> indicates that the TileList component scrolls vertically.         *		 * @default ScrollBarDirection.VERTICAL         *         * @includeExample examples/TileList.direction.1.as -noswf		 *         * @see ScrollBarDirection         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get direction () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set direction (value:String) : void;
		/**
		 * Gets or sets the scroll policy for the TileList component. This		 * value is used to specify the scroll policy for the scroll bar that		 * is set by the <code>direction</code> property.         *		 * <p><strong>Note:</strong> The TileList component supports scrolling only in 		 * one direction. Tiles are adjusted to fit into the viewable area of		 * the component, so that tiles are hidden in only one direction.</p>         *         * <p>The TileList component resizes to fit tiles only when the user 		 * manually sets the size or when the user sets the <code>rowCount</code> 		 * or <code>columnCount</code> properties.</p>		 *         * <p>When this value is set to <code>ScrollPolicy.AUTO</code>, the 		 * scroll bar is visible only when the TileList component must scroll 		 * to show all the items.</p>         *		 * @default ScrollPolicy.AUTO         *         * @includeExample examples/TileList.scrollPolicy.1.as -noswf         *          * @see #columnCount         * @see #rowCount         * @see ScrollPolicy         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scrollPolicy () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scrollPolicy (value:String) : void;
		/**
		 * @private (hidden)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get verticalScrollPolicy () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set verticalScrollPolicy (value:String) : void;
		/**
		 * @private (hidden)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalScrollPolicy () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalScrollPolicy (value:String) : void;
		/**
		 * Gets the maximum horizontal scroll position for the current content, in pixels.         *         * @see fl.containers.BaseScrollPane#horizontalScrollPosition         * @see fl.containers.BaseScrollPane#maxVerticalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxHorizontalScrollPosition () : Number;
		/**
		 * @private (setter)		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maxHorizontalScrollPosition (value:Number) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new List component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function TileList ();
		/**
		 * @copy fl.controls.SelectableList#scrollToIndex()         *         * @includeExample examples/TileList.scrollToIndex.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function scrollToIndex (newCaretIndex:int) : void;
		/**
		 * Retrieves the string that the renderer displays for a given data object         * based on the <code>labelField</code> and <code>labelFunction</code> properties.		 *		 * @param item The Object to be rendered.		 *         * @return The string to be displayed based on the data.         *         * @internal <code>var label:String = myTileList.itemToLabel(data);</code>         *         * @see #labelField         * @see #labelFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToLabel (item:Object) : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
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
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateAvailableHeight () : Number;
		/**
		 * @private (protected)		 * Moves the selection in a vertical direction in response		 * to the user selecting items using the up-arrow or down-arrow		 * Keys and modifiers such as the Shift and Ctrl keys.		 *		 * @param code The key that was pressed (e.g. Keyboard.DOWN)         *		 * @param shiftKey <code>true</code> if the shift key was held down when		 *        the keyboard key was pressed.         *		 * @param ctrlKey <code>true</code> if the ctrl key was held down when         *        the keyboard key was pressed         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionVertically (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)		 * Moves the selection in a horizontal direction in response		 * to the user selecting items using the left-arrow or right-arrow		 * keys and modifiers such as  the Shift and Ctrl keys.		 *		 * <p>Not implemented in List because the default list		 * is single column and does not scroll horizontally.</p>		 *		 * @param code The key that was pressed (e.g. Keyboard.LEFT)         *		 * @param shiftKey <code>true</code> if the shift key was held down when		 *        the keyboard key was pressed.         *		 * @param ctrlKey <code>true</code> if the ctrl key was held down when         *        the keyboard key was pressed         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function moveSelectionHorizontally (code:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)		 * Changes the selected index, or adds or subtracts the index and		 * all indices between when the shift key is used.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function doKeySelection (newCaretIndex:uint, shiftKey:Boolean, ctrlKey:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
	}
}
