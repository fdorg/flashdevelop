package fl.video
{
	import flash.events.Event;
	import flash.events.ProgressEvent;

	/**
	 * Flash<sup>&#xAE;</sup> Player dispatches a VideoProgressEvent object when the user      * makes a request for the number of bytes loaded during a progressive HTTP download of their video.      *      * @see flash.events.ProgressEvent ProgressEvent     *      * @tiptext VideoProgressEvent class     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class VideoProgressEvent extends ProgressEvent implements IVPEvent
	{
		/**
		 * Defines the value of the <code>type</code> property of a <code>progress</code> event object.	 *	 * <p>This event has the following properties:</p>	 * <table class="innertable" width="100%">	 *     <tr><th>Property</th><th>Value</th></tr>	 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>	 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>	 *     <tr><td><code>bytesLoaded</code></td><td>The number of items or bytes loaded at the time the listener processes the event.</td></tr>	 *     <tr><td><code>bytesTotal</code></td><td>The total number of items or bytes that will be loaded if the loading process succeeds.</td></tr>	 *     <tr><td><code>vp</code></td><td>The index of the VideoPlayer object.</td></tr>	 * </table>	 * @eventType progress         * @tiptext PROGRESS constant         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const PROGRESS : String = "progress";
		private var _vp : uint;

		/**
		 * The index of the VideoPlayer object involved in this event.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get vp () : uint;
		/**
		 * @private (setter)
		 */
		public function set vp (n:uint) : void;

		/**
		 * Creates an Event object that contains information about progress events. 	 * Event objects are passed as parameters to event listeners.	 * 	 * @param type The type of the event. Event listeners can access this information 	 * through the inherited <code>type</code> property. There is only one type of 	 * progress event: VideoProgressEvent.PROGRESS.	 * 	 * @param bubbles Determines whether the Event object participates in the bubbling 	 * stage of the event flow. Event listeners can access this information through the 	 * inherited bubbles property.	 * 	 * @param cancelable Determines whether the Event object can be canceled. Event listeners can 	 * access this information through the inherited cancelable property.	 *	 * @param bytesLoaded The number of items or bytes loaded at the time the listener processes the event.	 *	 * @param bytesTotal The total number of items or bytes that will be loaded if the loading process succeeds.	 *	 * @param vp Determines the index of the VideoPlayer object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function VideoProgressEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, bytesLoaded:uint = 0, bytesTotal:uint = 0, vp:uint = 0);
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
