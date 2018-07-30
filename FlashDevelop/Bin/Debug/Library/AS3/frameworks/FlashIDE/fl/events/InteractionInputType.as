package fl.events
{
	/**
	 * The InteractionInputType class defines constants for the values of the	 * <code>triggerEvent</code> property of the SliderEvent object. These constants define strings	 * to identify the sources of input that can trigger an event: the mouse and the keyboard.     *     * @see SliderEvent#triggerEvent     *     * @includeExample examples/InteractionInputTypeExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0	 * @internal Can a pointing device that is not a mouse cause these events to be dispatched?
	 */
	public class InteractionInputType
	{
		/**
		 * The <code>InteractionInputType.MOUSE</code> constant defines the value of          * the <code>type</code> property of a <code>mouse</code> event object.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * @internal Seems like another type of pointing device could also generate this event?
		 */
		public static const MOUSE : String = 'mouse';
		/**
		 * The <code>InteractionInputType.KEYBOARD</code> constant defines the value of          * the <code>type</code> property of a <code>keyboard</code> event object.           *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const KEYBOARD : String = 'keyboard';

	}
}
