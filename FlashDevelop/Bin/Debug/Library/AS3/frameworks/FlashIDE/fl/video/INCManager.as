package fl.video
{
	import flash.net.NetConnection;

	/**
	 * The INCManager is the interface for classes that create the <code>flash.net.NetConnection</code>      * for the VideoPlayer class. The default INCManager implementation is the NCManager class.      * Use the NCManagerNative class when streaming from a Flash Media Server (FMS).      * You can also create a custom class that implements the INCManager interface and      * then have the VideoPlayer class use that class to create the NetConnection.     *     * <p>Use the following code to register a custom class as the INCManager implementation      * used by the VideoPlayer object.     * Replace <code>fl.video.NCManagerNative</code> with your custom class.</p>     *      * <listing>fl.video.VideoPlayer.iNCManagerClass = fl.video.NCManagerNative;</listing>     *      * @see fl.video.VideoPlayer     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public interface INCManager
	{
		/**
		 * The VideoPlayer object that owns this object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get videoPlayer () : VideoPlayer;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set videoPlayer (v:VideoPlayer) : void;
		/**
		 * The time in milliseconds after which attempts to make a connection stop.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get timeout () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set timeout (t:uint) : void;
		/**
		 * Reference to the NetConnection object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get netConnection () : NetConnection;
		/**
		 * The bandwidth, in bits per second, used to switch between multiple         * streams.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bitrate () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set bitrate (b:Number) : void;
		/**
		 * The stream name passed into the         * <code>NetStream.play()</code> method.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamName () : String;
		/**
		 * Whether the URL is for RTMP streaming from a Flash Media Server (FMS)         * or a progressive download. If the         * stream is an RTMP stream from an FMS, then <code>true</code>. If the stream is a         * progressive download of an HTTP, local or other file, then <code>false</code>.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get isRTMP () : Boolean;
		/**
		 * Length of the stream, in seconds. After the <code>VideoPlayer.ncConnected()</code> method         * is called, if it returns undefined, <code>null</code> or less than 0,          * then the VideoPlayer object knows that there is no stream length information.          * If stream length information is returned, it overrides any existing steam length information          * including information set by the <code>totalTime</code> parameter of the          * <code>VideoPlayer.play()</code> method or the         * <code>VideoPlayer.load()</code> method or information received from the FLV file's metadata.         *         * @see VideoPlayer#ncConnected()         * @see VideoPlayer#play()         * @see VideoPlayer#load()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamLength () : Number;
		/**
		 * Width of the stream, in pixels.  After the         * <code>VideoPlayer.ncConnected()</code> method is called, if the <code>streamWidth</code> property         * is less than 0, that indicates to the VideoPlayer object that there is no stream width          * information.  If the VideoPlayer object has the <code>scaleMode</code> property set         * to <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or <code>VideoScaleMode.NO_SCALE</code>,         * then this value is used and the resizing happens instantly, rather than waiting.         * @see VideoPlayer#ncConnected()         * @see VideoPlayer#scaleMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamWidth () : int;
		/**
		 * Height of the stream, in pixels.  After the         * <code>VideoPlayer.ncConnected()</code> method is called, if the <code>streamHeight</code>         * property is less than 0, that indicates to the VideoPlayer object that there is no stream          * height information.  If the         * VideoPlayer object has the <code>scaleMode</code> property set         * to <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or <code>VideoScaleMode.NO_SCALE</code>,         * then this value will be used and the resizing will         * happen instantly, rather than waiting.         *         * @see VideoPlayer#ncConnected()         * @see VideoPlayer#scaleMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get streamHeight () : int;

		/**
		 * Called by the VideoPlayer object to ask for a connection to the         * URL. Once a connection is successful or failed, then call the         * <code>VideoPlayer.ncConnected()</code> method. If the connection has failed, then         * set <code>nc = null</code> before calling.         *         * @param url The URL to which the VideoPlayer object requests connection.         *         * @return If a connection is made synchronously, <code>true</code>. If an attempt is         * made asynchronously so caller should expect a "connected"         * event coming, <code>false</code>.         *         * @see #reconnect()         * @see VideoPlayer#ncConnected()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function connectToURL (url:String) : Boolean;
		/**
		 * Called by the VideoPlayer object if the connection is         * successfully made but the stream is not found.  If multiple alternate         * interpretations of the RTMP URL are possible, it tries         * to connect to the server with a different URL and hand back a         * different stream name.         *         * <p>This can be necessary in cases where the URL is something         * like rtmp://servername/path1/path2/path3.  When         * passing in an application name and an instance name, open the         * NetConnection object with         * rtmp://servername/path1/path2, or use the         * default instance so the stream is opened with         * path2/path3.  In general, this is possible whenever there are         * more than two parts to the path but not possible if there are         * only two (there should never be only one).</p>         *         * @return If an attempt is made to make another connection, then <code>true</code>.         * If an attempt has already been made or no additional attempts         * are merited, then <code>false</code>.         *         * @see #connectToURL()         * @see VideoPlayer#isRTMP         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function connectAgain () : Boolean;
		/**
		 * Called by the VideoPlayer object to ask for reconnection         * after the connection is lost.  Once the connection is either successful         * or failed, call the <code>VideoPlayer.ncReconnected()</code> method.  If the         * connection failed, set <code>nc = null</code> before calling.         *         * @see VideoPlayer#idleTimeout         * @see VideoPlayer#ncReconnected()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function reconnect () : void;
		/**
		 * Called by any helper object doing a task for the         * NCManager object to signal it has completed         * and whether it was successful. The NCManager object         * uses this with SMILManager.         *         * @param helper The helper object.         *         * @param success A setting to signal when a task is completed. If <code>success</code> is          * <code>true</code>, then the task was completed successfully; otherwise <code>false</code>.         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function helperDone (helper:Object, success:Boolean) : void;
		/**
		 * Closes the NetConnection.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function close () : void;
		/**
		 * Gets values of arbitrary properties supported         * by the class implementing INCManager.  See the         * specific implementing class for list of supported         * properties.  Calling this method on an unsupported         * property throws a VideoError object         * with code <code>VideoError.UNSUPPORTED_PROPERTY=1011</code>.         *          * @param propertyName The name of the property that the <code>getProperty</code> method         * is calling.         *         * @return The values of the properties.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getProperty (propertyName:String) : *;
		/**
		 * Sets values of arbitrary properties supported         * by the class implementing INCManager.  See the         * specific implementing class for list of supported         * properties.  Calling this method on an unsupported         * property throws a VideoError object         * with code <code>VideoError.UNSUPPORTED_PROPERTY=1011</code>.         *          * @param propertyName The name of the property that the <code>setProperty</code> method         * is calling.         *         * @param value The property value.         *         * @return The value of the property.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setProperty (propertyName:String, value:*) : void;
	}
}
