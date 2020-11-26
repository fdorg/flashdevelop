package fl.video
{
	import flash.events.Event;
	import flash.events.ErrorEvent;

	/**
	 * Flash<sup>&#xAE;</sup> Player dispatches a SkinErrorEvent object when there is an      * error loading a skin. The ErrorEvent class is a subclass to this event, and so if not handled     * an error is thrown.     *     * @see FLVPlayback#event:skinError skinError event     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SkinErrorEvent extends ErrorEvent
	{
		/**
		 * Defines the value of the 	 * <code>type</code> property of a <code>skinError</code> event object.          *         * <p>This event has the following properties:</p>	 * <table class="innertable" width="100%">	 *     <tr><th>Property</th><th>Value</th></tr>	 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>	 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>	 *     <tr><td><code>text</code></td><td>The error message.</td></tr>	 *     	 * </table>	 * @see FLVPlayback#event:skinError skinError event	 * @eventType skinError         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SKIN_ERROR : String = "skinError";

		/**
		 * Creates an Event object that contains information about <code>skinError</code> events. 	 * Event objects are passed as parameters to event listeners.         *         * @param type The type of the event. Event listeners can access this information 	 * through the inherited <code>type</code> property. There is only one type of 	 * skinError event: <code>SkinErrorEvent.SKIN_ERROR</code>.	 * 	 * @param bubbles Determines whether the Event object participates in the bubbling 	 * stage of the event flow. Event listeners can access this information through the 	 * inherited <code>bubbles</code> property.	 * 	 * @param cancelable Determines whether the Event object can be canceled. Event listeners can 	 * access this information through the inherited <code>cancelable</code> property.	 *	 * @param text The error message text.	 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SkinErrorEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, text:String = "");
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
