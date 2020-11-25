package fl.video
{
	import flash.accessibility.Accessibility;
	import flash.accessibility.AccessibilityImplementation;
	import flash.accessibility.AccessibilityProperties;
	import flash.system.Capabilities;
	import flash.ui.Keyboard;
	import flash.display.*;
	import flash.events.*;
	import flash.geom.ColorTransform;
	import flash.geom.Matrix;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.net.URLRequest;
	import flash.utils.*;

	/**
	 * <p>Functions you can plugin to seek bar or volume bar with, to	 * override std behavior: startHandleDrag(), stopHandleDrag(),	 * positionHandle(), calcPercentageFromHandle(), positionBar().	 * Return true to override standard behavior or return false to allow	 * standard behavior to execute.  These do not override default	 * behavior, but allow you to add addl functionality:	 * addBarControl()</p>	 *	 * <p>Functions you can use for swf based skin controls: layoutSelf()	 * - called after control is laid out to do additional layout.	 * Properties that can be set to customize layout: anchorLeft,	 * anchorRight, anchorTop, anchorLeft.</p>	 *	 * <p>Possible seek bar and volume bar customization variables:	 * handleLeftMargin, handleRightMargin, handleY, handle_mc,	 * progressLeftMargin, progressRightMargin, progressY, progress_mc,	 * fullnessLeftMargin, fullnessRightMargin, fullnessY, fullness_mc,	 * percentage.  These variables will also be set to defaults by	 * UIManager if values are not passed in.  Percentage is constantly	 * updated, others will be set by UIManager in addBarControl or	 * finishAddBarControl.</p>	 *	 * <p>These seek bar and volume bar customization variables do not	 * work with external skin swfs and are not set if no value is passed	 * in: handleLinkageID, handleBelow, progressLinkageID, progressBelow,	 * fullnessLinkageID, fullnessBelow</p>	 *	 * <p>Note that in swf skins, handle_mc must have same parent as	 * correpsonding bar.  fullness_mc and progress_mc may be at the same	 * level or nested, and either of those may have a fill_mc at the same	 * level or nested.  Note that if any of these nestable clips are	 * nested, then they must be scaled at 100% on stage, because	 * UIManager uses xscale and yscale to resize them and assumes 100% is	 * the original size.  If they are not scaled at 100% when placed on	 * stage, weird stuff might happen.</p>	 *	 * <p>Variables set in seek bar and volume bar that can be used by	 * custom methods, but should be treated as read only: isDragging,	 * uiMgr, controlIndex.  Also set on handle mc: controlIndex</p>	 *	 * <p>Note that when skinAutoHide is true, skin is hidden unless	 * mouse if over visible VideoPlayer or over the skin.  Over the	 * skin is measured by hitTest on border_mc clip from the layout_mc.	 * If there is no border_mc, then mouse over the skin doesn't make	 * it visible (unless skin is completely over the video, of course.)</p>	 *     * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class UIManager
	{
		public static const PAUSE_BUTTON : int = 0;
		public static const PLAY_BUTTON : int = 1;
		public static const STOP_BUTTON : int = 2;
		public static const SEEK_BAR_HANDLE : int = 3;
		public static const SEEK_BAR_HIT : int = 4;
		public static const BACK_BUTTON : int = 5;
		public static const FORWARD_BUTTON : int = 6;
		public static const FULL_SCREEN_ON_BUTTON : int = 7;
		public static const FULL_SCREEN_OFF_BUTTON : int = 8;
		public static const MUTE_ON_BUTTON : int = 9;
		public static const MUTE_OFF_BUTTON : int = 10;
		public static const VOLUME_BAR_HANDLE : int = 11;
		public static const VOLUME_BAR_HIT : int = 12;
		public static const NUM_BUTTONS : int = 13;
		public static const PLAY_PAUSE_BUTTON : int = 13;
		public static const FULL_SCREEN_BUTTON : int = 14;
		public static const MUTE_BUTTON : int = 15;
		public static const BUFFERING_BAR : int = 16;
		public static const SEEK_BAR : int = 17;
		public static const VOLUME_BAR : int = 18;
		public static const NUM_CONTROLS : int = 19;
		public static const NORMAL_STATE : uint = 0;
		public static const OVER_STATE : uint = 1;
		public static const DOWN_STATE : uint = 2;
		public static const FULL_SCREEN_SOURCE_RECT_MIN_WIDTH : uint = 320;
		public static const FULL_SCREEN_SOURCE_RECT_MIN_HEIGHT : uint = 240;
		local var controls : Array;
		local var delayedControls : Array;
		public var customClips : Array;
		public var ctrlDataDict : Dictionary;
		local var skin_mc : Sprite;
		local var skinLoader : Loader;
		local var skinTemplate : Sprite;
		local var layout_mc : Sprite;
		local var border_mc : DisplayObject;
		local var borderCopy : Sprite;
		local var borderPrevRect : Rectangle;
		local var borderScale9Rects : Array;
		local var borderAlpha : Number;
		local var borderColor : uint;
		local var borderColorTransform : ColorTransform;
		local var skinLoadDelayCount : uint;
		local var placeholderLeft : Number;
		local var placeholderRight : Number;
		local var placeholderTop : Number;
		local var placeholderBottom : Number;
		local var videoLeft : Number;
		local var videoRight : Number;
		local var videoTop : Number;
		local var videoBottom : Number;
		local var _bufferingBarHides : Boolean;
		local var _controlsEnabled : Boolean;
		local var _skin : String;
		local var _skinAutoHide : Boolean;
		local var _skinFadingMaxTime : int;
		local var _skinReady : Boolean;
		local var __visible : Boolean;
		local var _seekBarScrubTolerance : Number;
		local var _skinScaleMaximum : Number;
		local var _progressPercent : Number;
		local var cachedSoundLevel : Number;
		local var _lastVolumePos : Number;
		local var _isMuted : Boolean;
		local var _volumeBarTimer : Timer;
		local var _volumeBarScrubTolerance : Number;
		local var _vc : FLVPlayback;
		local var _bufferingDelayTimer : Timer;
		local var _bufferingOn : Boolean;
		local var _seekBarTimer : Timer;
		local var _lastScrubPos : Number;
		local var _playAfterScrub : Boolean;
		local var _skinAutoHideTimer : Timer;
		static const SKIN_AUTO_HIDE_INTERVAL : Number = 200;
		local var _skinFadingTimer : Timer;
		static const SKIN_FADING_INTERVAL : Number = 100;
		local var _skinFadingIn : Boolean;
		local var _skinFadeStartTime : int;
		local var _skinAutoHideMotionTimeout : int;
		local var _skinAutoHideMouseX : Number;
		local var _skinAutoHideMouseY : Number;
		local var _skinAutoHideLastMotionTime : int;
		static const SKIN_FADING_MAX_TIME_DEFAULT : Number = 500;
		static const SKIN_AUTO_HIDE_MOTION_TIMEOUT_DEFAULT : Number = 3000;
		local var mouseCaptureCtrl : int;
		local var fullScreenSourceRectMinWidth : uint;
		local var fullScreenSourceRectMinHeight : uint;
		local var fullScreenSourceRectMinAspectRatio : Number;
		local var _fullScreen : Boolean;
		local var _fullScreenTakeOver : Boolean;
		local var _fullScreenBgColor : uint;
		local var _fullScreenAccel : Boolean;
		local var _fullScreenVideoWidth : Number;
		local var _fullScreenVideoHeight : Number;
		local var cacheStageAlign : String;
		local var cacheStageScaleMode : String;
		local var cacheStageBGColor : uint;
		local var cacheFLVPlaybackParent : DisplayObjectContainer;
		local var cacheFLVPlaybackIndex : int;
		local var cacheFLVPlaybackLocation : Rectangle;
		local var cacheFLVPlaybackScaleMode : Array;
		local var cacheFLVPlaybackAlign : Array;
		local var cacheSkinAutoHide : Boolean;
		/**
		 * Default value of volumeBarInterval		 *         * @see #volumeBarInterval         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const VOLUME_BAR_INTERVAL_DEFAULT : Number = 250;
		/**
		 * Default value of volumeBarScrubTolerance.		 *         * @see #volumeBarScrubTolerance         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const VOLUME_BAR_SCRUB_TOLERANCE_DEFAULT : Number = 0;
		/**
		 * Default value of seekBarInterval		 *         * @see #seekBarInterval         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SEEK_BAR_INTERVAL_DEFAULT : Number = 250;
		/**
		 * Default value of seekBarScrubTolerance.		 *         * @see #seekBarScrubTolerance         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const SEEK_BAR_SCRUB_TOLERANCE_DEFAULT : Number = 5;
		/**
		 * Default value of bufferingDelayInterval.		 *         * @see #seekBarInterval         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const BUFFERING_DELAY_INTERVAL_DEFAULT : Number = 1000;
		static var layoutNameToIndexMappings : Object;
		static var layoutNameArray : Array;
		static var skinClassPrefixes : Array;
		static var customComponentClassNames : Array;
		local var hitTarget_mc : Sprite;
		public static const CAPTIONS_ON_BUTTON : Number = 28;
		public static const CAPTIONS_OFF_BUTTON : Number = 29;
		public static const SHOW_CONTROLS_BUTTON : Number = 30;
		public static const HIDE_CONTROLS_BUTTON : Number = 31;
		local var startTabIndex : int;
		local var endTabIndex : int;
		local var focusRect : Boolean;
		static var buttonSkinLinkageIDs : Array;

		/**
		 * If true, we hide and disable certain controls when the		 * buffering bar is displayed.  The seek bar will be hidden, the		 * play, pause, play/pause, forward and back buttons would be		 * disabled.  Default is false.  This only has effect if there         * is a buffering bar control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bufferingBarHidesAndDisablesOthers () : Boolean;
		public function set bufferingBarHidesAndDisablesOthers (b:Boolean) : void;
		public function get controlsEnabled () : Boolean;
		public function set controlsEnabled (flag:Boolean) : void;
		public function get fullScreenBackgroundColor () : uint;
		public function set fullScreenBackgroundColor (c:uint) : void;
		public function get fullScreenSkinDelay () : int;
		public function set fullScreenSkinDelay (i:int) : void;
		public function get fullScreenTakeOver () : Boolean;
		public function set fullScreenTakeOver (v:Boolean) : void;
		/**
		 * String URL of skin swf to download.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skin () : String;
		public function set skin (s:String) : void;
		public function get skinAutoHide () : Boolean;
		public function set skinAutoHide (b:Boolean) : void;
		public function get skinBackgroundAlpha () : Number;
		public function set skinBackgroundAlpha (a:Number) : void;
		public function get skinBackgroundColor () : uint;
		public function set skinBackgroundColor (c:uint) : void;
		public function get skinFadeTime () : int;
		public function set skinFadeTime (i:int) : void;
		public function get skinReady () : Boolean;
		/**
		 * Determines how often check the seek bar handle location when		 * scubbing, in milliseconds.         *         * @default 250		 * 		 * @see #SEEK_BAR_INTERVAL_DEFAULT
		 */
		public function get seekBarInterval () : Number;
		public function set seekBarInterval (s:Number) : void;
		/**
		 * Determines how often check the volume bar handle location when		 * scubbing, in milliseconds.         *         * @default 250		 *          * @see #VOLUME_BAR_INTERVAL_DEFAULT         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get volumeBarInterval () : Number;
		public function set volumeBarInterval (s:Number) : void;
		/**
		 * Determines how long after VideoState.BUFFERING state entered		 * we disable controls for buffering.  This delay is put into		 * place to avoid annoying rapid switching between states.         *		 * @default 1000		 *          * @see #BUFFERING_DELAY_INTERVAL_DEFAULT         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bufferingDelayInterval () : Number;
		public function set bufferingDelayInterval (s:Number) : void;
		/**
		 * Determines how far user can move scrub bar before an update		 * will occur.  Specified in percentage from 1 to 100.  Set to 0		 * to indicate no scrub tolerance--always update volume on		 * volumeBarInterval regardless of how far user has moved handle.         *         * @default 0		 *         * @see #VOLUME_BAR_SCRUB_TOLERANCE_DEFAULT         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get volumeBarScrubTolerance () : Number;
		public function set volumeBarScrubTolerance (s:Number) : void;
		/**
		 * <p>Determines how far user can move scrub bar before an update		 * will occur.  Specified in percentage from 1 to 100.  Set to 0		 * to indicate no scrub tolerance--always update position on		 * seekBarInterval regardless of how far user has moved handle.		 * Default is 5.</p>		 *         * @see #SEEK_BAR_SCRUB_TOLERANCE_DEFAULT         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get seekBarScrubTolerance () : Number;
		public function set seekBarScrubTolerance (s:Number) : void;
		/**
		 * controls max amount that we will allow skin to scale		 * up due to full screen video hardware acceleration         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get skinScaleMaximum () : Number;
		public function set skinScaleMaximum (value:Number) : void;
		/**
		 * whether or not skin swf controls         * should be shown or hidden         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get visible () : Boolean;
		public function set visible (v:Boolean) : void;

		/**
		 * Constructor.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function UIManager (vc:FLVPlayback);
		static function initLayoutNameToIndexMappings () : void;
		function handleFullScreenEvent (e:FullScreenEvent) : void;
		function handleEvent (e:Event) : void;
		function handleSoundEvent (e:SoundEvent) : void;
		function handleLayoutEvent (e:LayoutEvent) : void;
		function handleIVPEvent (e:IVPEvent) : void;
		public function getControl (index:int) : Sprite;
		public function setControl (index:int, ctrl:Sprite) : void;
		function resetPlayPause () : void;
		function addButtonControl (ctrl:Sprite) : void;
		function handleButtonEvent (e:MouseEvent) : void;
		function captureMouseEvent (e:MouseEvent) : void;
		function handleMouseUp (e:MouseEvent) : void;
		function removeButtonListeners (ctrl:Sprite) : void;
		/**
		 * start download of skin swf, called when skin property set.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function downloadSkin () : void;
		/**
		 * Loader event handler function         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleLoadErrorEvent (e:ErrorEvent) : void;
		/**
		 * Loader event handler function         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleLoad (e:Event) : void;
		function finishLoad (e:Event) : void;
		/**
		 * layout all controls from loaded swf		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function layoutSkin () : void;
		/**
		 * layout and recolor border_mc, via borderCopy filled with Bitmaps         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function bitmapCopyBorder () : void;
		/**
		 * layout individual control from loaded swf		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function layoutControl (ctrl:DisplayObject) : void;
		/**
		 * layout individual control from loaded swf		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function calcLayoutControl (ctrl:DisplayObject) : Rectangle;
		/**
		 * remove controls from prev skin swf		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function removeSkin () : void;
		/**
		 * set custom clip from loaded swf		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function setCustomClip (dispObj:DisplayObject) : void;
		/**
		 * set skin clip from loaded swf		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function setSkin (index:int, avatar:DisplayObject) : void;
		function setTwoButtonHolderSkin (holderIndex:int, firstIndex:int, firstName:String, secondIndex:int, secondName:String, avatar:DisplayObject) : Sprite;
		function setupButtonSkin (index:int, avatar:DisplayObject) : Sprite;
		function setupButtonSkinState (ctrl:Sprite, definitionHolder:Sprite, skinName:String, defaultSkin:DisplayObject = null) : DisplayObject;
		function setupBarSkinPart (ctrl:Sprite, avatar:DisplayObject, definitionHolder:Sprite, skinName:String, partName:String, required:Boolean = false) : DisplayObject;
		function createSkin (definitionHolder:DisplayObject, skinName:String) : DisplayObject;
		/**
		 * skin button		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function skinButtonControl (ctrlOrEvent:Object) : void;
		/**
		 * helper to skin button		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function applySkinState (ctrlData:ControlData, newState:DisplayObject) : void;
		/**
		 * adds seek bar or volume bar		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function addBarControl (ctrl:Sprite) : void;
		/**
		 * finish adding seek bar or volume bar onEnterFrame to allow for		 * initialization to complete		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function finishAddBarControl (ctrlOrEvent:Object) : void;
		/**
		 * When a bar with a handle is removed from stage, cleanup the handle.		 * When it is added back to stage, put the handle back!		 *		 * @private         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function cleanupHandle (ctrlOrEvent:Object) : void;
		/**
		 * Fix up progres or fullness bar		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function fixUpBar (definitionHolder:DisplayObject, propPrefix:String, ctrl:DisplayObject, name:String) : void;
		/**
		 * Gets left and right margins for progress or fullness		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function calcBarMargins (ctrl:DisplayObject, type:String, symmetricMargins:Boolean) : void;
		static function getNumberPropSafe (obj:Object, propName:String) : Number;
		static function getBooleanPropSafe (obj:Object, propName:String) : Boolean;
		/**
		 * finish adding buffer bar onEnterFrame to allow for initialization to complete		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function finishAddBufferingBar (e:Event = null) : void;
		/**
		 * Place the buffering pattern and mask over the buffering bar         *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function positionMaskedFill (ctrl:DisplayObject, percent:Number) : void;
		/**
		 * Default startHandleDrag function (can be defined on seek bar		 * movie clip instance) to handle start dragging the seek bar		 * handle or volume bar handle.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function startHandleDrag (ctrl:Sprite) : void;
		/**
		 * Default stopHandleDrag function (can be defined on seek bar		 * movie clip instance) to handle stop dragging the seek bar		 * handle or volume bar handle.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function stopHandleDrag (ctrl:Sprite) : void;
		/**
		 * Default positionHandle function (can be defined on seek bar		 * movie clip instance) to handle positioning seek bar handle.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function positionHandle (ctrl:Sprite) : void;
		/**
		 * helper for other positioning funcs		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function positionBar (ctrl:Sprite, type:String, percent:Number) : void;
		/**
		 * Default calcPercentageFromHandle function (can be defined on		 * seek bar movie clip instance) to handle calculating percentage		 * from seek bar handle position.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function calcPercentageFromHandle (ctrl:Sprite) : void;
		/**
		 * Called to signal end of seek bar scrub.  Call from		 * onRelease and onReleaseOutside event listeners.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleRelease (index:int) : void;
		/**
		 * Called on interval when user scrubbing by dragging seekbar handle.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function seekBarListener (e:TimerEvent) : void;
		/**
		 * Called on interval when user scrubbing by dragging volumebar handle.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function volumeBarListener (e:TimerEvent) : void;
		/**
		 * Called on interval do delay entering buffering state.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function doBufferingDelay (e:TimerEvent) : void;
		function dispatchMessage (index:int) : void;
		function setEnabledAndVisibleForState (index:int, state:String) : void;
		function setupSkinAutoHide (doFade:Boolean) : void;
		function skinAutoHideHitTest (e:TimerEvent, doFade:Boolean = true) : void;
		function skinFadeMore (e:TimerEvent) : void;
		public function enterFullScreenDisplayState () : void;
		function enterFullScreenTakeOver () : void;
		function exitFullScreenTakeOver () : void;
		function hookUpCustomComponents () : void;
		/**
		 * Creates an accessibility implementation for seek bar or volume bar control. 		 *          * @param index The index number of the bar control          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function configureBarAccessibility (index:int) : void;
		/**
		 * Handles keyboard events, and depending on the event target, sets keyboard focus to the appropriate control.		 *          * @param event a KeyboardEvent         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleKeyEvent (event:KeyboardEvent) : void;
		/**
		 * Handles keyboard focus events, to maintain focus on slider controls or the stop button.		 *          * @param event a FocusEvent         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function handleFocusEvent (event:FocusEvent) : void;
		/**
		 * Assigns tabIndex values to each of the FLVPlayback controls by sorting 		 * them horizontally left to right, and returns the next available tabIndex value.		 *          * @param startTabbing the starting tabIndex for FLVPlayback controls		 * 		 * @return the next available tabIndex after the FLVPlayback controls         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function assignTabIndexes (startTabbing:int) : int;
		/**
		 * Checks for the presence of an IFocusManagerComponent on the stage to determine whether or 		 * not to display the FlashPlayer's default yellow focus rectangle, 		 * when a control receives focus.		 * 		 * @return          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function isFocusRectActive () : Boolean;
		/**
		 * Handles a mouse focus change event and forces focus on the appropriate control.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function handleMouseFocusChangeEvent (event:FocusEvent) : void;
	}
}
