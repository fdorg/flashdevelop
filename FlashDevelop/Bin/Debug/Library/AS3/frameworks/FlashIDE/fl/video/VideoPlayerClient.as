package fl.video
{
	/**
	 * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public dynamic class VideoPlayerClient
	{
		protected var _owner : VideoPlayer;
		protected var gotMetadata : Boolean;

		public function get owner () : VideoPlayer;
		/**
		 * property that specifies whether early messages (onMetaData, any		 * custom messages that might be expected) have been received so		 * it is OK for the player to rewind back to the beginning.
		 */
		public function get ready () : Boolean;

		public function VideoPlayerClient (vp:VideoPlayer);
		/**
		 * handles NetStream.onMetaData callback		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function onMetaData (info:Object, ...rest) : void;
		/**
		 * handles NetStream.onCuePoint callback		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function onCuePoint (info:Object, ...rest) : void;
	}
}
