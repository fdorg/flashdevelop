package fl.video
{
	import flash.events.Event;
	import flash.geom.Rectangle;

	/**
	 * Event dispatched when the video player is resized and/or laid out. Here are two layout scenarios:	 * <ul>     *   <li>If the video player is laid out by using the <code>autoLayout</code> 	 *       event, calling the <code>setScale()</code>,  	 *       <code>setSize()</code> methods, or changing the <code>width</code>, <code>height</code>,	 *       <code>scaleX</code>, <code>scaleY</code>,	 *       <code>registrationWidth</code>, and <code>registrationHeight</code> properties.</li>	 *   <li>If there are two video players of different sizes or locations and the 	 *       <code>visibleVideoPlayerIndex</code> property is switched from one video player to another.</li>	 * </ul>  	 *	 * <p>There is only one type of LayoutEvent object: <code>LayoutEvent.LAYOUT</code>.</p>	 *      * @see #LAYOUT      *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class LayoutEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code>         * property of a <code>layout</code> event object.         *         * <p>This event has the following properties:</p>	 * <table class="innertable" width="100%">	 *     <tr><th>Property</th><th>Value</th></tr>	 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>	 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>	 *     <tr><td><code>oldBounds</code></td><td>The values of the <code>x</code>, <code>y</code>, 	 *        <code>width</code>, and <code>height</code> properties of the target before the <code>layout</code> event occurs.</td></tr>	 *     <tr><td><code>oldRegistrationBounds</code></td><td>The values of the <code>registrationX</code>, 	 * <code>registrationY</code>, <code>registrationWidth</code>, and 	 * <code>registrationHeight</code> properties of the target before the <code>layout</code> event occurs.</td></tr>	 *     	 * </table>	 * @see FLVPlayback#event:layout layout event         * @eventType layout         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const LAYOUT : String = "layout";
		private var _oldBounds : Rectangle;
		private var _oldRegistrationBounds : Rectangle;

		/**
		 * Indicates the values of the <code>x</code>, <code>y</code>,		 * <code>width</code>, and <code>height</code> properties         * of the target before the event occurs.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get oldBounds () : Rectangle;
		/**
		 * @private (setter)
		 */
		public function set oldBounds (r:Rectangle) : void;
		/**
		 * Indicates the values of the <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code>, and <code>registrationHeight</code> properties         * of the target before the event occurs.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get oldRegistrationBounds () : Rectangle;
		/**
		 * @private (setter)
		 */
		public function set oldRegistrationBounds (r:Rectangle) : void;

		/**
		 * Creates an Event object that contains information about <code>layout</code> events. 	 * Event objects are passed as parameters to event listeners.	 *         * @param type The type of the event. Event listeners can access this information 	 * through the inherited <code>type</code> property. There is only one type of 	 * auto layout event: <code>LayoutEvent.LAYOUT</code>.	 *	 * @param bubbles Determines whether the Event object participates in the bubbling 	 * stage of the event flow. Event listeners can access this information through 	 * the inherited <code>bubbles</code> property.	 * 	 * @param cancelable Determines whether the Event object can be canceled. Event listeners 	 * can access this information through the inherited <code>cancelable</code> property.	 * 	 * @param oldBounds Indicates the values of the <code>x</code>, <code>y</code>, 	 * <code>width</code>, and <code>height</code> properties of 	 * the target before the <code>layout</code> event occurs. Event listeners can access this information through 	 * the <code>oldBounds</code> property.	 *	 * @param oldRegistrationBounds Indicates the values of the <code>registrationX</code>, 	 * <code>registrationY</code>, <code>registrationWidth</code>, and          * <code>registrationHeight</code> properties of the target before the <code>layout</code> event occurs.         * Event listeners can access this information through 	 * the <code>oldRegistrationBounds</code> property.         *         * @see #oldBounds         * @see #oldRegistrationBounds         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function LayoutEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, oldBounds:Rectangle = null, oldRegistrationBounds:Rectangle = null);
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
