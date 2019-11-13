package fl.controls
{
	import fl.controls.progressBarClasses.IndeterminateBar;
	import fl.controls.ProgressBarDirection;
	import fl.controls.ProgressBarMode;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import flash.display.DisplayObject;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.EventDispatcher;
	import flash.events.ProgressEvent;

	/**
	 * Dispatched when the load operation completes.      *     * @eventType flash.events.Event.COMPLETE     *     * @includeExample examples/ProgressBar.complete.1.as -noswf     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("complete", type="flash.events.Event")] 
	/**
	 * Dispatched as content loads in event mode or polled mode.      *     * @eventType flash.events.ProgressEvent.PROGRESS     *     * @includeExample examples/ProgressBar.complete.1.as -noswf     *     * @see ProgressBarMode#EVENT ProgressBarMode.EVENT     * @see ProgressBarMode#POLLED ProgressBarMode.POLLED     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("progress", type="flash.events.ProgressEvent")] 
	/**
	 * Name of the class to use as the default icon. Setting any other icon       * style overrides this setting.     *       * @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="icon", type="Class")] 
	/**
	 * Name of the class to use as the progress indicator track.     *      * @default ProgressBar_trackSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackSkin", type="Class")] 
	/**
	 * Name of the class to use as the determinate progress bar.     *      * @default ProgressBar_barSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="barSkin", type="Class")] 
	/**
	 * Name of the class to use as the indeterminate progress bar. This is passed to the       * indeterminate bar renderer, which is specified by the <code>indeterminateBar</code>      * style.     *       * @default ProgressBar_indeterminateSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="indeterminateSkin", type="Class")] 
	/**
	 * The class to use as a renderer for the indeterminate bar animation.       * This is an advanced style.     *      * @default fl.controls.progressBarClasses.IndeterminateBar     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="indeterminateBar", type="Class")] 
	/**
	 * The padding that separates the progress bar indicator from the track, in pixels.     *      * @default 0     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="barPadding", type="Number", format="Length")] 

	/**
	 * The ProgressBar component displays the progress of content that is 	 * being loaded. The ProgressBar is typically used to display the status of 	 * images, as well as portions of applications, while they are loading. 	 * The loading process can be determinate or indeterminate. A determinate 	 * progress bar is a linear representation of the progress of a task over 	 * time and is used when the amount of content to load is known. An indeterminate      * progress bar has a striped fill and a loading source of unknown size.	 *     * @includeExample examples/ProgressBarExample.as     *     * @see ProgressBarDirection     * @see ProgressBarMode     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ProgressBar extends UIComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var track : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var determinateBar : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var indeterminateBar : UIComponent;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _direction : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _indeterminate : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _mode : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _minimum : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _maximum : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _value : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _source : Object;
		/**
		 * @private (protected)
		 */
		protected var _loaded : Number;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * Indicates the fill direction for the progress bar. A value of          * <code>ProgressBarDirection.RIGHT</code> indicates that the progress          * bar is filled from left to right. A value of <code>ProgressBarDirection.LEFT</code>         * indicates that the progress bar is filled from right to left.         *         * @default ProgressBarDirection.RIGHT		 *		 * @includeExample examples/ProgressBar.direction.1.as -noswf		 *         * @see ProgressBarDirection         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get direction () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set direction (value:String) : void;
		/**
		 * Gets or sets a value that indicates the type of fill that the progress 		 * bar uses and whether the loading source is known or unknown. A value of 		 * <code>true</code> indicates that the progress bar has a striped fill 		 * and a loading source of unknown size. A value of <code>false</code> 		 * indicates that the progress bar has a solid fill and a loading source 		 * of known size. 		 *		 * <p>This property can only be set when the progress bar mode 		 * is set to <code>ProgressBarMode.MANUAL</code>.</p>         *         * @default true		 *		 * @see #mode		 * @see ProgressBarMode         * @see fl.controls.progressBarClasses.IndeterminateBar IndeterminateBar         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get indeterminate () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set indeterminate (value:Boolean) : void;
		/**
		 * Gets or sets the minimum value for the progress bar when the 		 * <code>ProgressBar.mode</code> property is set to <code>ProgressBarMode.MANUAL</code>.         *         * @default 0         *         * @see #maximum         * @see #percentComplete         * @see #value         * @see ProgressBarMode#MANUAL         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get minimum () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set minimum (value:Number) : void;
		/**
		 * Gets or sets the maximum value for the progress bar when the 		 * <code>ProgressBar.mode</code> property is set to <code>ProgressBarMode.MANUAL</code>.         *         * @default 0         *         * @see #minimum         * @see #percentComplete         * @see #value         * @see ProgressBarMode#MANUAL         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maximum () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maximum (value:Number) : void;
		/**
		 * Gets or sets a value that indicates the amount of progress that has 		 * been made in the load operation. This value is a number between the 		 * <code>minimum</code> and <code>maximum</code> values.         *         * @default 0         *         * @see #maximum         * @see #minimum         * @see #percentComplete         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get value () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set value (value:Number) : void;
		/**
		 * @private (internal)
		 */
		public function set sourceName (name:String) : void;
		/**
		 * Gets or sets a reference to the content that is being loaded and for		 * which the ProgressBar is measuring the progress of the load operation.          * A typical usage of this property is to set it to a UILoader component.		 *		 * <p>Use this property only in event mode and polled mode.</p>         *         * @default null		 *         * @includeExample examples/ProgressBar.source.1.as -noswf         * @includeExample examples/ProgressBar.source.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set source (value:Object) : void;
		/**
		 * Gets a number between 0 and 100 that indicates the percentage 		 * of the content has already loaded. 		 *		 * <p>To change the percentage value, use the <code>setProgress()</code> method.</p>         *         * @default 0         *         * @includeExample examples/ProgressBar.percentComplete.1.as -noswf         * @includeExample examples/ProgressBar.percentComplete.2.as -noswf         *         * @see #maximum         * @see #minimum         * @see #setProgress()         * @see #value         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get percentComplete () : Number;
		/**
		 * Gets or sets the method to be used to update the progress bar. 		 *		 * <p>The following values are valid for this property:</p> 		 * <ul>		 *     <li><code>ProgressBarMode.EVENT</code></li>		 *     <li><code>ProgressBarMode.POLLED</code></li>		 *     <li><code>ProgressBarMode.MANUAL</code></li>		 * </ul>         *         * <p>Event mode and polled mode are the most common modes. In event mode, 		 * the <code>source</code> property specifies loading content that generates  		 * <code>progress</code> and <code>complete</code> events; you should use 		 * a UILoader object in this mode. In polled mode, the <code>source</code> 		 * property specifies loading content, such as a custom class, that exposes 		 * <code>bytesLoaded</code> and <code>bytesTotal</code> properties. Any object 		 * that exposes these properties can be used as a source in polled mode.</p>         *         * <p>You can also use the ProgressBar component in manual mode by manually          * setting the <code>maximum</code> and <code>minimum</code> properties and          * making calls to the <code>ProgressBar.setProgress()</code> method.</p>         *         * @default ProgressBarMode.EVENT		 *          * @see ProgressBarMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get mode () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set mode (value:String) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new ProgressBar component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ProgressBar ();
		/**
		 * Sets the state of the bar to reflect the amount of progress made when          * using manual mode. The <code>value</code> argument is assigned to the          * <code>value</code> property and the <code>maximum</code> argument is         * assigned to the <code>maximum</code> property. The <code>minimum</code>          * property is not altered.		 *         * @param value A value describing the progress that has been made. 		 *         * @param maximum The maximum progress value of the progress bar.		 *         * @see #maximum         * @see #value         * @see ProgressBarMode#MANUAL ProgressBarMode.manual         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setProgress (value:Number, maximum:Number) : void;
		/**
		 * Resets the progress bar for a new load operation.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function reset () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function _setProgress (value:Number, maximum:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setIndeterminate (value:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function resetProgress () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setupSourceEvents () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function cleanupSourceEvents () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function pollSource (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleProgress (event:ProgressEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleComplete (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawTrack () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBars () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawDeterminateBar () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
	}
}
