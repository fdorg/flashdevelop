package fl.controls
{
	import Error;
	import fl.controls.ScrollBar;
	import fl.controls.ScrollBarDirection;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ScrollEvent;
	import flash.events.Event;
	import flash.events.TextEvent;
	import flash.text.TextField;

	/**
	 * The UIScrollBar class includes all of the scroll bar functionality, but      * adds a <code>scrollTarget()</code> method so it can be attached	 * to a TextField component instance.	 *	 * <p><strong>Note:</strong> When you use ActionScript to update properties of 	 * the TextField component that affect the text layout, you must call the 	 * <code>update()</code> method on the UIScrollBar component instance to refresh its scroll 	 * properties. Examples of text layout properties that belong to the TextField 	 * component include <code>width</code>, <code>height</code>, and <code>wordWrap</code>.</p>	 *     * @includeExample examples/UIScrollBarExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class UIScrollBar extends ScrollBar
	{
		/**
		 * @private (private)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _scrollTarget : TextField;
		/**
		 * @private (private)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var inEdit : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var inScroll : Boolean;
		/**
		 * @private
		 */
		private static var defaultStyles : Object;

		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set minScrollPosition (minScrollPosition:Number) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maxScrollPosition (maxScrollPosition:Number) : void;
		/**
		 * Registers a TextField component instance with the ScrollBar component instance.         *         * @includeExample examples/UIScrollBar.scrollTarget.1.as -noswf         *         * @see #update()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scrollTarget () : TextField;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scrollTarget (target:TextField) : void;
		/**
		 * @private (internal)         * @internal For specifying in inspectable, and setting dropTarget         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scrollTargetName () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scrollTargetName (target:String) : void;
		/**
		 * @copy fl.controls.ScrollBar#direction         *         * @default ScrollBarDirection.VERTICAL         *         * @includeExample examples/UIScrollBar.direction.1.as -noswf         * @includeExample examples/UIScrollBar.direction.2.as -noswf         *         * @see ScrollBarDirection         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get direction () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set direction (dir:String) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new UIScrollBar component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function UIScrollBar ();
		/**
		 * Forces the scroll bar to update its scroll properties immediately.           * This is necessary after text in the specified <code>scrollTarget</code> text field		 * is added using ActionScript, and the scroll bar needs to be refreshed.         *         * @see #scrollTarget         *         * @includeExample examples/UIScrollBar.update.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function update () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateScrollTargetProperties () : void;
		/**
		 * @copy fl.controls.ScrollBar#setScrollProperties()         *         * @see ScrollBar#pageSize ScrollBar.pageSize         * @see ScrollBar#minScrollPosition ScrollBar.minScrollPosition         * @see ScrollBar#maxScrollPosition ScrollBar.maxScrollPosition         * @see ScrollBar#pageScrollSize ScrollBar.pageScrollSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setScrollProperties (pageSize:Number, minScrollPosition:Number, maxScrollPosition:Number, pageScrollSize:Number = 0) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setScrollPosition (scrollPosition:Number, fireEvent:Boolean = true) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateTargetScroll (event:ScrollEvent = null) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleTargetChange (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleTargetScroll (event:Event) : void;
	}
}
