package fl.events
{
	import flash.events.Event;

	/**
	 * The SliderEvent class defines events that are associated with the Slider component.     * These include the following:	 * <ul>	 * <li><code>SliderEvent.CHANGE</code>: dispatched after there is a change in the position of the slider.</li>	 * <li><code>SliderEvent.THUMB_DRAG</code>: dispatched when a user drags the thumb of the slider.</li>	 * <li><code>SliderEvent.THUMB_PRESS</code>: dispatched when a user presses the thumb of the slider.</li>	 * <li><code>SliderEvent.THUMB_RELEASE</code>: dispatched when the user releases the thumb of the slider.</li>	 * </ul>        * @see SliderEventClickTarget     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SliderEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code> property of a <code>change</code> event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *    <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default		 * 			behavior to cancel.</td></tr>		 * 	  <tr><td><code>clickTarget</code></td><td>Identifies whether the slider track		 * 			or a slider thumb was pressed.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener. </td></tr>		 *	  <tr><td><code>keyCode</code></td><td>If the event was triggered by a key press,		 *           the code for the key that was pressed.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 * 	  <tr><td><code>triggerEvent</code></td><td>The type of device that triggered the event. A value 		 *          of <code>InteractionInputType.MOUSE</code> indicates that a mouse was the source of input;  		 * 			a value of <code>InteractionInputType.KEYBOARD</code> indicates that a keyboard was		 *          the source of input.</td></tr>		 * 	  <tr><td><code>value</code></td><td>The value of the slider after the event.</td></tr>         *  </table>         *         * @eventType change         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const CHANGE : String = "change";
		/**
		 * Defines the value of the <code>type</code> property of a <code>thumbDrag</code> event		 * object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default		 * 			behavior to cancel.</td></tr>		 * 	  <tr><td><code>clickTarget</code></td><td>Identifies whether the slider track		 * 			or a slider thumb was pressed.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener. </td></tr>		 *	  <tr><td><code>keyCode</code></td><td>If the event was triggered by a key press,		 *           the code for the key that was pressed.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 * 	  <tr><td><code>triggerEvent</code></td><td>The type of device that triggered the event. A value 		 *          of <code>InteractionInputType.MOUSE</code> indicates that a mouse was the source of the		 *          input; a value of <code>InteractionInputType.KEYBOARD</code> indicates that a keyboard		 *          was the source of the input.</td></tr>		 * 	  <tr><td><code>value</code></td><td>The value of the slider after the event.</td></tr>		 *  </table>         *         * @eventType thumbDrag		 *          * @see #THUMB_PRESS         * @see #THUMB_RELEASE         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const THUMB_DRAG : String = "thumbDrag";
		/**
		 * Defines the value of the <code>type</code> property of a <code>thumbPress</code> 		 * event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default		 * 			behavior to cancel.</td></tr>		 * 	  <tr><td><code>clickTarget</code></td><td>Identifies whether the slider track		 * 			or a slider thumb was pressed.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener. </td></tr>		 *	  <tr><td><code>keyCode</code></td><td>If the event was triggered by a key press,		 *           the code for the key that was pressed.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 * 	  <tr><td><code>triggerEvent</code></td><td>The type of device that triggered the event. A value 		 *          of <code>InteractionInputType.MOUSE</code> indicates that a mouse was the source of the		 *          input; a value of <code>InteractionInputType.KEYBOARD</code> indicates that a keyboard		 *          was the source of the input.</td></tr>		 * 	  <tr><td><code>value</code></td><td>The value of the slider after the event.</td></tr>		 *  </table>         *         * @eventType thumbPress		 *          * @see #THUMB_DRAG         * @see #THUMB_RELEASE         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const THUMB_PRESS : String = "thumbPress";
		/**
		 * Defines the value of the <code>type</code> property of a <code>thumbRelease</code>		 * event object. 		 * 		 * <p>This event has the following properties:</p>	  	 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default		 * 			behavior to cancel.</td></tr>		 * 	  <tr><td><code>clickTarget</code></td><td>Identifies whether the slider track		 * 			or a slider thumb was pressed.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener.</td></tr>		 *	  <tr><td><code>keyCode</code></td><td>If the event was triggered by a key press,		 *           the code for the key that was pressed.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 * 	  <tr><td><code>triggerEvent</code></td><td>The type of device that triggered the event. A value 		 *          of <code>InteractionInputType.MOUSE</code> indicates that a mouse was the source of the		 *          input; a value of <code>InteractionInputType.KEYBOARD</code> indicates that a keyboard		 *          was the source of the input.</td></tr>		 * 	  <tr><td><code>value</code></td><td>The value of the slider after the event.</td></tr>		 *  </table>         *         * @eventType thumbRelease		 *          * @see #THUMB_DRAG         * @see #THUMB_PRESS         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const THUMB_RELEASE : String = "thumbRelease";
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _triggerEvent : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _value : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _keyCode : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _clickTarget : String;

		/**
		 * Gets the new value of the slider, based on its position.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get value () : Number;
		/**
		 * Gets the key code for the key that was pressed to trigger the event.		 * A key code is a numeric value that identifies the key that was pressed.		 *         * @see flash.events.KeyboardEvent#keyCode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get keyCode () : Number;
		/**
		 * Gets the type of device that was used to send the input. A value of <code>InteractionInputType.MOUSE</code> 		 * indicates that a mouse was the source of the input; a value of <code>InteractionInputType.KEYBOARD</code>		 * indicates that a keyboard was the source of the input.		 *         * @see SliderEventClickTarget SliderEventClickTarget         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get triggerEvent () : String;
		/**
		 * Gets a string that indicates whether the slider thumb or a slider track was pressed. 		 * A value of <code>SliderEventClickTarget.THUMB</code> indicates that the slider thumb was		 * pressed; a value of <code>SliderEventClickTarget.TRACK</code> indicates that the slider		 * track was pressed.		 *         * @see SliderEventClickTarget SliderEventClickTarget         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get clickTarget () : String;

		/**
		 * Creates a new SliderEvent object with the specified parameters.		 *		 * @param type The event type; this value identifies the action that triggered the event.         *		 * @param value The new value of the slider.         *		 * @param clickTarget Indicates whether a slider thumb or the slider track was         *        pressed. A value of <code>SliderEventClickTarget.THUMB</code> indicates that 		 *        the slider thumb was pressed; a value of <code>SliderEventClickTarget.TRACK</code>		 *        indicates that the slider track was pressed.		 * 		 * @param triggerEvent A String that indicates the source of the input. A value of		 *        <code>InteractionInputType.MOUSE</code> indicates that the mouse was the source of input;		 *        a value of <code>InteractionInputType.KEYBOARD</code> indicates that the keyboard was		 *        the source of input.          *         * @param keyCode If the event was triggered by a key press, this value is the key code 		 *        that identifies that key.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SliderEvent (type:String, value:Number, clickTarget:String, triggerEvent:String, keyCode:int = 0);
		/**
		 * Returns a string that contains all the properties of the SliderEvent object. The 		 * string is in the following format:		 * 		 * <p>[<code>SliderEvent type=<em>value</em> value=<em>value</em>		 * bubbles=<em>value</em> cancelable=<em>value</em> keycode=<em>value</em>         * triggerEvent=<em>value</em> clickTarget=<em>value</em></code>]</p>		 *         * @return A string representation of the SliderEvent object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
		/**
		 * Creates a copy of the SliderEvent object and sets the value of each parameter to match		 * the original.		 *         * @return A copy of the current SliderEvent instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
