package fl.controls
{
	import fl.controls.RadioButton;
	import flash.utils.Dictionary;
	import flash.events.EventDispatcher;
	import flash.events.Event;

	/**
	 * Dispatched when the selected RadioButton instance in a group changes.     *     * @includeExample examples/RadioButtonGroup.change.1.as -noswf     * @eventType flash.events.Event.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="flash.events.Event")] 
	/**
	 * Dispatched when a RadioButton instance is clicked.     *     * @eventType flash.events.MouseEvent.CLICK     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="click", type="flash.events.MouseEvent")] 

	/**
	 * The RadioButtonGroup class defines a group of RadioButton components 	 * to act as a single component. When one radio button is selected, no other	 * radio buttons from the same group can be selected.	 *     * @see RadioButton     * @see RadioButton#group RadioButton.group     *     * @includeExample examples/RadioButtonGroupExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class RadioButtonGroup extends EventDispatcher
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var groups : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var groupCount : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _name : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var radioButtons : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _selection : RadioButton;

		/**
		 * Gets the instance name of the radio button.         *         * @default "RadioButtonGroup"         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get name () : String;
		/**
		 * Gets or sets a reference to the radio button that is currently selected          * from the radio button group.         *         * @includeExample examples/RadioButtonGroup.selection.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selection () : RadioButton;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selection (value:RadioButton) : void;
		/**
		 * Gets or sets the selected radio button's <code>value</code> property.         * If no radio button is currently selected, this property is <code>null</code>.         *         * @includeExample examples/RadioButtonGroup.selectedData.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedData () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedData (value:Object) : void;
		/**
		 * Gets the number of radio buttons in this radio button group.         *         * @default 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get numRadioButtons () : int;

		/**
		 * Retrieves a reference to the specified radio button group.		 *		 * @param name The name of the group for which to retrieve a reference.		 *         * @return A reference to the specified RadioButtonGroup.         *		 * @includeExample examples/RadioButtonGroup.getGroup.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getGroup (name:String) : RadioButtonGroup;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function registerGroup (group:RadioButtonGroup) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function cleanUpGroups () : void;
		/**
		 * Creates a new RadioButtonGroup instance.  		 * This is usually done automatically when a radio button is instantiated.		 *          * @param name The name of the radio button group.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function RadioButtonGroup (name:String);
		/**
		 * Adds a radio button to the internal radio button array for use with 		 * radio button group indexing, which allows for the selection of a single radio button		 * in a group of radio buttons.  This method is used automatically by radio buttons, 		 * but can also be manually used to explicitly add a radio button to a group.		 *         * @param radioButton The RadioButton instance to be added to the current radio button group.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addRadioButton (radioButton:RadioButton) : void;
		/**
		 * Clears the RadioButton instance from the internal list of radio buttons.		 *         * @param radioButton The RadioButton instance to remove.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeRadioButton (radioButton:RadioButton) : void;
		/**
		 * Returns the index of the specified RadioButton instance.		 *		 * @param radioButton The RadioButton instance to locate in the current RadioButtonGroup.		 *         * @return The index of the specified RadioButton component, or -1 if the specified RadioButton was not found.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getRadioButtonIndex (radioButton:RadioButton) : int;
		/**
		 * Retrieves the RadioButton component at the specified index location.		 *         * @param index The index of the RadioButton component in the RadioButtonGroup component,          *        where the index of the first component is 0. 		 *         * @return The specified RadioButton component.		 *         * @throws RangeError The specified index is less than 0 or greater than or equal to the length of the data provider.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getRadioButtonAt (index:int) : RadioButton;
	}
}
