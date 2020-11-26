package fl.accessibility
{
	import flash.accessibility.Accessibility;
	import flash.accessibility.AccessibilityProperties;
	import fl.controls.ScrollBar;
	import fl.core.UIComponent;
	import flash.events.Event;

	/**
	 * The UIComponentAccImpl class, also called the UIComponent Accessibility Implementation class,	 * is used to make a UIComponent accessible. This class enables communication	 * between a List-based component and a screen reader. Screen readers are used to translate	 * screen content into synthesized speech or braille for visually impaired users.	 * 	 * <p>The ListAccImpl class supports system roles, object-based events, and states.</p>	 * 	 * @internal     * Adobe: Probably just a dup-and-revise error, but this last sentence references the BaseListAccImpl class. 	 * Probably should refer to the UIComponentAccImpl class?	 * [CW] the whole description is wrong here. UIComponentAccImpl extends AccessibilityProperties	 * and not AccessibilityImplementation, and exists purely to wrap the accessibilityProperties 	 * property of UIComponents (and to make the ScrollBar component silent to screenreaders).     *      * Metaliq: Claus, did you change the description above? I still see a mention of BaseListAccImpl class. (line 15, i changed it to ListAccImpl though)     *     * @see fl.core.UIComponent UIComponent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class UIComponentAccImpl extends AccessibilityProperties
	{
		/**
		 *  @private         *  Static variable triggering the <code>hookAccessibility()</code> method.		 *  This is used for initializing UIComponentAccImpl class to hook its         *  <code>createAccessibilityImplementation()</code> method to UIComponent class          *  before it gets called from <code>UIComponent.initialize()</code>.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var accessibilityHooked : Boolean;
		/**
		 *  @private (protected)         *  A reference to the UIComponent itself.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var master : UIComponent;

		/**
		 * @private		 * Static Method for swapping the <code>createAccessibilityImplementation()</code>          * method of UIComponent with the UIComponentAccImpl class.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function hookAccessibility () : Boolean;
		/**
		 *  @private		 *  Method for creating the Accessibility class.		 *  This method is called from UIComponent. 		 * 		 *  @param component The UIComponent instance that this AccImpl instance         *  is making accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function createAccessibilityImplementation (component:UIComponent) : void;
		/**
		 *  Enables accessibility for a UIComponent component.		 *  This method is required for the compiler to activate         *  the accessibility classes for a component.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function enableAccessibility () : void;
		/**
		 * @private         * @internal Nivesh says: I don't think we should document the constructors          *           for the accessibility classes.  End-users just have to call the          *           static enableAccessibility method.  They don't really create an          *           instance of the classes.         *		 *  Creates a new UIComponentAccImpl instance for the specified UIComponent component.		 *		 *  @param master The UIComponent instance that this AccImpl instance         *  makes accessible.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function UIComponentAccImpl (component:UIComponent);
		/**
		 *  @private (protected)		 *  Generic event handler.		 *  All UIComponentAccImpl subclasses must implement this		 *  to listen for events from its master component. 		 *          *  @param event The event object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function eventHandler (event:Event) : void;
	}
}
