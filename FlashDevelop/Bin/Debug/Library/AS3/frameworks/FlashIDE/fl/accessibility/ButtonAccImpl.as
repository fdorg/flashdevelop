package fl.accessibility
{
	import fl.controls.Button;
	import fl.core.UIComponent;

	/**
	 *  The ButtonAccImpl class, also called the Button Accessibility Implementation class, 	 *  enables communication between a Button component and a screen reader. Screen readers are used 	 *  to translate screen content into synthesized speech or braille for visually impaired users.	 * 	 * <p>The ButtonAccImpl class supports system roles, object-based events, and states.</p> 	 * 	 * <p>A Button reports the role <code>ROLE_SYSTEM_PUSHBUTTON</code> (0x2B) to a screen 	 * reader.</p>     *	 * <p>A Button reports the following states to a screen reader:</p>	 * <ul>	 *     <li><code>STATE_SYSTEM_NORMAL</code> (0x00000000)</li>	 *     <li><code>STATE_SYSTEM_UNAVAILABLE</code> (0x00000001)</li>	 *     <li><code>STATE_SYSTEM_FOCUSED</code> (0x00000004)</li>	 *     <li><code>STATE_SYSTEM_PRESSED</code> (0x00000008)</li>	 *     <li><code>STATE_SYSTEM_FOCUSABLE</code> (0x00100000)</li>	 * </ul>     *	 * <p>A Button dispatches the following events to a screen reader:</p>	 * <ul>	 *     <li><code>EVENT_OBJECT_STATECHANGE</code> (0x800A)</li>	 *     <li><code>EVENT_OBJECT_NAMECHANGE</code> (0x800C)</li>	 * </ul>	 * 	 * <p>The user of a screen reader can activate a button component by using the spacebar or the Enter key.</p>	 *      * @see fl.controls.Button Button     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ButtonAccImpl extends LabelButtonAccImpl
	{
		/**
		 *  @private         *         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing ButtonAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to Button class          *  before it gets called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;

		/**
		 *  @private         *         *  Static method for swapping the <code>createAccessibilityImplementation()</code>         *  method of Button with the ButtonAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent.		 * 		 *  @param component The UIComponent instance that this AccImpl instance		 *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 * Enables accessibility for a Button component.		 * This method is required for the compiler to activate		 * the accessibility classes for a component.		 *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a ButtonAccImpl instance for the specified Button component.		 *		 *  @param component The Button instance that this ButtonAccImpl instance		 *         makes accessible.		 *           * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ButtonAccImpl (component:UIComponent);
	}
}
