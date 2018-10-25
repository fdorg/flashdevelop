package fl.video
{
	import flash.events.*;
	import flash.geom.Rectangle;
	import flash.media.*;
	import flash.net.*;
	import flash.utils.*;

	/**
	 * Dispatched by the VideoPlayer instance when it closes the NetConnection  	 * by timing out or through a call to the <code>close()</code> method, or when 	 * you call the <code>load()</code> or <code>play()</code> methods or set the 	 * <code>source</code> property and cause the RTMP connection 	 * to close as a result. The FLVPlayback instance dispatches this event only when 	 * streaming from Flash Media Server (FMS) or other Flash Video Streaming Service (FVSS). 	 * 	 * <p>The <code>close</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.CLOSE</code>.</p>	 *  	 * @see VideoEvent#CLOSE 	 * @see #close() 	 * @see #source 	 * @see #load() 	 * @see #play() 	 * 	 * @tiptext close event     * @eventType fl.video.VideoEvent.CLOSE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("close", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when playing completes because the player reached the end of the FLV file. 	 * The component does not dispatch the event if you call the <code>stop()</code> or 	 * <code>pause()</code> methods 	 * or click the corresponding controls. 	 * 	 * <p>When the application uses progressive download, it does not set the 	 * <code>totalTime</code>	 * property explicitly, and it downloads an FLV file that does not specify the duration 	 * in the metadata. The video player sets the <code>totalTime</code> property to an approximate 	 * total value before it dispatches this event.</p>	 * 	 * <p>The video player also dispatches the <code>stateChange</code> and <code>stopped</code>	 * events.</p>	 * 	 * <p>The <code>complete</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.COMPLETE</code>.</p>	 * 	 * @see #event:stateChange stateChange event	 * @see #stop() 	 * @see #pause() 	 * @see #totalTime 	 * 	 * @tiptext complete event     * @eventType fl.video.VideoEvent.COMPLETE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0	 *
	 */
	[Event("complete", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when a cue point is reached. The event object has an 	 * <code>info</code> property that contains the info object received by the 	 * <code>NetStream.onCuePoint</code> event callback for FLV file cue points. 	 * For ActionScript cue points, it contains the object that was passed 	 * into the ActionScript cue point methods or properties.	 * 	 * <p>The <code>cuePoint</code> event is of type MetadataEvent and has the constant 	 * <code>MetadataEvent.CUE_POINT</code>.</p>	 * 	 * @see MetadataEvent#CUE_POINT 	 * @see flash.net.NetStream#event:onCuePoint NetStream.onCuePoint event	 * 	 * @tiptext cuePoint event     * @eventType fl.video.MetadataEvent.CUE_POINT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0	 *
	 */
	[Event("cuePoint", type="fl.video.MetadataEvent")] 
	/**
	 * Dispatched the first time the FLV file's metadata is reached.	 * The event object has an <code>info</code> property that contains the 	 * info object received by the <code>NetStream.onMetaData</code> event callback.	 *	 * <p>The <code>metadataReceived</code> event is of type MetadataEvent and has the constant 	 * <code>MetadataEvent.METADATA_RECEIVED</code>.</p>	 * 	 * @see MetadataEvent#METADATA_RECEIVED 	 * @see flash.net.NetStream#event:onMetaData NetStream.onMetaData event	 *      * @tiptext metadataReceived event	 * @eventType fl.video.MetadataEvent.METADATA_RECEIVED	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0	 *
	 */
	[Event("metadataReceived", type="fl.video.MetadataEvent")] 
	/**
	 * Dispatched while the FLV file is playing at the frequency specified by the 	 * <code>playheadUpdateInterval</code> property or when rewinding starts. 	 * The component does not dispatch this event when the video player is paused or stopped 	 * unless a seek occurs. 	 * 	 * <p>The <code>playheadUpdate</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.PLAYHEAD_UPDATE</code>.</p>      *      * @default 0.25	 * 	 * @see VideoEvent#PLAYHEAD_UPDATE 	 * @see #playheadUpdateInterval 	 * 	 * @tiptext change event	 * @eventType fl.video.VideoEvent.PLAYHEAD_UPDATE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("playheadUpdate", type="fl.video.VideoEvent")] 
	/**
	 * Indicates progress made in number of bytes downloaded. Dispatched at the frequency 	 * specified by the <code>progressInterval</code> property, starting 	 * when the load begins and ending when all bytes are loaded or there is a network error. 	 * The default is every 0.25 seconds starting when load is called and ending	 * when all bytes are loaded or if there is a network error. Use this event to check 	 * bytes loaded or number of bytes in the buffer. 	 *	 * <p>Dispatched only for a progressive HTTP download. Indicates progress in number of 	 * downloaded bytes. The event object has the <code>bytesLoaded</code> and <code>bytesTotal</code>	 * properties, which are the same as the FLVPlayback properties of the same names.</p>	 * 	 * <p>The <code>progress</code> event is of type VideoProgressEvent and has the constant 	 * <code>VideoProgressEvent.PROGRESS</code>.</p> 	 * 	 * @see #bytesLoaded 	 * @see #bytesTotal 	 * @see VideoProgressEvent#PROGRESS 	 * @see #progressInterval 	 * 	 * @tiptext progress event	 * @eventType fl.video.VideoProgressEvent.PROGRESS     *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("progress", type="fl.video.VideoProgressEvent")] 
	/**
	 * Event dispatched when an FLV file is loaded and ready to display. It starts the first 	 * time you enter a responsive state after you load a new FLV file with the <code>play()</code>	 * or <code>load()</code> method. It starts only once for each FLV file that is loaded.	 * 	 * <p>The <code>ready</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.READY</code>.</p> 	 * 	 * @see #load() 	 * @see #play()      * @see VideoEvent#READY      *	 * @tiptext ready event	 * @eventType fl.video.VideoEvent.READY	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0	 *
	 */
	[Event("ready", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the video player is resized or laid out. Here are two layout scenarios:<br/>	 * <ul><li>If the video player is laid out by either using the <code>autoLayout</code> 	 * event or calling the <code>setScale()</code> or 	 * <code>setSize()</code> methods or changing the <code>width</code>, <code>height</code>,	 * <code>scaleX</code>, and <code>scaleY</code> properties.</li>	 * <li>If there are two video players of different sizes and the 	 * <code>visibleVideoPlayerIndex</code> property is switched from one video player to another.</li>	 * </ul>  	 *	 * <p>The <code>layout</code> event is of type LayoutEvent and has the constant 	 * <code>LayoutEvent.LAYOUT</code>.</p> 	 * 	 * @see FLVPlayback#event:autoLayout autoLayout event 	 * @see LayoutEvent#LAYOUT 	 * @see FLVPlayback#visibleVideoPlayerIndex      *	 * @tiptext layout event	 * @eventType fl.video.LayoutEvent.LAYOUT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("layout", type="fl.video.LayoutEvent")] 
	/**
	 * Dispatched when the playhead is moved to the start of the video player because the 	 * <code>autoRewind</code> property is set to <code>true</code>.	 * 	 * <p>The <code>autoRewound</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.AUTO_REWOUND</code>.</p>	 * 	 * @see VideoEvent#AUTO_REWOUND 	 * @see #autoRewind 	 * @see VideoState#REWINDING 	 * 	 * @tiptext autoRewound event     * @eventType fl.video.VideoEvent.AUTO_REWOUND     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("autoRewound", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the playback state changes. When an <code>autoRewind</code> call is 	 * completed the <code>stateChange</code> event is dispatched with the rewinding	 * state. The <code>stateChange</code> event does not 	 * start until rewinding has completed. 	 * 	 * <p>This event can be used to track when playback enters or leaves unresponsive 	 * states such as in the middle of connecting, resizing, or rewinding. The  	 * <code>play()</code>, <code>pause()</code>, <code>stop()</code> and <code>seek()</code> 	 * methods queue the requests to be executed when the player enters	 * a responsive state.</p>	 *	 * <p>The <code>stateChange</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.STATE_CHANGE</code>.</p>	 *      * @see VideoEvent#STATE_CHANGE      *	 * @tiptext stateChange event	 * @eventType fl.video.VideoEvent.STATE_CHANGE	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("stateChange", type="fl.video.VideoEvent")] 

	/**
	 * The VideoPlayer class lets you create a video player with a slightly smaller SWF file	 * than if you used the FLVPlayback component. Unlike the FLVPlayback component, the 	 * VideoPlayer class does not let you include a skin or playback controls, and although 	 * you cannot find or seek to cue points, the <code>cuePoint</code> events will occur. 	 * The FLVPlayback class wraps the VideoPlayer class. 	 * 	 * <p>Use the FLVPlayback class in almost all cases because there is no 	 * functionality in the VideoPlayer class that cannot be accessed using the 	 * FLVPlayback class.</p>	 * 	 * <p>In addition, the FLVPlayback class automatically interfaces with the NCManager 	 * class to access streaming FLV files on a video server, such as Flash Media Server. You interact with the 	 * NCManager class when you set the <code>contentPath</code> property and when you pass 	 * a URL to the <code>play()</code> and <code>load()</code> methods. If you use the 	 * VideoPlayer class by itself, however, 	 * you must include the following statement in your ActionScript code to make sure 	 * the NCManager class is included:</p>	 * 	 * <listing>var _forceNCManager:fl.video.NCManager;</listing>	 * 	 * <p>The NCManager class implements an interface, INCManager, and it can be replaced	 * with a custom class for managing network communications that implements this interface.	 * If you do that, you also need to include the following statement, replacing NCManager 	 * with the name of the class you have provided:</p>	 * 	 * <listing>fl.video.VideoPlayer.iNCManagerClass = fl.video.NCManager;</listing>	 * 	 * <p>You do not need to add this statement if you are using the default NCManager class.</p>	 *      * <p><strong>Note</strong>: You also can set <code>iNCManagerClass</code> to replace the default 	 * fl.video.NCManager when using the FLVPlayback component.</p>	 * 	 * <p>To handle multiple streams for multiple bandwidths, NCManager supports a subset of 	 * SMIL.</p>	 *	 * @includeExample examples/VideoPlayerExample.as -noswf	 * 	 * @see NCManager 	 * @see FLVPlayback 	 * 	 * @tiptext	VideoPlayer class     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class VideoPlayer extends Video
	{
		/**
		 * @private
		 */
		static var BUFFER_EMPTY : String;
		/**
		 * @private
		 */
		static var BUFFER_FULL : String;
		/**
		 * @private
		 */
		static var BUFFER_FLUSH : String;
		/**
		 * @private
		 */
		protected var _state : String;
		/**
		 * @private
		 */
		local var _cachedState : String;
		/**
		 * @private
		 */
		local var _bufferState : String;
		/**
		 * @private
		 */
		local var _sawPlayStop : Boolean;
		/**
		 * @private
		 */
		local var _cachedPlayheadTime : Number;
		/**
		 * @private
		 */
		protected var _metadata : Object;
		/**
		 * @private
		 */
		protected var _registrationX : Number;
		/**
		 * @private
		 */
		protected var _registrationY : Number;
		/**
		 * @private
		 */
		protected var _registrationWidth : Number;
		/**
		 * @private
		 */
		protected var _registrationHeight : Number;
		/**
		 * @private
		 */
		local var _startingPlay : Boolean;
		/**
		 * @private
		 */
		local var _lastSeekTime : Number;
		/**
		 * @private
		 */
		local var _invalidSeekTime : Boolean;
		/**
		 * @private
		 */
		local var _invalidSeekRecovery : Boolean;
		/**
		 * @private
		 */
		local var _readyDispatched : Boolean;
		/**
		 * @private
		 */
		local var _autoResizeDone : Boolean;
		/**
		 * @private
		 */
		local var _lastUpdateTime : Number;
		/**
		 * @private
		 */
		local var lastUpdateTimeStuckCount : Number;
		/**
		 * @private
		 */
		local var _sawSeekNotify : Boolean;
		/**
		 * @private
		 */
		protected var _ncMgr : INCManager;
		/**
		 * To make all VideoPlayer objects use your 		 * custom class as the default INCManager implementation, set the 		 * <code>iNCManagerClass</code> property to the class object or string name		 * of your custom class.		 * The FLVPlayback class includes the definition		 * of the custom class; the video player does not.		 * 		 * @default "fl.video.NCManager" as a string		 * 		 * @tiptext	VideoPlayer class         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var iNCManagerClass : Object;
		/**
		 * Registers a custom class for the NetStream's client property. 	* By default, <code>fl.video.VideoPlayerClient</code> is used; this class handles the <code>onMetaData()</code> and <code>onCuePoint()</code> messages. 	* To add custom handling for other messages, such as <code>onImageData()</code> and <code>onTextData()</code>, 	* you can create your own class and set this property to the class.	*	* <p>The constructor for the class must take a <code>VideoPlayer</code> instance as its only parameter, and it must have a Boolean ready property. 	* This property should be <code>false</code> while the Client is still waiting for any data messages expected at the beginning of the file. 	* If the VideoPlayer does its resize autorewind before these messages are received, they may never be received. 	* By default, <code>fl.video.VideoPlayerClient</code> will return <code>true</code> from the ready property as soon as <code>onMetaData()</code> is called.</p>	* 	* <p>You can set the property to the class object itself (as in the example below) or the string name of the class:</p>	* <listing>        * import fl.video.*;        * VideoPlayer.netStreamClientClass = MyCustomClient;        * </listing>        * <p>Note that setting it to the string name of the class is not enough by itself to force the class to be compiled into the SWF. 	* The default value is the class object <code>fl.video.VideoPlayerClient</code>, not the string name.</p>	* <p>It is strongly recommended that any custom implementation should subclass <code>fl.video.VideoPlayer</code>. 	* To do otherwise risks breaking the metadata and cue point handling built into the VideoPlayer and the FLVPlayback component.</p>	*	* <p><strong>Player Version</strong>: Flash Player 9 <a target="mm_external" href="http://www.adobe.com/go/fp9_update3">Update 3</a>.</p>        *   * @throws fl.video.VideoError If this property is set to an invalid value, <code>VideoError.NETSTREAM_CLIENT_CLASS_UNSET</code> will be thrown.   * @langversion 3.0   * @internal Flash 9.0.xx.0   * @default fl.video.VideoPlayerClient
		 */
		public static var netStreamClientClass : Object;
		/**
		 * @private
		 */
		protected var _ns : NetStream;
		/**
		 * @private
		 */
		local var _currentPos : Number;
		/**
		 * @private
		 */
		local var _atEnd : Boolean;
		/**
		 * @private
		 */
		local var _atEndCheckPlayhead : Number;
		/**
		 * @private
		 */
		protected var _streamLength : Number;
		/**
		 * @private
		 */
		protected var _align : String;
		/**
		 * @private
		 */
		protected var _scaleMode : String;
		/**
		 * @private 		 * 		 * <p>If true, then video plays immediately, if false waits for		 * <code>play</code> to be called.  Set to true if stream is		 * loaded with call to <code>play()</code>, false if loaded		 * by call to <code>load()</code>.</p>		 *		 * <p>Even if <code>_autoPlay</code> is set to false, we will start		 * start loading the video after <code>initialize()</code> is		 * called.  In the case of Flash Media Server (FMS), this means creating the stream		 * and loading the first frame to display (and loading more if		 * <code>scaleMode</code> is		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or		 * <code>VideoScaleMode.NO_SCALE</code>).  In the case of HTTP		 * download, we will start downloading the stream and show the		 * first frame.</p>
		 */
		protected var _autoPlay : Boolean;
		/**
		 * @private
		 */
		protected var _autoRewind : Boolean;
		/**
		 * @private
		 */
		protected var _contentPath : String;
		/**
		 * @private
		 */
		protected var _bufferTime : Number;
		/**
		 * @private
		 */
		protected var _isLive : Boolean;
		/**
		 * @private
		 */
		protected var _volume : Number;
		/**
		 * @private
		 */
		protected var _soundTransform : SoundTransform;
		/**
		 * @private
		 */
		protected var __visible : Boolean;
		/**
		 * @private
		 */
		local var _hiddenForResize : Boolean;
		/**
		 * @private
		 */
		local var _hiddenForResizeMetadataDelay : Number;
		/**
		 * @private
		 */
		local var _resizeImmediatelyOnMetadata : Boolean;
		/**
		 * @private
		 */
		local var _hiddenRewindPlayheadTime : Number;
		/**
		 * @private
		 */
		protected var _videoWidth : int;
		/**
		 * @private
		 */
		protected var _videoHeight : int;
		/**
		 * @private
		 */
		local var _prevVideoWidth : int;
		/**
		 * @private
		 */
		local var _prevVideoHeight : int;
		/**
		 * @private
		 */
		local var oldBounds : Rectangle;
		/**
		 * @private
		 */
		local var oldRegistrationBounds : Rectangle;
		/**
		 * @private
		 */
		local var _updateTimeTimer : Timer;
		/**
		 * @private
		 */
		local var _updateProgressTimer : Timer;
		/**
		 * @private
		 */
		local var _idleTimeoutTimer : Timer;
		/**
		 * @private
		 */
		local var _autoResizeTimer : Timer;
		/**
		 * @private
		 */
		local var _rtmpDoStopAtEndTimer : Timer;
		/**
		 * @private
		 */
		local var _rtmpDoSeekTimer : Timer;
		/**
		 * @private
		 */
		local var _httpDoSeekTimer : Timer;
		/**
		 * @private
		 */
		local var _httpDoSeekCount : Number;
		/**
		 * @private
		 */
		local var _finishAutoResizeTimer : Timer;
		/**
		 * @private
		 */
		local var _delayedBufferingTimer : Timer;
		/**
		 * for progressive download auto start bandwidth detection		 * @private
		 */
		local var waitingForEnough : Boolean;
		/**
		 * for progressive download auto start bandwidth detection		 * @private
		 */
		local var baselineProgressTime : Number;
		/**
		 * for progressive download auto start bandwidth detection		 * @private
		 */
		local var startProgressTime : Number;
		/**
		 * for progressive download auto start bandwidth detection		 * @private
		 */
		local var totalDownloadTime : Number;
		/**
		 * for progressive download auto start bandwidth detection		 * @private
		 */
		local var totalProgressTime : Number;
		/**
		 * @private         * 		 * The default update-time interval is .25 seconds.		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const DEFAULT_UPDATE_TIME_INTERVAL : Number = 250;
		/**
		 * @private         *		 * The default update-progress interval is .25 seconds.		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const DEFAULT_UPDATE_PROGRESS_INTERVAL : Number = 250;
		/**
		 * @private         *		 * The default idle-timeout interval is five minutes.		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const DEFAULT_IDLE_TIMEOUT_INTERVAL : Number = 300000;
		/**
		 * @private
		 */
		static const AUTO_RESIZE_INTERVAL : Number = 100;
		/**
		 * @private
		 */
		static const DEFAULT_AUTO_RESIZE_PLAYHEAD_TIMEOUT : Number = .5;
		/**
		 * @private
		 */
		local var autoResizePlayheadTimeout : Number;
		/**
		 * @private
		 */
		static const DEFAULT_AUTO_RESIZE_METADATA_DELAY_MAX : Number = 5;
		/**
		 * @private
		 */
		local var autoResizeMetadataDelayMax : Number;
		/**
		 * @private
		 */
		static const FINISH_AUTO_RESIZE_INTERVAL : Number = 250;
		/**
		 * @private
		 */
		static const RTMP_DO_STOP_AT_END_INTERVAL : Number = 500;
		/**
		 * @private
		 */
		static const RTMP_DO_SEEK_INTERVAL : Number = 100;
		/**
		 * @private
		 */
		static const HTTP_DO_SEEK_INTERVAL : Number = 250;
		/**
		 * @private
		 */
		static const DEFAULT_HTTP_DO_SEEK_MAX_COUNT : Number = 4;
		/**
		 * @private
		 */
		local var httpDoSeekMaxCount : Number;
		/**
		 * @private
		 */
		static const HTTP_DELAYED_BUFFERING_INTERVAL : Number = 100;
		/**
		 * @private
		 */
		static const DEFAULT_LAST_UPDATE_TIME_STUCK_COUNT_MAX : int = 10;
		/**
		 * @private
		 */
		local var lastUpdateTimeStuckCountMax : int;
		/**
		 * @private
		 */
		local var _cmdQueue : Array;

		/**
		 * A number that is the horizontal scale. 		 *		 * @default 1         * @see #setScale()		 * @see #scaleY         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scaleX (xs:Number) : void;
		/**
		 * A number that is the vertical scale. 		 *		 * @default 1         * @see #setScale()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scaleY (ys:Number) : void;
		/**
		 * A number that specifies the horizontal position (in pixels) of the video player.		 * 		 * <p>Setting the <code>x</code> property also affects the <code>registrationX</code> property. 		 * When either the <code>x</code> or <code>registrationX</code> property is set,		 * the second property is changed to maintain its offset from the		 * first. For example, if <code>x</code> = 10 and <code>registrationX</code> = 20, setting 		 * <code>x</code> = 110 also sets <code>registrationX</code> = 120.</p>		 *         * @see #registrationX         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set x (x:Number) : void;
		/**
		 * A number that specifies the vertical position (in pixels) of the video player.		 * 		 * <p>Setting the <code>y</code> property also affects the <code>registrationY</code> property. 		 * When either the <code>y</code> or <code>registrationY</code> property is set,		 * the second property is changed to maintain its offset from the		 * first. For example, if <code>y</code> = 10 and <code>registrationY</code> = 20, setting 		 * <code>y</code> = 110 also sets <code>registrationY</code> = 120.</p>		 *         * @see #registrationY         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set y (y:Number) : void;
		/**
		 * A number that specifies the width of the VideoPlayer instance on the Stage.		 * 		 * <p><strong>Note</strong>: The <code>flash.media.Video.width</code> property is similar to the 		 *  <code>fl.video.VideoPlayer.videoWidth</code> property.</p>         *		 *		 * @see #setSize()         * @see flash.media.Video#videoWidth Video.videoWidth		 * @see #videoWidth VideoPlayer.videoWidth         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set width (w:Number) : void;
		/**
		 * A number that specifies the height of the VideoPlayer instance (in pixels). 		 * 		 * <p><strong>Note</strong>: Do not confuse this property with the 		 * <code>flash.media.Video.height</code> property which is similar to the 		 * <code>fl.video.VideoPlayer.videoHeight</code> property.</p>         *		 *		 * @see #setSize()         * @see flash.media.Video#videoHeight Video.videoHeight		 * @see #videoHeight VideoPlayer.videoHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set height (h:Number) : void;
		/**
		 * The x coordinate used to align the video content when 		 * autoresizing. Do not confuse with the <code>x</code> property,		 * which reflects the actual location of the video content.		 *		 * <p>Example 1, load an 80x80 FLV file with the following settings:		 * <ul>		 *     <li><code>registrationX</code> = 100</li> 		 *     <li><code>registrationY</code> = 100</li>		 *     <li><code>registrationWidth</code> = 60</li> 		 *     <li><code>registrationHeight</code> = 40</li>		 *     <li><code>align</code> = <code>VideoAlign.CENTER</code></li> 		 *     <li><code>scaleMode</code> = <code>VideoScaleMode.NO_SCALE</code></li>		 * </ul>		 * 		 * 		 * Then, after automatic resizing, you get the following:		 * <ul>         *     <li><code>x</code> = 90</li>         *     <li><code>y</code> = 80</li>         *     <li><code>width</code> = 80</li>         *     <li><code>height</code> = 80</li>  		 * </ul>		 * </p>		 * 		 * <p>Example 2, load the same scenario as example 1 but with the following settings:		 * <ul>		 *     <li><code>scaleMode</code> = <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code></li>		 * </ul>		 * 		 * You get the following:		 * <ul>		 * <li><code>x</code> = 110</li>		 * <li><code>y</code> = 100</li> 		 * <li><code>width</code> = 40</li> 		 * <li><code>height</code> = 80</li>		 * </ul>		 * </p>		 * 		 * <p>Example 3, load the same scenario as example 2 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.EXACT_FIT</code></li>		 * </ul>		 * 		 * You get all the same values as the registration values:		 * <ul>		 * <li><code>x</code> = 100</li> 		 * <li><code>y</code> = 100</li>		 * <li><code>width</code> = 60</li> 		 * <li><code>height</code> = 40</li>		 * </ul>		 * </p>		 * 		 * <p>When either the <code>x</code> or <code>registrationX</code>		 * property is set, the second property is changed to maintain its		 * offset from the first.  For example, if <code>x</code> = 10 and 		 * <code>registrationX</code> = 20, setting <code>x</code> = 110 also 		 * sets <code>registrationX</code> = 120.</p>		 *		 * @see #registrationY		 * @see #registrationWidth		 * @see #registrationHeight         * @see VideoAlign         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationX () : Number;
		/**
		 * @private (setter)
		 */
		public function set registrationX (x:Number) : void;
		/**
		 * The y coordinate used to align the video content when 		 * autoresizing. Do not confuse with the <code>y</code> property,		 * which reflects the actual location of the video content.		 *		 * <p>Example 1, load an 80x80 FLV file with the following settings:		 * <ul>		 * <li><code>registrationX</code> = 100</li> 		 * <li><code>registrationY</code> = 100</li>		 * <li><code>registrationWidth</code> = 60</li> 		 * <li><code>registrationHeight</code> = 40</li>		 * <li><code>align</code> = <code>VideoAlign.CENTER</code></li> 		 * <li><code>scaleMode</code> = <code>VideoScaleMode.NO_SCALE</code></li>		 * </ul>		 * 		 * 		 * Then, after automatic resizing, you get the following:		 * <ul>		 * <li><code>x</code> = 90</li>		 * <li><code>y</code> = 80</li>		 * <li><code>width</code> = 80</li>		 * <li><code>height</code> = 80</li>  		 * </ul>		 * </p>		 * 		 * <p>Example 2, load the same scenario as example 1 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code></li>		 * </ul>		 * 		 * You get the following:		 * <ul>		 * <li><code>x</code> = 110</li>		 * <li><code>y</code> = 100</li> 		 * <li><code>width</code> = 40</li> 		 * <li><code>height</code> = 80</li>		 * </ul>		 * </p>		 * 		 * <p>Example 3, load the same scenario as example 2 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.EXACT_FIT</code></li>		 * </ul>		 * 		 * You get all the same values as the registration values:		 * <ul>		 * <li><code>x</code> = 100</li> 		 * <li><code>y</code> = 100</li>		 * <li><code>width</code> = 60</li> 		 * <li><code>height</code> = 40</li>		 * </ul>		 * </p>		 * 		 * <p>When either the <code>y</code> or <code>registrationY</code>		 * property is set, the second property is changed to maintain its		 * offset from the first.  For example, if <code>y</code> = 10 and 		 * <code>registrationY</code> = 20, setting <code>y</code> = 110 also 		 * sets <code>registrationY</code> = 120.</p>		 *		 * @see #registrationX		 * @see #registrationWidth		 * @see #registrationHeight         * @see VideoAlign         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationY () : Number;
		/**
		 * @private (setter)
		 */
		public function set registrationY (y:Number) : void;
		/**
		 * The width used to align the video content when autoresizing.		 * Do not confuse the <code>registrationWidth</code> property 		 * with the <code>width</code> property. The <code>width</code>		 * property reflects the actual width of the video content.		 *		 * <p>Example 1, load an 80x80 FLV file with the following settings:		 * <ul>		 * <li><code>registrationX</code> = 100</li> 		 * <li><code>registrationY</code> = 100</li>		 * <li><code>registrationWidth</code> = 60</li> 		 * <li><code>registrationHeight</code> = 40</li>		 * <li><code>align</code> = <code>VideoAlign.CENTER</code></li> 		 * <li><code>scaleMode</code> = <code>VideoScaleMode.NO_SCALE</code></li>		 * </ul>		 * 		 * 		 * Then, after automatic resizing, you get the following:		 * <ul>		 * <li><code>x</code> = 90</li>		 * <li><code>y</code> = 80</li>		 * <li><code>width</code> = 80</li>		 * <li><code>height</code> = 80</li>  		 * </ul>		 * </p>		 * 		 * <p>Example 2, load the same scenario as example 1 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code></li>		 * </ul>		 * 		 * You get the following:		 * <ul>		 * <li><code>x</code> = 110</li>		 * <li><code>y</code> = 100</li> 		 * <li><code>width</code> = 40</li> 		 * <li><code>height</code> = 80</li>		 * </ul>		 * </p>		 * 		 * <p>Example 3, load the same scenario as example 2 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.EXACT_FIT</code></li>		 * </ul>		 * 		 * You get all the same values as the registration values:		 * <ul>		 * <li><code>x</code> = 100</li> 		 * <li><code>y</code> = 100</li>		 * <li><code>width</code> = 60</li> 		 * <li><code>height</code> = 40</li>		 * </ul>		 * </p>		 * 		 * <p>Setting the <code>registrationWidth</code> property is		 * equivalent to setting the <code>width</code> property and vice		 * versa. Setting any other property or calling any method that		 * alters the width, such as <code>scaleX</code>,		 * <code>setSize</code>, and <code>setScale</code>, also keeps		 * <code>width</code> and <code>registrationWidth</code> in sync.</p>		 *		 * @see #registrationX		 * @see #registrationY		 * @see #registrationHeight         * @see VideoAlign         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationWidth () : Number;
		/**
		 * @private (setter)
		 */
		public function set registrationWidth (w:Number) : void;
		/**
		 * The height used to align the video content when autoresizing.		 * Do not confuse the <code>registrationHeight</code> property 		 * with the <code>height</code> property. The <code>height</code>		 * property reflects the actual height of the video content.		 *		 * <p>Example 1, load an 80x80 FLV file with the following settings:		 * <ul>		 * <li><code>registrationX</code> = 100</li> 		 * <li><code>registrationY</code> = 100</li>		 * <li><code>registrationWidth</code> = 60</li> 		 * <li><code>registrationHeight</code> = 40</li>		 * <li><code>align</code> = <code>VideoAlign.CENTER</code></li> 		 * <li><code>scaleMode</code> = <code>VideoScaleMode.NO_SCALE</code></li>		 * </ul>		 * 		 * 		 * Then, after automatic resizing, you get the following:		 * <ul>		 * <li><code>x</code> = 90</li>		 * <li><code>y</code> = 80</li>		 * <li><code>width</code> = 80</li>		 * <li><code>height</code> = 80</li>  		 * </ul>		 * </p>		 * 		 * <p>Example 2, load the same scenario as example 1 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code></li>		 * </ul>		 * 		 * You get the following:		 * <ul>		 * <li><code>x</code> = 110</li>		 * <li><code>y</code> = 100</li> 		 * <li><code>width</code> = 40</li> 		 * <li><code>height</code> = 80</li>		 * </ul>		 * </p>		 * 		 * <p>Example 3, load the same scenario as example 2 but with the following settings:		 * <ul>		 * <li><code>scaleMode</code> = <code>VideoScaleMode.EXACT_FIT</code></li>		 * </ul>		 * 		 * You get all the same values as the registration values:		 * <ul>		 * <li><code>x</code> = 100</li> 		 * <li><code>y</code> = 100</li>		 * <li><code>width</code> = 60</li> 		 * <li><code>height</code> = 40</li>		 * </ul>		 * </p>		 * 		 * <p>Setting the <code>registrationHeight</code> property is		 * equivalent to setting the <code>height</code> property and vice		 * versa. Setting any other property or calling any method that		 * alters the height, such as <code>scaleX</code>,		 * <code>setSize</code>, and <code>setScale</code>, setting keeps		 * <code>height</code> and <code>registrationHeight</code> in sync.</p>		 *		 * @see #registrationX		 * @see #registrationY		 * @see #registrationWidth         * @see VideoAlign         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationHeight () : Number;
		/**
		 * @private (setter)
		 */
		public function set registrationHeight (h:Number) : void;
		/**
		 * The source width of the loaded FLV file. This property returns		 * -1 if no information is available yet.		 * 		 *         * @see #width         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get videoWidth () : int;
		/**
		 * The source width of the loaded FLV file. This property returns		 * -1 if no information is available yet.		 *         * @see #height         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get videoHeight () : int;
		/**
		 * A Boolean value that, if <code>true</code>, makes the VideoPlayer instance visible. 		 * If <code>false</code>, it makes the instance invisible. 		 * 		 * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get visible () : Boolean;
		/**
		 * @private (setter)
		 */
		public function set visible (v:Boolean) : void;
		/**
		 * Specifies how the video is displayed relative to the		 * <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code> and		 * <code>registrationHeight</code> properties. The <code>align</code> property does 		 * this autolayout when the <code>scaleMode</code> property is set to		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or		 * <code>VideoScaleMode.NO_SCALE</code>. Changing this property		 * after an FLV file is loaded causes an automatic layout to		 * start immediately.		 * Values come		 * from the VideoAlign class.         *         * @default VideoAlign.CENTER		 *		 * @see #registrationX		 * @see #registrationY		 * @see #registrationWidth		 * @see #registrationHeight		 * @see #scaleMode         * @see VideoAlign         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get align () : String;
		/**
		 * @private (setter)
		 */
		public function set align (s:String) : void;
		/**
		 * Specifies how the video resizes after loading.  If set to		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code>, maintains the		 * video aspect ratio within the rectangle defined by		 * <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code> and		 * <code>registrationHeight</code>.  If set to		 * <code>VideoScaleMode.NO_SCALE</code>, causes the video to size automatically		 * to the dimensions of the source FLV file.  If set to		 * <code>VideoScaleMode.EXACT_FIT</code>, causes the dimensions of		 * the source FLV file to be ignored and the video is stretched to		 * fit the rectangle defined by		 * <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code> and		 * <code>registrationHeight</code>. If this is set		 * after an FLV file has been loaded an automatic layout will start		 * immediately.  Values come from		 * <code>VideoScaleMode</code>.		 *		 * @see VideoScaleMode		 * @default VideoScaleMode.MAINTAIN_ASPECT_RATIO                 *                 * @langversion 3.0                 * @playerversion Flash 9.0.28.0
		 */
		public function get scaleMode () : String;
		/**
		 * @private (setter)
		 */
		public function set scaleMode (s:String) : void;
		/**
		 * A Boolean value that, if <code>true</code>, causes the FLV file to rewind to 		 * Frame 1 when play stops, either because the player reached the end of the 		 * stream or the <code>stop()</code> method was called. This property is 		 * meaningless for live streams.         *          * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoRewind () : Boolean;
		/**
		 * @private (setter)
		 */
		public function set autoRewind (flag:Boolean) : void;
		/**
		 * A number that is the current playhead time or position, measured in seconds, 		 * which can be a fractional value. Setting this property triggers a seek and 		 * has all the restrictions of a seek.		 * 		 * <p>When the playhead time changes, which includes once every .25 seconds 		 * while the FLV file plays, the component dispatches the <code>playheadUpdate</code>		 * event.</p>		 * 		 * <p>For several reasons, the <code>playheadTime</code> property might not have the expected 		 * value immediately after calling one of the seek methods or setting <code>playheadTime</code> 		 * to cause seeking. First, for a progressive download, you can seek only to a 		 * keyframe, so a seek takes you to the time of the first keyframe after the 		 * specified time. (When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.) Second, 		 * seeking is asynchronous, so if you call a seek method or set the 		 * playheadTime property, <code>playheadTime</code> does not update immediately. 		 * To obtain the time after the seek is complete, listen for the <code>seek</code> event, 		 * which does not fire until the <code>playheadTime</code> property has updated.</p>		 *		 * @tiptext Current position of the playhead in seconds		 * @see #seek()         * @see FLVPlayback#playheadTime		 *          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playheadTime () : Number;
		/**
		 * @private (setter)
		 */
		public function set playheadTime (position:Number) : void;
		/**
		 * A string that specifies the URL of the FLV file to stream and how to stream it.		 * The URL can be an HTTP URL to an FLV file, an RTMP URL to a stream, or an 		 * HTTP URL to an XML file.		 *		 * <p>If you set this property through the Component inspector or the Property inspector, 		 * the FLV file begins loading and playing at the next <code>enterFrame</code> event.		 * The delay provides time to set the <code>isLive</code>, <code>autoPlay</code>, 		 * and <code>cuePoints</code> properties, 		 * among others, which affect loading. It also allows ActionScript that is placed 		 * on the first frame to affect the FLVPlayback component before it starts playing.</p>		 *		 * <p>If you set this property through ActionScript, it immediately calls the		 * <code>VideoPlayer.load()</code> method when the <code>autoPlay</code> property is		 * set to <code>false</code>. Alternatively, it calls the <code>VideoPlayer.play()</code> method when		 * the <code>autoPlay</code> property is set to <code>true</code>.  The <code>autoPlay</code>, 		 * <code>totalTime</code>, and <code>isLive</code> properties affect how the new FLV file is 		 * loaded, so if you set these properties, you must set them before setting the		 * <code>source</code> property.</p>		 * 		 * <p>Set the <code>autoPlay</code> property to <code>false</code> to prevent the new 		 * FLV file from playing automatically.</p>		 * 		 * @see FLVPlayback#autoPlay 		 * @see #isLive		 * @see #totalTime		 * @see #load()		 * @see #play()		 * @see FLVPlayback#load()         * @see FLVPlayback#play()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : String;
		/**
		 * A number in the range of 0 to 1 that indicates the volume control setting. 		 * @default 1		 *		 * @tiptext The volume setting in value range from 0 to 1.		 *          * @see #soundTransform         *         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get volume () : Number;
		/**
		 * @private (setter)
		 */
		public function set volume (aVol:Number) : void;
		/**
		 * Provides direct access to the		 * <code>NetStream.soundTransform</code> property to expose		 * more sound control. Set the property to change the settings;		 * use the getter accessor method of the property to retrieve 		 * the current settings.		 *         * @see #volume         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get soundTransform () : SoundTransform;
		/**
		 * @private (setter)
		 */
		public function set soundTransform (st:SoundTransform) : void;
		/**
		 * A Boolean value that is <code>true</code> if the FLV file is streaming from Flash Media Server (FMS) using RTMP. 		 * Its value is <code>false</code> for any other FLV file source. 		 *         * @see FLVPlayback#isRTMP         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get isRTMP () : Boolean;
		/**
		 * A Boolean value that is <code>true</code> if the video stream is live. This property 		 * is effective only when streaming from a video server, such as Flash Media Server or other Flash Video Streaming Service. The value of this 		 * property is ignored for an HTTP download.		 * 		 * <p>Set the <code>isLive</code> property to <code>false</code> when sending a prerecorded video 		 * stream to the video player and to <code>true</code> when sending real-time data 		 * such as a live broadcast. For better performance when you set 		 * the <code>isLive</code> property to <code>false</code>, do not set the 		 * <code>bufferTime</code> property to <code>0</code>.</p>		 * 		 * @see #bufferTime           * @see FLVPlayback#isLive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get isLive () : Boolean;
		/**
		 * A string that specifies the state of the component. This property is set by the 		 * <code>load()</code>, <code>play()</code>, <code>stop()</code>, <code>pause()</code>, 		 * and <code>seek()</code> methods. 		 * 		 * <p>The possible values for the state property are: <code>buffering</code>, 		 * <code>connectionError</code>, <code>disconnected</code>, <code>loading</code>, 		 * <code>paused</code>, <code>playing</code>, <code>rewinding</code>, <code>seeking</code>, 		 * and <code>stopped</code>. You can use the FLVPlayback class properties to test for 		 * these states. </p>		 *		 * @see VideoState#DISCONNECTED		 * @see VideoState#STOPPED		 * @see VideoState#PLAYING		 * @see VideoState#PAUSED		 * @see VideoState#BUFFERING		 * @see VideoState#LOADING		 * @see VideoState#CONNECTION_ERROR		 * @see VideoState#REWINDING         * @see VideoState#SEEKING         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get state () : String;
		/**
		 * A Boolean value that is <code>true</code> if the state is responsive. If the state is 		 * unresponsive, calls to the <code>play()</code>, <code>load()</code>, <code>stop()</code>, 		 * <code>pause()</code>, and <code>seek()</code>		 * methods are queued and executed later, when the state changes to a 		 * responsive one. Because these calls are queued and executed later, 		 * it is usually not necessary to track the value of the <code>stateResponsive </code>		 * property. The responsive states are: 		 * <code>stopped</code>, <code>playing</code>, <code>paused</code>, and <code>buffering</code>. 		 *         * @see FLVPlayback#stateResponsive		 * @see VideoState#DISCONNECTED		 * @see VideoState#STOPPED		 * @see VideoState#PLAYING		 * @see VideoState#PAUSED		 * @see VideoState#LOADING		 * @see VideoState#RESIZING		 * @see VideoState#CONNECTION_ERROR         * @see VideoState#REWINDING         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get stateResponsive () : Boolean;
		/**
		 * A number that indicates the extent of downloading, in number of bytes, for an 		 * HTTP download.  Returns 0 when there		 * is no stream, when the stream is from Flash Media Server (FMS), or if the information		 * is not yet available. The returned value is useful only for an HTTP download.		 *		 * @tiptext Number of bytes already loaded         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesLoaded () : uint;
		/**
		 * A number that specifies the total number of bytes downloaded for an HTTP download.  		 * Returns -1 when there is no stream, when the stream is from Flash Media Server (FMS), or if 		 * the information is not yet available. The returned value is useful only 		 * for an HTTP download. 		 *		 * @tiptext Number of bytes to be loaded         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesTotal () : uint;
		/**
		 * A number that is the total playing time for the video in seconds.		 *		 * <p>When streaming from Flash Media Server (FMS) and using the default		 * <code>NCManager</code>, this value is determined		 * automatically by server-side application programming interfaces (APIs), and that value 		 * overrides anything set through this property or gathered		 * from metadata. The property is ready for reading when the		 * <code>stopped</code> or <code>playing</code> state is reached after setting the		 * <code>source</code> property. This property is meaningless for live streams		 * from an FMS.</p>		 *		 * <p>With an HTTP download, the value is determined		 * automatically if the FLV file has metadata embedded; otherwise,		 * set it explicitly, or it will be NaN.  If you set it		 * explicitly, the metadata value in the stream is		 * ignored.</p>		 *		 * <p>When you set this property, the value takes effect for the next		 * FLV file that is loaded by setting <code>source</code>. It has no effect		 * on an FLV file that has already loaded.  Also, this property does not return 		 * the new value passed in until an FLV file is loaded.</p>		 *		 * <p>Playback still works if this property is never set (either		 * explicitly or automatically), but it can cause problems		 * with seek controls.</p>		 *		 * <p>Unless set explicitly, the value will be NaN until it is set to a valid value from metadata.</p>		 *		 * @see #source		 * @tiptext The total length of the FLV in seconds         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get totalTime () : Number;
		/**
		 * A number that specifies the number of seconds to buffer in memory before 		 * beginning to play a video stream. For FLV files streaming over RTMP, 		 * which are not downloaded and buffer only in memory, it can be important 		 * to increase this setting from the default value of 0.1. For a progressively 		 * downloaded FLV file over HTTP, there is little benefit to increasing this 		 * value although it could improve viewing a high-quality video on an older, 		 * slower computer.		 * 		 * <p>For prerecorded (not live) video, do not set the <code>bufferTime</code>          * property to <code>0</code>; use the default buffer time or increase the buffer          * time.</p>		 * 		 * <p>This property does not specify the amount of the FLV file to download before 		 * starting playback.</p>		 *          * @see FLVPlayback#bufferTime		 * @see #isLive         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bufferTime () : Number;
		/**
		 * @private (setter)
		 */
		public function set bufferTime (aTime:Number) : void;
		/**
		 * The amount of time, in milliseconds, before Flash terminates an idle connection 		 * to a video server, such as Flash Media Server, because playing paused or stopped. 		 * This property has no effect on an 		 * FLV file downloading over HTTP.		 * 		 * <p>If this property is set when a video stream is already idle, it restarts the 		 * timeout period with the new value.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get idleTimeout () : Number;
		/**
		 * @private (setter)
		 */
		public function set idleTimeout (aTime:Number) : void;
		/**
		 * A number that is the amount of time, in milliseconds, between each 		 * <code>playheadUpdate</code> event. Setting this property while the FLV file is 		 * playing restarts the timer. 		 * 		 * <p>Because ActionScript cue points start on playhead updates, lowering 		 * the value of the <code>playheadUpdateInterval</code> property can increase the accuracy 		 * of ActionScript cue points.</p>		 * 		 * <p>Because the playhead update interval is set by a call to the global 		 * <code>setInterval()</code> method, the update cannot fire more frequently than the 		 * SWF file frame rate, as with any interval that is set this way. 		 * So, as an example, for the default frame rate of 12 frames per second, 		 * the lowest effective interval that you can create is approximately 		 * 83 milliseconds, or one second (1000 milliseconds) divided by 12.</p>		 *         * @see FLVPlayback#playheadUpdateInterval         * @default 250         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playheadUpdateInterval () : Number;
		/**
		 * @private (setter)
		 */
		public function set playheadUpdateInterval (aTime:Number) : void;
		/**
		 * A number that is the amount of time, in milliseconds, between each 		 * <code>progress</code> event. If you set this property while the video 		 * stream is playing, the timer restarts.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get progressInterval () : Number;
		/**
		 * @private (setter)
		 */
		public function set progressInterval (aTime:Number) : void;
		/**
		 * An INCManager object that provides access to an instance of the class implementing 		 * <code>INCManager</code>, which is an interface to the NCManager class.		 * 		 * <p>You can use this property to implement a custom INCManager that requires 		 * custom initialization.</p>		 *         * @see FLVPlayback#ncMgr         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get ncMgr () : INCManager;
		/**
		 * Allows direct access to the NetConnection instance created by the video player.           *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get netConnection () : NetConnection;
		/**
		 * Allows direct access to the NetStream instance created by the video player.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get netStream () : NetStream;
		/**
		 * * An object that is a metadata information packet that is received from a call to 		 * the <code>NetSteam.onMetaData()</code> callback method, if available.  		 * Ready when the <code>metadataReceived</code> event is dispatched.		 * 		 * <p>If the FLV file is encoded with the Flash 8 encoder, the <code>metadata</code> 		 * property contains the following information. Older FLV files contain 		 * only the <code>height</code>, <code>width</code>, and <code>duration</code> values.</p>		 * 		 * <table class="innertable" width="100%">		 * 	<tr><th><b>Parameter</b></th><th><b>Description</b></th></tr>		 * 		<tr><td><code>canSeekToEnd</code></td><td>A Boolean value that is <code>true</code> if the FLV file is encoded with a keyframe on the last frame that allows seeking to the end of a progressive download movie clip. It is <code>false</code> if the FLV file is not encoded with a keyframe on the last frame.</td></tr>		 * 		<tr><td><code>cuePoints</code></td><td>An array of objects, one for each cue point embedded in the FLV file. Value is undefined if the FLV file does not contain any cue points. Each object has the following properties:	     *   			 * 			<ul>		 * 				<li><code>type</code>&#x2014;A string that specifies the type of cue point as either "navigation" or "event".</li>		 * 				<li><code>name</code>&#x2014;A string that is the name of the cue point.</li>		 * 				<li><code>time</code>&#x2014;A number that is the time of the cue point in seconds with a precision of three decimal places (milliseconds).</li>		 * 				<li><code>parameters</code>&#x2014;An optional object that has name-value pairs that are designated by the user when creating the cue points.</li>		 * 			</ul>		 * 		</td></tr>		 * <tr><td><code>audiocodecid</code></td><td>A number that indicates the audio codec (code/decode technique) that was used.</td></tr>		 * <tr><td><code>audiodelay</code></td><td> A number that represents time <code>0</code> in the source file from which the FLV file was encoded. 		 * <p>Video content is delayed for the short period of time that is required to synchronize the audio. For example, if the <code>audiodelay</code> value is <code>.038</code>, the video that started at time <code>0</code> in the source file starts at time <code>.038</code> in the FLV file.</p> 		 * <p>Note that the FLVPlayback and VideoPlayer classes compensate for this delay in their time settings. This means that you can continue to use the time settings that you used in your the source file.</p></td></tr> 		 * <tr><td><code>audiodatarate</code></td><td>A number that is the kilobytes per second of audio.</td></tr> 		 * <tr><td><code>videocodecid</code></td><td>A number that is the codec version that was used to encode the video.</td></tr> 		 * <tr><td><code>framerate</code></td><td>A number that is the frame rate of the FLV file.</td></tr> 		 * <tr><td><code>videodatarate</code></td><td>A number that is the video data rate of the FLV file.</td></tr>		 * <tr><td><code>height</code></td><td>A number that is the height of the FLV file.</td></tr> 		 * <tr><td><code>width</code></td><td>A number that is the width of the FLV file.</td></tr> 		 * <tr><td><code>duration</code></td><td>A number that specifies the duration of the FLV file in seconds.</td></tr>		 * </table>		 *         * @see FLVPlayback#metadata         *		 *		 * @see #load()         * @see #play()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get metadata () : Object;

		/**
		 * Creates a VideoPlayer object with a specified width and height.		 * 		 * @param width The width of the video player in pixels.		 * @param height The height of the video player in pixels.		 *		 * @see INCManager 		 * @see NCManager 		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function VideoPlayer (width:int = 320, height:int = 240);
		/**
		 * Sets the <code>width</code> and <code>height</code> properties simultaneously.                   * Setting the <code>width</code> or <code>height</code> individually                 * triggers two <code>autolayout</code> events whereas calling the                  * <code>setSize()</code> method causes only one <code>autolayout</code> event. 		 *		 * <p>If the <code>scaleMode</code> property is		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or		 * <code>VideoScaleMode.NO_SCALE</code> then calling this method		 * triggers an immediate <code>autolayout</code> event.</p>		 *		 * @param width The width of the video player.		 * @param height The height of the video player.		 * @see #width 		 * @see #height 		 * @see VideoScaleMode#MAINTAIN_ASPECT_RATIO		 * @see VideoScaleMode#NO_SCALE		 * 		 * @langversion 3.0                 * @playerversion Flash 9.0.28.0
		 */
		public function setSize (width:Number, height:Number) : void;
		/**
		 * Sets the <code>scaleX</code> and <code>scaleY</code> properties simultaneously.                   * Setting the <code>scaleX</code> or <code>scaleY</code> individually                 * triggers two <code>autolayout</code> events whereas calling the                  * <code>setScale()</code> method causes only one <code>autolayout</code> event.                  * 		 * <p>If the <code>scaleMode</code> property is		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or		 * <code>VideoScaleMode.NO_SCALE</code>, calling this method		 * causes an immediate <code>autolayout</code> event.</p>		 *		 * @param scaleX A number that represents the horizontal scale.		 * @param scaleY A number that represents the vertical scale.		 * 		 * @see #scaleX 		 * @see #scaleY 		 * @see VideoScaleMode#MAINTAIN_ASPECT_RATIO	         * @see VideoScaleMode#NO_SCALE		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setScale (scaleX:Number, scaleY:Number) : void;
		/**
		 * Causes the video to play. Can be called while the video is		 * paused or stopped, or while the video is already playing.  Call this		 * method with no arguments to play an already loaded video or pass		 * in a URL to load a new stream.		 *		 * <p>If the player is in an unresponsive state, queues the request.</p>		 *		 * <p>Throws an exception if called with no arguments at a time when no stream		 * is connected. Use the <code>stateChange</code> event and the		 * <code>connected</code> property to determine when it is		 * safe to call this method.</p>		 *		 * @param url Pass in a URL string if you want to load and play a		 * new FLV file. If you have already loaded an FLV file and want to continue		 * playing it, pass in <code>null</code>. 		 * 		 * @param totalTime Pass in the length of the FLV file. Pass in <code>0</code> or <code>NaN</code>		 * to automatically detect the length from metadata, server, or		 * XML. If the <code>INCManager.streamLength</code> property is not <code>0</code>, 		 * <code>null</code>, or undefined when the <code>ncConnected</code> property is called,		 * that value takes precedence over this one.  		 * 		 * @param isLive Pass in <code>true</code> if streaming a live feed from Flash Media Server (FMS). 		 * 		 * @tiptext play method		 * 		 * @see #stateResponsive 		 * @see #load() 		 * 		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function play (url:String = null, totalTime:Number = NaN, isLive:Boolean = false) : void;
		/**
		 * Plays the FLV file when enough of it has downloaded. If the FLV file has downloaded 		 * or you are streaming from Flash Media Server (FMS), then calling the <code>playWhenEnoughDownloaded()</code>		 * method is identical to the <code>play()</code> method with no parameters. Calling 		 * this method does not pause playback, so in many cases, you may want to call 		 * the <code>pause()</code> method before you call this method.		 * 		 * @tiptext playWhenEnoughDownloaded method		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function playWhenEnoughDownloaded () : void;
		/**
		 * Similar to the <code>play()</code> method, but causes the FLV file 		 * to load without playing. Autoresizing occurs if appropriate, and the first		 * frame of the FLV file is shown. 		 * After initial loading and autolayout, the state is <code>VideoState.PAUSED</code>.		 *		 * <p>This method takes the same parameters as the <code>play()</code> method, 		 * but you cannot call the <code>load()</code> method without a URL.		 * If you do, an error is thrown. If the video player is in an unresponsive state, 		 * the <code>load()</code> method queues the request.</p>		 *		 * @param url A URL string for the FLV file that you want to load. If no value is 		 * passed for URL, an error is thrown with the message 		 * <code>null URL sent to VideoPlayer.load</code>.		 * 		 * @param totalTime The length of an FLV file. Pass in 0, <code>null</code>, or		 * undefined to automatically detect length from metadata, server, or XML. 		 *  		 * @param isLive The value is <code>true</code> if you stream a live feed from Flash Media Server (FMS). 		 * 		 * @see #play() 		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function load (url:String, totalTime:Number = NaN, isLive:Boolean = false) : void;
		/**
		 * Does loading work for play and load.		 *		 * @private
		 */
		function _load (url:String, totalTime:Number, isLive:Boolean) : void;
		/**
		 * Pauses video playback.  If video is paused or stopped, has		 * no effect.  To start playback again, call <code>play()</code>.		 * Takes no parameters.		 *		 * <p>If player is in an unresponsive state, the <code>pause()</code> method		 * queues the request.</p>		 *		 * <p>Throws an exception if called when no stream is         * connected.  Use the <code>stateChange</code> event and		 * <code>connected</code> property to determine when it is		 * safe to call this method.</p>		 *         * <p>If the player is in a stopped state, a call to the <code>pause()</code> 		 * method has no effect and the player remains in a stopped state.</p>		 *		 * @see #stateResponsive         * @see #play()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function pause () : void;
		/**
		 * Stops video playback.  If <code>autoRewind</code> is set to		 * <code>true</code>, rewinds to first frame.  If video is already		 * stopped, has no effect.  To start playback again, call		 * <code>play()</code>.  Takes no parameters.		 *		 * <p>If player is in an unresponsive state, queues the request.</p>		 *		 * <p>Throws an exception if called when no stream is         * connected.  Use the <code>stateChange</code> event and		 * <code>connected</code> property to determine when it is		 * safe to call this method.</p>		 *		 * @see #stateResponsive		 * @see #autoRewind         * @see #play()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function stop () : void;
		/**
		 * Seeks to a given time in the file, specified in seconds, with a precision of three		 * decimal places (milliseconds). If a video is playing, the video continues to play		 * from that point. If a video is paused, the video seeks to that point and remains paused. 		 * If a video is stopped, the video seeks 		 * to that point and enters the paused state. Has no effect with live streams.		 *         * <p>The <code>playheadTime</code> property might not have the expected value 		 * immediately after you call one of the seek methods or set  		 * <code>playheadTime</code> to cause seeking. For a progressive download,		 * you can seek only to a keyframe; therefore, a seek takes you to the 		 * time of the first keyframe after the specified time.</p>		 * 		 * <p><strong>Note</strong>: When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.</p>		 *		 * <p>Seeking is asynchronous, so if you call a seek method or set the 		 * <code>playheadTime</code> property, <code>playheadTime</code> does not update immediately. 		 * To obtain the time after the seek is complete, listen for the <code>seek</code> event, 		 * which does not start until the <code>playheadTime</code> property is updated.</p>		 *		 * <p>Throws an exception if called when no stream is         * connected.  Use the <code>stateChange</code> event and		 * the <code>connected</code> property to determine when it is		 * safe to call this method.</p>		 *		 * @param time A number that specifies the time, in seconds, at which to place the 		 * playhead.		 * @throws fl.video.VideoError If time is &lt; 0 or <code>NaN</code>.         * @see #stateResponsive         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seek (time:Number) : void;
		/**
		 * Forces the video stream and Flash Media Server connection to close. This		 * method triggers the <code>close</code> event. You usually do not need to call this 		 * method directly, because the idle timeout functionality takes care of closing the stream.		 *         * @see #idleTimeout         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function close () : void;
		/**
		 * Called on interval determined by         * <code>playheadUpdateInterval</code> to send <code>playheadUpdate</code>		 * events.  Events only sent when playhead is moving, sent every		 * 0.25 seconds by default.		 *		 * @private
		 */
		function doUpdateTime (e:TimerEvent = null) : void;
		/**
		 * Called at interval determined by         * <code>progressInterval</code> to send <code>progress</code> events.		 * Object dispatch starts when <code>_load</code> is called, ends		 * when all bytes downloaded or a network error of some kind		 * occurs.         *         * @default 0.25		 *		 * @private
		 */
		function doUpdateProgress (e:TimerEvent) : void;
		/**
		 * @private
		 */
		function checkEnoughDownloaded (curBytesLoaded:uint, curBytesTotal:uint) : void;
		/**
		 * @private
		 */
		function checkReadyForPlay (curBytesLoaded:uint, curBytesTotal:uint) : void;
		/**
		 * <code>NetStatusEvent.NET_STATUS</code> event listener		 * for rtmp.  Handles automatic resizing, autorewind and		 * buffering messaging.		 *		 * @private
		 */
		function rtmpNetStatus (e:NetStatusEvent) : void;
		/**
		 * <code>NetStatusEvent.NET_STATUS</code> event listener		 * for http.  Handles autorewind.		 *		 * @private
		 */
		function httpNetStatus (e:NetStatusEvent) : void;
		/**
		 * Called by INCManager after the connection is complete or failed after a call to the		 * <code>INCManager.connectToURL()</code> method. If the connection failed, set 		 * the <code>INCManager.netConnection</code> property to <code>null</code> or		 * undefined before calling.		 *		 * @see #ncReconnected()		 * @see INCManager#connectToURL()         * @see NCManager#connectToURL()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ncConnected () : void;
		/**
		 * Called by INCManager after the reconnection is complete or has failed after a call to		 * the <code>INCManager.reconnect()</code> method. If the connection fails, 		 * set the <code>INCManager.netconnection</code> property to <code>null</code>		 * before you call it.		 *		 * @see #ncConnected()		 * @see INCManager#reconnect()         * @see NCManager#reconnect()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ncReconnected () : void;
		/**
		 * handles NetStream.onMetaData callback		 *		 * @private
		 */
		function onMetaData (info:Object) : void;
		/**
		 * handles NetStream.onCuePoint callback		 *		 * @private
		 */
		function onCuePoint (info:Object) : void;
		/**
		 * sets state, dispatches event, execs queued commands.  Always try to call		 * this AFTER you do your work, because the state might change again after		 * you call this if you set it to a responsive state becasue of the call		 * to exec queued commands.  If you set this to a responsive state and		 * then do more state based logic, check _state to make sure it did not		 * change out from under you.		 * 		 * @private
		 */
		function setState (s:String, execQueued:Boolean = true) : void;
		/**
		 * Sets state to _cachedState if the _cachedState is VideoState.PLAYING,		 * VideoState.PAUSED or VideoState.BUFFERING, otherwise sets state to VideoState.STOPPED.		 *		 * @private
		 */
		function setStateFromCachedState (execQueued:Boolean = true) : void;
		/**
		 * creates our implementatino of the <code>INCManager</code>.		 * We put this off until we need to do it to give time for the		 * user to customize the <code>iNCManagerClass</code>		 * static variable.		 *		 * @private
		 */
		function createINCManager () : void;
		/**
		 * creates an instance of the class specified by the		 * <code>netStreamClientClass</code> static property.		 *		 * @private
		 */
		function createNetStreamClient () : Object;
		/**
		 * <p>ONLY CALL THIS WITH RTMP STREAMING</p>		 *		 * <p>Has the logic for what to do when we decide we have come to		 * a stop by coming to the end of an rtmp stream.  There are a few		 * different ways we decide this has happened, and we sometimes		 * even set an interval that calls this function repeatedly to		 * check if the time is still changing, which is why it has its		 * own special function.</p>		 *		 * @private
		 */
		function rtmpDoStopAtEnd (e:TimerEvent = null) : void;
		/**
		 * <p>ONLY CALL THIS WITH RTMP STREAMING</p>		 *		 * <p>Wait until time goes back to zero to leave rewinding state.</p>		 *		 * @private
		 */
		function rtmpDoSeek (e:TimerEvent) : void;
		/**
		 * <p>ONLY CALL THIS WITH HTTP PROGRESSIVE DOWNLOAD</p>		 *		 * <p>Call this when playing stops by hitting the end.</p>		 *		 * @private
		 */
		function httpDoStopAtEnd () : void;
		/**
		 * <p>ONLY CALL THIS WITH HTTP PROGRESSIVE DOWNLOAD</p>		 *		 * <p>If we get an onStatus callback indicating a seek is over,		 * but the playheadTime has not updated yet, then we wait on a		 * timer before moving forward.</p>		 *		 * @private
		 */
		function httpDoSeek (e:TimerEvent) : void;
		/**
		 * <p>Wrapper for <code>NetStream.close()</code>.  Never call		 * <code>NetStream.close()</code> directly, always call this		 * method because it does some other housekeeping.</p>		 *		 * @private
		 */
		function closeNS (updateCurrentPos:Boolean = false) : void;
		/**
		 * <p>We do a brief timer before entering VideoState.BUFFERING state to avoid		 * quick switches from VideoState.BUFFERING to VideoState.PLAYING and back.</p>		 *		 * @private
		 */
		function doDelayedBuffering (e:TimerEvent) : void;
		/**
		 * Wrapper for <code>NetStream.pause()</code>.  Never call		 * <code>NetStream.pause()</code> directly, always call this		 * method because it does some other housekeeping.		 *		 * @private
		 */
		function _pause (doPause:Boolean) : void;
		/**
		 * Wrapper for <code>NetStream.play()</code>.  Never call		 * <code>NetStream.play()</code> directly, always call this		 * method because it does some other housekeeping.		 *		 * @private
		 */
		function _play (startTime:int = 0, endTime:int = -1) : void;
		/**
		 * Wrapper for <code>NetStream.seek()</code>.  Never call		 * <code>NetStream.seek()</code> directly, always call		 * this method because it does some other housekeeping.		 *		 * @private
		 */
		function _seek (time:Number) : void;
		/**
		 * Gets whether connected to a stream.  If not, then calls to APIs		 * <code>play() with no args</code>, <code>stop()</code>,		 * <code>pause()</code> and <code>seek()</code> will throw		 * exceptions.		 *		 * @see #stateResponsive		 * @private
		 */
		function isXnOK () : Boolean;
		/**
		 * Kicks off autoresize process		 *		 * @private
		 */
		function startAutoResize () : void;
		/**
		 * <p>Does the actual work of resetting the width and height.</p>		 *		 * <p>Called on an interval which is stopped when width and height		 * of the <code>Video</code> object are not zero.  Finishing the		 * resize is done in another method which is either called on a		 * interval set up here for live streams or on a		 * NetStream.Play.Stop event in <code>rtmpNetStatus</code> after		 * stream is rewound if it is not a live stream.  Still need to		 * get a http solution.</p>		 *		 * @private
		 */
		function doAutoResize (e:TimerEvent = null) : void;
		/**
		 * <p>Makes video visible, turns on sound and starts		 * playing if live or autoplay.</p>		 *		 * @private
		 */
		function finishAutoResize (e:TimerEvent = null) : void;
		/**
		 * <p>Creates <code>NetStream</code> and does some basic		 * initialization.</p>		 *		 * @private
		 */
		function _createStream () : void;
		/**
		 * Does initialization after first connecting to the server		 * and creating the stream.  Will get the stream duration from		 * the <code>INCManager</code> if it has it for us.		 *		 * <p>Starts resize if necessary, otherwise starts playing if		 * necessary, otherwise loads first frame of video.  In http case,		 * starts progressive download in any case.</p>		 *		 * @private
		 */
		function _setUpStream () : void;
		/**
		 * <p>ONLY CALL THIS WITH RTMP STREAMING</p>		 *		 * <p>Only used for rtmp connections.  When we pause or stop,		 * setup an interval to call this after a delay (see property		 * <code>idleTimeout</code>).  We do this to spare the server from		 * having a bunch of extra xns hanging around, although this needs		 * to be balanced with the load that creating connections puts on		 * the server, and keep in mind that Flash Media Server (FMS) can be configured to		 * terminate idle connections on its own, which is a better way to		 * manage the issue.</p>		 *		 * @private
		 */
		function doIdleTimeout (e:TimerEvent) : void;
		/**
		 * Dumps all queued commands without executing them		 *		 * @private
		 */
		function flushQueuedCmds () : void;
		/**
		 * Executes as many queued commands as possible, obviously		 * stopping when state becomes unresponsive.		 *		 * @private
		 */
		function execQueuedCmds () : void;
		/**
		 * @private
		 */
		function queueCmd (type:Number, url:String = null, isLive:Boolean = false, time:Number = NaN) : void;
	}
}
