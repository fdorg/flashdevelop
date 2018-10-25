package fl.video
{
	import flash.events.Event;

	/**
	 * Flash<sup>&#xAE;</sup> Player dispatches a MetadataEvent object when the user 		 * requests the FLV file's metadata information packet (<code>NetStream.onMetaData</code>) 		 * and when cue points (<code>NetStream.onCuePoint</code>) are encountered in the FLV file.		 * 		 * @see flash.net.NetStream#event:onCuePoint NetStream.onCuePoint event		 * @see flash.net.NetStream#event:onMetaData NetStream.onMetaData event		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
	 */
	public class MetadataEvent extends Event implements IVPEvent
	{
		/**
		 * Defines the value of the 		 * <code>type</code> property of a <code>metadataReceived</code> event object.		 *		 * <p>This event has the following properties:</p>		 * <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>		 *     <tr><td><code>info</code></td><td>The object with properties describing the FLV file.</td></tr>		 *     <tr><td><code>vp</code></td><td>The index of the VideoPlayer object.</td></tr>		 *     	         * </table>		 * 		 * @see FLVPlayback#event:metadataReceived metadataReceived event		 * @eventType metadataReceived		 * @langversion 3.0                 * @playerversion Flash 9.0.28.0
		 */
		public static const METADATA_RECEIVED : String = "metadataReceived";
		/**
		 * Defines the value of the 		 * <code>type</code> property of a <code>cuePoint</code> event object.		 * 		 * <p>This event has the following properties:</p>		 * <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>		 *     <tr><td><code>info</code></td><td>The object with properties describing the cue point.</td></tr>		 *     <tr><td><code>vp</code></td><td>The index of the VideoPlayer object.</td></tr>		 *     	         * </table>		 * @see FLVPlayback#event:cuePoint cuePoint event		 * @eventType cuePoint		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const CUE_POINT : String = "cuePoint";
		private var _info : Object;
		private var _vp : uint;

		/**
		 * An object with dynamic properties added depending on the event type.		 * 		 * @see flash.net.NetStream#event:onCuePoint NetStream.onCuePoint event		 * @see flash.net.NetStream#event:onMetaData NetStream.onMetaData event		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get info () : Object;
		public function set info (i:Object) : void;
		/**
		 * The index of the VideoPlayer object involved in this event.		 *		 * @see FLVPlayback#activeVideoPlayerIndex		 * @see FLVPlayback#visibleVideoPlayerIndex		 * @see FLVPlayback#getVideoPlayer()		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get vp () : uint;
		public function set vp (n:uint) : void;

		/**
		 * Creates an Event object that contains information about metadata events. 		 * Event objects are passed as parameters to event listeners.		 * 		 * @param type The type of the event. Event listeners can access this information          * through the inherited <code>type</code> property. Possible values are <code>MetadataEvent.CUE_POINT</code>          * and <code>MetadataEvent.METADATA_RECEIVED</code>.		 * 		 * @param bubbles Determines whether the Event object participates in the bubbling 		 * stage of the event flow. Event listeners can access this information through the 		 * inherited <code>bubbles</code> property.		 * 		 * @param cancelable Determines whether the Event object can be canceled. Event listeners can 		 * access this information through the inherited <code>cancelable</code> property.		 * 		 * @param info Determines the dynamic properties to add.		 * 		 * @param vp Determines the index of the VideoPlayer object.		 * 		 * @langversion 3.0		 * @playerversion Flash 9.0.28.0		 *
		 */
		public function MetadataEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, info:Object = null, vp:uint = 0);
		/**
		 *  @private
		 */
		public function clone () : Event;
	}
}
