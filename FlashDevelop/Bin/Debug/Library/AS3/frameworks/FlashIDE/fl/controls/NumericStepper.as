package fl.controls
{
	import fl.controls.BaseButton;
	import fl.controls.TextInput;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ComponentEvent;
	import fl.managers.IFocusManagerComponent;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.FocusEvent;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.events.TextEvent;
	import flash.ui.Keyboard;

	/**
	 * The class that provides the skin for the down arrow when it is disabled.	 *     * @default NumericStepperDownArrow_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDisabledSkin", type="Class")] 
	/**
	 * The class that provides the skin for the down arrow when it is in a down state.	 *     * @default NumericStepperDownArrow_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDownSkin", type="Class")] 
	/**
	 * The class that provides the skin for the down arrow when the mouse is over the component.	 *	 * @default NumericStepperDownArrow_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowOverSkin", type="Class")] 
	/**
	 * The class that provides the skin for the down arrow when it is in its default state.	 * 	 * @default NumericStepperDownArrow_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowUpSkin", type="Class")] 
	/**
	 * The class that provides the skin for the up arrow when it is disabled.	 *     * @default NumericStepperUpArrow_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDisabledSkin", type="Class")] 
	/**
	 * The class that provides the skin for the up arrow when it is in the down state.	 *     * @default NumericStepperUpArrow_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDownSkin", type="Class")] 
	/**
	 * The class that provides the skin for the down arrow during mouse over.	 *     * @default NumericStepperUpArrow_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowOverSkin", type="Class")] 
	/**
	 * The class that provides the skin for the up arrow when it is in the up state.	 *     * @default NumericStepperUpArrow_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowUpSkin", type="Class")] 
	/**
	 * The class that provides the skin for the text input box.	 *     * @default NumericStepper_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="TextInput_upskin", type="Class")] 
	/**
	 * The skin used for the up arrow when it is in an up state.	 *	 * @default NumericStepper_disabledSkin	 *	 * @internal [kenos] Is this description correct? Most of the skins are provides by classes and this looks	 *                   more like some of the width or height specifiers. The style name also doesn't seem to     *                   match the description.     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="TextInput_disabledSkin", type="Number", format="Length")] 
	/**
	 * @copy fl.controls.BaseButton#style:repeatDelay     *      * @default 500     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="repeatDelay", type="Number", format="Time")] 
	/**
	 * @copy fl.controls.BaseButton#style:repeatInterval     *     * @default 35     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="repeatInterval", type="Number", format="Time")] 
	/**
	 * @copy fl.controls.LabelButton#style:embedFonts     *      * @default false     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="embedFonts", type="Boolean")] 
	/**
	 *  Dispatched when the user changes the value of the NumericStepper component.     *	 *  <p><strong>Note:</strong> This event is not dispatched if ActionScript 	 *  is used to change the value.</p>	 *     *  @eventType flash.events.Event.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="flash.events.Event")] 

	/**
	 * The NumericStepper component displays an ordered set of numbers from which	 * the user can make a selection. This component includes a single-line field 	 * for text input and a pair of arrow buttons that can be used to step through 	 * the set of values. The Up and Down arrow keys can also be used to view the 	 * set of values. The NumericStepper component dispatches a <code>change</code> 	 * event after there is a change in its current value. This component also contains	 * the <code>value</code> property; you can use this property to obtain the current 	 * value of the component.	 *     * @includeExample examples/NumericStepperExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class NumericStepper extends UIComponent implements IFocusManagerComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var inputField : TextInput;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var upArrow : BaseButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var downArrow : BaseButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _maximum : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _minimum : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _value : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _stepSize : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _precision : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected static const DOWN_ARROW_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const UP_ARROW_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const TEXT_INPUT_STYLES : Object;

		/**
		 * @copy fl.core.UIComponent#enabled         *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function get enabled () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets the maximum value in the sequence of numeric values.         *         * @default 10         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maximum () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maximum (value:Number) : void;
		/**
		 * Gets or sets the minimum number in the sequence of numeric values.         *         * @default 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get minimum () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set minimum (value:Number) : void;
		/**
		 * Gets the next value in the sequence of values.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get nextValue () : Number;
		/**
		 * Gets the previous value in the sequence of values.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get previousValue () : Number;
		/**
		 * Gets or sets a nonzero number that describes the unit of change between 		 * values. The <code>value</code> property is a multiple of this number 		 * less the minimum. The NumericStepper component rounds the resulting value to the 		 * nearest step size.         *         * @default 1         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get stepSize () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set stepSize (value:Number) : void;
		/**
		 * Gets or sets the current value of the NumericStepper component.         *         * @default 1         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get value () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set value (value:Number) : void;
		/**
		 * Gets a reference to the TextInput component that the NumericStepper		 * component contains. Use this property to access and manipulate the 		 * underlying TextInput component. For example, you can use this		 * property to change the current selection in the text box or to		 * restrict the characters that the text box accepts.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get textField () : TextInput;
		/**
		 * @copy fl.controls.TextArea#imeMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get imeMode () : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set imeMode (value:String) : void;

		/**
		 * Creates a new NumericStepper component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function NumericStepper ();
		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setValue (value:Number, fireEvent:Boolean = true) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function stepperPressHandler (event:ComponentEvent) : void;
		/**
		 * @copy fl.core.UIComponent#drawFocus()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function drawFocus (event:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusOutHandler (event:FocusEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onTextChange (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function passEvent (event:Event) : void;
		/**
		 * Sets focus to the component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFocus () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function isOurFocus (target:DisplayObject) : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setStyles () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function inRange (num:Number) : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function inStep (num:Number) : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getValidValue (num:Number) : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function getPrecision () : Number;
	}
}
