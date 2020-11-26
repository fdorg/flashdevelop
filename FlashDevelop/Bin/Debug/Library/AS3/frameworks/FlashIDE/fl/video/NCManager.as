package fl.video
{
	import flash.net.*;
	import flash.events.TimerEvent;
	import flash.events.NetStatusEvent;
	import flash.utils.Timer;

	/**
	 * Creates the <code>NetConnection</code> object for the VideoPlayer class, a	 * helper class for that user facing class.	 *	 * <p>The NCManager class searches a URL and assumes the following:</p>	 * <ul>	 * <li>If the URL string host starts with a valid FMS streaming protocol (such as rtmp://, rtmps://, or	 * rtmpt://) it infers that the URL is streaming from an FMS.</li>	 * <li>If it does not stream from an FMS and if the URL contains a question mark (?), it infers that the URL points to an SMIL file.</li>	 * <li>If it does not stream from an FMS and if it does not contain a question mark (?), the NCManager class	 * checks to see whether it ends in .flv.  If it ends in .flv, it infers it is for a progressive download FLV. Otherwise,	 * it is an SMIL file to download and parse.</li>	 * </ul>	 *	 * @includeExample examples/NCManagerExample.as -noswf     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class NCManager implements INCManager
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _owner : VideoPlayer;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _contentPath : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _protocol : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _serverName : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _portNumber : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _wrappedURL : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _appName : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _streamName : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _streamLength : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _streamWidth : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _streamHeight : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _streams : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _isRTMP : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _smilMgr : SMILManager;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _fpadMgr : FPADManager;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _fpadZone : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _objectEncoding : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _proxyType : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _bitrate : Number;
		/**
		 * Exposes the <code>fallbackServerName</code> property indirectly or directly.	 * 	 * <ul>	 * <li>Indirectly&#8212;Supply a second &lt;meta base&gt; in SMIL.	 * The <code>fallbackServerName</code> property uses that base attribute.	 * (The <code>fallbackServerName</code> property uses the server name only, 	 * nothing else.)</li>	 *	 * <li>Directly&#8212;Access the <code>ncMgr</code> property in 	 * FLVPlayback or VideoPlayer and set the         * <code>fallbackServerName</code> property or use         * the <code>setProperty()</code> method.</li>         * </ul>         *		 * @see #setProperty()         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var fallbackServerName : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _timeoutTimer : Timer;
		/**
		 * The default timeout in milliseconds.         * @see INCManager#timeout          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public const DEFAULT_TIMEOUT : uint = 60000;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _payload : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _autoSenseBW : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _nc : NetConnection;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _ncUri : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _ncConnected : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _tryNC : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _tryNCTimer : Timer;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _connTypeCounter : uint;

		/**
		 * @copy INCManager#timeout         * @see INCManager#timeout          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get timeout () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set timeout (t:uint) : void;
		/**
		 * When streaming from Flash Media Server (FMS), the <code>bitrate</code> property		 * returns the value calculated from autodetection,		 * not the value set through the <code>bitrate()</code> property.		 *         * @see INCManager#bitrate          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bitrate () : Number;
		/**
		 * @private         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set bitrate (b:Number) : void;
		/**
		 * @copy INCManager#videoPlayer         * @see INCManager#videoPlayer          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get videoPlayer () : VideoPlayer;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set videoPlayer (v:VideoPlayer) : void;
		/**
		 * @copy INCManager#netConnection         * @see INCManager#netConnection          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get netConnection () : NetConnection;
		/**
		 * @copy INCManager#streamName         * @see INCManager#streamName          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamName () : String;
		/**
		 * @copy INCManager#isRTMP         * @see INCManager#isRTMP           *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get isRTMP () : Boolean;
		/**
		 * @copy INCManager#streamLength         * @see INCManager#streamLength           *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamLength () : Number;
		/**
		 * @copy INCManager#streamWidth         * @see INCManager#streamWidth          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamWidth () : int;
		/**
		 * @copy INCManager#streamHeight         * @see INCManager#streamHeight          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamHeight () : int;

		/**
		 * Creates a new NCManager instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function NCManager ();
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function initNCInfo () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function initOtherInfo () : void;
		/**
		 * Allows getting of the <code>fallbackServerName</code>, <code>fpadZone</code>, <code>objectEncoding</code>,		 * and <code>proxyType</code> properties.  See <code>setProperty()</code> for		 * an explanation of these properties.		 *		 * @param propertyName The name of the property that the <code>getProperty</code> 		 * method is calling.		 * 		 * @see #setProperty()		 * @see #fallbackServerName         * @return String representation of the fallbackServerName, fpadZone, objectEncoding, and proxyType properties.         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getProperty (propertyName:String) : *;
		/**
		 * Allows setting of the <code>fallbackServerName</code>, <code>fpadZone</code>, <code>objectEncoding</code>,		 * and <code>proxyType</code> properties.		 *		 * <p>The <code>fallbackServerName</code> property specifies a Flash Media Server (FMS) URL		 * to be used if the primary server address derived from the URL passed		 * into <code>connectToURL()</code> cannot be reached.  This property can also be set		 * directly through the <code>fallbackServerName</code> property.</p>		 *	 * <p>The <code>fpadZone</code> property specifies the <code>fpadZone</code> property for Flash Media Server (FMS).	 * If the returned value is not a number (NaN),         * then no zone is set.         * The <code>fpadZone</code> property must be set before 	 * the connection process begins to have effect.         * If you do not want to set a zone, set the <code>fpadZone</code> property to <code>NaN</code>.</p>		 * Default is <code>NaN</code>.		 *		 * <p>The <code>objectEncoding</code> property specifies the value to be used for the <code>objectEncoding</code>		 * property on <code>NetConnection</code> instances.  Default is <code>flash.net.ObjectEncoding.AMF0</code>.</p>		 *		 * <p>The <code>proxyType</code> property specifies the value to be used for the <code>proxyType</code>		 * property on <code>NetConnection</code> instances.  Default is "best".</p>		 *		 * @param propertyName The name of the property that the <code>setProperty</code> 		 * method is calling.		 *		 * @param value The property value.		 *		 * @see #getProperty()		 * @see #fallbackServerName		 *		 * @langversion 3.0	 	 * @playerversion Flash 9.0.28.0
		 */
		public function setProperty (propertyName:String, value:*) : void;
		/**
		 * @copy INCManager#connectToURL()         * @see INCManager#connectToURL()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function connectToURL (url:String) : Boolean;
		/**
		 * @copy INCManager#connectAgain()         * @see INCManager#connectAgain()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function connectAgain () : Boolean;
		/**
		 * @copy INCManager#reconnect()         * @see INCManager#reconnect()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function reconnect () : void;
		/**
		 * Dispatches reconnect event, called by internal class method	 * <code>ReconnectClient.onBWDone()</code>         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function onReconnected () : void;
		/**
		 * @copy INCManager#close()         * @see INCManager#close()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function close () : void;
		/**
		 *	 * @copy INCManager#helperDone()	 *         * @see INCManager#helperDone()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function helperDone (helper:Object, success:Boolean) : void;
		/**
		 * Matches bitrate with stream.	 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function bitrateMatch () : void;
		/**
		 * Parses URL to determine if it is http or rtmp.  If it is rtmp,		 * breaks it into pieces to extract server URL and port, application		 * name and stream name.  If .flv is at the end of an rtmp URL, it		 * will be stripped off.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseURL (url:String) : ParseResults;
		/**
		 * <p>Compares connection info with previous NetConnection,		 * will reuse existing connection if possible.</p>		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function canReuseOldConnection (parseResults:ParseResults) : Boolean;
		/**
		 * <p>Handles creating <code>NetConnection</code> instance for		 * progressive download of FLV via http.</p>		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectHTTP () : Boolean;
		/**
		 * <p>Top level function for creating <code>NetConnection</code>		 * instance for streaming playback of FLV via rtmp.  Actually		 * tries to create several different connections using different		 * protocols and ports in a pipeline, so multiple connection		 * attempts may be occurring simultaneously, and will use the		 * first one that connects successfully.</p>		 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectRTMP () : Boolean;
		/**
		 * <p>Top level function for downloading fpad XML from FMS 2.0		 * server.  Creates and kicks off a FPADManager instance		 * which does all the work.</p>		 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectFPAD (url:String) : Boolean;
		/**
		 * <p>Does work of trying to open rtmp connections.  Called either		 * by <code>connectRTMP</code> or on an interval set up in		 * that method.</p>		 *		 * <p>For creating rtmp connections.</p>		 *		 * @see #connectRTMP()          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function nextConnect (e:TimerEvent = null) : void;
		/**
		 * <p>Stops all intervals, closes all unneeded connections, and other		 * cleanup related to the <code>connectRTMP</code> strategy of		 * pipelining connection attempts to different protocols and		 * ports.</p>		 *		 * <p>For creating rtmp connections.</p>		 *		 * @see #connectRTMP()         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function cleanConns () : void;
		/**
		 * <p>Starts another pipelined connection attempt with		 * <code>connectRTMP</code> with the fallback server.</p>		 *		 * <p>For creating rtmp connections.</p>		 *		 * @see #connectRTMP()         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function tryFallBack () : void;
		/**
		 * <p>Starts another pipelined connection attempt with		 * <code>connectRTMP</code> with the fallback server.</p>		 *		 * <p>For creating rtmp connections.</p>		 *		 * @see #connectRTMP()         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function onConnected (p_nc:NetConnection, p_bw:Number) : void;
		/**
		 * netStatus event listener when connecting		 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectOnStatus (e:NetStatusEvent) : void;
		/**
		 * @private         * netStatus event listener when reconnecting         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function reconnectOnStatus (e:NetStatusEvent) : void;
		/**
		 * @private         *		 * netStatus event listener for disconnecting extra         * NetConnections that were opened in parallel         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function disconnectOnStatus (e:NetStatusEvent) : void;
		/**
		 * @private         *		 * Responder function to receive streamLength result from         * server after making rpc         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getStreamLengthResult (length:Number) : void;
		/**
		 * @private         *		 * Called on interval to timeout all connection attempts.		 *		 * <p>For creating rtmp connections.</p>		 *		 * @see #connectRTMP()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function _onFMSConnectTimeOut (e:TimerEvent = null) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static function stripFrontAndBackWhiteSpace (p_str:String) : String;
	}
}
