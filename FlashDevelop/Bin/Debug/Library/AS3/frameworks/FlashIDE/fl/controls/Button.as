package fl.controls
{
	import fl.controls.LabelButton;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.managers.IFocusManagerComponent;
	import flash.display.DisplayObject;
	import flash.display.Sprite;

	/**
	 * The skin to be used when a button has emphasis.     *     * @default Button_emphasizedSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="emphasizedSkin", type="Class")] 
	/**
	 * The padding to be applied around the Buttons in an 	 * emphasized skin, in pixels.     *     * @default 2     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="emphasizedPadding", type="Number", format="Length")] 

	/**
	 * The Button component represents a commonly used rectangular button. 	 * Button components display a text label, an icon, or both.	 *	 * <p>A Button component is typically associated with an event handler	 * method that listens for a <code>click</code> event and performs the 	 * specified task after the <code>click</code> event is dispatched. When  	 * the user clicks an enabled button, the button dispatches the <code>click</code> 	 * and <code>buttonDown</code> events. Even if it is not enabled, a button 	 * dispatches other events including <code>mouseMove</code>, <code>mouseOver</code>, <code>mouseOut</code>, 	 * <code>rollOver</code>, <code>rollOut</code>, <code>mouseDown</code>, 	 * and <code>mouseUp</code>.</p>	 *	 * <p>You can change the appearance of the button by associating a different	 * skin with each button state. A Button component can also be set to	 * function as a push button or a toggle button. </p>	 *	 * @langversion 3.0     * @playerversion Flash 9.0.28.0	 * @includeExample examples/ButtonExample.as
	 */
	public class Button extends LabelButton implements IFocusManagerComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _emphasized : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var emphasizedBorder : DisplayObject;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 *  @private         *		 *  Method for creating the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * Gets or sets a Boolean value that indicates whether a border is drawn 		 * around the Button component when the button is in its up state. A value 		 * of <code>true</code> indicates that the button is surrounded by a border 		 * when it is in its up state; a value of <code>false</code> indicates that 		 * it is not.         *         * @default false         *         * @includeExample examples/Button.emphasized.1.as -noswf         *          * @see #style:emphasizedPadding         * @see #style:emphasizedSkin         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get emphasized () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set emphasized (value:Boolean) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new Button component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function Button ();
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawEmphasized () : void;
		/**
		 * @private         *		 * @copy fl.core.UIComponent#drawFocus()         * @internal Added logic to resize focusRect if button has emphasis         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function drawFocus (focused:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
	}
}
