package fl.controls
{
	import fl.controls.ButtonLabelPlacement;
	import fl.controls.LabelButton;
	import fl.core.UIComponent;
	import flash.display.DisplayObject;
	import flash.display.Graphics;
	import flash.display.Sprite;
	import flash.display.Shape;
	import Error;

	/**
	 *  @copy fl.controls.LabelButton#style:icon     *     *  @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="icon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:upIcon     *     *  @default CheckBox_upIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:downIcon     *     *  @default CheckBox_downIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:overIcon     *     *  @default CheckBox_overIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="overIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:disabledIcon     *     *  @default CheckBox_disabledIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedDisabledIcon     *     *  @default CheckBox_selectedDisabledIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDisabledIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedUpIcon     *     *  @default CheckBox_selectedUpIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedUpIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedDownIcon     *     *  @default CheckBox_selectedDownIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDownIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:selectedOverIcon     *     *  @default CheckBox_selectedOverIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedOverIcon", type="Class")] 
	/**
	 *  @copy fl.controls.LabelButton#style:textPadding     *     *  @default 5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 

	/**
	 *  The CheckBox component displays a small box that can contain 	 *  a check mark. A CheckBox component can also display an optional	 *  text label that is positioned to the left, right, top, or bottom 	 *  of the CheckBox.     *	 *  <p>A CheckBox component changes its state in response to a mouse	 *  click, from selected to cleared, or from cleared to selected.      *  CheckBox components include a set of <code>true</code> or <code>false</code> values	 *  that are not mutually exclusive.</p>	 *     * @includeExample examples/CheckBoxExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class CheckBox extends LabelButton
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 *  @private		 *  Creates the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * A CheckBox toggles by definition, so the <code>toggle</code> property is set to          * <code>true</code> in the constructor and cannot be changed for a CheckBox.         *         * @default true		 * 		 * @throws Error This value cannot be changed for a CheckBox component.         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get toggle () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set toggle (value:Boolean) : void;
		/**
		 * A CheckBox never auto-repeats by definition, so the <code>autoRepeat</code> property is set          * to <code>false</code> in the constructor and cannot be changed for a CheckBox.         *         * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoRepeat () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoRepeat (value:Boolean) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new CheckBox component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function CheckBox ();
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBackground () : void;
		/**
		 * Shows or hides the focus indicator around this component.		 *		 * @param focused Show or hide the focus indicator.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function drawFocus (focused:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
	}
}
