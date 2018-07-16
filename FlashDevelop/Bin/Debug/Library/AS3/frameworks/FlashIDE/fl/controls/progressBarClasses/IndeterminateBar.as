package fl.controls.progressBarClasses
{
	import fl.controls.ProgressBar;
	import fl.core.UIComponent;
	import fl.core.InvalidationType;
	import flash.display.BitmapData;
	import flash.display.DisplayObject;
	import flash.display.Graphics;
	import flash.display.Sprite;
	import flash.events.Event;

	/**
	 * @copy fl.controls.ProgressBar#style:indeterminateSkin     *     * @default ProgressBar_indeterminateSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="indeterminateSkin", type="Class")] 

	/**
	 * The IndeterminateBar class handles the drawing of the progress bar component when the 	 * size of the source that is being loaded is unknown. This class can be replaced with any 	 * other UIComponent class to render the bar differently. The default implementation uses 	 * the drawing API create a striped fill to indicate the progress of the load operation.	 *	 * @includeExample examples/IndeterminateBarExample.as	 *     * @see fl.controls.ProgressBar     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class IndeterminateBar extends UIComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var animationCount : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var bar : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var barMask : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var patternBmp : BitmapData;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * Gets or sets a Boolean value that indicates whether the indeterminate bar is visible.		 * A value of <code>true</code> indicates that the indeterminate bar is visible; a value		 * of <code>false</code> indicates that it is not.         *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get visible () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set visible (value:Boolean) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new instance of the IndeterminateBar component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function IndeterminateBar ();
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function startAnimation () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function stopAnimation () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleEnterFrame (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawPattern () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawMask () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBar () : void;
	}
}
