package fl.controls
{
	import fl.controls.BaseButton;
	import fl.controls.LabelButton;
	import fl.controls.ScrollBarDirection;
	import fl.core.UIComponent;
	import fl.core.InvalidationType;
	import fl.events.ComponentEvent;
	import fl.events.ScrollEvent;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.TimerEvent;
	import flash.text.TextField;
	import flash.text.TextFormat;
	import flash.utils.Timer;
	import fl.controls.TextInput;

	/**
	 * Dispatched when the ScrollBar instance's <code>scrollPosition</code> property changes. 	 *     * @eventType fl.events.ScrollEvent.SCROLL     *     * @see #scrollPosition     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="scroll", type="fl.events.ScrollEvent"))] 
	/**
	 * Name of the class to use as the skin for the down arrow button of the scroll bar      * when it is disabled. If you change the skin, either graphically or programmatically,      * you should ensure that the new skin is the same height (for horizontal scroll bars)     * or width (for vertical scroll bars) as the track.     *     * @default ScrollArrowDown_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDisabledSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the down arrow button of the scroll bar      * when you click the arrow button. If you change the skin, either graphically or      * programmatically, you should ensure that the new skin is the same height (for      * horizontal scroll bars) or width (for vertical scroll bars) as the track.     *     * @default ScrollArrowDown_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDownSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the down arrow button of the scroll bar      * when the mouse pointer is over the arrow button. If you change the skin, either      * graphically or programmatically, you should ensure that the new skin is the same      * height (for horizontal scroll bars) or width (for vertical scroll bars) as the track.     *     * @default ScrollArrowDown_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowOverSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the down arrow button of the scroll bar.      * If you change the skin, either graphically or programmatically, you should ensure      * that the new skin is the same height (for horizontal scroll bars) or width (for      * vertical scroll bars) as the track.     *     * @default ScrollArrowDown_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowUpSkin", type="Class")] 
	/**
	 * The skin that is used to indicate the disabled state of the thumb.     *     * @default ScrollThumb_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbDisabledSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the thumb of the scroll bar when you      * click the thumb.     *     * @default ScrollThumb_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbDownSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the thumb of the scroll bar when the      * mouse pointer is over the thumb.     *     * @default ScrollThumb_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbOverSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin used for the thumb of the scroll	 * bar.     *     * @default ScrollThumb_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbUpSkin", type="Class")] 
	/**
	 * The skin that is used to indicate a disabled track.     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackDisabledSkin", type="Class")] 
	/**
	 * The skin that is used to indicate the down state of a disabled skin.     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackDownSkin", type="Class")] 
	/**
	 * The skin that is used to indicate the mouseover state for the scroll track.     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackOverSkin", type="Class")] 
	/**
	 * The skin used to indicate the mouse up state for the scroll track.     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackUpSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the up arrow button of the scroll bar      * when it is disabled. If you change the skin, either graphically or programmatically,      * you should ensure that the new skin is the same height (for horizontal scroll bars)      * or width (for vertical scroll bars) as the track.     *     * @default ScrollArrowUp_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDisabledSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the up arrow button of the scroll bar when      * you click the arrow button. If you change the skin, either graphically or programmatically,      * you should ensure that the new skin is the same height (for horizontal scroll bars) or width      * (for vertical scroll bars) as the track.     *     * @default ScrollArrowUp_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDownSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the up arrow button of the scroll bar when the      * mouse pointer is over the arrow button. If you change the skin, either graphically or      * programmatically, you should ensure that the new skin is the same height (for horizontal      * scroll bars) or width (for vertical scroll bars) as the track.     *     * @default ScrollArrowUp_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowOverSkin", type="Class")] 
	/**
	 * Name of the class to use as the skin for the up arrow button of the scroll bar. If you      * change the skin, either graphically or programmatically, you should ensure that the new      * skin is the same height (for horizontal scroll bars) or width (for vertical scroll bars)      * as the track.     *     * @default ScrollArrowUp_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowUpSkin", type="Class")] 
	/**
	 * Name of the class to use as the icon for the thumb of the scroll bar.     *     * @default ScrollBar_thumbIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
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
	 * The ScrollBar component provides the end user with a way to control the 	 * portion of data that is displayed when there is too much data to 	 * fit in the display area. The scroll bar consists of four parts: 	 * two arrow buttons, a track, and a thumb. The position of the 	 * thumb and display of the buttons depends on the current state of 	 * the scroll bar. The scroll bar uses four parameters to calculate 	 * its display state: a minimum range value; a maximum range value; 	 * a current position that must be within the range values; and a 	 * viewport size that must be equal to or less than the range and 	 * represents the number of items in the range that can be      * displayed at the same time.     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ScrollBar extends UIComponent
	{
		/**
		 * @private (internal)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const WIDTH : Number = 15;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _pageSize : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _pageScrollSize : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _lineScrollSize : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _minScrollPosition : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _maxScrollPosition : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _scrollPosition : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _direction : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var thumbScrollOffset : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var inDrag : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var upArrow : BaseButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var downArrow : BaseButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var thumb : LabelButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var track : BaseButton;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected static const DOWN_ARROW_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const THUMB_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const TRACK_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const UP_ARROW_STYLES : Object;

		/**
		 * @copy fl.core.UIComponent#width         *         * @see #height         * @see #setSize()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get width () : Number;
		/**
		 * @copy fl.core.UIComponent#height         *         * @see #setSize()         * @see #width         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get height () : Number;
		/**
		 * Gets or sets a Boolean value that indicates whether the scroll bar is enabled.		 * A value of <code>true</code> indicates that the scroll bar is enabled; a value of		 * <code>false</code> indicates that it is not.          *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get enabled () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets the current scroll position and updates the position          * of the thumb. The <code>scrollPosition</code> value represents a relative position between		 * the <code>minScrollPosition</code> and <code>maxScrollPosition</code> values.         *         * @default 0         *         * @see #setScrollProperties()         * @see #minScrollPosition		 * @see #maxScrollPosition		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scrollPosition () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scrollPosition (newScrollPosition:Number) : void;
		/**
		 * Gets or sets a number that represents the minimum scroll position.  The 		 * <code>scrollPosition</code> value represents a relative position between the		 * <code>minScrollPosition</code> and the <code>maxScrollPosition</code> values.		 * This property is set by the component that contains the scroll bar,		 * and is usually zero.         *         * @default 0         *         * @see #setScrollProperties()		 * @see #maxScrollPosition		 * @see #scrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get minScrollPosition () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set minScrollPosition (value:Number) : void;
		/**
		 * Gets or sets a number that represents the maximum scroll position. The		 * <code>scrollPosition</code> value represents a relative position between the		 * <code>minScrollPosition</code> and the <code>maxScrollPosition</code> values.		 * This property is set by the component that contains the scroll bar,		 * and is the maximum value. Usually this property describes the number		 * of pixels between the bottom of the component and the bottom of		 * the content, but this property is often set to a different value to change the		 * behavior of the scrolling.  For example, the TextArea component sets this		 * property to the <code>maxScrollH</code> value of the text field, so that the 		 * scroll bar scrolls appropriately by line of text.         *         * @default 0         *         * @see #setScrollProperties()		 * @see #minScrollPosition		 * @see #scrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxScrollPosition () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maxScrollPosition (value:Number) : void;
		/**
		 * Gets or sets the number of lines that a page contains. The <code>lineScrollSize</code>		 * is measured in increments between the <code>minScrollPosition</code> and 		 * the <code>maxScrollPosition</code>. If this property is 0, the scroll bar 		 * will not scroll.         *         * @default 10         *		 * @see #maxScrollPosition		 * @see #minScrollPosition         * @see #setScrollProperties()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get pageSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set pageSize (value:Number) : void;
		/**
		 * Gets or sets a value that represents the increment by which the page is scrolled		 * when the scroll bar track is pressed. The <code>pageScrollSize</code> value is 		 * measured in increments between the <code>minScrollPosition</code> and the 		 * <code>maxScrollPosition</code> values. If this value is set to 0, the value of the		 * <code>pageSize</code> property is used.         *         * @default 0		 *		 * @see #maxScrollPosition		 * @see #minScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get pageScrollSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set pageScrollSize (value:Number) : void;
		/**
		 * Gets or sets a value that represents the increment by which to scroll the page		 * when the scroll bar track is pressed. The <code>pageScrollSize</code> is measured 		 * in increments between the <code>minScrollPosition</code> and the <code>maxScrollPosition</code>          * values. If this value is set to 0, the value of the <code>pageSize</code> property is used.         *         * @default 0		 *		 * @see #maxScrollPosition		 * @see #minScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get lineScrollSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set lineScrollSize (value:Number) : void;
		/**
		 * Gets or sets a value that indicates whether the scroll bar scrolls horizontally or vertically.         * Valid values are <code>ScrollBarDirection.HORIZONTAL</code> and          * <code>ScrollBarDirection.VERTICAL</code>.         *         * @default ScrollBarDirection.VERTICAL         *         * @see fl.controls.ScrollBarDirection ScrollBarDirection         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get direction () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set direction (value:String) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * @copy fl.core.UIComponent#setSize()         *         * @see #height         * @see #width         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSize (width:Number, height:Number) : void;
		/**
		 * Sets the range and viewport size of the ScrollBar component. The ScrollBar          * component updates the state of the arrow buttons and size of the scroll          * thumb accordingly. All of the scroll properties are relative to the		 * scale of the <code>minScrollPosition</code> and the <code>maxScrollPosition</code>.		 * Each number between the maximum and minumum values represents one scroll position.		 *		 * @param pageSize Size of one page. Determines the size of the thumb, and the increment by which the scroll bar moves when the arrows are clicked. 		 * @param minScrollPosition Bottom of the scrolling range.		 * @param maxScrollPosition Top of the scrolling range.		 * @param pageScrollSize Increment to move when a track is pressed, in pixels.		 *         * @see #maxScrollPosition         * @see #minScrollPosition         * @see #pageScrollSize         * @see #pageSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setScrollProperties (pageSize:Number, minScrollPosition:Number, maxScrollPosition:Number, pageScrollSize:Number = 0) : void;
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
		protected function scrollPressHandler (event:ComponentEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function thumbPressHandler (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleThumbDrag (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function thumbReleaseHandler (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setScrollPosition (newScrollPosition:Number, fireEvent:Boolean = true) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setStyles () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateThumb () : void;
	}
}
