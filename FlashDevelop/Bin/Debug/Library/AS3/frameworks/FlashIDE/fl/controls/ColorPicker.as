package fl.controls
{
	import fl.core.UIComponent;
	import fl.core.InvalidationType;
	import fl.controls.BaseButton;
	import fl.controls.TextInput;
	import fl.controls.TextArea;
	import fl.events.ComponentEvent;
	import fl.events.ColorPickerEvent;
	import fl.managers.IFocusManager;
	import fl.managers.IFocusManagerComponent;
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.FocusEvent;
	import flash.display.DisplayObject;
	import flash.display.Graphics;
	import flash.display.Sprite;
	import flash.geom.ColorTransform;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.text.TextField;
	import flash.text.TextFieldType;
	import flash.text.TextFormat;
	import flash.ui.Keyboard;
	import flash.system.IME;

	/**
	 * Dispatched when the user opens the color palette.     *     * @eventType flash.events.Event.OPEN     *     * @includeExample examples/ColorPicker.open.2.as -noswf     *     * @see #event:close     * @see #open()     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="open", type="flash.events.Event")] 
	/**
	 * Dispatched when the user closes the color palette.     *     * @eventType flash.events.Event.CLOSE     *     * @see #event:open     * @see #close()     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="close", type="flash.events.Event")] 
	/**
	 * Dispatched when the user clicks a color in the palette.     *     * @includeExample examples/ColorPicker.hexValue.1.as -noswf     *     * @eventType fl.events.ColorPickerEvent.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="fl.events.ColorPickerEvent")] 
	/**
	 * Dispatched when the user rolls over a swatch in the color palette.     *     * @eventType fl.events.ColorPickerEvent.ITEM_ROLL_OVER     *     * @see #event:itemRollOut     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemRollOver", type="fl.events.ColorPickerEvent")] 
	/**
	 * Dispatched when the user rolls out of a swatch in the color palette.     *     * @eventType fl.events.ColorPickerEvent.ITEM_ROLL_OUT     *     * @see #event:itemRollOver     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="itemRollOut", type="fl.events.ColorPickerEvent")] 
	/**
	 * Dispatched when the user presses the Enter key after editing the internal text field of the ColorPicker component.     *     * @eventType fl.events.ColorPickerEvent.ENTER     *     * @includeExample examples/ColorPicker.enter.1.as -noswf     *     * @see #editable     * @see #textField     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="enter", type="fl.events.ColorPickerEvent")] 
	/**
	 * Defines the padding that appears around each swatch in the color palette, in pixels.     *     * @default 1     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="swatchPadding", type="Number", format="Length")] 
	/**
	 * The class that provides the skin for a disabled button in the ColorPicker.     *     * @default ColorPicker_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0     *     * @internal [kenos] Changed description from "Disabled skin for the ColorPicker button" to the current.     * Is this correct?
	 */
	[Style(name="disabledSkin", type="Class")] 
	/**
	 * The padding that appears around the color TextField, in pixels.     *     * @default 3     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 
	/**
	 * The class that provides the skin for the color well when the pointing device rolls over it.     *     * @default ColorPicker_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="overSkin", type="Class")] 
	/**
	 * The padding that appears around the group of color swatches, in pixels.     *     * @default 5     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="backgroundPadding", type="Number", format="Length")] 
	/**
	 * The class that provides the skin for the color well when it is filled with a color.     *     * @default ColorPicker_colorWell     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0     *
	 */
	[Style(name="colorWell", type="Class")] 
	/**
	 * The class that provides the skin for the ColorPicker button when it is in the down position.     *     * @default ColorPicker_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0     *     * @internal [kenos] Description was "Down skin for the ColorPicker button." Is the revised description correct?
	 */
	[Style(name="downSkin", type="Class")] 
	/**
	 * The class that provides the background for the text field of the ColorPicker component.     *     * @default ColorPicker_textFieldSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textFieldSkin", type="Class")] 
	/**
	 * The class that provides the background of the palette that appears in the ColorPicker component.      *     * @default ColorPicker_backgroundSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="background", type="Class")] 
	/**
	 * The class that provides the skin which is used to draw the swatches contained in the ColorPicker component.     *     * @default ColorPicker_swatchSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="swatchSkin", type="Class")] 
	/**
	 * The class that provides the skin which is used to highlight the currently selected color.     *     * @default ColorPicker_swatchSelectedSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="swatchSelectedSkin", type="Class")] 
	/**
	 * The width of each swatch, in pixels.     *     * @default 10     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="swatchWidth", type="Number", format="Length")] 
	/**
	 * The height of each swatch, in pixels.     *     * @default 10     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="swatchHeight", type="Number", format="Length")] 
	/**
	 * The number of columns to be drawn in the ColorPicker color palette.     *     * @default 18     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="columnCount", type="Number", format="Length")] 
	/**
	 * The class that provides the skin for the ColorPicker button when it is in the up position.     *     * @default ColorPicker_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0     *     * @internal [kenos] The previous description was "Up skin for the ColorPicker button." Is the revised description     *                   correct?
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:embedFonts     *     * @default false     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="embedFonts", type="Boolean")] 

	/**
	 * The ColorPicker component displays a list of one or more swatches      * from which the user can select a color.      *     * <p>By default, the component displays a single swatch of color on a      * square button. When the user clicks this button, a panel opens to       * display the complete list of swatches.</p>     *     * @includeExample examples/ColorPickerExample.as     *     * @see fl.events.ColorPickerEvent ColorPickerEvent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ColorPicker extends UIComponent implements IFocusManagerComponent
	{
		/**
		 * A reference to the internal text field of the ColorPicker component.         *         * @see #showTextField         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var textField : TextField;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var customColors : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var defaultColors : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var colorHash : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var paletteBG : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var selectedSwatch : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _selectedColor : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var rollOverColor : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _editable : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _showTextField : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var isOpen : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var doOpen : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var swatchButton : BaseButton;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var colorWell : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var swatchSelectedSkin : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var palette : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var textFieldBG : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var swatches : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var swatchMap : Array;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var currRowIndex : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var currColIndex : int;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected static const POPUP_BUTTON_STYLES : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected const SWATCH_STYLES : Object;

		/**
		 * Gets or sets the swatch that is currently highlighted in the palette of the ColorPicker component.         *         * @default 0x000000         *         * @includeExample examples/ColorPicker.selectedColor.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectedColor () : uint;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selectedColor (value:uint) : void;
		/**
		 * Gets the string value of the current color selection.         *         * @includeExample examples/ColorPicker.hexValue.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get hexValue () : String;
		/**
		 * @copy fl.core.UIComponent#enabled         *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get enabled () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the internal text field of the         * ColorPicker component is editable. A value of <code>true</code> indicates that          * the internal text field is editable; a value of <code>false</code> indicates          * that it is not.         *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get editable () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set editable (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the internal text field          * of the ColorPicker component is displayed. A value of <code>true</code> indicates         * that the internal text field is displayed; a value of <code>false</code> indicates         * that it is not.         *         * @default true         *         * @includeExample examples/ColorPicker.showTextField.1.as -noswf         *         * @see #textField         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get showTextField () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set showTextField (value:Boolean) : void;
		/**
		 * Gets or sets the array of custom colors that the ColorPicker component         * provides. The ColorPicker component draws and displays the colors that are          * described in this array.         *         * <p><strong>Note:</strong> The maximum number of colors that the ColorPicker          * component can display is 1024.</p>         *         * <p>By default, this array contains 216 autogenerated colors.</p>         *         * @includeExample examples/ColorPicker.colors.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0         *
		 */
		public function get colors () : Array;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set colors (value:Array) : void;
		/**
		 * @copy fl.controls.TextArea#imeMode         *         * @see flash.system.IMEConversionMode IMEConversionMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get imeMode () : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set imeMode (value:String) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *         * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf         *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Shows the color palette. Calling this method causes the <code>open</code>          * event to be dispatched. If the color palette is already open or disabled,          * this method has no effect.         *         * @includeExample examples/ColorPicker.open.2.as -noswf         *         * @see #close()         * @see #event:open         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function open () : void;
		/**
		 * Hides the color palette. Calling this method causes the <code>close</code>          * event to be dispatched. If the color palette is already closed or disabled,          * this method has no effect.         *         * @see #event:close         * @see #open()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function close () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function addCloseListener (event:Event);
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onStageClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setStyles () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function cleanUpSelected () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onPopupButtonClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function showPalette () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setEmbedFonts () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawSwatchHighlight () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawPalette () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawTextField () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawSwatches () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBG () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function positionPalette () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setTextEditable () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyUpHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function positionTextField () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setColorDisplay () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setSwatchHighlight (swatch:Sprite) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function findSwatch (color:uint) : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onSwatchClick (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onSwatchOver (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onSwatchOut (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setColorText (color:uint) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function colorToString (color:uint) : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setColorWellColor (colorTransform:ColorTransform) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function createSwatch (color:uint) : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function addStageListener (event:Event = null) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function removeStageListener (event:Event = null) : void;
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
		protected function isOurFocus (target:DisplayObject) : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
	}
}
