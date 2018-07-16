package fl.core
{
	/**
	 * The InvalidationType class defines <code>InvalidationType</code> constants 	 * that are used by the <code>type</code> property of an event object that is 	 * dispatched after a component is invalidated. These constants are used 	 * by component developers to specify the portion of the component that is to be redrawn 	 * after the component is invalidated.     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class InvalidationType
	{
		/**
		 * The <code>InvalidationType.ALL</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the component should		 * redraw itself entirely.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ALL : String = "all";
		/**
		 * The <code>InvalidationType.SIZE</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the screen dimensions of		 * the component are invalid.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] This one implies that the dimensions of the component on the screen 		 *                   should be redrawn, but should say so if that is correct.
		 */
		public static const SIZE : String = "size";
		/**
		 * The <code>InvalidationType.STYLES</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the styles of the component 		 * are invalid.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] Would it be correct to say "that the component should redraw its styles?"
		 */
		public static const STYLES : String = "styles";
		/**
		 * The <code>InvalidationType.RENDERER_STYLES</code> constant defines the value of		 * the <code>type</code> property of the event object that is dispatched to indicate that		 * the renderer styles of the component are invalid.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] Should this one say, "that the component should redraw its renderer styles?"
		 */
		public static const RENDERER_STYLES : String = "rendererStyles";
		/**
		 * The <code>InvalidationType.STATE</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the state of the component		 * is invalid. For example, this constant is used when the <code>enabled</code> state of a component		 * is no longer valid.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] What would be redrawn for a component with an invalid state?
		 */
		public static const STATE : String = "state";
		/**
		 * The <code>InvalidationType.DATA</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the data that belongs to		 * a component is invalid.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] For this one, would the data require refreshing? Recreation? Replacement?
		 */
		public static const DATA : String = "data";
		/**
		 * The <code>InvalidationType.SCROLL</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the scroll position of the		 * component is invalid.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] When the scroll position should be redrawn?
		 */
		public static const SCROLL : String = "scroll";
		/**
		 * The <code>InvalidationType.SELECTED</code> constant defines the value of the <code>type</code>          * property of the event object that is dispatched to indicate that the <code>selected</code> 		 * property of the component is invalid.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal [kenos] For this one, does the component need to be redrawn to remove the focus indicator?
		 */
		public static const SELECTED : String = "selected";

	}
}
