package fl.video
{
	/**
	 * The VideoState class provides constant values for the read-only <code>FLVPlayback.state</code> and	 * <code>VideoPlayer.state</code> properties.	 *      * @see FLVPlayback#state      * @see VideoPlayer#state      *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class VideoState
	{
		/**
		 * The video player is in the disconnected state.                 * It enters this state when the stream is closed by a call                  * to the <code>closeVideoPlayer()</code>		 * method or timed out on idle. Use the <code>FLVPlayback.state</code> property to 		 * obtain the current state of the video player.		 * 		 * <p>The FLVPlayback instance is in a disconnected state until you set the 		 * <code>FLVPlayback.source</code> property.</p>		 *          * @see FLVPlayback#closeVideoPlayer()          * @see FLVPlayback#idleTimeout          * @see FLVPlayback#source          * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          *         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const DISCONNECTED : String = "disconnected";
		/**
		 * The video player is in the stopped state.		 * It enters this state when the FLV file is loaded and 		 * play is stopped by calling the <code>stop()</code> method or when the		 * playhead reaches the end of the stream. Use the <code>FLVPlayback.state</code> 		 * property to obtain the current state of the video player.		 *		 * <p>This is a responsive state.</p>		 *         * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          * @see FLVPlayback#stop()          * @see FLVPlayback#stopped          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const STOPPED : String = "stopped";
		/**
		 * The video player is in the playing state.		 * It enters this state when the FLV file is loaded and is playing. 		 * Use the <code>FLVPlayback.state</code> 		 * property to obtain the current state of the video player.		 *		 * <p>This is a responsive state.</p>		 *         * @see FLVPlayback#playing          * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          * @see FLVPlayback#play()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const PLAYING : String = "playing";
		/**
		 * The video player is in the paused state.		 * It enters this state when the FLV file is loaded, but play is paused		 * by calling the <code>pause()</code> or		 * <code>load()</code> method. 		 * Use the <code>FLVPlayback.state</code> 		 * property to obtain the current state of the video player.		 *		 * <p>This is a responsive state.</p>		 *         * @see FLVPlayback#load()          * @see FLVPlayback#pause()          * @see FLVPlayback#paused          * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          *                                                    * @langversion 3.0                                   * @playerversion Flash 9.0.28.0
		 */
		public static const PAUSED : String = "paused";
		/**
		 * The video player is in the buffering state. 		 * It enters this state immediately 		 * after a call is made to the <code>play()</code> or <code>load()</code> method. 		 * Use the <code>FLVPlayback.state</code> property to obtain the current state of 		 * the video player.		 *		 * <p>This is a responsive state.</p>		 *         * @see FLVPlayback#buffering          * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const BUFFERING : String = "buffering";
		/**
		 * The video player is in the loading state. 		 * It enters this state immediately after the		 * <code>play()</code> or <code>load()</code> method is called or		 * after the <code>FLVPlayback.source</code> property is set.		 * Use the <code>FLVPlayback.state</code> property to obtain the current state of 		 * the video player.		 *		 * <p>This is an unresponsive state.</p>		 *         * @see FLVPlayback#load()          * @see FLVPlayback#play()          * @see FLVPlayback#source          * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const LOADING : String = "loading";
		/**
		 * The video player is in the connection error state.		 * It enters this state when a video stream attempted to 		 * load but was unsuccessful. There are two possible reasons for the error: 		 * no connection to the server or the stream was not found.		 * Use the <code>FLVPlayback.state</code> property to obtain the current 		 * state of the video player.		 *		 * <p>This is an unresponsive state.</p>		 *         * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const CONNECTION_ERROR : String = "connectionError";
		/**
		 * The video player is in the rewinding state.		 * It enters this state when the video rewinds automatically.		 * The video rewinds automatically after it has stopped, either		 * by clicking the stop button or by the video playing to the end. 		 * After rewinding is complete, the state is stopped.		 * Use the <code>FLVPlayback.state</code> property to obtain the current 		 * state of the video player.		 *		 * <p>This is an unresponsive state.</p>		 *         * @see FLVPlayback#autoRewind          * @see FLVPlayback#state          * @see FLVPlayback#stateResponsive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const REWINDING : String = "rewinding";
		/**
		 * The video player is in the seeking state.		 * It enters this state after the <code>seek()</code> method		 * is called and also while the user is scrubbing with the seek bar.		 * Use the <code>FLVPlayback.state</code> property to obtain the current 		 * state of the video player.		 * 		 * <p>This is an unresponsive state.</p>		 *         * @see FLVPlayback#stateResponsive          * @see FLVPlayback#state          * @see FLVPlayback#seek()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SEEKING : String = "seeking";
		/**
		 * The video player is in the resizing state.		 * It enters this state during autolayout.		 * The <code>FLVPlayback</code> instance never has this constant's state value, 		 * only the <code>VideoPlayer</code> instance. Use the <code>VideoPlayer.state</code> 		 * property to obtain the current 		 * state of the video player.		 *		 * <p>This is a unresponsive state.</p>		 *                 * @see VideoPlayer#state          * @see VideoPlayer#stateResponsive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const RESIZING : String = "resizing";
		/**
		 * The video player is in the execQueuedCmd state.  		 * It enters this state during execution of queued command.		 * There will never get a "stateChange" event notification with		 * this state; it is internal only.		 *		 * <p>This is a unresponsive state.</p>		 *         * @see VideoPlayer#state          * @see VideoPlayer#stateResponsive          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static var EXEC_QUEUED_CMD : String;

	}
}
