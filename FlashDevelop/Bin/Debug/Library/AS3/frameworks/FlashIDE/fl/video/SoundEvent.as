package fl.video
{
	import flash.events.Event;
	import flash.media.SoundTransform;

	/**
	 * Flash<sup>&#xAE;</sup> Player dispatches a SoundEvent object when the user changes      * the sound by either moving the handle of the volumeBar control or setting the <code>volume</code>      * or <code>soundTransform</code> property.     *     * @tiptext SoundEvent class     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SoundEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code>          * property of a soundUpdate event object.         *         * <p>This event has the following properties:</p>	 * <table class="innertable" width="100%">	 *     <tr><th>Property</th><th>Value</th></tr>	 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>	 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>	 *     <tr><td><code>soundTransform</code></td><td>Indicates new values for volume and panning.</td></tr>	 *     	 * </table>         * @eventType soundUpdate         * @tiptext SOUND_UPDATE constant         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SOUND_UPDATE : String = "soundUpdate";
		private var _soundTransform : SoundTransform;

		/**
		 * Indicates new values for volume and panning.         *         * @see flash.media.SoundTransform SoundTransform         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get soundTransform () : SoundTransform;

		/**
		 * Creates an Event object that contains information about sound events.          * Event objects are passed as parameters to event listeners.         *         * @param type The type of the event. Event listeners can access this information          * through the inherited <code>type</code> property.          * There is only one type of sound event: <code>SoundEvent.SOUND_UPDATE</code>.         *         * @param bubbles Determines whether the Event object participates in the bubbling          * stage of the event flow. Event listeners can access this information through the          * inherited bubbles property.         *         * @param cancelable Determines whether the Event object can be canceled. Event listeners can          * access this information through the inherited cancelable property.         *         * @param soundTransform Indicates new values for volume and panning.         * @tiptext SoundEvent construtor         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SoundEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, soundTransform:SoundTransform = null);
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
