package fl.controls
{
	import fl.controls.TextInput;
	import fl.controls.TextArea;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ComponentEvent;
	import fl.managers.IFocusManager;
	import fl.managers.IFocusManagerComponent;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.TextEvent;
	import flash.events.Event;
	import flash.events.FocusEvent;
	import flash.events.KeyboardEvent;
	import flash.system.IME;
	import flash.text.TextField;
	import flash.text.TextFieldType;
	import flash.text.TextFormat;
	import flash.text.TextLineMetrics;
	import flash.ui.Keyboard;

	/**
	 *  Dispatched when user input changes text in the TextInput component.     *	 *  <p><strong>Note:</strong> This event does not occur if ActionScript      *  is used to change the text.</p>	 *     *  @eventType flash.events.Event.CHANGE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="flash.events.Event")] 
	/**
	 *  Dispatched when the user presses the Enter key.     *     *  @eventType fl.events.ComponentEvent.ENTER     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="enter", type="fl.events.ComponentEvent")] 
	/**
	 *  Dispatched when the user inputs text.     *     *  @eventType flash.events.TextEvent.TEXT_INPUT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="textInput", type="flash.events.TextEvent")] 
	/**
	 * The name of the class to use as a background for the TextInput	 * component.	 *     * @default TextInput_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 * The padding that separates the component border from the text, in pixels.	 *     * @default 0     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="textPadding", type="Number", format="Length")] 
	/**
	 * The name of the class to use as a background for the TextInput     * component when its <code>enabled</code> property is set to <code>false</code>.	 *     * @default TextInput_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:embedFonts     *     * @default false     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="embedFonts", type="Boolean")] 

	/**
	 * The TextInput component is a single-line text component that	 * contains a native ActionScript TextField object. 	 *	 * <p>A TextInput component can be enabled or disabled in an application.	 * When the TextInput component is disabled, it cannot receive input 	 * from mouse or keyboard. An enabled TextInput component implements focus, 	 * selection, and navigation like an ActionScript TextField object.</p>	 *	 * <p>You can use styles to customize the TextInput component by	 * changing its appearance--for example, when it is disabled.	 * Some other customizations that you can apply to this component	 * include formatting it with HTML or setting it to be a	 * password field whose text must be hidden. </p>	 *     * @includeExample examples/TextInputExample.as     *     * @see TextArea     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class TextInput extends UIComponent implements IFocusManagerComponent
	{
		/**
		 * A reference to the internal text field of the TextInput component.         *         * @includeExample examples/TextInput.textField.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var textField : TextField;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _editable : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var background : DisplayObject;
		/**
		 * @private (protected)
		 */
		protected var _html : Boolean;
		/**
		 * @private (protected)
		 */
		protected var _savedHTML : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;
		/**
		 *  @private         *		 *  The method to be used to create the Accessibility class.         *  This method is called from UIComponent.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static var createAccessibilityImplementation : Function;

		/**
		 * Gets or sets a string which contains the text that is currently in 		 * the TextInput component. This property contains text that is unformatted 		 * and does not have HTML tags. To retrieve this text formatted as HTML, use 		 * the <code>htmlText</code> property.		 *          * @default ""         *         * @see #htmlText         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get text () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set text (value:String) : void;
		/**
		 * @copy fl.core.UIComponent#enabled         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get enabled () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * @copy fl.controls.TextArea#imeMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get imeMode () : String;
		/**
		 * @private (protected)
		 */
		public function set imeMode (value:String) : void;
		/**
		 * Gets or sets a Boolean value that indicates how a selection is		 * displayed when the text field does not have focus. 		 *		 * <p>When this value is set to <code>true</code> and the text field does 		 * not have focus, Flash Player highlights the selection in the text field 		 * in gray. When this value is set to <code>false</code> and the text field 		 * does not have focus, Flash Player does not highlight the selection in the 		 * text field.</p>		 *         * @default false         *         * @includeExample examples/TextInput.setSelection.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get alwaysShowSelection () : Boolean;
		/**
		 * @private (setter)
		 */
		public function set alwaysShowSelection (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the text field 		 * can be edited by the user. A value of <code>true</code> indicates		 * that the user can edit the text field; a value of <code>false</code>		 * indicates that the user cannot edit the text field.          *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get editable () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set editable (value:Boolean) : void;
		/**
		 * Gets or sets the position of the thumb of the horizontal scroll bar.         *         * @default 0         *         * @includeExample examples/TextInput.horizontalScrollPosition.1.as -noswf         *         * @see #maxHorizontalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get horizontalScrollPosition () : int;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set horizontalScrollPosition (value:int) : void;
		/**
		 * Gets a value that describes the furthest position to which the text 		 * field can be scrolled to the right.         *         * @default 0         *         * @see #horizontalScrollPosition         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxHorizontalScrollPosition () : int;
		/**
		 * Gets the number of characters in a TextInput component.         *         * @default 0         *         * @includeExample examples/TextInput.maxChars.1.as -noswf         *         * @see #maxChars         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get length () : int;
		/**
		 * Gets or sets the maximum number of characters that a user can enter		 * in the text field.		 *          * @default 0         *         * @includeExample examples/TextInput.maxChars.1.as -noswf         *         * @see #length         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maxChars () : int;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maxChars (value:int) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the current TextInput 		 * component instance was created to contain a password or to contain text. A value of 		 * <code>true</code> indicates that the component instance is a password text		 * field; a value of <code>false</code> indicates that the component instance		 * is a normal text field. 		 *         * <p>When this property is set to <code>true</code>, for each character that the		 * user enters into the text field, the TextInput component instance displays an asterisk.		 * Additionally, the Cut and Copy commands and their keyboard shortcuts are 		 * disabled. These measures prevent the recovery of a password from an		 * unattended computer.</p>         *		 * @default false         *         * @includeExample examples/TextInput.displayAsPassword.1.as -noswf         *         * @see flash.text.TextField#displayAsPassword TextField.displayAsPassword         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get displayAsPassword () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set displayAsPassword (value:Boolean) : void;
		/**
		 * Gets or sets the string of characters that the text field accepts from a user. 		 * Note that characters that are not included in this string are accepted in the 		 * text field if they are entered programmatically.		 *		 * <p>The characters in the string are read from left to right. You can specify a 		 * character range by using the hyphen (-) character. </p>		 *		 * <p>If the value of this property is null, the text field accepts all characters. 		 * If this property is set to an empty string (""), the text field accepts no characters.</p>		 *		 * <p>If the string begins with a caret (^) character, all characters are initially 		 * accepted and succeeding characters in the string are excluded from the set of 		 * accepted characters. If the string does not begin with a caret (^) character, 		 * no characters are initially accepted and succeeding characters in the string 		 * are included in the set of accepted characters.</p>		 * 		 * @default null         *         * @see flash.text.TextField#restrict TextField.restrict         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get restrict () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set restrict (value:String) : void;
		/**
		 * Gets the index value of the first selected character in a selection 		 * of one or more characters. 		 *		 * <p>The index position of a selected character is zero-based and calculated 		 * from the first character that appears in the text area. If there is no 		 * selection, this value is set to the position of the caret.</p>		 *         * @default 0         *         * @includeExample examples/TextInput.selectionBeginIndex.1.as -noswf         *         * @see #selectionEndIndex         * @see #setSelection()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectionBeginIndex () : int;
		/**
		 * Gets the index position of the last selected character in a selection 		 * of one or more characters. 		 *		 * <p>The index position of a selected character is zero-based and calculated 		 * from the first character that appears in the text area. If there is no 		 * selection, this value is set to the position of the caret.</p>         *         * @default 0         *         * @see #selectionBeginIndex         * @see #setSelection()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selectionEndIndex () : int;
		/**
		 * Gets or sets a Boolean value that indicates whether extra white space is		 * removed from a TextInput component that contains HTML text. Examples 		 * of extra white space in the component include spaces and line breaks.		 * A value of <code>true</code> indicates that extra 		 * white space is removed; a value of <code>false</code> indicates that extra 		 * white space is not removed.		 *         * <p>This property affects only text that is set by using the <code>htmlText</code> 		 * property; it does not affect text that is set by using the <code>text</code> property.          * If you use the <code>text</code> property to set text, the <code>condenseWhite</code>          * property is ignored.</p>		 *         * <p>If the <code>condenseWhite</code> property is set to <code>true</code>, you 		 * must use standard HTML commands, such as &lt;br&gt; and &lt;p&gt;, to place line          * breaks in the text field.</p>		 *         * @default false         *         * @see flash.text.TextField#condenseWhite TextField.condenseWhite         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get condenseWhite () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set condenseWhite (value:Boolean) : void;
		/**
		 * Contains the HTML representation of the string that the text field contains.		 *         * @default ""         *         * @includeExample examples/TextInput.htmlText.1.as -noswf         *         * @see #text         * @see flash.text.TextField#htmlText         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get htmlText () : String;
		/**
		 * @private (setter)
		 */
		public function set htmlText (value:String) : void;
		/**
		 * The height of the text, in pixels.		 *         * @default 0         *         * @see #textWidth         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 *		 * @internal [kenos] What is the "height" of the text? Is this the vertical size of the text field that contains the text?		 *                   Same for the textWidth property below.
		 */
		public function get textHeight () : Number;
		/**
		 * The width of the text, in pixels.		 *         * @default 0         *         * @includeExample examples/TextInput.textWidth.1.as -noswf         *         * @see #textHeight         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get textWidth () : Number;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle()         * @see fl.core.UIComponent#setStyle()         * @see fl.managers.StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new TextInput component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function TextInput ();
		/**
		 * @copy fl.core.UIComponent#drawFocus()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function drawFocus (draw:Boolean) : void;
		/**
		 * Sets the range of a selection made in a text area that has focus.		 * The selection range begins at the index that is specified by the start 		 * parameter, and ends at the index that is specified by the end parameter.		 * If the parameter values that specify the selection range are the same,		 * this method sets the text insertion point in the same way that the 		 * <code>caretIndex</code> property does.		 *		 * <p>The selected text is treated as a zero-based string of characters in which		 * the first selected character is located at index 0, the second 		 * character at index 1, and so on.</p>		 *		 * <p>This method has no effect if the text field does not have focus.</p>		 *		 * @param beginIndex The index location of the first character in the selection.		 *         * @param endIndex The index location of the last character in the selection.         *		 * @includeExample examples/TextInput.setSelection.1.as -noswf         * @includeExample examples/TextInput.setSelection.2.as -noswf		 *         * @see #selectionBeginIndex         * @see #selectionEndIndex         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSelection (beginIndex:int, endIndex:int) : void;
		/**
		 * Retrieves information about a specified line of text.		 * 		 * @param lineIndex The line number for which information is to be retrieved.         *		 * @includeExample examples/TextInput.getLineMetrics.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function getLineMetrics (index:int) : TextLineMetrics;
		/**
		 * Appends the specified string after the last character that the TextArea 		 * contains. This method is more efficient than concatenating two strings 		 * by using an addition assignment on a text property; for example, 		 * <code>myTextArea.text += moreText</code>. This method is particularly		 * useful when the TextArea component contains a significant amount of		 * content.          *         * @param text The string to be appended to the existing text.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function appendText (text:String) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function updateTextFieldType () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleKeyDown (event:KeyboardEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleChange (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleTextInput (event:TextEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setEmbedFont ();
		/**
		 * @private (protected)
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBackground () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawTextFormat () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFocus () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function isOurFocus (target:DisplayObject) : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusInHandler (event:FocusEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function focusOutHandler (event:FocusEvent) : void;
	}
}
