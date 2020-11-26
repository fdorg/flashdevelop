package fl.video
{
	import flash.net.*;
	import flash.events.TimerEvent;
	import flash.events.NetStatusEvent;
	import flash.utils.Timer;
	import flash.utils.getTimer;

	/**
	 * The NCManagerNative class is a subclass of the NCManager class and supports 	 * native bandwidth detection, which some Flash Video Streaming Service providers	 * may support. Check with your FVSS provider to see whether they support native bandwidth	 * detection. Native bandwidth detection means that the bandwidth detection is built in	 * to the streaming server and performs better.	 * 	 * <p>When an NCManagerNative object is	 * used, the main.asc file is not required on the server. If bandwidth detection is not required, 	 * the NCManagerNative object allows	 * connection to any version of the Flash Media Server (FMS) without the main.asc file.</p>	 *	 * <p>To use this instead of the default fl.video.NCManager, put the following code	 * in Frame 1 of your FLA file:</p>	 *     * <listing version="3.0">     * import fl.video.~~;     * VideoPlayer.iNCManagerClass = fl.video.NCManagerNative;     * </listing>	 *     * @see NCManager     * @tiptext NCManagerNative class     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class NCManagerNative extends NCManager implements INCManager
	{
		/**
		 * Length of the stream, in milliseconds. After the <code>VideoPlayer.ncConnected()</code> method	 * is called, if it returns undefined, <code>null</code> or less than 0, 	 * then the VideoPlayer object knows that there is no stream length information. 	 * If stream length information is returned, it overrides any existing steam length information 	 * including information set by the <code>totalTime</code> parameter of the 	 * <code>VideoPlayer.play()</code> method, the	 * <code>VideoPlayer.load()</code> method or information received from the FLV file's metadata.	 *          * @see INCManager#streamLength         * @tiptext streamLength property         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamLength () : Number;

		/**
		 * Creates a new NCManagerNative instance.         * @tiptext NCManagerNative  constructor         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function NCManagerNative ();
		/**
		 * Overridden to create ConnectClientNative instead of ConnectClient.	 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectRTMP () : Boolean;
		/**
		 * Overridden to avoid call to getStreamLength     * @private     * @tiptext onConnected method     * @langversion 3.0     * @playerversion Flash 9.0.28.0
		 */
		function onConnected (p_nc:NetConnection, p_bw:Number) : void;
		/**
		 * overriden to call run() when _autoSenseBW is on, and to immediately         * call onConnected() if it is not, instead of waiting for a call to         * onBWDone from the server, like NCManager does.         *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectOnStatus (e:NetStatusEvent) : void;
		/**
		 * netStatus event listener when reconnecting         *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function reconnectOnStatus (e:NetStatusEvent) : void;
	}
}
