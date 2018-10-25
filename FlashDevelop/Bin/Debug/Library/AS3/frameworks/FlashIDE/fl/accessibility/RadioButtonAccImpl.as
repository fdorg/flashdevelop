package fl.accessibility
{
	import fl.controls.RadioButton;
	import fl.core.UIComponent;

	/**
	 * The RadioButtonAccImpl class, also called the RadioButton Accessibility Implementation class,	 * is used to make a RadioButton component accessible. This class enables communication	 * between a RadioButton component and a screen reader. Screen readers are used to translate	 * screen content into synthesized speech or braille for visually impaired users. 	 * 	 * <p>The RadioButtonAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * <p>A RadioButton reports the role <code>ROLE_SYSTEM_RADIOBUTTON</code> (0x2D) to a screen 	 * reader.</p>     *	 * <p>A RadioButton reports the following states to a screen reader:</p>	 * <ul>     *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>     *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>     *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>     *     <li><code>STATE_SYSTEM_PRESSED</code> (0x00000008)</li>     *     <li><code>STATE_SYSTEM_CHECKED</code> (0x00000010)</li>     *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>     *	 * <p>A RadioButton dispatches the following events to a screen reader:</p>	 * <ul>	 *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>	 *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>	 * </ul>	 * 	 * @see fl.controls.RadioButton RadioButton     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class RadioButtonAccImpl extends CheckBoxAccImpl
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing CheckBoxAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to CheckBox class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;

		/**
		 *  @private         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of CheckBox with the CheckBoxAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a RadioButton component.		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a RadioButtonAccImpl instance for the specified RadioButton component.		 *		 *  @param master The RadioButton instance that this AccImpl instance         *         makes accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function RadioButtonAccImpl (master:UIComponent);
	}
}
