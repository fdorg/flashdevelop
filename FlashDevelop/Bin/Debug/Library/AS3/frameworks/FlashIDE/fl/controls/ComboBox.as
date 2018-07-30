package fl.controls
{
	import fl.data.DataProvider;
	import fl.containers.BaseScrollPane;
	import fl.controls.BaseButton;
	import fl.controls.List;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.TextInput;
	import fl.controls.TextArea;
	import fl.controls.ScrollBar;
	import fl.controls.SelectableList;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ComponentEvent;
	import fl.events.DataChangeEvent;
	import fl.events.DataChangeType;
	import fl.events.ListEvent;
	import fl.managers.IFocusManagerComponent;
	import fl.data.DataProvider;
	import fl.data.SimpleCollectionItem;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.FocusEvent;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.events.TextEvent;
	import flash.geom.Point;
	import flash.system.IME;
	import flash.text.TextField;
	import flash.text.TextFormat;
	import flash.ui.Keyboard;

	/**
	 * Dispatched when the user changes the selection in the ComboBox component or, if      * the ComboBox component is editable, each time the user enters a keystroke in the      * text field.     *     * @eventType flash.events.Event.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="flash.events.Event")] 
	/**
	 * @copy fl.controls.SelectableList#event:itemRollOver     *     * @eventType fl.events.ListEvent.ITEM_ROLL_OVER     *     * @includeExample examples/ComboBox.itemRollOver.1.as -noswf     *     * @see #event:itemRollOut     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemRollOver", type="fl.events.ListEvent")] 
	/**
	 * @copy fl.controls.SelectableList#event:itemRollOut     *     * @eventType fl.events.ListEvent.ITEM_ROLL_OUT     *     * @see #event:itemRollOver     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemRollOut", type="fl.events.ListEvent")] 
	/**
	 * Dispatched when the drop-down list is dismissed for any reason.     *     * @eventType flash.events.Event.CLOSE     *     * @includeExample examples/ComboBox.close.1.as -noswf     *     * @see #close()     * @see #event:open     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="close", type="flash.events.Event")] 
	/**
	 * Dispatched if the <code>editable</code> property is set to <code>true</code> and the user      * presses the Enter key while typing in the editable text field.     *     * @eventType fl.events.ComponentEvent.ENTER     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="enter", type="fl.events.ComponentEvent")] 
	/**
	 * Dispatched when the user clicks the drop-down button to display      * the drop-down list. Also dispatched when the user clicks the text     * field, if the ComboBox component is not editable.     *     * @eventType flash.events.Event.OPEN     *     * @includeExample examples/ComboBox.open.1.as -noswf     *     * @see #event:close     * @see #open()     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="open", type="flash.events.Event")] 
	/**
	 * Dispatched when the user scrolls the drop-down list of the ComboBox component.     *     * @eventType fl.events.ScrollEvent.SCROLL     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="scroll", type="fl.events.ScrollEvent")] 
	/**
	 * The space that separates the right edge of the component from the     * text representing the selected item, in pixels. The button is      * part of the background skin.     *     * @default 24     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="buttonWidth", type="Number", format="Length")] 
	/**
	 * The space that separates the border from the text representing the     * selected item, in pixels.     *     * @default 3     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 
	/**
	 * The name of the class that provides the background of the ComboBox component.     *     * @default ComboBox_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 * The name of the class that provides the background that appears     * in the ComboBox component when the mouse is over it.     *     * @default ComboBox_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="overSkin", type="Class")] 
	/**
	 * The name of the class that provides the background that appears     * in the ComboBox component when the mouse is down.     *     * @default ComboBox_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downSkin", type="Class")] 
	/**
	 * The name of the class that provides the background that appears       * in the ComboBox component when the <code>enabled</code> property of the      * component is set to <code>false</code>.     *     * @default ComboBox_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.SelectableList#style:cellRenderer     *     * @default fl.controls.listClasses.CellRenderer     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="cellRenderer", type="Class")] 
	/**
	 * @copy fl.containers.BaseScrollPane#style:contentPadding     *     * @default 3     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="contentPadding", type="Number", format="Length")] 
	/**
	 * @copy fl.controls.SelectableList#style:disabledAlpha     *     * @default 0.5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledAlpha", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowDisabledSkin     *     * @default ScrollArrowDown_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowDownSkin     *     * @default ScrollArrowDown_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowOverSkin     *     * @default ScrollArrowDown_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:downArrowUpSkin     *     * @default ScrollArrowDown_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downArrowUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbDisabledSkin     *     * @default ScrollThumb_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbDownSkin     *     * @default ScrollThumb_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbOverSkin     *     * @default ScrollThumb_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbUpSkin     *      * @default ScrollThumb_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbArrowUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackDisabledSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackDownSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackOverSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:trackUpSkin     *     * @default ScrollTrack_Skin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="trackUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowDisabledSkin     *     * @default ScrollArrowUp_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowDownSkin     *     * @default ScrollArrowUp_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowOverSkin     *     * @default ScrollArrowUp_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowOverSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:upArrowUpSkin     *     * @default ScrollArrowUp_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upArrowUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.ScrollBar#style:thumbIcon     *     * @default ScrollBar_thumbIcon     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="thumbIcon", type="Class")] 
	/**
	 * @copy fl.controls.BaseButton#style:repeatDelay     *     * @default 500     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
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
	 * The ComboBox component contains a drop-down list from which the     * user can select one value. Its functionality is similar to that      * of the SELECT form element in HTML. The ComboBox component can be editable,      * in which case the user can type entries that are not in the list      * into the TextInput portion of the ComboBox component.     *     * @includeExample examples/ComboBoxExample.as     *     * @see List     * @see TextInput     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ComboBox extends UIComponent implements IFocusManagerComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var inputField : TextInput;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var background : BaseButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var list : List;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _rowCount : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _editable : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var isOpen : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var highlightedCell : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var editableValue : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _prompt : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var isKeyDown : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var currentIndex : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var listOverIndex : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _dropdownWidth : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _labels : Array;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var collectionItemImport : SimpleCollectionItem;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected static const LIST_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const BACKGROUND_STYLES : Object;
		/**
		 * @private (internal)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * Gets or sets a Boolean value that indicates whether the ComboBox          * component is editable or read-only. A value of <code>true</code> indicates          * that the ComboBox component is editable; a value of <code>false</code>          * indicates that it is not.         *         * <p>In an editable ComboBox component, a user can enter values into the text          * box that do not appear in the drop-down list. The text box displays the text		 * of the item in the list. If a ComboBox component is not editable, text cannot 		 * be entered into the text box. </p>         *         * @default false         *         * @includeExample examples/ComboBox.editable.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get editable () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set editable (value:Boolean) : void;
		/**
		 * Gets or sets the maximum number of rows that can appear in a drop-down           * list that does not have a scroll bar. If the number of items in the          * drop-down list exceeds this value, the list is resized and a scroll bar is displayed,          * if necessary. If the number of items in the drop-down list is less than this value,         * the drop-down list is resized to accommodate the number of items that it contains.         *         * <p>This behavior differs from that of the List component, which always shows the number         * of rows specified by its <code>rowCount</code> property, even if this includes empty space.</p>         *         * @default 5         *         * @includeExample examples/ComboBox.rowCount.1.as -noswf         *         * @see #length         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowCount () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set rowCount (value:uint) : void;
		/**
		 * Gets or sets the characters that a user can enter in the text field.         * If the value of the <code>restrict</code> property is          * a string of characters, you can enter only characters in the string          * into the text field. The string is read from left to right. If          * the value of the <code>restrict</code> property is <code>null</code>, you          * can enter any character. If the value of the <code>restrict</code>          * property is an empty string (""), you cannot enter any character.          * You can specify a range by using the hyphen (-) character. This restricts          * only user interaction; a script can put any character into the text          * field.         *         * @default null         *         * @includeExample examples/ComboBox.restrict.1.as -noswf         *         * @see flash.text.TextField#restrict TextField.restrict         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get restrict () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set restrict (value:String) : void;
		/**
		 * @copy fl.controls.SelectableList#selectedIndex         *         * @default 0         *         * @includeExample examples/ComboBox.selectedIndex.1.as -noswf         * @includeExample examples/ComboBox.selectedIndex.2.as -noswf         *         * @see #selectedItem         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedIndex () : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedIndex (value:int) : void;
		/**
		 * Gets or sets the text that the text box contains in an editable ComboBox component.          * For ComboBox components that are not editable, this value is read-only.          *         * @default ""         *         * @includeExample examples/ComboBox.text.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get text () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set text (value:String) : void;
		/**
		 * @copy fl.controls.List#labelField         *          * @default "label"         *         * @includeExample examples/ComboBox.labelField.1.as -noswf         *         * @see #labelFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelField () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelField (value:String) : void;
		/**
		 * @copy fl.controls.List#labelFunction         *         * @includeExample examples/List.labelFunction.1.as -noswf         * @includeExample examples/ComboBox.labelFunction.2.as -noswf         *         * @see #labelField         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get labelFunction () : Function;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set labelFunction (value:Function) : void;
		/**
		 * Gets or sets the value of the item that is selected in the drop-down list.         * If the user enters text into the text box of an editable ComboBox component, the 		 * <code>selectedItem</code> property is <code>undefined</code>. This  		 * property has a value only if the user selects an item 		 * or if ActionScript is used to select an item from the drop-down list. 		 * If the ComboBox component is not editable, the value of the <code>selectedItem</code> 		 * property is always valid. If there are no items in the drop-down list of 		 * an editable ComboBox component, the value of this property is <code>null</code>.		 *         * @default null         *         * @includeExample examples/ComboBox.selectedItem.1.as -noswf         * @includeExample examples/ComboBox.selectedItem.2.as -noswf         *         * @see #selectedIndex         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedItem () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedItem (value:Object) : void;
		/**
		 * Gets a reference to the List component that the ComboBox component contains. 		 * The List subcomponent is not instantiated within the ComboBox until it 		 * must be displayed. However, the list is created when the <code>dropdown</code>		 * property is accessed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get dropdown () : List;
		/**
		 * Gets the number of items in the list. This property belongs to		 * the List component but can be accessed from a ComboBox instance.		 *         * @default 0         *         * @includeExample examples/ComboBox.length.1.as -noswf         * @includeExample examples/ComboBox.length.2.as -noswf         *         * @see #rowCount         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get length () : int;
		/**
		 * Gets a reference to the TextInput component that the ComboBox 		 * component contains. Use this property to access and manipulate		 * the underlying TextInput component. For example, you can use		 * this property to change the selection of the text box or to 		 * restrict the set of characters that can be entered into it.         *         * @internal includeExample examples/ComboBox.textField.1.as -noswf         *         * @includeExample examples/ComboBox.textField.3.as -noswf         * @includeExample examples/ComboBox.textField.2.as -noswf         * @includeExample examples/ComboBox.textField.4.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get textField () : TextInput;
		/**
		 * Gets the label of an item in an editable ComboBox component. For a ComboBox          * component that is not editable, this property gets the data that the item contains.         *         * @includeExample examples/ComboBox.value.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get value () : String;
		/**
		 * @copy fl.controls.SelectableList#dataProvider         *         * @see fl.data.DataProvider DataProvider         *		 * @includeExample examples/ComboBox.dataProvider.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get dataProvider () : DataProvider;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set dataProvider (value:DataProvider) : void;
		/**
		 * Gets or sets the maximum width of the drop-down list, in pixels. 		 * The default value of this property is the width of the ComboBox 		 * component (the width of the TextInput instance plus the width of the BaseButton instance).         *         * @default 100         *         * @includeExample examples/ComboBox.dropdownWidth.1.as -noswf         * @includeExample examples/ComboBox.dropdownWidth.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get dropdownWidth () : Number;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set dropdownWidth (value:Number) : void;
		/**
		 * Gets or sets the prompt for the ComboBox component. This prompt is a string 		 * that is displayed in the TextInput portion of the ComboBox when         * the <code>selectedIndex</code> is -1. It is usually a string like "Select one...".         * If a prompt is not set, the ComboBox component sets the <code>selectedIndex</code> property         * to 0 and displays the first item in the <code>dataProvider</code> property.         *         * @default ""         *         * @includeExample examples/ComboBox.prompt.1.as -noswf         * @includeExample examples/ComboBox.prompt.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get prompt () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set prompt (value:String) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get imeMode () : String;
		/**
		 * @private (protected)
		 */
		public function set imeMode (value:String) : void;
		/**
		 * Gets the string that is displayed in the TextInput portion 		 * of the ComboBox component. This value is calculated from the data by using          * the <code>labelField</code> or <code>labelFunction</code> property.         *         * @includeExample examples/ComboBox.selectedLabel.1.as -noswf		 *		 * @see #labelField         * @see #labelFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedLabel () : String;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getStyleDefinition () : Object;
		/**
		 * Creates a new ComboBox component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ComboBox ();
		/**
		 * @copy fl.controls.List#itemToLabel()         *         * @see #labelField         * @see #labelFunction         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function itemToLabel (item:Object) : String;
		/**
		 * @copy fl.controls.SelectableList#addItem()         *         * @see #addItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addItem (item:Object) : void;
		/**
		 * @copy fl.controls.SelectableList#addItemAt()         *         * @see #addItem()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addItemAt (item:Object, index:uint) : void;
		/**
		 * @copy fl.controls.SelectableList#removeAll()         *         * @includeExample examples/ComboBox.removeAll.1.as -noswf         *         * @see #removeItem()         * @see #removeItemAt()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeAll () : void;
		/**
		 * @copy fl.controls.SelectableList#removeItem()         *         * @see #removeAll()         * @see #removeItemAt()         *		 * @includeExample examples/ComboBox.removeItem.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeItem (item:Object) : Object;
		/**
		 * Removes the item at the specified index position. The index          * locations of items whose indices are greater than the specified		 * index advance in the array by 1.         *         * <p>This is a method of the List component that is		 * available from an instance of the ComboBox component.</p>		 *		 * @param index Index of the item to be removed.         *         * @throws RangeError The specified index is less than 0 or greater than or 		 *                    equal to the length of the data provider.          *         * @see #removeAll()         * @see #removeItem()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeItemAt (index:uint) : void;
		/**
		 * @copy fl.controls.SelectableList#getItemAt()         *         * @includeExample examples/ComboBox.getItemAt.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getItemAt (index:uint) : Object;
		/**
		 * @copy fl.controls.SelectableList#replaceItemAt()         *         * @includeExample examples/ComboBox.replaceItemAt.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function replaceItemAt (item:Object, index:uint) : Object;
		/**
		 * @copy fl.controls.SelectableList#sortItems()         *         * @see Array#sort()         * @see #sortItemsOn()         *		 * @includeExample examples/ComboBox.sortItems.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function sortItems (...sortArgs:Array) : *;
		/**
		 * @copy fl.controls.SelectableList#sortItemsOn()         *         * @includeExample examples/ComboBox.sortItemsOn.1.as -noswf         * @includeExample examples/ComboBox.sortItemsOn.2.as -noswf         *         * @see Array#sortOn()         * @see #sortItems()		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function sortItemsOn (field:String, options:Object = null) : *;
		/**
		 * Opens the drop-down list.         *         * <p><strong>Note:</strong> Calling this method causes the <code>open</code> 		 * event to be dispatched. If the ComboBox component is already open, calling this method has no effect.</p>         *         * @see #close()         * @see #event:open         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function open () : void;
		/**
		 * Closes the drop-down list.         *         * <p><strong>Note:</strong> Calling this method causes the <code>close</code>          * event to be dispatched. If the ComboBox is already closed, calling this method has no effect.</p>         *         * @includeExample examples/ComboBox.close.1.as -noswf         *         * @see #open()         * @see #event:close         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function close () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusInHandler (event:FocusEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusOutHandler (event:FocusEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleDataChange (event:DataChangeEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setEmbedFonts () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function showPrompt () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setStyles () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawTextFormat () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawList () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function positionList () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawTextField () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onInputFieldFocus (event:FocusEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onInputFieldFocusOut (event:FocusEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onEnter (event:ComponentEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onToggleListVisibility (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onListItemUp (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onListChange (event:Event) : void;
		/**
		 * @private (protected)
		 */
		protected function onStageClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function passEvent (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function addCloseListener (event:Event);
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onTextInput (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function calculateAvailableHeight () : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function highlightCell (index:int = -1) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyUpHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initializeAccessibility () : void;
	}
}
