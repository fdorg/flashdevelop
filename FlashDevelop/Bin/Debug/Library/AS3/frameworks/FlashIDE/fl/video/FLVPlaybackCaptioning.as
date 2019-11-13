package fl.video
{
	import flash.ui.Keyboard;
	import flash.accessibility.Accessibility;
	import flash.accessibility.AccessibilityProperties;
	import flash.display.*;
	import flash.events.*;
	import flash.text.*;
	import flash.geom.Rectangle;
	import flash.geom.Point;
	import flash.system.Capabilities;
	import flash.utils.*;

	/**
	 * Dispatched when a caption is added or removed from the caption target	 * text field.  	 * 	 * <p>The event is also dispatched when the following conditions are true:</p>	 * <ul>	 * <li>the <code>captionTargetName</code> property is not set</li>	 * <li>the <code>captionTarget</code> property is not set</li>	 * <li>the FLVPlaybackCaptioning instance creates a TextField object automatically for captioning.</li>	 * </ul>	 * 	 * <p>The <code>captionChange</code> event has the constant	 * <code>CaptionChangeEvent.CAPTION_CHANGE</code>.</p>	 * 	 * 	 * @see #captionTargetName 	 * @see #captionTarget 	 * @tiptext captionChange event	 * @eventType fl.video.CaptionChangeEvent.CAPTION_CHANGE	 * 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("captionChange", type="fl.video.CaptionChangeEvent")] 
	/**
	 * Dispatched after the <code>captionTarget</code> property is created, 	 * but before any captions are added (the <code>captionTarget</code> property	 * is empty). 	 * 	 * <p>If the <code>captionTarget</code> property is set with a custom	 * DisplayObject, or if the <code>captionTargetName</code> property is	 * set, this event does not dispatch.</p>	 *	 * <p>Listen for this event if you are customizing the	 * properties of the TextField object, for example, the <code>defaultTextFormat</code> property.</p>	 * 	 * <p>The <code>captionTargetCreated</code> event has the constant	 * <code>CaptionTargetEvent.CAPTION_TARGET_CREATED</code>.</p>	 *	 * 	 * @see #captionTargetName 	 * @see #captionTarget 	 * @see flash.display.DisplayObject 	 * @tiptext captionTargetCreated event	 * @eventType fl.video.CaptionTargetEvent.CAPTION_TARGET_CREATED	 * 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("captionTargetCreated", type="fl.video.CaptionTargetEvent")] 
	/**
	 * Dispatched after all of the Timed Text XML data is loaded. 	 * 	 * @see flash.net.URLLoader#event:complete URLLoader.complete event	 * 	 * @eventType flash.events.Event.COMPLETE	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("complete", type="flash.events.Event")] 
	/**
	 * Dispatched if a call to the <code>URLLoader.load()</code> event attempts to access a Timed Text XML file	 * over HTTP and the current Flash Player environment is able to detect	 * and return the status code for the request.	 * 	 * @see flash.net.URLLoader#event:httpStatus URLLoader.httpStatus event	 * @eventType flash.events.HTTPStatusEvent.HTTP_STATUS	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("httpStatus", type="flash.events.HTTPStatusEvent")] 
	/**
	 * Dispatched if a call to the <code>URLLoader.load()</code> event results in a fatal error	 * that terminates the download of the Timed Text XML file.	 * 	 * <p>If this event is not handled, it will throw an error.</p>	 *	 * @see flash.net.URLLoader#event:ioError URLLoader.ioError event	 * @eventType flash.events.IOErrorEvent.IO_ERROR 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("ioError", type="flash.events.IOErrorEvent")] 
	/**
	 * Dispatched when the download operation to load the Timed Text XML file	 * begins, following a call to the <code>URLLoader.load()</code> method.	 * 	 * @see flash.net.URLLoader#event:open URLLoader.open event	 * @eventType flash.events.Event.OPEN 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("open", type="flash.events.Event")] 
	/**
	 * Dispatched when data is received as the download of the 	 * Timed Text XML file progresses. 	 * 	 * @see flash.net.URLLoader#event:progress URLLoader.progress event	 * @eventType flash.events.ProgressEvent.PROGRESS 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("progress", type="flash.events.ProgressEvent")] 
	/**
	 * Dispatched if a call to the <code>URLLoader.load()</code> event attempts to load a 	 * Timed Text XML file from a server outside the security sandbox. 	 * 	 * <p>If this event is not handled, it will throw an error.</p>	 * 	 * @see flash.net.URLLoader#event:securityError URLLoader.securityError event	 * @eventType flash.events.SecurityErrorEvent.SECURITY_ERROR 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0
	 */
	[Event("securityError", type="flash.events.SecurityErrorEvent")] 

	/**
	 The FLVPlaybackCaptioning component enables captioning for the FLVPlayback component.	 *	 * 	 * <hr/>     * <strong>NOTE: </strong> This documentation is intended for use with the FLVPlaybackCaptioning with Accessibility component.	 * <p>It updates the ActionScript 3.0 Language and Components Reference for the FLVPlaybackCaptioning class to include the following 	 * properties and methods which were added to improve the component's accessibility.</p>	 * <br/>	 * <div class="seeAlso">	 *  <a href="#findInCaptions()"><code>findInCaptions()</code></a><br/>	 * 	<a href="#getCaptionsAsArray()"><code>getCaptionsAsArray()</code></a><br/>	 * 	<a href="#getCaptionsAsTranscript()"><code>getCaptionsAsTranscript()</code></a><br/>	 * 	<a href="#secondsToTime()"><code>secondsToTime()</code></a>	 * </div>	 * 	 * <p>Make sure you are including the &quot; with Accessibility&quot; version of the component in your project 	 * before attempting to access the new properties or methods.</p> 	 * <hr/>	 * 	 * 	 * <p>The FLVPlaybackCaptioning component downloads a Timed Text (TT) XML file	 * and applies those captions to an FLVPlayback component to which this	 * component is partnered.</p>	 * <p>For more information on Timed Text format, see	 * <a href="http://www.w3.org/AudioVideo/TT/" target="_blank">http://www.w3.org/AudioVideo/TT/</a>.  The FLVPlaybackCaptioning component	 * supports a subset of the	 * Timed Text 1.0 specification.  For detailed information on the supported subset, see	 * <a href="../../TimedTextTags.html">Timed Text Tags</a>. The following is a 	 * brief example:</p>	 *	 * <pre>	 * &lt;?xml version="1.0" encoding="UTF-8"?&gt;	 * &lt;tt xml:lang="en" xmlns="http://www.w3.org/2006/04/ttaf1"  xmlns:tts="http://www.w3.org/2006/04/ttaf1#styling"&gt;	 *     &lt;head&gt;	 *         &lt;styling&gt;	 *             &lt;style id="1" tts:textAlign="right"/&gt;	 *             &lt;style id="2" tts:color="transparent"/&gt;	 *             &lt;style id="3" style="2" tts:backgroundColor="white"/&gt;	 *             &lt;style id="4" style="2 3" tts:fontSize="20"/&gt;	 *         &lt;/styling&gt;	 *     &lt;/head&gt;	 *     &lt;body&gt;	 *          &lt;div xml:lang="en"&gt;	 *             &lt;p begin="00:00:00.50" dur="500ms"&gt;Four score and twenty years ago&lt;/p&gt;	 *             &lt;p begin="00:00:02.50"&gt;&lt;span tts:fontFamily="monospaceSansSerif,proportionalSerif,TheOther"tts:fontSize="+2"&gt;our forefathers&lt;/span&gt; brought forth&lt;br /&gt; on this continent&lt;/p&gt;	 *             &lt;p begin="00:00:04.40" dur="10s" style="1"&gt;a &lt;span tts:fontSize="12 px"&gt;new&lt;/span&gt; &lt;span tts:fontSize="300%"&gt;nation&lt;/span&gt;&lt;/p&gt;	 *             &lt;p begin="00:00:06.50" dur="3"&gt;conceived in &lt;span tts:fontWeight="bold" tts:color="#ccc333"&gt;liberty&lt;/span&gt; &lt;span tts:color="#ccc333"&gt;and dedicated to&lt;/span&gt; the proposition&lt;/p&gt;	 *             &lt;p begin="00:00:11.50" tts:textAlign="right"&gt;that &lt;span tts:fontStyle="italic"&gt;all&lt;/span&gt; men are created equal.&lt;/p&gt;	 * 			&lt;p begin="15s" style="4"&gt;The end.&lt;/p&gt;	 *         &lt;/div&gt;    	 *     &lt;/body&gt;	 * &lt;/tt&gt;	 * </pre>	 *     * @includeExample examples/FLVPlaybackCaptioningExample.as -noswf     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class FLVPlaybackCaptioning extends Sprite
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var ttm : TimedTextManager;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var visibleCaptions : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var hasSeeked : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var flvPos : Rectangle;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var prevCaptionTargetHeight : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var captionTargetOwned : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var captionTargetLastHeight : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var captionToggleButton : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var onButton : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var offButton : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var captionToggleButtonWaiting : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static const AUTO_VALUE : String = "auto";
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _captionTarget : TextField;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _captionTargetContainer : DisplayObjectContainer;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var cacheCaptionTargetParent : DisplayObjectContainer;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var cacheCaptionTargetIndex : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var cacheCaptionTargetAutoLayout : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var cacheCaptionTargetLocation : Rectangle;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var cacheCaptionTargetScaleY : Number;
		/**
		 * Used when keeing flvplayback skin above the caption         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var skinHijacked : Boolean;
		private var _autoLayout : Boolean;
		private var _captionsOn : Boolean;
		private var _captionURL : String;
		private var _flvPlaybackName : String;
		private var _flvPlayback : FLVPlayback;
		private var _captionTargetName : String;
		private var _videoPlayerIndex : uint;
		private var _limitFormatting : Boolean;
		private var _track : uint;
		private var _captionsOnCache : Boolean;

		/**
		 * Used to display captions; <code>true</code> = display captions, 		 * <code>false</code> = do not display captions.         *          * <p>If you use the <code>captionButton</code> property to allow the         * user to turn captioning on and off, set the <code>showCaptions</code>         * property to <code>false</code>.</p>         *          * @default true         *         * @see #captionButton          *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get showCaptions () : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set showCaptions (b:Boolean) : void;
		/**
		 * URL of the Timed Text XML file that contains caption information (<b>required property</b>).         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set source (url:String) : void;
		/**
		 * Determines whether the FLVPlaybackCaptioning component	 * automatically moves and resizes the TextField object for captioning.	 *	 * 	 * <p>If the <code>autoLayout</code> property is set to <code>true</code>, the DisplayObject instance or 	 * the TextField object containing the captions displays 10 pixels from the bottom	 * of the FLVPlayback instance. The captioning area covers the width of	 * the FLVPlayback instance, maintaining a margin of 10 pixels on each side.</p>	 * 	 * <p>When this property is set to <code>true</code>, the DisplayObject instance	 * or TextField object displays directly over the FLVPlayback instance.	 * If you are creating your own TextField object, you should set	 * <code>autoLayout</code> to <code>false</code>. 	 * If <code>wordWrap = false</code>, the captioning area centers over the FLVPlayback	 * instance, but it can be wider than the FLVPlayback instance.</p>	 * 	 * <p>To control layout, you need to listen for the <code>captionChange</code> event to	 * determine when the TextField object instance is created.</p>	 * 	 * @default true	 *          * @see #captionTarget          * @see #event:captionChange captionChange event         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoLayout () : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoLayout (b:Boolean) : void;
		/**
		 * The instance name of the TextField object or MovieClip enclosing a Textfield object		 * that contains the captions.		 *		 * <p>To specify no target, set this property to an empty string (that is, no target specified)		 * or <code>auto</code>. This property is primarily used		 * in the Component inspector. If you are writing code, use the		 * <code>captionTarget</code> property instead.</p>		 * 		 * @default auto		 *         * @see #captionTarget          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get captionTargetName () : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set captionTargetName (tname:String) : void;
		/**
		 * Sets the DisplayObject instance in which to display captions. 	 *	 * <p>If you set the instance as a TextField object, it is targeted directly.	 * If you set the instance as a DisplayObjectContainer	 * containing one or more TextField objects, the captions display in the	 * TextField object with the lowest display index.</p>	 *	 * <p>The <code>DisplayObjectContainer</code> method supports a movie-clip like object	 * with a scale-9 background, which can be scaled when the TextField object size changes.</p>	 *	 * <p>For more complex scaling and drawing, write code to have the	 * <code>DisplayObjectContainer</code> method listen for a <code>captionChange</code> event.</p>	 * <p><b>Note</b> If the <code>captionTargetName</code> or the <code>captionTarget</code> property 	 * is not set, the FLVPlaybackCaptioning instance creates a text field set by the	 * <code>captionTarget</code> property with this formatting:</p>	 *	 * <ul>	 * <li>black background (background = <code>true</code>; backgroundColor = <code>0x000000</code>;)</li>		 * <li>white text (textColor = <code>0xFFFFFF</code>)</li>	 * <li>autoSize = <code>TextFieldAutoSize.LEFT</code></li>	 * <li>multiLine = <code>true</code></li>	 * <li>wordWrap = <code>true</code></li>	 * <li>font = <code>"_sans"</code></li>	 * <li>size = <code>12</code></li>	 * </ul>	 * 	 * <p>To customize these values, listen for the <code>captionTargetCreated</code> event.</p>	 * 	 *         * @see #captionTargetName          * @see flash.display.DisplayObjectContainer         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get captionTarget () : DisplayObject;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set captionTarget (ct:DisplayObject) : void;
		/**
		 * Defines the captionButton FLVPlayback custom UI component instance	 * which provides toggle capabilities to turn captioning on and off. 	 * 	 * <p>The <code>captionButton</code> property functions similarly to the FLVPlayback	 * properties <code>playButton</code>,	 * <code>pauseButton</code>, <code>muteButton</code>, and so on.</p>	 *	 * @see FLVPlayback	 * 	 * @langversion 3.0	 * @playerversion Flash 9.0.28.0	 *
		 */
		public function get captionButton () : Sprite;
		public function set captionButton (s:Sprite) : void;
		/**
		 * Sets an FLVPlayback instance name for the FLVPlayback instance	 * that you want to caption. 	 * 	 * <p>To specify no target, set this to an empty string or <code>auto</code>.	 * The FLVPlayback instance must have the same	 * parent as the FLVPlaybackCaptioning instance.</p>	 * 	 * <p>The FLVPlayback instance name is primarily used in	 * the Component inspector.  If you are writing code,	 * use the <code>flvPlayback</code> property.</p>	 * 	 * <p>If the <code>flvPlaybackName</code> or  	 * the <code>flvPlayback</code> property is not set or set to <code>auto</code>, 	 * the FLVPlaybackCaptioning instance searches for a FLVPlayback	 * instance with the same parent and captions the first one it finds.</p>	 * 	 * @default auto	 *         * @see FLVPlayback FLVPlayback class         * @see #flvPlayback flvPlayback property         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get flvPlaybackName () : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set flvPlaybackName (flvname:String) : void;
		/**
		 * Sets the FLVPlayback instance to caption.  The FLVPlayback	 * instance must have the same parent as the	 * FLVPlaybackCaptioning instance.	 * <p>If the 	 * <code>flvPlaybackName</code> or	 * <code>flvPlayback</code> property is <b>not</b> set, the	 * FLVPlaybackCaptioning instance will look for a FLVPlayback	 * instance with the same parent and caption the first one it	 * finds.</p>	 *         * @see #flvPlaybackName          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get flvPlayback () : FLVPlayback;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set flvPlayback (fp:FLVPlayback) : void;
		/**
		 * Support for multiple language tracks.  		 * 		 * <p>The best utilization of the <code>track</code> property		 * is to support multiple language tracks with		 * embedded cue points.</p>		 * 		 * <p>You must follow the supported formats for FLVPlaybackCaptioning		 * cue points.</p>		 *		 * <p>If the <code>track</code> property is set to something other than <code>0</code>,		 * the FLVPlaybackCaptioning component searches for a text&lt;n&gt; property on the cue point,		 * where <em>n</em> is the track value.</p> 		 * 		 * <p>For example, if <code>track == 1</code>, then the FLVPlayBackCaptioning component		 * searches for the parameter <code>text1</code> on the cue point.  If 		 * no matching parameter is found, the text property in the cue point parameter is used.</p>		 *		 * @default 0		 * 		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get track () : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set track (i:uint) : void;
		/**
		 * Connects the captioning to a specific VideoPlayer in the		 * FLVPlayback component.		 *		 * <p> If you want to use captioning in		 * multiple video players (using the <code>activeVideoPlayerIndex</code>		 * and <code>visibleVideoPlayerIndex</code> properties in the FLVPlayback		 * component), you		 * should create one instance of the FLVPlaybackCaptioning		 * component for each <code>VideoPlayer</code> that you will be using and set		 * this property to correspond to the index.</p>		 * 		 * <p>The VideoPlayer index defaults to         * 0 when only one video player is used.</p>          *          * @see FLVPlayback#activeVideoPlayerIndex          * @see FLVPlayback#visibleVideoPlayerIndex          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get videoPlayerIndex () : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set videoPlayerIndex (v:uint) : void;
		/**
		 * Limits formatting instructions 		 * from the Timed Text file when set to <code>true</code>.		 *		 * <p>The following styles are <b>not</b> supported if the <code>simpleFormatting</code>		 * property is set to <code>true</code>:</p>		 * 		 * <ul>		 *   <li>tts:backgroundColor</li>		 *   <li>tts:color</li>		 *   <li>tts:fontSize</li>		 *   <li>tts:fontFamily</li>		 *   <li>tts:wrapOption</li>		 * </ul>		 *		 * <p>The following styles <b>are</b> supported if the <code>simpleFormatting</code>		 * property is set to <code>true</code>:</p>		 * 		 * <ul>		 *   <li>tts:fontStyle</li>		 *   <li>tts:fontWeight</li>		 *   <li>tts:textAlign</li>         * </ul>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get simpleFormatting () : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set simpleFormatting (b:Boolean) : void;

		/**
		 * Creates a new FLVPlaybackCaptioning instance.          *           * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function FLVPlaybackCaptioning ();
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function forwardEvent (e:Event) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function startLoad (e:Event) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function hookupCaptionToggle (e:Event) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleCaption (e:MetadataEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleStateChange (e:VideoEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleComplete (e:VideoEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handlePlayheadUpdate (e:VideoEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleSkinLoad (e:VideoEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleAddedToStage (e:Event) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleFullScreenEvent (e:FullScreenEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function enterFullScreenTakeOver () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function exitFullScreenTakeOver () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function cleanupCaptionButton () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function setupButton (ctrl:Sprite, prefix:String, vis:Boolean) : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleButtonClick (e:MouseEvent) : void;
		/**
		 * This function stolen from UIManager and tweaked.		 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function setupButtonSkin (prefix:String) : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function removeOldCaptions (playheadTime:Number, removedCaptionsIn:Array = null) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function findFLVPlayback () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function createCaptionTarget () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function layoutCaptionTarget (e:LayoutEvent = null) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function addFLVPlaybackListeners () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function removeFLVPlaybackListeners () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getCaptionText (cp:Object) : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function displayCaptionNow () : void;
		/**
		 * Keeps screen reader from reading captionTarget		 * 		 * @private
		 */
		function silenceCaptionTarget () : void;
		/**
		 * Defines accessibility properties for caption toggle buttons		 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function setupCaptionButtonAccessibility () : void;
		/**
		 * Handles keyboard events for accessibility		 *          * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleKeyEvent (e:KeyboardEvent) : void;
		/**
		 * Handles a mouse focus change event and forces focus on the appropriate control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function handleMouseFocusChangeEvent (event:FocusEvent) : void;
		/**
		 * Returns a string containing all captions as an HTML-formatted transcript. 		 * <p>Each caption is contained within a <code>p</code> tag with the <code>class</code> attribute &quot;fl_video_caption,&quot; 		 * a unique <code>id</code> attribute starting with the string &quot;fl_video_caption_2_0_,&quot; 		 * and a <code>title</code> attribute indicating the timecode at which the caption appears in the video.</p>		 * <p>Following is a brief example of the HTML returned:  </p>		 * <pre>		 * &lt;div id=&quot;fl_video_transcript" class=&quot;fl_video_transcript&quot;&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_1&quot; title=&quot;0:00.50&quot;&gt;&lt;i&gt;( speaking French ): &lt;/i&gt;&lt;br/&gt;&lt;b&gt;George: &lt;/b&gt; Bonjour, Marie.&lt;/p&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_2&quot; title=&quot;0:01.75&quot;&gt;&lt;b&gt;Marie: &lt;/b&gt; Bonjour, George.&lt;/p&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_3&quot; title=&quot;0:03.75&quot;&gt;Voil&agrave; une sucette.&lt;/p&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_4&quot; title=&quot;0:05.50&quot;&gt;&lt;b&gt;George: &lt;/b&gt; C'est pour moi?&lt;/p&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_5&quot; title=&quot;0:06.50&quot;&gt;&lt;b&gt;Marie: &lt;/b&gt; Oui, c'est pour toi.&lt;/p&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_6&quot; title=&quot;0:08.00&quot;&gt;&lt;b&gt;George: &lt;/b&gt; Merci, Marie!&lt;/p&gt;		 * 	&lt;p class=&quot;fl_video_caption&quot; id=&quot;fl_video_caption_2_0_7&quot; title=&quot;0:09.50&quot;&gt;&lt;b&gt;Marie: &lt;/b&gt; De rien, George.&lt;/p&gt;		 * &lt;/div&gt;		 * </pre>		 *          * @param preserveFormatting Preserves the HTML tags used to format the caption text in Flash 		 * 		 * @return String A string containing all captions as an HTML-formatted transcript		 * 		 * @includeExample examples/FLVPlaybackCaptioning.getCaptionsAsTranscript.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getCaptionsAsTranscript (preserveFormatting:Boolean = false) : String;
		/**
		 * Returns an array of FLVPlayback component cuepoints that contain the captions.		 * 		 * @return Array An array of FLVPlayback component cuepoints		 * 		 * @includeExample examples/FLVPlaybackCaptioning.getCaptionsAsArray.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getCaptionsAsArray () : Array;
		/**
		 * Returns a number of seconds as a timecode string.		 * 		 * @param sec A number of seconds.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function secondsToTime (sec:Number) : String;
		/**
		 * Returns an array of FLVPlayback component cuepoints whose caption text contains the search string.		 * 		 * @param searchString A string to search for in the captions text.		 * 		 * @return Array An array of FLVPlayback component cuepoints		 * 		 * @includeExample examples/FLVPlaybackCaptioning.findInCaptions.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function findInCaptions (searchString:String) : Array;
	}
}
