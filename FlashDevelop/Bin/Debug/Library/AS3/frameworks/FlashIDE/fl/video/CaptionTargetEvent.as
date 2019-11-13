package fl.video
{
	import flash.display.DisplayObject;
	import flash.events.Event;

	/**
	 * Type for the <code>captionTargetCreated</code> event, dispatched after the	 * <code>captionTargetCreated</code> event is created automatically and before any captions	 * have been added to it. This event is not dispatched if the	 * <code>captionTarget</code> property is set with a custom display object, or	 * if the <code>captionTargetName</code> property is set. This is useful for	 * customizing the properties of the TextField object, such as the	 * <code>defaultTextFormat</code> property.     *     * @tiptext CaptionTargetEvent class     * @see flash.text.TextField#defaultTextFormat     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class CaptionTargetEvent extends Event
	{
		/**
		 * The <code>CaptionTargetEvent.CAPTION_TARGET_CREATED</code> constant defines the value of the          * <code>type</code> property of a <code>captionTargetCreated</code> event object.          *         * <p>This event has the following properties:</p>         * <table class="innertable" width="100%">	 *     <tr><th>Property</th><th>Value</th></tr>	 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>	 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>	 *     <tr><td><code>captionTarget</code></td><td>A display object that is from the <code>captionTarget</code> 	 * property of the FLVPlaybackCaptioning instance.</td></tr>	 *              * </table>         * @eventType captionTargetCreated         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const CAPTION_TARGET_CREATED : String = "captionTargetCreated";
		private var _captionTarget : DisplayObject;

		/**
		 * The caption target from the FLVPlaybackCaptioning instance property of the		 * same name.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get captionTarget () : DisplayObject;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set captionTarget (d:DisplayObject) : void;

		/**
		 * Creates an Event object that contains information about caption target events.	 * Event objects are passed as parameters to event listeners.	 * 	 * @param type The type of the event. Event listeners can access this information 	 * through the inherited <code>type</code> property. There is only one type of 	 * <code>captionTargetCreated</code> event: <code>CaptionTargetEvent.CAPTION_CHANGE</code>.	 	 * 	 * @param bubbles Determines whether the Event object participates in the bubbling 	 * stage of the event flow. Event listeners can access this information through the 	 * inherited <code>bubbles</code> property.	 * 	 * @param cancelable Determines whether the Event object can be canceled. Event listeners can 	 * access this information through the inherited <code>cancelable</code> property.	 * 	 * @param captionTarget A display object which is from the <code>captionTarget</code> 	 * property of the FLVPlaybackCaptioning instance.	 * 	 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function CaptionTargetEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, captionTarget:DisplayObject = null);
		/**
		 *  @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
