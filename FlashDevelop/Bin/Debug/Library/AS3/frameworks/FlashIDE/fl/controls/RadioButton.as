package fl.controls
{
	import fl.controls.ButtonLabelPlacement;
	import fl.controls.LabelButton;
	import fl.controls.RadioButtonGroup;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.managers.IFocusManager;
	import fl.managers.IFocusManagerGroup;
	import flash.display.DisplayObject;
	import flash.display.Graphics;
	import flash.display.Shape;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.ui.Keyboard;

	/**
	 * Dispatched when the radio button instance's <code>selected</code> property changes.	 *     * @includeExample examples/RadioButton.change.1.as -noswf     *     * @eventType flash.events.Event.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change" , type="flash.events.Event")] 
	/**
	 * Dispatched when the user clicks the radio button with the mouse or spacebar.     *     * @includeExample examples/RadioButton.change.1.as -noswf     *     * @eventType flash.events.MouseEvent.CLICK     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="click" , type="flash.events.MouseEvent")] 
	/**
	 * @copy fl.controls.LabelButton#style:icon     *     * @default null     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="icon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:upIcon     *     * @default RadioButton_upIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:downIcon     *     * @default RadioButton_downIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:overIcon     *     * @default RadioButton_overIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="overIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:disabledIcon     *     * @default RadioButton_disabledIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedDisabledIcon     *     * @default RadioButton_selectedDisabledIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDisabledIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedUpIcon     *     * @default RadioButton_selectedUpIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedUpIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedDownIcon     *     * @default RadioButton_selectedDownIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDownIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedOverIcon     *     * @default RadioButton_selectedOverIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedOverIcon", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:textPadding     *     * @default 5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 

	/**
	 * The RadioButton component lets you force a user to make a single	 * selection from a set of choices. This component must be used in a	 * group of at least two RadioButton instances. Only one member of	 * the group can be selected at any given time. Selecting one radio	 * button in a group deselects the currently selected radio button     * in the group. You set the <code>groupName</code> parameter to indicate which     * group a radio button belongs to. When the user clicks or tabs into a RadioButton     * component group, only the selected radio button receives focus.     *     * <p>A radio button can be enabled or disabled. A disabled radio button does not receive mouse or	 * keyboard input.</p>     *     * @see RadioButtonGroup	 *     * @includeExample examples/RadioButtonExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class RadioButton extends LabelButton implements IFocusManagerGroup
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _value : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 * The RadioButtonGroup object to which this RadioButton component instance belongs.
		 */
		protected var _group : RadioButtonGroup;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var defaultGroupName : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * A radio button is a toggle button; its <code>toggle</code> property is set to         * <code>true</code> in the constructor and cannot be changed.		 * 		 * @throws Error This property cannot be set on the RadioButton.		 *          * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get toggle () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set toggle (value:Boolean) : void;
		/**
		 * A radio button never auto-repeats by definition, so the <code>autoRepeat</code> property is set to         * <code>false</code> in the constructor and cannot be changed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoRepeat () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoRepeat (value:Boolean) : void;
		/**
		 * Indicates whether a radio button is currently selected (<code>true</code>) or deselected (<code>false</code>).         * You can only set this value to <code>true</code>; setting it to <code>false</code> has no effect. To         * achieve the desired effect, select a different radio button in the same radio button group.		 *          * @default false         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selected (value:Boolean) : void;
		/**
		 * The group name for a radio button instance or group. You can use this property to get          * or set a group name for a radio button instance or for a radio button group.         *         * @default "RadioButtonGroup"         *		 * @includeExample examples/RadioButton.groupName.1.as -noswf		 *		 * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get groupName () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set groupName (group:String) : void;
		/**
		 * The RadioButtonGroup object to which this RadioButton belongs.         *		 * @includeExample examples/RadioButton.group.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get group () : RadioButtonGroup;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set group (name:RadioButtonGroup) : void;
		/**
		 * A user-defined value that is associated with a radio button.		 *          * @default null         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get value () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set value (val:Object) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new RadioButton component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function RadioButton ();
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * Shows or hides the focus indicator around this component instance.		 *         * @param focused Show or hide the focus indicator.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function drawFocus (focused:Boolean) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleChange (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBackground () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyUpHandler (event:KeyboardEvent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function setPrev (moveSelection:Boolean = true) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function setNext (moveSelection:Boolean = true) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function setThis () : void;
	}
}
