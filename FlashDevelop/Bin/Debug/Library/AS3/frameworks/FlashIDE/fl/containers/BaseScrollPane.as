package fl.containers
{
	import fl.core.UIComponent;
	import fl.controls.BaseButton;
	import fl.controls.ScrollBar;
	import fl.events.ScrollEvent;
	import fl.controls.ScrollPolicy;
	import fl.controls.ScrollBarDirection;
	import fl.core.InvalidationType;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.display.DisplayObject;
	import flash.display.Shape;
	import flash.display.Graphics;
	import flash.geom.Rectangle;

	/**
	 * Dispatched when the user scrolls content by using the scroll bars on the	 * component or the wheel on a mouse device.     *     * @eventType fl.events.ScrollEvent.SCROLL     *     * @includeExample examples/ScrollPane.scroll.1.as -noswf     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="scroll", type="fl.events.ScrollEvent")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowDisabledSkin     *     * @default ScrollArrowDown_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowDownSkin     *     * @default ScrollArrowDown_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowOverSkin     *     * @default ScrollArrowDown_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowUpSkin     *     * @default ScrollArrowDown_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbDisabledSkin     *     * @default ScrollThumb_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbDownSkin     *     * @default ScrollThumb_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbOverSkin     *     * @default ScrollThumb_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbUpSkin	 *     * @default ScrollThumb_upSkin     *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackDisabledSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackDownSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackOverSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackUpSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowDisabledSkin     *     * @default ScrollArrowUp_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowDownSkin     *     * @default ScrollArrowUp_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowOverSkin     *     * @default ScrollArrowUp_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowUpSkin     *     * @default ScrollArrowUp_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbIcon     *     * @default ScrollBar_thumbIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbIcon", type="Class")] 
	/**
	 * @copy fl.controls.BaseButton#style:repeatDelay	 *      * @default 500     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="repeatDelay", type="Number", format="Time")] 
	/**
	 * @copy fl.controls.BaseButton#style:repeatInterval	 *      * @default 35     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="repeatInterval", type="Number", format="Time")] 
	/**
	 * The skin to be used as the background of the scroll pane.     *     * @default ScrollPane_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="skin", type="Class")] 
	/**
	 * Padding between the content (the component and scroll bar), and the outside edge of the background, in pixels.     *     * @default 0     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0	 * @internal [kenos] What does "control and scrollbar" mean here -- what is the relationship between these	 *                   elements and the content? I'd like to make this more understandable	 *                   but don't know how it should be written based on the current description.
	 */
	[Style(name="contentPadding", type="Number", format="Length")] 
	/**
	 * When the <code>enabled</code> property is set to <code>false</code>,      * interaction with the component is prevented and a white overlay is      * displayed over the component, dimming the component contents.  The      * <code>disabledAlpha</code> style specifies the level of transparency	 * that is applied to this overlay. Valid values range from 0, for an      * overlay that is completely transparent, to 1 for an overlay that is opaque.      *     * @default 0.5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledAlpha", type="Number", format="Length")] 

	/**
	 * The BaseScrollPane class handles basic scroll pane functionality including events, styling, 	 * drawing the mask and background, the layout of scroll bars, and the handling of scroll positions.	 * 	 * <p>By default, the BaseScrollPane class is extended by the ScrollPane and SelectableList classes,	 * for all list-based components.  This means that any component that uses horizontal or vertical 	 * scrolling does not need to implement any scrolling, masking or layout logic, except for behavior	 * that is specific to the component.</p>     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class BaseScrollPane extends UIComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _verticalScrollBar : ScrollBar;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _horizontalScrollBar : ScrollBar;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentScrollRect : Rectangle;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var disabledOverlay : Shape;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var background : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentWidth : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentHeight : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _horizontalScrollPolicy : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _verticalScrollPolicy : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentPadding : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var availableWidth : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var availableHeight : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var vOffset : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var vScrollBar : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var hScrollBar : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _maxHorizontalScrollPosition : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _horizontalPageScrollSize : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _verticalPageScrollSize : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var defaultLineScrollSize : Number;
		/**
		 * @private (protected)         *         * If <code>false</code>, uses <code>contentWidth</code> to determine hscroll, otherwise uses fixed <code>_maxHorizontalScroll</code> value.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var useFixedHorizontalScrolling : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _useBitmpScrolling : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected static const SCROLL_BAR_STYLES : Object;

		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets a value that indicates the state of the horizontal scroll		 * bar. A value of <code>ScrollPolicy.ON</code> indicates that the horizontal 		 * scroll bar is always on; a value of <code>ScrollPolicy.OFF</code> indicates		 * that the horizontal scroll bar is always off; and a value of <code>ScrollPolicy.AUTO</code>		 * indicates that its state automatically changes. This property is used with          * other scrolling properties to set the <code>setScrollProperties()</code> method		 * of the scroll bar.		 *		 * @default ScrollPolicy.AUTO         *         * @see #verticalScrollPolicy         * @see fl.controls.ScrollPolicy ScrollPolicy         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalScrollPolicy () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalScrollPolicy (value:String) : void;
		/**
		 * Gets or sets a value that indicates the state of the vertical scroll		 * bar. A value of <code>ScrollPolicy.ON</code> indicates that the vertical		 * scroll bar is always on; a value of <code>ScrollPolicy.OFF</code> indicates		 * that the vertical scroll bar is always off; and a value of <code>ScrollPolicy.AUTO</code>		 * indicates that its state automatically changes. This property is used with          * other scrolling properties to set the <code>setScrollProperties()</code> method		 * of the scroll bar.		 *		 * @default ScrollPolicy.AUTO         *         * @see #horizontalScrollPolicy         * @see fl.controls.ScrollPolicy ScrollPolicy         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get verticalScrollPolicy () : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set verticalScrollPolicy (value:String) : void;
		/**
		 * Gets or sets a value that describes the amount of content to be scrolled,		 * horizontally, when a scroll arrow is clicked. This value is measured in pixels.         *         * @default 4         *         * @see #horizontalPageScrollSize         * @see #verticalLineScrollSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalLineScrollSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalLineScrollSize (value:Number) : void;
		/**
		 * Gets or sets a value that describes how many pixels to scroll vertically when a scroll arrow is clicked.          *         * @default 4         *         * @see #horizontalLineScrollSize         * @see #verticalPageScrollSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get verticalLineScrollSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set verticalLineScrollSize (value:Number) : void;
		/**
		 * Gets or sets a value that describes the horizontal position of the 		 * horizontal scroll bar in the scroll pane, in pixels.         *         * @default 0         *         * @includeExample examples/BaseScrollPane.horizontalScrollPosition.1.as -noswf         *         * @see #maxHorizontalScrollPosition         * @see #verticalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalScrollPosition () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalScrollPosition (value:Number) : void;
		/**
		 * Gets or sets a value that describes the vertical position of the 		 * vertical scroll bar in the scroll pane, in pixels.         *         * @default 0         *         * @includeExample examples/BaseScrollPane.horizontalScrollPosition.1.as -noswf         *         * @see #horizontalScrollPosition         * @see #maxVerticalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get verticalScrollPosition () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set verticalScrollPosition (value:Number) : void;
		/**
		 * Gets the maximum horizontal scroll position for the current content, in pixels.         *         * @see #horizontalScrollPosition         * @see #maxVerticalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxHorizontalScrollPosition () : Number;
		/**
		 * Gets the maximum vertical scroll position for the current content, in pixels.         *         * @see #maxHorizontalScrollPosition         * @see #verticalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxVerticalScrollPosition () : Number;
		/**
		 * When set to <code>true</code>, the <code>cacheAsBitmap</code> property for the scrolling content is set 		 * to <code>true</code>; when set to <code>false</code> this value is turned off.         *		 * <p><strong>Note:</strong> Setting this property to <code>true</code> increases scrolling performance.</p>         *         * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get useBitmapScrolling () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set useBitmapScrolling (value:Boolean) : void;
		/**
		 * Gets or sets the count of pixels by which to move the scroll thumb 		 * on the horizontal scroll bar when the scroll bar track is pressed. When 		 * this value is 0, this property retrieves the available width of the component.         *         * @default 0         *         * @see #horizontalLineScrollSize         * @see #verticalPageScrollSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalPageScrollSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalPageScrollSize (value:Number) : void;
		/**
		 * Gets or sets the count of pixels by which to move the scroll thumb 		 * on the vertical scroll bar when the scroll bar track is pressed. When 		 * this value is 0, this property retrieves the available height of the component.         *         * @default 0         *         * @see #horizontalPageScrollSize         * @see #verticalLineScrollSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] Is available height specified in pixels?
		 */
		public function get verticalPageScrollSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set verticalPageScrollSize (value:Number) : void;
		/**
		 * Gets a reference to the horizontal scroll bar.         *         * @see #verticalScrollBar         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalScrollBar () : ScrollBar;
		/**
		 * Gets a reference to the vertical scroll bar.         *         * @see #horizontalScrollBar         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get verticalScrollBar () : ScrollBar;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setContentSize (width:Number, height:Number) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleScroll (event:ScrollEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleWheel (event:MouseEvent) : void;
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
		protected function setStyles () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBackground () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawDisabledOverlay () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateAvailableSize () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateContentWidth () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateChildren () : void;
	}
}
