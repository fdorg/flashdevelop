package fl.events
{
	import flash.events.Event;

	/**
	 * The ColorPickerEvent class defines events that are associated with the ColorPicker component.	 * These include the following events:	 * <ul>	 *     <li><code>ColorPickerEvent.CHANGE</code>: dispatched when a user clicks a different color in the ColorPicker component.</li>	 *     <li><code>ColorPickerEvent.ENTER</code>: dispatched when a user presses the Enter key after entering a value in the text field of the ColorPicker component.</li>	 *     <li><code>ColorPickerEvent.ITEM_ROLL_OUT</code>: dispatched when the device pointer moves out of a color cell in the ColorPicker component.</li>	 *     <li><code>ColorPickerEvent.ITEM_ROLL_OVER</code>: dispatched when the device pointer moves over a color cell in the ColorPicker component.</li>	 * </ul>     *     * @see fl.controls.ColorPicker ColorPicker     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ColorPickerEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code> property for an <code>itemRollOut</code> 		 * event object.		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 * 	   <tr><td><code>bubbles</code></td><td><code>true</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>true</code></td></tr>			 *	   <tr><td><code>color</code></td><td>The current color value of the ColorPicker component.</td></tr>		 *	   <tr><td><code>currentTarget</code></td><td>The object that is actively processing the event object with an event listener.</td></tr>	     *     <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>         *         * @eventType itemRollOut         *         * @see #ITEM_ROLL_OVER         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ITEM_ROLL_OUT : String = "itemRollOut";
		/**
		 * Defines the value of the <code>type</code> property for an <code>itemRollOver</code>		 * event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 * 	   <tr><td><code>bubbles</code></td><td><code>true</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>true</code></td></tr>			 *	   <tr><td><code>color</code></td><td>The current color value of the ColorPicker component.</td></tr>		 *	   <tr><td><code>currentTarget</code></td><td>The object that is actively processing the event object with an event listener.</td></tr>		 *     <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>         *         * @eventType itemRollOver         *         * @see #ITEM_ROLL_OUT         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ITEM_ROLL_OVER : String = "itemRollOver";
		/**
		 * Defines the value of the <code>type</code> property of an <code>enter</code>		 * event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 * 	   <tr><td><code>bubbles</code></td><td><code>true</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>true</code></td></tr>			 *	   <tr><td><code>color</code></td><td>The current color value of the ColorPicker component.</td></tr>		 *	   <tr><td><code>currentTarget</code></td><td>The object that is actively processing the event object with an event listener.</td></tr>		 *     <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>         *         * @eventType enter         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ENTER : String = "enter";
		/**
		 * Defines the value of the <code>type</code> property of the <code>change</code>		 * event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 * 	   <tr><td><code>bubbles</code></td><td><code>true</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>true</code></td></tr>			 *	   <tr><td><code>color</code></td><td>The current color value of the ColorPicker component.</td></tr>		 *	   <tr><td><code>currentTarget</code></td><td>The object that is actively processing the event object with an event listener.</td></tr>		 *     <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>         *  </table>         *         * @includeExample ../controls/examples/ColorPicker.hexValue.1.as -noswf         *         * @eventType change         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const CHANGE : String = "change";
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _color : uint;

		/**
		 * Gets the color value that is associated with the event.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get color () : uint;

		/**
		 * Creates a new ColorPickerEvent object.		 * 		 * @param type Indicates the current event type.		 *          * @param color Indicates the color that is associated with the current event.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ColorPickerEvent (type:String, color:uint);
		/**
		 * Returns a string that contains all the properties of the ColorPickerEvent object.		 *          * @return A string representation of the ColorPickerEvent object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
		/**
		 * Creates a copy of the ColorPickerEvent object and sets the value of each parameter to match		 * the original.		 *         * @return A copy of the ColorPickerEvent instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
