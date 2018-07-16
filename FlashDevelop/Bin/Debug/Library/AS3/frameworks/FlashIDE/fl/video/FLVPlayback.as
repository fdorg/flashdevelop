package fl.video
{
	import flash.display.*;
	import flash.events.*;
	import flash.geom.Rectangle;
	import flash.media.*;
	import flash.net.*;
	import flash.utils.*;

	/**
	 * Dispatched when the video player is resized or laid out automatically. A video player is 	 * laid out automatically based on the values of the <code>align</code> and <code>scaleMode</code> properties	 * when a new FLV file is loaded or when one of those two properties is changed.	 * 	 * <p>The <code>autoLayout</code> event is of type AutoLayoutEvent and has the constant 	 * <code>AutoLayoutEvent.AUTO_LAYOUT</code>.</p>	 * 	 * <p>After an attempt to automatically lay out a video player, the event object is dispatched even if the dimensions were	 * not changed. </p>	 * 	 * <p>A <code>LayoutEvent</code> is also dispatched in these three scenarios:</p>	 * <ul>	 * <li>If the video player that laid itself out is visible.</li>	 * <li>If there are two video players of different sizes or positions and the 	 * <code>visibleVideoPlayerIndex</code> property is switched from one video player to another.</li>	 * <li>If methods or properties that change the size of the video player such	 * as <code>setSize()</code>, <code>setScale()</code>, 	 * <code>width</code>, <code>height</code>, <code>scaleX</code>, <code>scaleY</code>,	 * <code>registrationWidth</code> and <code>registrationHeight</code>, are called.</li>	 * </ul>	 *	 * <p>If multiple video player instances are in use, this event may	 * not apply to the visible video player.</p>	 * 	 * @see AutoLayoutEvent#AUTO_LAYOUT 	 * @see #scaleMode  	 * @see #event:layout layout event	 * @see #visibleVideoPlayerIndex  	 * 	 * @tiptext autoLayout event     * @eventType fl.video.AutoLayoutEvent.AUTO_LAYOUT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("autoLayout", type="fl.video.AutoLayoutEvent")] 
	/**
	 * Dispatched when the playhead is moved to the start of the video player because the 	 * <code>autoRewind</code> property is set to <code>true</code>. When the	 * <code>autoRewound</code> event is dispatched, the <code>rewind</code> event is also	 * dispatched. 	 * 	 * <p>The <code>autoRewound</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.AUTO_REWOUND</code>.</p>	 * 	 * @see VideoEvent#AUTO_REWOUND 	 * @see #autoRewind  	 * @see #event:rewind rewind event	 * 	 * @tiptext autoRewound event     * @eventType fl.video.VideoEvent.AUTO_REWOUND     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("autoRewound", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the FLVPlayback instance enters the buffering state. 	 * The FLVPlayback instance typically enters this state immediately after a call to the 	 * <code>play()</code> method or when the <code>Play</code> control is clicked, 	 * before entering the playing state. 	 *	 * <p>The <code>stateChange</code> event is also dispatched.</p>	 * 	 * <p>The <code>bufferingStateEntered</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.BUFFERING_STATE_ENTERED</code>.</p>	 * 	 * @see VideoState#BUFFERING 	 * @see VideoEvent#BUFFERING_STATE_ENTERED 	 * @see #play() 	 * @see #playheadTime 	 * @see #state 	 * @see #event:stateChange stateChange event	 * 	 * @tiptext buffering event     * @eventType fl.video.VideoEvent.BUFFERING_STATE_ENTERED     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("bufferingStateEntered", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the event object closes the NetConnection, 	 * by timing out or through a call to the <code>closeVideoPlayer()</code> method or when 	 * you call the <code>load()</code> or <code>play()</code> methods or set the 	 * <code>source</code> property and cause the RTMP connection 	 * to close as a result. The FLVPlayback instance dispatches this event only when 	 * streaming from Flash Media Server (FMS) or other Flash Video Streaming Service (FVSS). 	 * 	 * <p>The <code>close</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.CLOSE</code>.</p>	 *  	 * @see VideoEvent#CLOSE 	 * @see #closeVideoPlayer() 	 * @see #source 	 * @see #load() 	 * @see #play() 	 * 	 * @tiptext close event     * @eventType fl.video.VideoEvent.CLOSE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("close", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when playing completes because the player reached the end of the FLV file. 	 * The component does not dispatch the event if you call the <code>stop()</code> or 	 * <code>pause()</code> methods 	 * or click the corresponding controls. 	 * 	 * <p>When the application uses progressive download, it does not set the 	 * <code>totalTime</code>	 * property explicitly, and it downloads an FLV file that does not specify the duration 	 * in the metadata. The video player sets the <code>totalTime</code> property to an approximate 	 * total value before it dispatches this event.</p>	 * 	 * <p>The video player also dispatches the <code>stateChange</code> and <code>stoppedStateEntered</code>	 * events.</p>	 * 	 * <p>The <code>complete</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.COMPLETE</code>.</p>	 * 	 * @see #event:stateChange stateChange event	 * @see #event:stoppedStateEntered stoppedStateEntered event	 * @see #stop() 	 * @see #pause() 	 * @see #totalTime 	 * 	 * @tiptext complete event     * @eventType fl.video.VideoEvent.COMPLETE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("complete", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when a cue point is reached. The event object has an 	 * <code>info</code> property that contains the info object received by the 	 * <code>NetStream.onCuePoint</code> event callback for FLV file cue points. 	 * For ActionScript cue points, it contains the object that was passed 	 * into the ActionScript cue point methods or properties.	 * 	 * <p>The <code>cuePoint</code> event is of type MetadataEvent and has the constant 	 * <code>MetadataEvent.CUE_POINT</code>.</p>	 * 	 * @see MetadataEvent#CUE_POINT 	 * @see flash.net.NetStream#event:onCuePoint NetStream.onCuePoint event	 * 	 * @tiptext cuePoint event     * @eventType fl.video.MetadataEvent.CUE_POINT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("cuePoint", type="fl.video.MetadataEvent")] 
	/**
	 * Dispatched when the location of the playhead moves forward by a call to	 * the <code>seek()</code> method or by clicking the ForwardButton control. 	 * 	 * <p>The FLVPlayback instance also dispatches <code>playheadUpdate</code> event.</p>	 * 	 * <p>The <code>fastForward</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.FAST_FORWARD</code>.</p> 	 * 	 * @see VideoEvent#FAST_FORWARD 	 * @see #event:playheadUpdate playheadUpdate event	 * @see #seek() 	 * 	 * @tiptext fastForward event     * @eventType fl.video.VideoEvent.FAST_FORWARD     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("fastForward", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched the first time the FLV file's metadata is reached.	 * The event object has an <code>info</code> property that contains the 	 * info object received by the <code>NetStream.onMetaData</code> event callback.	 *	 * <p>The <code>metadataReceived</code> event is of type MetadataEvent and has the constant 	 * <code>MetadataEvent.METADATA_RECEIVED</code>.</p>	 * 	 * @see MetadataEvent#METADATA_RECEIVED 	 * @see flash.net.NetStream#event:onMetaData Netstream.onMetaData event	 *      * @tiptext metadataReceived event	 * @eventType fl.video.MetadataEvent.METADATA_RECEIVED	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("metadataReceived", type="fl.video.MetadataEvent")] 
	/**
	 * Dispatched when the player enters the paused state. This happens when you call 	 * the <code>pause()</code> method or click the corresponding control and it also happens 	 * in some cases when the FLV file is loaded and the <code>autoPlay</code> property 	 * is <code>false</code> (the state may be stopped instead). 	 * 	 * <p>The <code>stateChange</code> event is also dispatched. </p>	 * 	 * <p>The <code>pausedStateEntered</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.PAUSED_STATE_ENTERED</code>.</p> 	 * 	 * @see #autoPlay 	 * @see #pause() 	 * @see VideoEvent#PAUSED_STATE_ENTERED 	 * @see #event:stateChange stateChange event	 * 	 * @tiptext pausedStateEntered event     * @eventType fl.video.VideoEvent.PAUSED_STATE_ENTERED     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("pausedStateEntered", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the playing state is entered. This may not occur immediately 	 * after the <code>play()</code> method is called or the corresponding control is clicked; 	 * often the buffering state is entered first, and then the playing state. 	 * 	 * <p>The FLVPlayback instance also dispatches the <code>stateChange</code> event.</p>	 * 	 * <p>The <code>playingStateEntered</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.PLAYING_STATE_ENTERED</code>.</p> 	 * 	 * @see #play() 	 * @see #event:stateChange stateChange event	 * 	 * @tiptext playingStateEntered event     * @eventType fl.video.VideoEvent.PLAYING_STATE_ENTERED     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("playingStateEntered", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched while the FLV file is playing at the frequency specified by the 	 * <code>playheadUpdateInterval</code> property or when rewinding starts. 	 * The component does not dispatch this event when the video player is paused or stopped 	 * unless a seek occurs. 	 * 	 * <p>The <code>playheadUpdate</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.PLAYHEAD_UPDATE</code>.</p> 	 * 	 * 	 * @see VideoEvent#PLAYHEAD_UPDATE 	 * @see #playheadUpdateInterval 	 * 	 * @tiptext change event	 * @eventType fl.video.VideoEvent.PLAYHEAD_UPDATE     * @default .25 seconds     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("playheadUpdate", type="fl.video.VideoEvent")] 
	/**
	 * Indicates progress made in number of bytes downloaded. Dispatched at the frequency 	 * specified by the <code>progressInterval</code> property, starting 	 * when the load begins and ending when all bytes are loaded or there is a network error. 	 * The default is every .25 seconds starting when load is called and ending	 * when all bytes are loaded or if there is a network error. Use this event to check 	 * bytes loaded or number of bytes in the buffer. 	 *	 * <p>Dispatched only for a progressive HTTP download. Indicates progress in number of 	 * downloaded bytes. The event object has the <code>bytesLoaded</code> and <code>bytesTotal</code>	 * properties, which are the same as the FLVPlayback properties of the same names.</p>	 * 	 * <p>The <code>progress</code> event is of type VideoProgressEvent and has the constant 	 * <code>VideoProgressEvent.PROGRESS</code>.</p> 	 * 	 * @see #bytesLoaded 	 * @see #bytesTotal 	 * @see VideoProgressEvent#PROGRESS 	 * @see #progressInterval 	 * 	 * @tiptext progress event	 * @eventType fl.video.VideoProgressEvent.PROGRESS     *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("progress", type="fl.video.VideoProgressEvent")] 
	/**
	 * Dispatched when an FLV file is loaded and ready to display. It starts the first 	 * time you enter a responsive state after you load a new FLV file with the <code>play()</code>	 * or <code>load()</code> method. It starts only once for each FLV file that is loaded.	 * 	 * <p>The <code>ready</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.READY</code>.</p> 	 * 	 * @see #load() 	 * @see #play()      * @see VideoEvent#READY      *	 * @tiptext ready event	 * @eventType fl.video.VideoEvent.READY	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("ready", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the video player is resized or laid out. Here are two layout scenarios:<br/>	 * <ul><li>If the video player is laid out by either using the <code>autoLayout</code> 	 * event or calling the <code>setScale()</code> or 	 * <code>setSize()</code> methods or changing the <code>width</code>, <code>height</code>,	 * <code>scaleX</code>, and <code>scaleY</code> properties.</li>	 * <li>If there are two video players of different sizes and the 	 * <code>visibleVideoPlayerIndex</code> property is switched from one video player to another.</li>	 * </ul>  	 *	 * <p>The <code>layout</code> event is of type LayoutEvent and has the constant 	 * <code>LayoutEvent.LAYOUT</code>.</p> 	 * 	 * @see #event:autoLayout autoLayout event 	 * @see LayoutEvent#LAYOUT 	 * @see #visibleVideoPlayerIndex      *	 * @tiptext layout event	 * @eventType fl.video.LayoutEvent.LAYOUT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("layout", type="fl.video.LayoutEvent")] 
	/**
	 * Dispatched when the location of the playhead moves backward by	 * a call to <code>seek()</code> or when an <code>autoRewind</code> call is	 * completed. When an <code>autoRewind</code> call is completed, an <code>autoRewound</code>	 * event is triggered first. 	 * 	 * <p>The <code>rewind</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.REWIND</code>.</p> 	 * 	 * @see #autoRewind 	 * @see #event:autoRewound autoRewound event	 * @see VideoEvent#REWIND 	 * @see VideoState#REWINDING 	 * @see #seek() 	 * 	 * @tiptext rewind event	 * @eventType fl.video.VideoEvent.REWIND     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("rewind", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the user stops scrubbing the FLV file with the seek bar. 	 * Scrubbing refers to grabbing the handle of the SeekBar and dragging it in 	 * either direction to locate a particular scene in the FLV file. Scrubbing stops 	 * when the user releases the handle of the seek bar.	 * 	 * <p>The component also dispatches the <code>stateChange</code> event with 	 * the <code>state</code> property, which is either playing, paused, 	 * stopped, or buffering. The <code>state</code> property is	 * seeking until the user finishes scrubbing.</p>	 *      * <p>The <code>scrubFinish</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.SCRUB_FINISH</code>.</p> 	 * 	 * @see VideoEvent#SCRUB_FINISH 	 * @see #state      * @see #event:stateChange stateChange event     *	 * @tiptext scrubFinish event	 * @eventType fl.video.VideoEvent.SCRUB_FINISH	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("scrubFinish", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the user begins scrubbing the FLV file with the seek bar. 	 * Scrubbing refers to grabbing the handle of the SeekBar and dragging it in either direction 	 * to locate a particular scene in the FLV file. 	 * Scrubbing begins when the user clicks the SeekBar handle and ends when the user 	 * releases it.	 * 	 * <p>The component also dispatches the <code>stateChange</code> event with the 	 * <code>state</code> property equal to seeking. The state remains seeking until the user 	 * stops scrubbing. </p>	 * 	 * <p>The <code>scrubStart</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.SCRUB_START</code>.</p>	 * 	 * @see VideoEvent#SCRUB_START 	 * @see #state      * @see #event:stateChange stateChange event     *	 * @tiptext scrubStart event	 * @eventType fl.video.VideoEvent.SCRUB_START	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("scrubStart", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the location of the playhead is changed by a call to	 * <code>seek()</code> or by setting the <code>playheadTime</code> property or 	 * by using the SeekBar control. The	 * <code>playheadTime</code> property is the destination time. 	 *     * <p>The <code>seeked</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.SEEKED</code>.</p>	 *	 * <p>The FLVPlayback instance dispatches the <code>rewind</code> event when the seek 	 * is backward and the <code>fastForward</code> event when the seek is forward. It also 	 * dispatches the <code>playheadUpdate</code> event.</p>	 *	 * <p>For several reasons, the <code>playheadTime</code> property might not have the 	 * expected value immediately after you call one of the seek methods or set 	 * <code>playheadTime</code> to cause seeking. First, for a progressive download, 	 * you can seek only to a keyframe, so a seek takes you to the time of the first 	 * keyframe after the specified time. (When streaming, a seek always goes to the precise 	 * specified time even if the source FLV file doesn't have a keyframe there.) Second, 	 * seeking is asynchronous, so if you call a seek method or set the <code>playheadTime</code> 	 * property, <code>playheadTime</code> does not update immediately. To obtain the time after the 	 * seek is complete, listen for the <code>seek</code> event, which does not start until the 	 * <code>playheadTime</code> property has updated.</p>	 * 	 * @see #event:fastForward fastForward event	 * @see #playheadTime 	 * @see #event:playheadUpdate playheadUpdate event	 * @see #seek() 	 * @see VideoEvent#SEEKED 	 * 	 * @tiptext seeked event	 * @eventType fl.video.VideoEvent.SEEKED     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("seeked", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when an error occurs loading a skin SWF file.  	 	 * The event has a message property that contains the error message.	 * If a skin SWF file is set, playback begins when the <code>ready</code> event 	 * and <code>skinLoaded</code> (or <code>skinError</code>) events have both started.	 *	 * <p>The <code>skinError</code> event is of type SkinErrorEvent and has the constant 	 * <code>SkinErrorEvent.SKIN_ERROR</code>.</p>	 * 	 * @see #event:ready ready event	 * @see SkinErrorEvent#SKIN_ERROR      * @see #event:skinLoaded skinLoaded event     *	 * @tiptext skinError event	 * @eventType fl.video.SkinErrorEvent.SKIN_ERROR	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("skinError", type="fl.video.SkinErrorEvent")] 
	/**
	 * Dispatched when a skin SWF file is loaded. The component 	 * does not begin playing an FLV file until the <code>ready</code> event 	 * and <code>skinLoaded</code> (or <code>skinError</code>) events have both started.	 * 	 * <p>The <code>skinLoaded</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.SKIN_LOADED</code>.</p>	 * 	 * @see #event:ready ready event	 * @see VideoEvent#SKIN_LOADED      * @see #event:skinError skinError event     *	 * @tiptext skinLoaded event	 * @eventType fl.video.VideoEvent.SKIN_LOADED	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("skinLoaded", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when the playback state changes. When an <code>autoRewind</code> call is 	 * completed the <code>stateChange</code> event is dispatched with the rewinding	 * state. The <code>stateChange</code> event does not 	 * start until rewinding has completed. 	 * 	 * <p>This event can be used to track when playback enters or leaves unresponsive 	 * states such as in the middle of connecting, resizing, or rewinding. The  	 * <code>play()</code>, <code>pause()</code>, <code>stop()</code> and <code>seek()</code> 	 * methods queue the requests to be executed when the player enters	 * a responsive state.</p>	 *	 * <p>The <code>stateChange</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.STATE_CHANGE</code>.</p>	 *      * @see VideoEvent#STATE_CHANGE      *	 * @tiptext stateChange event	 * @eventType fl.video.VideoEvent.STATE_CHANGE	 *      * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("stateChange", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when entering the stopped state.	 * This happens when you call the <code>stop()</code> method or click the 	 * <code>stopButton</code> control. It also happens, in some cases, if the 	 * <code>autoPlay</code> property is 	 * <code>false</code> (the state might become paused	 * instead) when the FLV file is loaded. The FLVPlayback instance also dispatches this 	 * event when the playhead stops at the end of the FLV file	 * because it has reached the end of the timeline. 	 *	 * <p>The FLVPlayback instance also dispatches the <code>stateChange</code> event.</p>	 *	 * <p>The <code>stoppedStateEntered</code> event is of type VideoEvent and has the constant 	 * <code>VideoEvent.STOPPED_STATE_ENTERED</code>.</p>	 * 	 * @see #autoPlay 	 * @see #event:stateChange stateChange event	 * @see #stop() 	 * @see VideoState#STOPPED 	 * @see VideoEvent#STOPPED_STATE_ENTERED 	 * 	 * @tiptext stopped event	 * @eventType fl.video.VideoEvent.STOPPED_STATE_ENTERED     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("stoppedStateEntered", type="fl.video.VideoEvent")] 
	/**
	 * Dispatched when sound changes by the user either moving the handle of the 	 * volumeBar control or setting the <code>volume</code> or <code>soundTransform</code> 	 * property.  	 * 	 * <p>The <code>soundUpdate</code> event is of type SoundEvent and has the constant 	 * <code>SoundEvent.SOUND_UPDATE</code>.</p>	 * 	 * @see #soundTransform 	 * @see SoundEvent#SOUND_UPDATE 	 * @see #volume 	 * 	 * @tiptext soundUpdate event	 * @eventType fl.video.SoundEvent.SOUND_UPDATE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("soundUpdate", type="fl.video.SoundEvent")] 

	/**
	 * FLVPlayback extends the Sprite class and wraps a VideoPlayer object. 	 * 	 * <hr/>     * <strong>NOTE: </strong> This documentation is intended for use with the FLVPlayback with Accessibility component.	 * <p>It updates the ActionScript 3.0 Language and Components Reference for the FLVPlayback class to include the following 	 * properties and methods which were added to improve the component's keyboard and screenreader accessibility.</p>	 * <br/>	 * <div class="seeAlso">	 *  <a href="#assignTabIndexes()"><code>assignTabIndexes()</code></a><br/>	 * 	<a href="#startTabIndex"><code>startTabIndex</code></a><br/>	 * 	<a href="#endTabIndex"><code>endTabIndex</code></a>	 * </div>	 * 	 * <p>Make sure you are including the &quot; with Accessibility&quot; version of the component in your project 	 * before attempting to access the new properties or methods.</p> 	 * <hr/>	 *      *      * <p>The FLVPlayback class allows you to include a video player in your application to play      * progressively downloaded video (FLV) files over HTTP, or play streaming FLV files      * from Flash Media Server (FMS) or other Flash Video Streaming Service (FVSS).</p>     *      * <p>Unlike other ActionScript 3.0 components, the FLVPlayback component does not extend      * UIComponent; therefore, it does not support the methods and properties of that class.</p>     *      * <p>To access the properties, methods, and events of the FLVPlayback class, you must import the     * class to your application either by dragging the FLVPlayback component to the Stage in your Flash     * application, or by explicitly importing it in ActionScript using the <code>import</code> statement.     * The following statement imports the FLVPlayback class:</p>     *      * <listing>     * import fl.video.FLVPlayback;</listing>     *      * <p>The FLVPlayback class has a <code>VERSION</code> constant, which is a class property. Class properties are     * available only on the class itself. The <code>VERSION</code> constant returns a string that indicates the      * version of the component. The following code shows the version in the Output panel:</p>     *      * <listing>     * import fl.video.FLVPlayback;     * trace(FLVPlayback.VERSION);</listing>     *      * @see AutoLayoutEvent      * @see FLVPlaybackCaptioning      * @see MetadataEvent      * @see NCManager      * @see LayoutEvent      * @see SoundEvent      * @see VideoPlayer      * @see VideoError      * @see VideoEvent      * @see VideoProgressEvent      *     * @includeExample examples/FLVPlaybackExample.as -noswf     *      * @tiptext	FLVPlayback     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class FLVPlayback extends Sprite
	{
		/**
		 * bounding box movie clip inside of component on stage		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var boundingBox_mc : DisplayObject;
		private var preview_mc : MovieClip;
		private var previewImage_mc : Loader;
		private var previewImageUrl : String;
		private var isLivePreview : Boolean;
		private var livePreviewWidth : Number;
		private var livePreviewHeight : Number;
		private var _componentInspectorSetting : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var videoPlayers : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var videoPlayerStates : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var videoPlayerStateDict : Dictionary;
		private var _activeVP : uint;
		private var _visibleVP : uint;
		private var _topVP : uint;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var uiMgr : UIManager;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var cuePointMgrs : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _firstStreamReady : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var _firstStreamShown : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var resizingNow : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		local var skinShowTimer : Timer;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static const DEFAULT_SKIN_SHOW_TIMER_INTERVAL : Number = 2000;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static const skinShowTimerInterval : Number = DEFAULT_SKIN_SHOW_TIMER_INTERVAL;
		private var _align : String;
		private var _autoRewind : Boolean;
		private var _bufferTime : Number;
		private var _idleTimeout : Number;
		private var _aspectRatio : Boolean;
		private var _playheadUpdateInterval : Number;
		private var _progressInterval : Number;
		private var _origWidth : Number;
		private var _origHeight : Number;
		private var _scaleMode : String;
		private var _seekToPrevOffset : Number;
		private var _soundTransform : SoundTransform;
		private var _volume : Number;
		private var __forceNCMgr : NCManager;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SEEK_TO_PREV_OFFSET_DEFAULT : Number = 1;

		/**
		 * A number that specifies the current <code>playheadTime</code> as a percentage of the 		 * <code>totalTime</code> property. If you access this property, it contains the percentage 		 * of playing time that has elapsed. If you set this property, it causes a seek 		 * operation to the point representing that percentage of the FLV file's playing time.		 * 		 * <p>The value of this property is relative to the value of the <code>totalTime</code> 		 * property.</p>		 * 		 * @throws fl.video.VideoError If you specify a percentage that is invalid or if the 		 * <code>totalTime</code> property is		 * undefined, <code>null</code>, or less than or equal to zero.	          * 		 * @tiptext playheadPercentage method         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playheadPercentage () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set playheadPercentage (percent:Number) : void;
		/**
		 * Only for live preview. Reads in a PNG file for the preview.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set preview (filename:String) : void;
		/**
		 * This flag is set by code automatically generated by the		 * authoring tool.  It is turned on before properties		 * automatically set by the component inspector start getting		 * set and it is turned off after the last property is set.		 * 		 * With AS2 these properties were set before the constructor		 * was called, which gave us a way to know that was how they		 * were set, although it was generally annoying.  Now we		 * have a way to know in AS3 and we don't have to deal with		 * having properties called before our constructor.  Cool!		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set componentInspectorSetting (b:Boolean) : void;
		/**
		 * A number that specifies which video player instance is affected by other application                 * programming interfaces (APIs). 		 * Use this property to manage multiple FLV file streams. 		 * 		 * <p>This property does not make the video player visible; 		 * use the <code>visibleVideoPlayerIndex</code> property to do that.</p>		 * 		 * <p>A new video player is created the first time <code>activeVideoPlayerIndex</code> 		 * is set to a number. When the new video player is created, 		 * its properties are set to the value of the default video player 		 * (<code>activeVideoPlayerIndex == 0</code>) except for <code>source</code>, 		 * <code>totalTime</code>, and <code>isLive</code>, which are always set to the 		 * default values (empty string, NaN, and <code>false</code>, respectively), 		 * and <code>autoPlay</code>, which is always <code>false</code> (the default is <code>true</code> 		 * only for the default video player, 0). The <code>cuePoints</code> 		 * property has no effect, as it would have no effect 		 * on a subsequent load into the default video player.</p>		 * 		 * <p>APIs that control volume, positioning, dimensions, visibility, 		 * and UI controls are always global, and their behavior is not 		 * affected by setting <code>activeVideoPlayerIndex</code>. Specifically, setting 		 * the <code>activeVideoPlayerIndex</code> property does not affect the following 		 * properties and methods:</p>				 * 		 * <strong>Properties and Methods Not Affected by <code>activeVideoPlayerIndex</code></strong>		 * <table class="innertable" width="100%">		 * 		<tr><td><code>backButton</code></td><td><code>playPauseButton</code></td><td><code>skin</code></td><td><code>width</code></td></tr>		 * 		<tr><td><code>bufferingBar</code></td><td><code>scaleX</code></td><td><code>stopButton</code></td><td><code>x</code></td></tr>		 * 		<tr><td><code>bufferingBarHidesAndDisablesOthers</code></td><td><code>transform</code></td><td><code>y</code></td><td><code>setSize()</code></td></tr>		 * 		<tr><td><code>forwardButton</code></td><td><code>scaleY</code></td><td><code>visible</code></td><td><code>setScale()</code></td></tr> 		 * 		<tr><td><code>height</code></td><td><code>seekBar</code></td><td><code>volume</code></td><td><code>fullScreenBackgroundColor</code></td></tr> 		 * 		<tr><td><code>muteButton</code></td><td><code>seekBarInterval</code></td><td><code>volumeBar</code></td><td><code>fullScreenButton</code></td></tr> 		 * 		<tr><td><code>pauseButton</code></td><td><code>seekBarScrubTolerance</code></td><td><code>volumeBarInterval</code></td><td><code>fullScreenSkinDelay</code></td></tr> 		 * 		<tr><td><code>playButton</code></td><td><code>seekToPrevOffset</code></td><td><code>volumeBarScrubTolerance</code></td><td><code>fullScreenTakeOver</code></td></tr>		 *  		<tr><td><code>registrationX</code></td><td><code>registrationY</code></td><td><code>registrationWidth</code></td><td><code>registrationHeight</code></td></tr>		 *  		<tr><td><code>skinBackgroundAlpha</code></td><td><code>skinBackgroundColor</code></td><td><code>skinFadeTime</code></td><td></td></tr>		 * </table>		 *                  * <p><b>Note</b>: The <code>visibleVideoPlayerIndex</code> property, not the 		 * <code>activeVideoPlayerIndex</code> property, determines which 		 * video player the skin controls. Additionaly, APIs that control dimensions 		 * do interact with the <code>visibleVideoPlayerIndex</code> property.</p>		 * 		 * <p>The remaining APIs target a specific video player based on the 		 * setting of <code>activeVideoPlayerIndex</code>.</p>		 * 		 * <p>To load a second FLV file in the background, set <code>activeVideoPlayerIndex</code>		 * to 1 and call the <code>load()</code> method. When you are 		 * ready to show this FLV file and hide the first one, 		 * set <code>visibleVideoPlayerIndex</code> to 1.</p>		 * 		                  * @see #visibleVideoPlayerIndex                 * @default 0                 * @langversion 3.0                 * @playerversion Flash 9.0.28.0
		 */
		public function get activeVideoPlayerIndex () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set activeVideoPlayerIndex (index:uint) : void;
		/**
		 * Specifies the video layout when the <code>scaleMode</code> property  is set to		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or <code>VideoScaleMode.NO_SCALE</code>.		 * The video dimensions are based on the		 * <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code>, and		 * <code>registrationHeight</code> properties. When you set the <code>align</code> property,		 * values come from the VideoAlign class. The default is <code>VideoAlign.CENTER</code>.		 *		 * @see #scaleMode		 * @see #registrationX		 * @see #registrationY		 * @see #registrationWidth		 * @see #registrationHeight		 * @see VideoAlign         * @see VideoPlayer#align         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get align () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set align (s:String) : void;
		/**
		 * A Boolean value that, if set to <code>true</code>, causes the FLV file to start		 * playing automatically after the <code>source</code> property is set.  If it is set to <code>false</code>,		 * the FLV file loads but does not start playing until the <code>play()</code>		 * or <code>playWhenEnoughDownloaded()</code> method is called.  		 * 		 * <p>Playback starts immediately when you are streaming an FLV file from Flash Media Server (FMS) and the 		 * <code>autoPlay</code> property is set to <code>true</code>. However, 		 * when loading an FLV file by progressive download, playback starts only		 * when enough of the FLV file has download so that the FLV file can play from start		 * to finish. </p>		 * 		 * <p>To force playback before enough of the FLV file has downloaded,		 * call the <code>play()</code> method with no parameters. If playback		 * has begun and you want to return to the state of waiting		 * for enough to download and then automatically begin playback, 		 * call the <code>pause()</code> method, and then the 		 * <code>playWhenEnoughDownloaded()</code> method.</p>		 * 		 * <p>If you set the property to <code>true</code> between the loading of new FLV files, 		 * it has no effect until the <code>source</code> property is set.</p>		 * 		 * <p>Setting the <code>autoPlay</code> property to <code>true</code> and		 * then setting the <code>source</code> property to a URL has the same		 * effect as calling the <code>play()</code> method with that URL.</p>		 * 		 * <p>Calling the <code>load()</code> method with a URL has the same effect		 * as setting the <code>source</code> property to that URL with the		 * <code>autoPlay</code> property set to <code>false</code>.</p>		 *		 * @see #source		 * @see #load()		 * @see #play()		 * @see #playWhenEnoughDownloaded()         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoPlay () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoPlay (flag:Boolean) : void;
		/**
		 * A Boolean value that, if <code>true</code>, causes the FLV file to rewind to Frame 1 when 		 * play stops, either because the player reached the end of the stream or the 		 * <code>stop()</code> method was called. This property is meaningless for live streams.          * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoRewind () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoRewind (flag:Boolean) : void;
		/**
		 * A number that specifies the bits per second at which to transfer the FLV file.	 *	 * <p>When streaming from a Flash Video Streaming service that supports	 * native bandwidth detection, you can provide a SMIL file that	 * describes how to switch between multiple streams based on the	 * bandwidth. Depending on your FVSS, bandwidth may automatically be	 * detected, and if this value is set, it is ignored.</p>	 *	 * <p>When doing HTTP progressive download, you can use the same SMIL	 * format, but you must set the bitrate as there is no automatic         * detection.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bitrate () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set bitrate (b:Number) : void;
		/**
		 * A Boolean value that is <code>true</code> if the video is in a buffering state.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get buffering () : Boolean;
		/**
		 * Buffering bar control. This control is displayed when the FLV file is in          * a loading or buffering state.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bufferingBar () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set bufferingBar (s:Sprite) : void;
		/**
		 * If set to <code>true</code>, hides the SeekBar control and disables the 		 * Play, Pause, PlayPause, BackButton and ForwardButton controls while the 		 * FLV file is in the buffering state. This can be useful to prevent a 		 * user from using these controls to try to speed up playing the FLV file          * when it is downloading or streaming over a slow connection.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bufferingBarHidesAndDisablesOthers () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set bufferingBarHidesAndDisablesOthers (b:Boolean) : void;
		/**
		 * BackButton playback control. Clicking calls the 		 * <code>seekToPrevNavCuePoint()</code> method.		 *          * @see #seekToPrevNavCuePoint()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get backButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set backButton (s:Sprite) : void;
		/**
		 * A number that specifies the number of seconds to buffer in memory before 		 * beginning to play a video stream. For FLV files streaming over RTMP, 		 * which are not downloaded and buffer only in memory, it can be important 		 * to increase this setting from the default value of 0.1. For a progressively 		 * downloaded FLV file over HTTP, there is little benefit to increasing this 		 * value although it could improve viewing a high-quality video on an older, 		 * slower computer.		 * 		 * <p>For prerecorded (not live) video, do not set the <code>bufferTime</code> 		 * property to <code>0</code>: 		 * use the default buffer time or increase the buffer time.</p>		 * 		 * <p>This property does not specify the amount of the FLV file to download before 		 * starting playback. </p>		 * 		 * @see VideoPlayer#bufferTime          * @see #isLive          *		 * @default 0.1         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bufferTime () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set bufferTime (aTime:Number) : void;
		/**
		 * A number that indicates the extent of downloading, in number of bytes, for an 		 * HTTP download.  Returns 0 when there		 * is no stream, when the stream is from Flash Media Server (FMS), or if the information		 * is not yet available. The returned value is useful only for an HTTP download.		 *		 * @tiptext Number of bytes already loaded         * @helpid 3455         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesLoaded () : uint;
		/**
		 * A number that specifies the total number of bytes downloaded for an HTTP download.  		 * Returns 0 when there is no stream, when the stream is from Flash Media Server (FMS), or if 		 * the information is not yet available. The returned value is useful only 		 * for an HTTP download. 		 *		 * @tiptext Number of bytes to be loaded         * @helpid 3456         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesTotal () : uint;
		/**
		 * A string that specifies the URL of the FLV file to stream and how to stream it.		 * The URL can be an HTTP URL to an FLV file, an RTMP URL to a stream, or an 		 * HTTP URL to an XML file.		 *		 * <p>If you set this property through the Component inspector or the Property inspector, the		 * FLV file begins loading and playing at the next "<code>enterFrame</code>" event.		 * The delay provides time to set the <code>isLive</code>, <code>autoPlay</code>, 		 * and <code>cuePoints</code> properties, 		 * among others, which affect loading. It also allows ActionScript that is placed 		 * on the first frame to affect the FLVPlayback component before it starts playing.</p>		 *		 * <p>If you set this property through ActionScript, it immediately calls the		 * <code>VideoPlayer.load()</code> method when the <code>autoPlay</code> property is		 * set to <code>false</code> or it calls the <code>VideoPlayer.play()</code> method when		 * the <code>autoPlay</code> property is set to <code>true</code>.  The <code>autoPlay</code>, 		 * <code>totalTime</code>, and <code>isLive</code> properties affect how the new FLV file is 		 * loaded, so if you set these properties, you must set them before setting the		 * <code>source</code> property.</p>		 * 		 * <p>Set the <code>autoPlay</code> property to <code>false</code> to prevent the new 		 * FLV file from playing automatically.</p>		 *		 * @see #autoPlay		 * @see #isLive		 * @see #totalTime		 * @see #load()		 * @see #play()		 * @see VideoPlayer#load()         * @see VideoPlayer#play()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set source (url:String) : void;
		/**
		 * An array that describes ActionScript cue points and disabled embedded 		 * FLV file cue points. This property is created specifically for use by 		 * the Component inspector and Property inspector. It does not work if it is 		 * set any other way. Its value has an effect only on the first FLV file 		 * loaded and only if it is loaded by setting the <code>source</code> 		 * property in the Component inspector or the Property inspector.		 * 		 * <p><b>Note</b>: This property is not accessible in ActionScript. To 		 * access cue point information in ActionScript, use the <code>metadata</code> property.</p>		 * 		 * <p>To add, remove, enable or disable cue points with ActionScript, 		 * use the <code>addASCuePoint()</code>, <code>removeASCuePoint()</code>, or		 * <code>setFLVCuePointEnabled()</code> methods.</p>		 *		 * @see #source		 * @see #addASCuePoint()		 * @see #removeASCuePoint()         * @see #setFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set cuePoints (cuePointsArray:Array) : void;
		/**
		 * Forward button control. Clicking calls the		 * <code>seekToNextNavCuePoint()</code> method.		 *          * @see #seekToNextNavCuePoint()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get forwardButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set forwardButton (s:Sprite) : void;
		/**
		 * Background color used when in full-screen takeover		 * mode.  This color is visible if the video does		 * not cover the entire screen based on the <code>scaleMode</code>		 * property value.         *         * @default 0x000000         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get fullScreenBackgroundColor () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set fullScreenBackgroundColor (c:uint) : void;
		/**
		 * FullScreen button control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get fullScreenButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set fullScreenButton (s:Sprite) : void;
		/**
		 * Specifies the delay time in milliseconds to hide the skin. 		 * When in full-screen takeover mode, if the <code>skinAutoHide</code> property		 * is <code>true</code>, autohiding is triggered when the user doesn't move the		 * mouse for more than the seconds indicated by the <code>fullScreenSkinDelay</code> 		 * property. If the mouse is over the skin itself, autohiding is not triggered.         *		 * @default 3000 milliseconds (3 seconds)         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get fullScreenSkinDelay () : int;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set fullScreenSkinDelay (i:int) : void;
		/**
		 * When the stage enters full-screen mode, the		 * FLVPlayback component is on top of all		 * content and takes over the entire screen.  When		 * the stage exits full-screen mode, the screen returns to 		 * how it was before. 		 *		 * <p>The recommended settings for full-screen		 * takeover mode are <code>scaleMode = VideoScaleMode.MAINTAIN_ASPECT_RATIO</code>		 * and <code>align = VideoAlign.CENTER</code>.</p>		 *		 * <p>If the SWF file with the FLVPlayback component		 * is loaded and does not have access to the		 * stage because of security restrictions, full-screen		 * takeover mode does not function. No         * errors are thrown.</p>         *		 * @default true         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get fullScreenTakeOver () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set fullScreenTakeOver (v:Boolean) : void;
		/**
		 * A number that specifies the height of the FLVPlayback instance. 		 * This property affects only the height of the FLVPlayback instance 		 * and does not include the height of a skin SWF file that might be loaded.		 * Setting the height property also sets the <code>registrationHeight</code> property		 * to the same value.		 * 		 * @see #setSize()         * @helpid 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get height () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set height (h:Number) : void;
		/**
		 * The amount of time, in milliseconds, before Flash terminates an idle connection 		 * to Flash Media Server (FMS) because playing paused or stopped. This property has no effect on an 		 * FLV file downloading over HTTP.		 * 		 * <p>If this property is set when a video stream is already idle, it restarts the 		 * timeout period with the new value.</p>		 *          * @default 300,000 milliseconds (5 minutes)         *         * @see #event:close         *         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get idleTimeout () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set idleTimeout (aTime:Number) : void;
		/**
		 * A Boolean value that is <code>true</code> if the FLV file is streaming from 		 * Flash Media Server (FMS) using RTMP. Its value is <code>false</code> for any other FLV file source. 		 *         * @see VideoPlayer#isRTMP         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get isRTMP () : Boolean;
		/**
		 * A Boolean value that is <code>true</code> if the video stream is live. This property 		 * is effective only when streaming from Flash Media Server (FMS) or other Flash Video Streaming Service (FVSS). The value of this 		 * property is ignored for an HTTP download.		 * 		 * <p>If you set this property between loading new FLV files, it has no 		 * effect until the <code>source</code> property is set for the new FLV file.</p>		 * 		 * <p>Set the <code>isLive</code> property to <code>false</code> when sending a prerecorded video 		 * stream to the video player and to <code>true</code> when sending real-time data 		 * such as a live broadcast. For better performance when you set 		 * the <code>isLive</code> property to <code>false</code>, do not set the 		 * <code>bufferTime</code> property to <code>0</code>.</p>		 *		 * @see #bufferTime  		 * @see #source          * @see VideoPlayer#isLive          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get isLive () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set isLive (flag:Boolean) : void;
		/**
		 * An object that is a metadata information packet that is received from a call to 		 * the <code>NetSteam.onMetaData()</code> callback method, if available.  		 * Ready when the <code>metadataReceived</code> event is dispatched.		 * 		 * <p>If the FLV file is encoded with the Flash 8 encoder, the <code>metadata</code> 		 * property contains the following information. Older FLV files contain 		 * only the <code>height</code>, <code>width</code>, and <code>duration</code> values.</p>		 * 		 * <table class="innertable" width="100%">		 * 	<tr><th><b>Parameter</b></th><th><b>Description</b></th></tr>		 * 		<tr><td><code>canSeekToEnd</code></td><td>A Boolean value that is <code>true</code> if the FLV file is encoded with a keyframe on the last frame that allows seeking to the end of a progressive download movie clip. It is <code>false</code> if the FLV file is not encoded with a keyframe on the last frame.</td></tr>		 * 		<tr><td><code>cuePoints</code></td><td>An array of objects, one for each cue point embedded in the FLV file. Value is undefined if the FLV file does not contain any cue points. Each object has the following properties:	     *   			 * 			<ul>		 * 				<li><code>type</code>&#x2014;A string that specifies the type of cue point as either "navigation" or "event".</li>		 * 				<li><code>name</code>&#x2014;A string that is the name of the cue point.</li>		 * 				<li><code>time</code>&#x2014;A number that is the time of the cue point in seconds with a precision of three decimal places (milliseconds).</li>		 * 				<li><code>parameters</code>&#x2014;An optional object that has name-value pairs that are designated by the user when creating the cue points.</li>		 * 			</ul>		 * 		</td></tr>		 * <tr><td><code>audiocodecid</code></td><td>A number that indicates the audio codec (code/decode technique) that was used.</td></tr>		 * <tr><td><code>audiodelay</code></td><td>A number that represents time <code>0</code> in the source file from which the FLV file was encoded. 		 * <p>Video content is delayed for the short period of time that is required to synchronize the audio. For example, if the <code>audiodelay</code> value is <code>.038</code>, the video that started at time <code>0</code> in the source file starts at time <code>.038</code> in the FLV file.</p> 		 * <p>Note that the FLVPlayback and VideoPlayer classes compensate for this delay in their time settings. This means that you can continue to use the time settings that you used in your the source file.</p></td></tr> 		 * <tr><td><code>audiodatarate</code></td><td>A number that is the kilobytes per second of audio.</td></tr> 		 * <tr><td><code>videocodecid</code></td><td>A number that is the codec version that was used to encode the video.</td></tr> 		 * <tr><td><code>framerate</code></td><td>A number that is the frame rate of the FLV file.</td></tr> 		 * <tr><td><code>videodatarate</code></td><td>A number that is the video data rate of the FLV file.</td></tr>		 * <tr><td><code>height</code></td><td>A number that is the height of the FLV file.</td></tr> 		 * <tr><td><code>width</code></td><td>A number that is the width of the FLV file.</td></tr> 		 * <tr><td><code>duration</code></td><td>A number that specifies the duration of the FLV file in seconds.</td></tr>		 * </table>		 *         * @see VideoPlayer#metadata         * @see http://livedocs.adobe.com/flash/9.0/main/00001037.html Working with cue points         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get metadata () : Object;
		/**
		 * A Boolean value that is <code>true</code> if a metadata packet has been 		 * encountered and processed or if the FLV file was encoded without the 		 * metadata packet. In other words, the value is <code>true</code> if the metadata is 		 * received, or if you are never going to get any metadata. So, you 		 * know whether you have the metadata; and if you don't have the metadata, 		 * you know not to wait around for it. If you just want to know whether 		 * or not you have metadata, you can check the value with:  		 * 		 * <listing>FLVPlayback.metadata != null</listing>		 * 		 * <p>Use this property to check whether you can retrieve useful 		 * information with the methods for finding and enabling or 		 * disabling cue points (<code>findCuePoint</code>, <code>findNearestCuePoint</code>,		 * <code>findNextCuePointWithName</code>, <code>isFLVCuePointEnabled</code>).</p>		 *		 * @see #findCuePoint()		 * @see #findNearestCuePoint()		 * @see #findNextCuePointWithName()         * @see #isFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get metadataLoaded () : Boolean;
		/**
		 * Mute button control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get muteButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set muteButton (s:Sprite) : void;
		/**
		 * An INCManager object that provides access to an instance of the class implementing 		 * <code>INCManager</code>, which is an interface to the NCManager class.		 * 		 * <p>You can use this property to implement a custom INCManager that requires 		 * custom initialization.</p>		 *         * @see VideoPlayer#ncMgr         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get ncMgr () : INCManager;
		/**
		 * Pause button control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get pauseButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set pauseButton (s:Sprite) : void;
		/**
		 * A Boolean value that is <code>true</code> if the FLV file is in a paused state.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get paused () : Boolean;
		/**
		 * Play button control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set playButton (s:Sprite) : void;
		/**
		 * A number that is the current playhead time or position, measured in seconds, 		 * which can be a fractional value. Setting this property triggers a seek and 		 * has all the restrictions of a seek.		 * 		 * <p>When the playhead time changes, which occurs once every .25 seconds 		 * while the FLV file plays, the component dispatches the <code>playheadUpdate</code>		 * event.</p>		 * 		 * <p>For several reasons, the <code>playheadTime</code> property might not have the expected 		 * value immediately after you call one of the seek methods or set <code>playheadTime</code> 		 * to cause seeking. First, for a progressive download, you can seek only to a 		 * keyframe, so a seek takes you to the time of the first keyframe after the 		 * specified time. (When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.) Second, 		 * seeking is asynchronous, so if you call a seek method or set the 		 * <code>playheadTime</code> property, <code>playheadTime</code> does not update immediately. 		 * To obtain the time after the seek is complete, listen for the <code>seek</code> event, 		 * which does not fire until the <code>playheadTime</code> property has updated.</p>		 *		 * @tiptext Current position of the playhead in seconds		 * @helpid 3463		 * @see #seek()         * @see VideoPlayer#playheadTime         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playheadTime () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set playheadTime (position:Number) : void;
		/**
		 * A number that is the amount of time, in milliseconds, between each 		 * <code>playheadUpdate</code> event. Setting this property while the FLV file is 		 * playing restarts the timer. 		 * 		 * <p>Because ActionScript cue points start on playhead updates, lowering 		 * the value of the <code>playheadUpdateInterval</code> property can increase the accuracy 		 * of ActionScript cue points.</p>		 * 		 * <p>Because the playhead update interval is set by a call to the global 		 * <code>setInterval()</code> method, the update cannot fire more frequently than the 		 * SWF file frame rate, as with any interval that is set this way. 		 * So, as an example, for the default frame rate of 12 frames per second, 		 * the lowest effective interval that you can create is approximately 		 * 83 milliseconds, or one second (1000 milliseconds) divided by 12.</p>		 *         * @see VideoPlayer#playheadUpdateInterval         * @default 250         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playheadUpdateInterval () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set playheadUpdateInterval (aTime:Number) : void;
		/**
		 * A Boolean value that is <code>true</code> if the FLV file is in the playing state.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playing () : Boolean;
		/**
		 * Play/pause button control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get playPauseButton () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set playPauseButton (s:Sprite) : void;
		/**
		 * A number that specifies the height of the source FLV file. This information 		 * is not valid immediately upon calling the <code>play()</code> or <code>load()</code> 		 * methods. It is valid when the <code>ready</code> event starts. If the value of the 		 * <code>scaleMode</code> property is <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code>		 * or <code>VideoScaleMode.NO_SCALE</code>, it is best to read 		 * the value after the <code>layout</code> event is dispatched. This property returns		 * -1 if no information is available yet.		 * 		 * @see #scaleMode		 * @tiptext The preferred height of the display         * @helpid 3465         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get preferredHeight () : int;
		/**
		 * Gives the width of the source FLV file. This information is not valid immediately 		 * when the <code>play()</code> or <code>load()</code> methods are called; it is valid 		 * after the <code>ready</code> event is dispatched. If the value of the 		 * <code>scaleMode</code> property is <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code>		 * or <code>VideoScaleMode.NO_SCALE</code>, it is best to read 		 * the value after the <code>layout</code> event is dispatched. This property returns		 * -1 if no information is available yet.		 * 		 * @see #scaleMode		 * @tiptext The preferred width of the display         * @helpid 3466         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get preferredWidth () : int;
		/**
		 * A number that is the amount of time, in milliseconds, between each <code>progress</code> 		 * event. If you set this property while the video stream is playing, the timer restarts. 		 * 		 *         * @see VideoPlayer#progressInterval         * @default 250         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get progressInterval () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set progressInterval (aTime:Number) : void;
		/**
		 * @copy fl.video.VideoPlayer#registrationX         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationX () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set registrationX (x:Number) : void;
		/**
		 * @copy fl.video.VideoPlayer#registrationY         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationY () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set registrationY (y:Number) : void;
		/**
		 * @copy fl.video.VideoPlayer#registrationWidth         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationWidth () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set registrationWidth (w:Number) : void;
		/**
		 * @copy fl.video.VideoPlayer#registrationHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get registrationHeight () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set registrationHeight (h:Number) : void;
		/**
		 * Specifies how the video will resize after loading.  If set to		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code>, maintains the		 * video aspect ratio within the rectangle defined by		 * <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code> and		 * <code>registrationHeight</code>.  If set to		 * <code>VideoScaleMode.NO_SCALE</code>, causes the video to size automatically		 * to the dimensions of the source FLV file.  If set to		 * <code>VideoScaleMode.EXACT_FIT</code>, causes the dimensions of		 * the source FLV file to be ignored and the video is stretched to		 * fit the rectangle defined by		 * <code>registrationX</code>, <code>registrationY</code>,		 * <code>registrationWidth</code> and		 * <code>registrationHeight</code>. If this is set		 * after an FLV file has been loaded an automatic layout will start		 * immediately.  Values come from		 * <code>VideoScaleMode</code>.		 *		 * @see #preferredHeight		 * @see #preferredWidth		 * @see VideoScaleMode		 * @default VideoScaleMode.MAINTAIN_ASPECT_RATIO         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scaleMode () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scaleMode (s:String) : void;
		/**
		 * A number that is the horizontal scale. The standard scale is 1.		 *		 * @see #setScale()		 * @tiptext Specifies the horizontal scale factor         * @helpid 3974         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scaleX () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scaleX (xs:Number) : void;
		/**
		 * A number that is the vertical scale. The standard scale is 1.		 *		 * @see #setScale()		 * @tiptext Specifies the vertical scale factor         * @helpid 3975         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scaleY () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scaleY (ys:Number) : void;
		/**
		 * A Boolean value that is <code>true</code> if the user is scrubbing with the SeekBar 		 * and <code>false</code> otherwise. 		 * 		 * <p>Scrubbing refers to grabbing the handle of the SeekBar and dragging          * it in either direction to locate a particular scene in the FLV file.</p>          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scrubbing () : Boolean;
		/**
		 * The SeekBar control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get seekBar () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set seekBar (s:Sprite) : void;
		/**
		 * A number that specifies, in milliseconds, how often to check the SeekBar handle 		 * when scrubbing. 		 * 		 * <p>Because this interval is set by a call to the global <code>setInterval()</code> method, 		 * the update cannot start more frequently than the SWF file frame rate. So, for 		 * the default frame rate of 12 frames per second, for example, the lowest 		 * effective interval that you can create is approximately 83 milliseconds,          * or 1 second (1000 milliseconds) divided by 12.</p>         *		 * @default 250         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get seekBarInterval () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set seekBarInterval (s:Number) : void;
		/**
		 * A number that specifies how far a user can move the SeekBar handle before an 		 * update occurs. The value is specified as a percentage, ranging from 1 to 100. 		 *          * @default 5         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get seekBarScrubTolerance () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set seekBarScrubTolerance (s:Number) : void;
		/**
		 * The number of seconds that the <code>seekToPrevNavCuePoint()</code> method uses 		 * when it compares its time against the previous cue point. The method uses this 		 * value to ensure that, if you are just ahead of a cue point, you can hop 		 * over it to the previous one and avoid going to the same cue point that          * just occurred.         *         * @default 1         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get seekToPrevOffset () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set seekToPrevOffset (s:Number) : void;
		/**
		 * A string that specifies the URL to a skin SWF file. This string could contain a 		 * file name, a relative path such as Skins/MySkin.swf, or an absolute URL such          * as http://www.&#37;somedomain&#37;.com/MySkin.swf.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skin () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set skin (s:String) : void;
		/**
		 * A Boolean value that, if <code>true</code>, hides the component skin when the mouse 		 * is not over the video. This property affects only skins that are loaded by setting 		 * the <code>skin</code> property and not a skin that you create from the FLVPlayback 		 * Custom UI components.		 *		 * <p>When the component is in full-screen takeover mode and the skin is one that does not lay over		 * the video, then <code>skinAutoHide</code> mode is turned on automatically. Setting <code>skinAutoHide = false</code>		 * after you enter full-screen mode overrides this behavior. Also when the component is		 * in full-screen takeover mode, autohiding is triggered if the user doesn't move the         * mouse for more than <code>fullScreenSkinDelay</code> seconds, unless the mouse is over the skin itself.</p>         *         * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skinAutoHide () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set skinAutoHide (b:Boolean) : void;
		/**
		 * The alpha for the background of the skin. The <code>skinBackgroundAlpha</code>		 * property works only with SWF files that have skins loaded by using the		 * <code>skin</code> property and with skins that support setting the color		 * and alpha. You can set the <code>skinBackgroundAlpha</code> property		 * to a number between 0.0 and 1.0. The default is the last value chosen by the		 * user as the default.		 *		 * <p>To get the skin colors that come with the ActionScript 2.0 FLVPlayback component,		 * use the following values for the		 * <code>skinBackgroundAlpha</code> and <code>skinBackgroundColor</code> properties:		 * Arctic - 0.85, 0x47ABCB;		 * Clear - 0.20, 0xFFFFFF; Mojave - 0.85, 0xBFBD9F; Steel - 0.85, 0x666666. 		 * The default is .85.</p>		 *                 *		 * @see #skin         * @see #skinBackgroundColor         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skinBackgroundAlpha () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set skinBackgroundAlpha (a:Number) : void;
		/**
		 * The color for the background of the skin (0xRRGGBB).  The <code>skinBackgroundColor</code>		 * property works only with SWF files that have skins loaded by using the		 * <code>skin</code> property and with skins that support setting the color		 * and alpha. The default is the last value chosen by the		 * user as the default.		 *		 * <p>To get the skin colors that come with the ActionScript 2.0 FLVPlayback component,		 * use the following values for the		 * <code>skinBackgroundAlpha</code> and <code>skinBackgroundColor</code> properties:		 * Arctic - 0.85, 0x47ABCB;		 * Clear - 0.20, 0xFFFFFF; Mojave - 0.85, 0xBFBD9F; Steel - 0.85, 0x666666. 		 * The default is 0x47ABCB.</p>		 *                 *		 * @see #skin         * @see #skinBackgroundAlpha         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skinBackgroundColor () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set skinBackgroundColor (c:uint) : void;
		/**
		 * The amount of time in milliseconds that it takes for the skin to fade in or fade out when 		 * hiding or showing. Hiding and showing occurs because the <code>skinAutoHide</code> 		 * property is set to <code>true</code>. Set the <code>skinFadeTime</code> property to          * 0 to eliminate the fade effect.          * 		 * @default 500 milliseconds (.5 seconds)         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skinFadeTime () : int;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set skinFadeTime (i:int) : void;
		/**
		 * This property specifies the largest multiple that FLVPlayback will use to scale up	* its skin when it enters full screen mode with a Flash Player that supports	* hardware acceleration. With hardware acceleration, the video and the skin are scaled	* by the same factor. By default, FLVPlayback renders the video at its native dimensions	* and allows hardware acceleration to do the rest of the scaling. If, for example,	* your video has dimensions of 640 x 512 and it goes to full screen size on a monitor with	* a resolution of 1280 x 1024, the video and the skin will be scaled up to twice their	* size.	*	* <p>This property enables you to restrict scaling of the skin when hardware acceleration 	* is used, based on the specific content that is being scaled and your aesthetic	* tastes regarding the appearance of large skins. To limit scaling of the skin to the 	* specified multiplier, FLVPlayback uses a mix of software and hardware scaling for the skin, 	* which can have a negative impact on the performance of video playback and the	* appearance of the FLV.</p>	* 	* <p>For example, if this property was set to 5.0 or greater, going to full screen on a 	* monitor that has a resolution of 1600 x 1200 with a video that has dimensions of	* 320 x 240 would scale the skin five times. If this property was set to 2.5, the player 	* would render the video (but not the skin) at 640 x 480, twice it's original size, and hardware acceleration	* would do the remainder of the scaling (640 x 2.5 = 1600 and 480 x 2.5 = 1200).</p>	*	* <p>Setting this property after full screen mode has already	* been entered has no effect until the next time FLVPlayback enters full screen	* mode.</p>	*	* <p>If the FLV is large (for example, 640 pixels wide or more, 480 pixels 	* tall or more) you should not set this property to a small value because it	* could cause noticeable performance problems on large monitors.</p>	*    * <p><strong>Player Version</strong>: Flash Player 9 <a target="mm_external" href="http://www.adobe.com/go/fp9_update3">Update 3</a>.</p>    *	* @default 4.0	*	* @see #enterFullScreenDisplayState()	* @see flash.display.Stage#displayState 	*	* @includeExample examples/FLVPlayback.skinScaleMaximum.1.as -noswf	*        * @langversion 3.0        * @internal Flash 9.0.xx.0
		 */
		public function get skinScaleMaximum () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set skinScaleMaximum (value:Number) : void;
		/**
		 * Provides direct access to the		 * <code>VideoPlayer.soundTransform</code> property to expose		 * more sound control. You need to set this property for changes to take effect, 		 * or you can get the value of this property to get a copy of the current settings. 		 *		 * @see #volume         * @see VideoPlayer#soundTransform         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get soundTransform () : SoundTransform;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set soundTransform (st:SoundTransform) : void;
		/**
		 * A string that specifies the state of the component. This property is set by the 		 * <code>load()</code>, <code>play()</code>, <code>stop()</code>, <code>pause()</code>, 		 * and <code>seek()</code> methods. 		 * 		 * <p>The possible values for the state property are: <code>"buffering"</code>, <code>"connectionError"</code>, 		 * <code>"disconnected"</code>, <code>"loading"</code>, <code>"paused"</code>, <code>"playing"</code>, <code>"rewinding"</code>, <code>"seeking"</code>, and 		 * <code>"stopped"</code>. You can use the FLVPlayback class properties to test for 		 * these states. </p>		 *		 * @see VideoState#DISCONNECTED		 * @see VideoState#STOPPED		 * @see VideoState#PLAYING		 * @see VideoState#PAUSED		 * @see VideoState#BUFFERING		 * @see VideoState#LOADING		 * @see VideoState#CONNECTION_ERROR		 * @see VideoState#REWINDING         * @see VideoState#SEEKING         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get state () : String;
		/**
		 * A Boolean value that is <code>true</code> if the state is responsive. If the state is 		 * unresponsive, calls to the <code>play()</code>, <code>load()</code>, <code>stop()</code>, <code>pause()</code> and <code>seek()</code>		 * methods are queued and executed later, when the state changes to a 		 * responsive one. Because these calls are queued and executed later, 		 * it is usually not necessary to track the value of the <code>stateResponsive </code>		 * property. The responsive states are: 		 * <code>stopped</code>, <code>playing</code>, <code>paused</code>, and <code>buffering</code>. 		 *         * @see VideoPlayer#stateResponsive         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get stateResponsive () : Boolean;
		/**
		 * The Stop button control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get stopButton () : Sprite;
		public function set stopButton (s:Sprite) : void;
		/**
		 * A Boolean value that is <code>true</code> if the state of the FLVPlayback instance is stopped.          *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get stopped () : Boolean;
		/**
		 * A number that is the total playing time for the video in seconds.		 *		 * <p>When streaming from Flash Media Server (FMS) and using the default		 * <code>NCManager</code>, this value is determined		 * automatically by server-side APIs, and that value 		 * overrides anything set through this property or gathered		 * from metadata. The property is ready for reading when the		 * <code>stopped</code> or <code>playing</code> state is reached after setting the		 * <code>source</code> property. This property is meaningless for live streams		 * from a FMS.</p>		 *		 * <p>With an HTTP download, the value is determined		 * automatically if the FLV file has metadata embedded; otherwise,		 * set it explicitly or it will be NaN.  If you set it		 * explicitly, the metadata value in the stream is		 * ignored.</p>		 *		 * <p>When you set this property, the value takes effect for the next		 * FLV file that is loaded by setting the <code>source</code> property. It has no effect		 * on an FLV file that has already loaded.  Also, this property does not return 		 * the new value passed in until an FLV file is loaded.</p>		 *		 * <p>Playback still works if this property is never set (either		 * explicitly or automatically), but it can cause problems		 * with seek controls.</p>		 *		 * <p>Unless set explicitly, the value will be NaN until it is set to a valid value from metadata.</p>		 *		 * @see #source		 * @tiptext The total length of the FLV file in seconds         * @helpid 3467         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get totalTime () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set totalTime (aTime:Number) : void;
		/**
		 * A number that you can use to manage multiple FLV file streams. 		 * Sets which video player instance is visible, audible, and		 * controlled by the skin or playback controls, while the rest 		 * of the video players are hidden and muted. It does		 * not make the video player the		 * target for most APIs; use the <code>activeVideoPlayerIndex</code> property to do that.		 *		 * <p>Methods and properties that control dimensions interact with this property. 		 * The methods and properties that set the dimensions of the video player 		 * (<code>setScale()</code>, <code>setSize()</code>, <code>width</code>, <code>height</code>, <code>scaleX</code>, <code>scaleY</code>) can be used 		 * for all video players. However, depending on the value of the		 * <code>scaleMode</code> property		 * on those video players, they might have different dimensions. Reading 		 * the dimensions using the <code>width</code>, <code>height</code>, <code>scaleX,</code> and <code>scaleY</code> 		 * properties gives you 		 * the dimensions only of the visible video player. Other video players might have 		 * the same dimensions or might not.</p>		 *		 * <p>To get the dimensions of various video players when they are		 * not visible, listen for the <code>layout</code> event, and store the size		 * value.</p>		 *		 * <p>This property does not have any implications for visibility of the 		 * component as a whole, only which video player is visible when the component is visible.		 * To set visibility for the entire component, use the <code>visible</code> property.</p>		 *		 * @see #activeVideoPlayerIndex         * @see flash.display.DisplayObject#visible DisplayObject.visible         * 		 * @default 0         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get visibleVideoPlayerIndex () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set visibleVideoPlayerIndex (i:uint) : void;
		/**
		 * A number in the range of 0 to 1 that indicates the volume control setting. 		 * @default 1		 *		 * @tiptext The volume setting in value range from 0 to 1.		 * @helpid 3468         * @see #soundTransform         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get volume () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set volume (aVol:Number) : void;
		/**
		 * The volume bar control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get volumeBar () : Sprite;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set volumeBar (s:Sprite) : void;
		/**
		 * A number that specifies, in milliseconds, how often 		 * to check the volume bar handle location when scrubbing.         *         * @default 250         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get volumeBarInterval () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set volumeBarInterval (s:Number) : void;
		/**
		 * A number that specifies how far a user can move the volume bar handle before 		 * an update occurs. The value is expressed as a percentage from 1 to 100.  Set to 0		 * to indicate no scrub tolerance. Always update the volume on the		 * <code>volumeBarInterval</code> property regardless of how far the user moved the handle.         *         * @default 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 *
		 */
		public function get volumeBarScrubTolerance () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set volumeBarScrubTolerance (s:Number) : void;
		/**
		 * A number that specifies the width of the FLVPlayback instance on the Stage. 		 * This property affects only the width of the FLVPlayback instance and does 		 * not include the width of a skin SWF file that might be loaded. Use the 		 * FLVPlayback <code>width</code> property and not the <code>DisplayObject.width</code> property because 		 * the <code>width</code> property might give a different value if a skin SWF file is loaded.		 * Setting the width property also sets the <code>registrationWidth</code> property		 * to the same value.		 *		 * @see #setSize()         * @helpid 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get width () : Number;
		/**
		 * @private (setter)
		 */
		public function set width (w:Number) : void;
		/**
		 * @copy fl.video.VideoPlayer#x         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get x () : Number;
		/**
		 * @private (setter)         *
		 */
		public function set x (x:Number) : void;
		/**
		 * @copy fl.video.VideoPlayer#y         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get y () : Number;
		/**
		 * @private (setter)         *
		 */
		public function set y (y:Number) : void;
		/**
		 * Returns the next available tabIndex value after the FLVPlayback controls. The value is set after the <code>assignTabIndexes</code> method is called.		 * 		 * @return the next available tabIndex after the FLVPlayback controls		 * 		 * @see #assignTabIndexes()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get endTabIndex () : int;
		/**
		 * Returns the first tabIndex value for the FLVPlayback controls. The value is set after the <code>assignTabIndexes</code> method is called.		 * 		 * @return first tabIndex value for the FLVPlayback controls		 * 		 * @see #assignTabIndexes()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get startTabIndex () : int;

		/**
		 * Creates a new FLVPlayback instance. After creating the FLVPlayback 		 * instance, call the <code>addChild()</code> or <code>addChildAt()</code> 		 * method to place the instance on the Stage or another display object container.		 * 		 * @see flash.display.DisplayObjectContainer#addChild() DisplayObjectContainer#addChild()         * 		 * @tiptext FLVPlayback constructor         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function FLVPlayback ();
		/**
		 * Sets width and height simultaneously. Because setting either one,          * individually, can cause automatic resizing, setting them simultaneously          * is more efficient than setting the <code>width</code> and <code>height</code>         * properties individually.         *         * <p>If <code>scaleMode</code> property is set to         * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or <code>VideoScaleMode.NO_SCALE</code>, then         * calling this causes an immediate <code>autolayout</code> event.</p>         *         * @param width A number that specifies the width of the video player.         *          * @param height A number that specifies the height of the video player.         *          * @see #width          * @see #height          * @see VideoScaleMode          *          * @tiptext setSize method         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSize (width:Number, height:Number) : void;
		/**
		 * Sets the <code>scaleX</code> and <code>scaleY</code> properties simultaneously. 		 * Because setting either one, individually, can cause automatic		 * resizing, setting them simultaneously can be more efficient 		 * than setting the <code>scaleX</code> and <code>scaleY</code> properties individually.		 * 		 * <p>If <code>scaleMode</code> property is set to		 * <code>VideoScaleMode.MAINTAIN_ASPECT_RATIO</code> or <code>VideoScaleMode.NO_SCALE</code>, then		 * calling this causes an immediate <code>autolayout</code> event.</p>		 *		 * @param scaleX A number representing the horizontal scale.		 * @param scaleY A number representing the vertical scale.		 * 		 * @see #scaleX                  * @see #scaleY                  * @see VideoScaleMode                 *		 * @tiptext setScale method		 *                  * @langversion 3.0                 * @playerversion Flash 9.0.28.0
		 */
		public function setScale (scaleX:Number, scaleY:Number) : void;
		/**
		 * handles events		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleAutoLayoutEvent (e:AutoLayoutEvent) : void;
		/**
		 * handles events		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleMetadataEvent (e:MetadataEvent) : void;
		/**
		 * handles events		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleVideoProgressEvent (e:VideoProgressEvent) : void;
		/**
		 * handles events		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleVideoEvent (e:VideoEvent) : void;
		/**
		 * Begins loading the FLV file and provides a shortcut for setting the 		 * <code>autoPlay</code> property to <code>false</code> and setting the 		 * <code>source</code>, <code>totalTime</code>, and <code>isLive</code> 		 * properties, if given. If the <code>totalTime</code> and <code>isLive</code> 		 * properties are undefined, they are not set. If the <code>source</code> 		 * property is undefined, <code>null</code>, or an empty string, this method 		 * does nothing.		 *		 * @param source A string that specifies the URL of the FLV file to stream 		 * and how to stream it. The URL can be a local path, an HTTP URL to an FLV file, 		 * an RTMP URL to an FLV file stream, or an HTTP URL to an XML file.		 * 		 * @param totalTime A number that is the total playing time for the video. Optional.		 * 		 * @param isLive A Boolean value that is <code>true</code> if the video stream is live. 		 * This value is effective only when streaming from Flash Media Server (FMS) or other Flash 		 * Video Streaming Service (FVSS). The value of 		 * this property is ignored for an HTTP download. Optional.		 * 		 * @see #autoPlay 		 * @see #source 		 * @see #isLive                  * @see #totalTime                  *		 * @tiptext load method		 *                  * @langversion 3.0                 * @playerversion Flash 9.0.28.0
		 */
		public function load (source:String, totalTime:Number = NaN, isLive:Boolean = false) : void;
		/**
		 * Plays the video stream. With no parameters, the method simply takes the FLV 		 * file from a paused or stopped state to the playing state.		 * 		 * <p>If parameters are used, the method acts as a shortcut for setting the 		 * <code>autoPlay</code> property to <code>true</code> and setting the <code>isLive</code>, 		 * <code>totalTime</code> and, <code>source</code>		 * properties. If the <code>totalTime</code> and <code>isLive</code> properties are undefined, 		 * they are not set. </p>		 *		 * <p>When waiting for enough of a progressive download FLV to load before playing		 * starts automatically, call the <code>play()</code> method with no parameters		 * to force playback to start immediately.</p>		 * 		 * @param source A string that specifies the URL of the FLV file to stream 		 * and how to stream it. The URL can be a local path, an HTTP URL to an FLV file, 		 * an RTMP URL to an FLV file stream, or an HTTP URL to an XML file. It is optional, 		 * but the <code>source</code> property must be set either through the 		 * Component inspector or through ActionScript or this method has no effect.		 * 		 * @param totalTime A number that is the total playing time for the video. Optional.		 * 		 * @param isLive A Boolean value that is <code>true</code> if the video stream is live. 		 * This value is effective only when streaming from Flash Media Server (FMS) or other Flash Video Streaming Service (FVSS). 		 * The value of this property is ignored for an HTTP download. Optional.		 * 		 * @see #autoPlay 		 * @see #source 		 * @see #isLive          * @see #totalTime          *		 * @tiptext play method		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function play (source:String = null, totalTime:Number = NaN, isLive:Boolean = false) : void;
		/**
		 * Plays the FLV file when enough of it has downloaded. If the FLV file has 		 * downloaded or you are streaming from Flash Media Server (FMS), then calling the 		 * <code>playWhenEnoughDownloaded()</code> method		 * is identical to the <code>play()</code> method with no parameters.  Calling this		 * method does not pause playback, so in many cases, you may want to call the <code>pause()</code> method         * before you call this method.         *		 * @tiptext playWhenEnoughDownloaded method		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function playWhenEnoughDownloaded () : void;
		/**
		 * Pauses playing the video stream.          *         * <p>If playback has begun and you want to          * return to the state of waiting for enough to download and then automatically          * begin playback, call the <code>pause()</code> method, and then the          * <code>playWhenEnoughDownloaded()</code> method.</p>         *	 * @tiptext pause method         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function pause () : void;
		/**
		 * Stops the video from playing. 		 * If the <code>autoRewind</code> property is <code>true</code>,          * the FLV file rewinds to the beginning.         *		 * @tiptext stop method         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function stop () : void;
		/**
		 * Seeks to a given time in the file, specified in seconds, 		 * with a precision of three decimal places (milliseconds).		 *		 * <p>For several reasons, the <code>playheadTime</code> property might not 		 * have the expected value immediately after you call one of 		 * the seek methods or set <code>playheadTime</code> to cause seeking. 		 * First, for a progressive download, you can seek only to a 		 * keyframe, so a seek takes you to the time of the first 		 * keyframe after the specified time. (When streaming, a seek 		 * always goes to the precise specified time even if the source 		 * FLV file doesn't have a keyframe there.) Second, seeking 		 * is asynchronous, so if you call a seek method or set the 		 * <code>playheadTime</code> property, <code>playheadTime</code> does not update immediately. 		 * To obtain the time after the seek is complete, listen for the 		 * seek event, which does not start until the <code>playheadTime</code> property 		 * has updated.</p>		 * 		 * @param time A number that specifies the time, in seconds, at which to 		 * place the playhead.		 * 		 * @throws fl.video.VideoError If time is &lt; 0.		 *          * @see VideoPlayer#seek()         *		 * @tiptext seek method         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seek (time:Number) : void;
		/**
		 * Seeks to a given time in the file, specified in seconds, with a precision up to 		 * three decimal places (milliseconds). This method performs the same operation as the 		 * <code>seek()</code> method; it is provided for symmetry with the <code>seekPercent()</code>		 * method.		 * 		 * <p>For several reasons, the <code>playheadTime</code> property might not have the expected 		 * value immediately after you call one of the seek methods or set <code>playheadTime</code> 		 * to cause seeking. First, for a progressive download, you can seek only to a 		 * keyframe, so a seek takes you to the time of the first keyframe after the 		 * specified time. (When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.) Second, 		 * seeking is asynchronous, so if you call a seek method or set the <code>playheadTime</code> 		 * property, <code>playheadTime</code> does not update immediately. To obtain the time after 		 * the seek is complete, listen for the seek event, which does not start until 		 * the <code>playheadTime</code> property has updated. </p>		 *		 * @param time A number that specifies the time, in seconds, 		 * of the total play time at which to place the playhead.		 *          * @see #seek()         *		 * @tiptext seekSeconds method         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seekSeconds (time:Number) : void;
		/**
		 * Seeks to a percentage of the file and places the playhead there. 		 * The percentage is a number between 0 and 100.		 * 		 * <p>For several reasons, the <code>playheadTime</code> property might not have 		 * the expected value immediately after you call one of the seek 		 * methods or set <code>playheadTime</code> to cause seeking. First, for 		 * a progressive download, you can seek only to a keyframe, 		 * so a seek takes you to the time of the first keyframe after 		 * the specified time. (When streaming, a seek always goes 		 * to the precise specified time even if the source FLV file 		 * doesn't have a keyframe there.) Second, seeking is asynchronous, 		 * so if you call a seek method or set the <code>playheadTime</code> property, 		 * <code>playheadTime</code> does not update immediately. To obtain the time 		 * after the seek is complete, listen for the seek event, which 		 * does not start until the <code>playheadTime</code> property has updated.</p>		 * 		 * @param A number that specifies a percentage of the length of the FLV file 		 * at which to place the playhead.		 *		 * @throws fl.video.VideoError If <code>percent</code> is invalid or if <code>totalTime</code> is		 * undefined, <code>null</code> or &lt;= 0.         * @see #seek()         *		 * @tiptext seekPercent method         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seekPercent (percent:Number) : void;
		/**
		 * Seeks to a navigation cue point that matches the specified time, name, or 		 * time and name.		 *		 * <p>The time is the starting time, in seconds, 		 * from which to look for the next navigation cue point. The default is the 		 * current <code>playheadTime</code> property. If you specify a time, the method seeks 		 * to a cue point that matches that time or is later. If time is undefined, 		 * <code>null</code>, or less than 0, the method starts its 		 * search at time 0.</p>		 *		 * <p>The name is the cue point to seek. The method seeks to the first enabled navigation 		 * cue point with this name. </p>		 * 		 * <p>The time and name together are a navigation cue point with the specified 		 * name at or after the specified time. 		 * </p>		 * 		 * <p>For several reasons, the <code>playheadTime</code> property might not have the 		 * expected value immediately after you call one of the seek methods or 		 * set <code>playheadTime</code> to cause seeking. First, for a progressive 		 * download, you can seek only to a keyframe, so a seek takes you 		 * to the time of the first keyframe after the specified time. 		 * (When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.) 		 * Second, seeking is asynchronous, so if you call a seek method or 		 * set the <code>playheadTime</code> property, <code>playheadTime</code> does not 		 * update immediately. To obtain the time after the seek is complete, 		 * listen for the <code>seek</code> event, 		 * which does not start until the <code>playheadTime</code> property has updated.</p>		 * 		 * @param timeNameOrCuePoint A number that is the time, a string that is the name, or		 * both a number and string that are the specified name and time.		 * 		 * @throws fl.video.VideoError No cue point matching the criteria is found.		 * @see #seek()		 * @see #seekToPrevNavCuePoint()		 * @see #seekToNextNavCuePoint()		 * @see #findCuePoint()         * @see #isFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seekToNavCuePoint (timeNameOrCuePoint:*) : void;
		/**
		 * Seeks to the next navigation cue point, based on the current value of the 		 * <code>playheadTime</code> property. The method skips navigation cue points that have 		 * been disabled and goes to the end of the FLV file if there is no other cue point.		 *		 * <p>For several reasons, the <code>playheadTime</code> property might not have the 		 * expected value immediately after you call one of the seek methods or 		 * set <code>playheadTime</code> to cause seeking. First, for a progressive 		 * download, you can seek only to a keyframe, so a seek takes you 		 * to the time of the first keyframe after the specified time. 		 * (When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.) 		 * Second, seeking is asynchronous, so if you call a seek method or 		 * set the <code>playheadTime</code> property, <code>playheadTime</code> does not 		 * update immediately. To obtain the time after the seek is complete, 		 * listen for the <code>seek</code> event, 		 * which does not start until the <code>playheadTime</code> property has updated.</p>		 *		 * @param time A number that is the starting time, in seconds, from which to look for 		 * the next navigation cue point. The default is the current <code>playheadTime</code>		 * property. Optional.		 * 		 * @see #cuePoints		 * @see #seek()		 * @see #seekToNavCuePoint()		 * @see #seekToPrevNavCuePoint()		 * @see #findCuePoint()         * @see #isFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seekToNextNavCuePoint (time:Number = NaN) : void;
		/**
		 * Seeks to the previous navigation cue point, based on the current 		 * value of the <code>playheadTime</code> property. It goes to the beginning if 		 * there is no previous cue point. The method skips navigation cue 		 * points that have been disabled.		 *         * <p>For several reasons, the <code>playheadTime</code> property might not have the 		 * expected value immediately after you call one of the seek methods or 		 * set <code>playheadTime</code> to cause seeking. First, for a progressive 		 * download, you can seek only to a keyframe, so a seek takes you 		 * to the time of the first keyframe after the specified time. 		 * (When streaming, a seek always goes to the precise specified 		 * time even if the source FLV file doesn't have a keyframe there.) 		 * Second, seeking is asynchronous, so if you call a seek method or 		 * set the <code>playheadTime</code> property, <code>playheadTime</code> does not 		 * update immediately. To obtain the time after the seek is complete, 		 * listen for the <code>seek</code> event, 		 * which does not start until the <code>playheadTime</code> property has updated.</p>		 * 		 * @param time A number that is the starting time in seconds from which to 		 * look for the previous navigation cue point. The default is the current 		 * value of the <code>playheadTime</code> property. Optional.		 * 		 * @see #cuePoints		 * @see #seek()		 * @see #seekToNavCuePoint()		 * @see #seekToNextNavCuePoint()		 * @see #findCuePoint()         * @see #isFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function seekToPrevNavCuePoint (time:Number = NaN) : void;
		/**
		 * Adds an ActionScript cue point and has the same effect as adding an ActionScript 		 * cue point using the Cue Points dialog box, except that it occurs when an application 		 * executes rather than during application development.		 * 		 * <p>Cue point information is wiped out when the <code>source</code> property is 		 * set. To set cue point information for the next FLV file to be loaded, 		 * set the <code>source</code> property first.</p>		 * 		 * <p>It is valid to add multiple ActionScript cue points with the same name and time. 		 * When you remove ActionScript cue points with the <code>removeASCuePoint()</code> 		 * method, all cue points with the same name and time are removed.</p>		 *		 * @param timeOrCuePoint An object having <code>name</code> and 		 * <code>time</code> properties, which describe the cue point. It also might have a 		 * <code>parameters</code> property that holds name/value pairs. It may have the		 * <code>type</code> property set 		 * to "<code>actionscript</code>". If the type is missing or set to something else, it is set 		 * automatically. If the object does not conform to these conventions, the 		 * method throws a VideoError error.		 *		 * <p>The <code>time</code> property sets the time in seconds for a new cue point 		 * to be added and the <code>name</code> parameter must follow.</p>		 * 		 * @param name A string that specifies the name of the cue point if you submit 		 * a <code>time</code> parameter instead of the cue point.		 * @param parameters Optional parameters for the cue point if the		 * <code>timeOrCuePoint</code> parameter is a number.		 * 		 * @return The cue point object that was added. Edits to this		 * object affect the <code>cuePoint</code> event dispatch.		 * 		 * @throws fl.video.VideoError Parameters are invalid.		 * 		 * @see #findCuePoint()          * @see #removeASCuePoint()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addASCuePoint (timeOrCuePoint:*, name:String = null, parameters:Object = null) : Object;
		/**
		 * Removes an ActionScript cue point from the currently loaded FLV file. 		 * Only the <code>name</code> and <code>time</code> properties are used from the		 * <code>timeNameOrCuePoint</code> parameter to find the cue point to remove.		 *		 * <p>If multiple ActionScript cue points match the search criteria, only one is removed. 		 * To remove all, call this function repeatedly in a loop with the same parameters 		 * until it returns <code>null</code>.</p>		 *		 * <p>Cue point information is wiped out when the <code>source</code> property is 		 * set, so to set cue point information for the next FLV file to be loaded, set the 		 * <code>source</code> property first.</p>		 *		 * @param timeNameOrCuePoint A cue point string that contains the <code>time</code> and 		 * <code>name</code> properties for the cue point to remove. The method removes the 		 * first cue point with this name.		 * Or, if this parameter is a number, the method removes the 		 * first cue point with this time. 		 * If this parameter is an object, the method removes the cue point with both the 		 * <code>time</code> and <code>name</code> properties.		 * 		 * @return The cue point object that was removed. If there is no		 * matching cue point, the method returns <code>null</code>.		 * 		 * 		 * @see #addASCuePoint()         * @see #findCuePoint()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeASCuePoint (timeNameOrCuePoint:*) : Object;
		/**
		 * Finds the cue point of the type specified by the <code>type</code> parameter and 		 * having the time, name, or combination of time and name that you specify 		 * through the parameters.		 * 		 * <p>If you do not provide a value for either the time or name of the 		 * cue point, or if the time is <code>null</code>, undefined, or less than zero 		 * and the name is <code>null</code> or undefined, 		 * the method throws VideoError error 1002. </p>		 * 		 * <p>The method includes disabled cue points in the search. 		 * Use the <code>isFLVCuePointEnabled()</code> method to determine 		 * whether a cue point is disabled.</p>		 *		 * @param timeNameOrCuePoint This can be a number specifying a time, a string		 * specifying a name, or an object with time and/or name properties.		 * 		 * <p>If this parameter is a string, the method searches for the 		 * first cue point with this name and returns <code>null</code> if there is no match.</p>		 *		 * <p>If this parameter is a number, the method searches for and returns the first		 * cue point with this time. If there are multiple cue points with the same time, 		 * which is possible only with ActionScript cue points, the cue point with the		 * first name alphabetically is returned. Returns <code>null</code> if there is no match.		 * The first three decimal places for the time are used. More than		 * three decimal places are rounded.</p>		 * 		 * <p>If this parameter is an object, the method searches for the cue point object 		 * that contains the specified <code>time</code> and/or <code>name</code>		 * properties.  If only time or name is specified, then the behavior is the same		 * as calling with a number or a string.  If both the <code>time</code>		 * and <code>name</code> properties are defined and a cue point object exists with both of them,		 * then the cue point object is returned; otherwise, <code>null</code> is returned.</p>		 *		 * <p>If time is <code>null</code>, NaN or less than 0 and name is <code>null</code>		 * or undefined, a VideoError object is thrown.</p>		 *		 * @param type A string that specifies the type of cue point for which to 		 * search. The possible values for this parameter are <code>"actionscript"</code>, <code>"all"</code>, <code>"event"</code>, 		 * <code>"flv"</code>, or <code>"navigation"</code>. You can specify these values using the following class 		 * properties: <code>CuePointType.ACTIONSCRIPT</code>, <code>CuePointType.ALL</code>, <code>CuePointType.EVENT</code>, 		 * <code>CuePointType.FLV</code>, and <code>CuePointType.NAVIGATION</code>. If this parameter 		 * is not specified, the default is <code>"all"</code>, which means the method 		 * searches all cue point types. Optional. 		 * 		 * @return An object that is a copy of the found cue point object 		 * with the following additional properties:		 *		 * <ul>		 * 		 * <li><code>array</code>&#x2014;The array of cue points that were		 * searched. Treat this array as read-only because adding, removing, 		 * or editing objects within it can cause cue points to malfunction.</li>		 *		 * <li><code>index</code>&#x2014;The index into the array for the returned cue point.</li>		 *		 * </ul>		 * 		 * <p>Returns <code>null</code> if no match is found.</p>		 * 		 * @throws fl.video.VideoError If the <code>time</code> property is <code>null</code>, 		 * undefined or less than 0 and the <code>name</code> property is <code>null</code>		 * or undefined.		 * 		 * @see #addASCuePoint() 		 * @see #cuePoints 		 * @see #findNearestCuePoint() 		 * @see #findNextCuePointWithName() 		 * @see #isFLVCuePointEnabled() 		 * @see #removeASCuePoint() 		 * @see #seekToNavCuePoint() 		 * @see #seekToNextNavCuePoint() 		 * @see #seekToPrevNavCuePoint()          * @see #setFLVCuePointEnabled()          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function findCuePoint (timeNameOrCuePoint:*, type:String = CuePointType.ALL) : Object;
		/**
		 * Finds a cue point of the specified type that matches or is 		 * earlier than the time that you specify. If you specify both 		 * a time and a name and no earlier cue point matches that name, 		 * it finds a cue point that matches the name that you specify. 		 * Otherwise, it returns <code>null</code>.		 * Default is to search all cue points. 		 * 		 * <p>The method includes disabled cue points in the search. Use 		 * the <code>isFLVCuePointEnabled()</code> method to determine whether 		 * a cue point is disabled.</p>		 * 		 *		 * @param timeNameOrCuePoint This can be a number specifying a time, a string		 * specifying a name, or an object with time and/or name properties.		 * 		 * <p>If this parameter is a string, the method searches for the 		 * first cue point with this name and returns <code>null</code> if there is no match.</p>		 *		 * <p>If this parameter is a number then the closest cue point to this time that is		 * an exact match or earlier will be returned.  If there is no cue point at or earlier		 * than that time, then the first cue point is returned.		 * If there are multiple cue points with the same time, 		 * which is possible only with ActionScript cue points, the cue point with the		 * first name alphabetically is returned. Returns <code>null</code> if there is no match.		 * The first three decimal places for the time are used. More than		 * three decimal places are rounded.</p>		 * 		 * <p>If this parameter is an object, the method searches for the cue point object 		 * that contains the specified <code>time</code> and/or <code>name</code>		 * properties.  If only time or name is specified, then the behavior is the same		 * as calling with a number or a string.  If both the <code>time</code>		 * and <code>name</code> properties are defined and a cue point object exists with both of them,		 * then the cue point object is returned.  Otherwise the nearest cue point with an		 * earlier time and the same name is returned.  If no cue point earlier than that time		 * with that name is found <code>null</code> is returned.</p>		 *		 * <p>If time is <code>null</code>, undefined or less than 0 and name is <code>null</code>		 * or undefined, a VideoError object is thrown.</p>		 *		 * @param type A string that specifies the type of cue point for which to 		 * search. The possible values for this parameter are <code>"actionscript"</code>, <code>"all"</code>, <code>"event"</code>, 		 * <code>"flv"</code>, or <code>"navigation"</code>. You can specify these values using the following class 		 * properties: <code>CuePointType.ACTIONSCRIPT</code>, <code>CuePointType.ALL</code>, <code>CuePointType.EVENT</code>, 		 * <code>CuePointType.FLV</code>, and <code>CuePointType.NAVIGATION</code>. If this parameter 		 * is not specified, the default is <code>"all"</code>, which means the method 		 * searches all cue point types. Optional. 		 *		 * @return An object that is a copy of the found cue point 		 * object with the following additional properties:		 *		 * <ul>		 * 		 * <li><code>array</code>&#x2014;The array of cue points searched. 		 * Treat this array as read-only as adding, removing or editing 		 * objects within it can cause cue points to malfunction.</li>		 *		 * <li><code>index</code>&#x2014;The index into the array for the returned cue point.</li>         * </ul>		 * <p>Returns <code>null</code> if no match was found.</p>		 *		 * 		 * 		 * @throws fl.video.VideoError If the time is <code>null</code>, undefined, or less 		 * than 0 and the name is <code>null</code> or undefined.		 * 		 * @see CuePointType#ALL		 * @see CuePointType#EVENT		 * @see CuePointType#NAVIGATION		 * @see CuePointType#FLV		 * @see CuePointType#ACTIONSCRIPT		 * @see #cuePoints		 * @see #addASCuePoint()		 * @see #removeASCuePoint()         * @see #findCuePoint()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function findNearestCuePoint (timeNameOrCuePoint:*, type:String = CuePointType.ALL) : Object;
		/**
		 * Finds the next cue point in <code>my_cuePoint.array</code> that has the same name as 		 * <code>my_cuePoint.name</code>. The <code>my_cuePoint</code> object must be a 		 * cue point object that has been returned by the <code>findCuePoint()</code> method, the 		 * <code>findNearestCuePoint()</code> method, or a previous call to this method. 		 * This method uses the <code>array</code> parameter that these methods add to the 		 * CuePoint object.		 * 		 * <p>The method includes disabled cue points in the search. Use the 		 * <code>isFLVCuePointEnabled()</code> method to determine whether a cue point is disabled.</p>		 * 		 * @param cuePoint A cue point object that has been returned by either the 		 * <code>findCuePoint()</code> method, 		 * the <code>findNearestCuePoint()</code> method, or a previous call to this method.		 *          * @return If there are no more cue points in the array with a matching 		 * name, then <code>null</code>; otherwise, returns a		 * copy of the cue point object with additional properties:		 *		 * <ul>		 * 		 * <li><code>array</code>&#x2014;The array of cue points searched. 		 * Treat this array as read-only because adding, removing or 		 * editing objects within it can cause cue points to malfunction.</li>		 *		 * <li><code>index</code>&#x2014;The index into the array for the returned cue point.</li>		 *		 * </ul>		 *          * @throws fl.video.VideoError When argument is invalid.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function findNextCuePointWithName (cuePoint:Object) : Object;
		/**
		 * Enables or disables one or more FLV file cue points. Disabled cue points 		 * are disabled for purposes of being dispatched as events and for navigating 		 * to them with the <code>seekToPrevNavCuePoint()</code>, 		 * <code>seekToNextNavCuePoint()</code>, and <code>seekToNavCuePoint()</code> methods.		 * 		 * <p>Cue point information is deleted when you set the <code>source</code>		 * property to a different FLV file, so set the <code>source</code> property before 		 * setting cue point information for the next FLV file to be loaded.</p>		 * 		 * <p>Changes caused by this method are not reflected by calls to the 		 * <code>isFLVCuePointEnabled()</code> method until metadata is loaded.</p>		 *		 * @param enabled A Boolean value that specifies whether to enable (<code>true</code>) 		 * or disable (<code>false</code>) an FLV file cue point.		 * 		 * @param timeNameOrCuePoint If this parameter is a string, the method enables or disables		 * the cue point with this name. If this parameter is a number, the method enables or		 * disables the cue point with this time. If this parameter is an object, the method 		 * enables or disables the cue point with both the <code>name</code> and <code>time</code>		 * properties.		 * 		 * @return If <code>metadataLoaded</code> is <code>true</code>, the method returns the		 * number of cue points whose enabled state was changed.  If		 * <code>metadataLoaded</code> is <code>false</code>, the method returns -1 because the 		 * component cannot yet determine which, if any, cue points to set. 		 * When the metadata arrives, however, the component sets the specified cue points appropriately.		 * 		 * @see #cuePoints		 * @see #isFLVCuePointEnabled()		 * @see #findCuePoint()		 * @see #findNearestCuePoint()         * @see #findNextCuePointWithName()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFLVCuePointEnabled (enabled:Boolean, timeNameOrCuePoint:*) : Number;
		/**
		 * Returns <code>false</code> if the FLV file embedded cue point is disabled. 		 * You can disable cue points either by setting the <code>cuePoints</code> property through 		 * the Flash Video Cue Points dialog box or by calling the 		 * <code>setFLVCuePointEnabled()</code> method.		 * 		 * <p>The return value from this function is meaningful only when the 		 * <code>metadataLoaded</code> property is <code>true</code>, the 		 * <code>metadata</code> property is not <code>null</code>, or after a 		 * <code>metadataReceived</code> event. When <code>metadataLoaded</code> is 		 * <code>false</code>, this function always returns <code>true</code>.</p>		 *		 * @param timeNameOrCuePoint If this parameter is a string, returns the name of 		 * the cue point to check; returns <code>false</code> only if all cue points with 		 * this name are disabled.  		 * 		 * <p>If this parameter is a number, it is the time of the cue point to check.</p>		 * 		 * <p>If this parameter is an object, returns the object with the matching		 * <code>name</code> and <code>time</code> properties.</p>		 * 		 * @return Returns <code>false</code> if the FLV file embedded cue point is disabled. 		 * You can disable cue points either by setting the <code>cuePoints</code> property through the 		 * Flash Video Cue Points dialog box or by calling the <code>setFLVCuePointEnabled()</code> method.		 * 		 * <p>The return value from this function is meaningful only when the 		 * <code>metadataLoaded</code> property is <code>true</code>, the <code>metadata</code> 		 * property is not <code>null</code>, or after a 		 * <code>metadataReceived</code> event. When <code>metadataLoaded</code> is <code>false</code>, 		 * this function always returns <code>true</code>.</p>		 * 		 * 		 * @see #findCuePoint()		 * @see #findNearestCuePoint()		 * @see #findNextCuePointWithName()         * @see #setFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function isFLVCuePointEnabled (timeNameOrCuePoint:*) : Boolean;
		/**
		 * Brings a video player to the front of the stack of video players. 		 * Useful for custom transitions between video players. The default stack 		 * order is the same as it is for the <code>activeVideoPlayerIndex</code> property: 		 * 0 is on the bottom, 1 is above it, 2 is above 1, and so on. However, when you		 * call the <code>bringVideoPlayerToFront()</code> method this order may change. For		 * example, 2 may be the bottom.		 * 		 * @param index A number that is the index of the video player to move to the front.		 * 		 * @see #activeVideoPlayerIndex 		 * @see #getVideoPlayer() 		 * @see VideoPlayer          * @see #visibleVideoPlayerIndex          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function bringVideoPlayerToFront (index:uint) : void;
		/**
		 * Gets the video player specified by the <code>index</code> parameter. 		 * When possible, it is best to access the VideoPlayer methods and properties 		 * using FLVPlayback methods and properties. Each <code>DisplayObject.name</code> property 		 * is its index converted to a string.		 * 		 * @param index A number that is the index of the video player to get.		 *          * @return A VideoPlayer object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getVideoPlayer (index:Number) : VideoPlayer;
		/**
		 * Closes NetStream and deletes the video player specified by the <code>index</code>		 * parameter. If the closed video player is the active or visible video player, 		 * the FLVPlayback instance sets the active and or visible video player to the 		 * default player (with index 0). You cannot close the default player, and 		 * trying to do so causes the component to throw an error.		 * 		 * @param index A number that is the index of the video player to close.		 * 		 * @see #activeVideoPlayerIndex 		 * @see #event:close close event         * @see #visibleVideoPlayerIndex          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function closeVideoPlayer (index:uint) : void;
		/**
		 * Sets the FLVPlayback video player to full screen. Calling this method has 	* the same effect as clicking the full screen toggle button that's built into some	* FLVPlayback skins and is also available as the FullScreenButton in the Video section	* of the Components panel.	*	* <p>This method supports hardware acceleration in Flash Player for 	* full screen video.  If the user's version of Flash Player does not support	* hardware acceleration, this method still works and full screen video will work	* the same as it does without hardware acceleration support.</p>	*	* <p>Because a call to this method sets the <code>displayState</code> property of the 	* Stage class to <code>StageDisplayState.FULL_SCREEN</code>, it has	* the same restrictions as the <code>displayState</code> property. 	*	* If, instead of calling this method, you implement full screen mode by directly setting the 	* <code>stage.displayState</code> property to <code>StageDisplayState.FULL_SCREEN</code>, 	* hardware acceleration is not used.</p>	*  	* <p>Full screen support occurs only if the <code>fullScreenTakeOver</code>	* property is set to true, which it is by default.</p>    *    * <p><strong>Player Version</strong>: Flash Player 9 <a target="mm_external" href="http://www.adobe.com/go/fp9_update3">Update 3</a>.</p>    *	* @see #fullScreenTakeover	* @see #skinScaleMaximum	* @see flash.display.Stage#displayState	* 	* @includeExample examples/FLVPlayback.enterFullScreenDisplayState.1.as -noswf	*	* @langversion 3.0	* @internal Flash 9.0.xx.0
		 */
		public function enterFullScreenDisplayState () : void;
		/**
		 * Creates and configures VideoPlayer movie clip.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function createVideoPlayer (index:Number) : void;
		/**
		 * Creates live preview placeholder.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function createLivePreviewMovieClip () : void;
		/**
		 * Handles load of live preview image		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function onCompletePreview (e:Event) : void;
		/**
		 * Called on <code>onEnterFrame</code> to initiate loading the new		 * source url.  We delay to give the user time to set other		 * vars as well.  Only done this way when source set from the		 * component inspector or property inspector, not when set with AS.		 *		 * @see #source         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function doContentPathConnect (eventOrIndex:*) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function showFirstStream () : void;
		/**
		 * Called by UIManager when SeekBar scrubbing starts		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function _scrubStart () : void;
		/**
		 * Called by UIManager when seekbar scrubbing finishes		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function _scrubFinish () : void;
		/**
		 * Called by UIManager when skin errors		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function skinError (message:String) : void;
		/**
		 * Called by UIManager when skin loads		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function skinLoaded () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function showSkinNow (e:TimerEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function queueCmd (vpState:VideoPlayerState, type:Number, time:Number = NaN) : void;
		/**
		 * Assigns a tabIndex value to each of the FLVPlayback controls by sorting 		 * them by position horizontally left to right. This method returns the next available tabIndex value.		 * 		 * <p>If you call <code>assignTabIndexes</code> with <code>NaN</code> as the <code>startTabbing</code> 		 * parameter and the FLVPlayback component instance has a <code>tabIndex</code> value assigned to it, 		 * the method will use the FLVPlayback component instance's assigned <code>tabIndex</code> as the <code>startTabIndex</code>.</p>		 * <p>When an FLVPlayback skin is specified, you should wait a frame after the <code>FLVPlayback.SKIN_LOADED</code> event 		 * to allow the skin controls to initialize before calling this method.</p>		 * 		 * <p>When using custom controls, wait a frame after the <code>FLVPlayback.READY</code> event to allow		 * the custom controls to initialize befor calling this method.</p>		 *          * @param startTabbing The starting tabIndex for FLVPlayback controls.		 * 		 * @return The next available tabIndex after the FLVPlayback controls.		 * 		 * @see #endTabIndex		 * @see #startTabIndex         *		 * @includeExample examples/FLVPlaybackTabIndexExample.as -noswf         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function assignTabIndexes (startTabIndex:int) : int;
	}
}
