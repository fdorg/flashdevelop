package fl.controls
{
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ComponentEvent;
	import flash.display.DisplayObject;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.TimerEvent;
	import flash.utils.Timer;

	/**
	 * Dispatched when the user presses the Button component.	 * If the <code>autoRepeat</code> property is <code>true</code>,	 * this event is dispatched at specified intervals until the	 * button is released. 	 *	 * <p>The <code>repeatDelay</code> style is used to	 * specify the delay before the <code>buttonDown</code> event is 	 * dispatched a second time. The <code>repeatInterval</code> style 	 * specifies the interval at which this event is dispatched thereafter,	 * until the user releases the button.</p>	 *     * @eventType fl.events.ComponentEvent.BUTTON_DOWN     *     * @includeExample examples/BaseButton.autoRepeat.1.as -noswf     *     * @see #autoRepeat	 * @see #style:repeatDelay style	 * @see #style:repeatInterval style     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="buttonDown", type="fl.events.ComponentEvent")] 
	/**
	 * Dispatched when the value of the <code>selected</code> property	 * of a toggle Button component changes. A toggle Button component is a 	 * Button component whose <code>toggle</code> property is set to <code>true</code>.	 *	 * <p>The CheckBox and RadioButton components dispatch this event after       * there is a change in the <code>selected</code> property.</p>	 *	 * @eventType flash.events.Event.CHANGE     *     * @includeExample examples/LabelButton.toggle.1.as -noswf     *     * @see #selected selected     * @see LabelButton#toggle LabelButton.toggle     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="change", type="flash.events.Event")] 
	/**
	 * @copy fl.controls.LabelButton#style:upSkin     *     * @default Button_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:downSkin     *     * @default Button_downSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="downSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:overSkin     *     * @default Button_overSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="overSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:disabledSkin     *     * @default Button_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedDisabledSkin     *     * @default Button_selectedDisabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDisabledSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedUpSkin     *     * @default Button_selectedUpSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedUpSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedDownSkin     *     * @default Button_selectedDownSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedDownSkin", type="Class")] 
	/**
	 * @copy fl.controls.LabelButton#style:selectedOverSkin     *     * @default Button_selectedOverSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="selectedOverSkin", type="Class")] 
	/**
	 * The number of milliseconds to wait after the <code>buttonDown</code> 	 * event is first dispatched before sending a second <code>buttonDown</code> 	 * event.     *     * @default 500	 *	 * @see #event:buttonDown     * @see #autoRepeat	 * @see #style:repeatInterval     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="repeatDelay", type="Number", format="Time")] 
	/**
	 * The interval, in milliseconds, between <code>buttonDown</code> events 	 * that are dispatched after the delay that is specified by the <code>repeatDelay</code>	 * style.      *     * @default 35     *	 * @see #event:buttonDown     * @see #autoRepeat	 * @see #style:repeatDelay	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="repeatInterval", type="Number", format="Time")] 

	/**
	 * The BaseButton class is the base class for all button components, defining      * properties and methods that are common to all buttons. This class handles      * drawing states and the dispatching of button events.     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class BaseButton extends UIComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var background : DisplayObject;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var mouseState : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _selected : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _autoRepeat : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var pressTimer : Timer;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var _mouseStateLocked : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var unlockedMouseState : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * Gets or sets a value that indicates whether the component can accept user 		 * input. A value of <code>true</code> indicates that the component can accept		 * user input; a value of <code>false</code> indicates that it cannot. 		 *		 * <p>When this property is set to <code>false</code>, the button is disabled.		 * This means that although it is visible, it cannot be clicked. This property is 		 * useful for disabling a specific part of the user interface. For example, a button		 * that is used to trigger the reloading of a web page could be disabled		 * by using this technique.</p>         *         * @default true         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get enabled () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set enabled (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether a toggle button 		 * is selected. A value of <code>true</code> indicates that the button is		 * selected; a value of <code>false</code> indicates that it is not. 		 * This property has no effect if the <code>toggle</code> property 		 * is not set to <code>true</code>. 		 *		 * <p>For a CheckBox component, this value indicates whether the box is		 * checked. For a RadioButton component, this value indicates whether the 		 * component is selected.</p>		 *		 * <p>This value changes when the user clicks the component           * but can also be changed programmatically. If the <code>toggle</code> 		 * property is set to <code>true</code>, changing this property causes  		 * a <code>change</code> event object to be dispatched.</p>		 *         * @default false         *         * @includeExample examples/LabelButton.toggle.1.as -noswf		 *         * @see #event:change change         * @see LabelButton#toggle LabelButton.toggle         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selected (value:Boolean) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the <code>buttonDown</code> event 		 * is dispatched more than one time when the user holds the mouse button down over the component.         * A value of <code>true</code> indicates that the <code>buttonDown</code> event 		 * is dispatched repeatedly while the mouse button remains down; a value of <code>false</code>		 * indicates that the event is dispatched only one time.         *          * <p>If this value is <code>true</code>, after the delay specified by the 		 * <code>repeatDelay</code> style, the <code>buttonDown</code>          * event is dispatched at the interval that is specified by the <code>repeatInterval</code> style.</p>         *         * @default false         *         * @includeExample examples/BaseButton.autoRepeat.1.as -noswf         *         * @see #style:repeatDelay         * @see #style:repeatInterval         * @see #event:buttonDown         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoRepeat () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoRepeat (value:Boolean) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set mouseStateLocked (value:Boolean) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new BaseButton instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function BaseButton ();
		/**
		 * Set the mouse state via ActionScript. The BaseButton class		 * uses this property internally, but it can also be invoked manually,		 * and will set the mouse state visually.         *         * @param state A string that specifies a mouse state. Supported values are         * "up", "over", and "down".         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setMouseState (state:String) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setupMouseEvents () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function mouseEventHandler (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function startPress () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function buttonDown (event:TimerEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function endPress () : void;
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
		protected function drawLayout () : void;
	}
}
