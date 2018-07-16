package fl.events
{
	import flash.events.Event;

	/**
	 * The ScrollEvent class defines the scroll event that is associated with the ScrollBar component.	 * 	 * @see fl.controls.ScrollBar ScrollBar	 * @see fl.controls.ScrollBarDirection ScrollBarDirection     * @see fl.core.UIComponent UIComponent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ScrollEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code> property of a <code>scroll</code>		 * event object.		 *		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default		 * 			behavior to cancel.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing 		 * 			the event object with an event listener.</td></tr>		 *     <tr><td><code>delta</code></td><td><code>1</code>; a value that indicates		 *          how much scrolling was done.		 * 			</td></tr>		 *     <tr><td><code>direction</code></td><td><code>vertical</code>; the direction of the		 *			ScrollBar.</td></tr>		 *     <tr><td><code>position</code></td><td><code>0</code>; the position of the		 * 			Scrollbar thumb after it was moved. </td></tr>		 *		<tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>		 *          * @eventType scroll         *         * @includeExample ../containers/examples/ScrollPane.scroll.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SCROLL : String = "scroll";
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _direction : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _delta : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _position : Number;

		/**
		 * Gets a constant value that indicates the direction of movement associated with the event. 		 * A value of <code>ScrollBarDirection.HORIZONTAL</code> indicates horizontal movement; a value 		 * of <code>ScrollBarDirection.VERTICAL</code> indicates vertical movement.		 *         * @see fl.controls.ScrollBarDirection         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get direction () : String;
		/**
		 * Gets the size of the change in scroll position, in pixels. A positive value 		 * indicates that the direction of the scroll was down or to the right. A negative value indicates that		 * the direction of the scroll was up or to the left.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get delta () : Number;
		/**
		 * Gets the current scroll position, in pixels.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get position () : Number;

		/**
		 * Creates a new ScrollEvent object with the specified parameters.		 *         * @param direction The direction of movement associated with the event. A value of 		 * <code>ScrollBarDirection.HORIZONTAL</code> indicates horizontal movement; a value of 		 * <code>ScrollBarDirection.VERTICAL</code> indicates vertical movement.         *		 * @param delta The change in scroll position, in pixels. A positive value indicates that the direction 		 *        of the scroll was down or to the right. A negative value indicates that the		 * 		  direction of the scroll was up or to the left.		 *          * @param position The current scroll position.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ScrollEvent (direction:String, delta:Number, position:Number);
		/**
		 * Returns a string that contains all the properties of the ScrollEvent object. The		 * string has the following format:		 * 		 * <p>[<code>ScrollEvent type=<em>value</em> bubbles=<em>value</em>		 * cancelable=<em>value</em> direction=<em>value</em> delta=<em>value</em>		 * position=<em>value</em></code>]</p>		 *         * @return A string representation of the ScrollEvent object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
		/**
		 * Creates a copy of the ScrollEvent object and sets the value of each parameter to 		 * match the original.		 *         * @return A new ScrollEvent object with parameter values that match the original.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
