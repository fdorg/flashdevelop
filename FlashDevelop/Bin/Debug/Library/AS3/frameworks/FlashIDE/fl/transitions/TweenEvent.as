package fl.transitions
{
	import flash.events.Event;

	/**
	 * The TweenEvent class represents events that are broadcast by the fl.transitions.Tween class. * @playerversion Flash 9 * @langversion 3.0 * @keyword Tween event     * @see fl.transitions.Tween
	 */
	public class TweenEvent extends Event
	{
		/**
		 * Indicates that the motion has started playing.      *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code> property.</td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *     <tr><td><code>time</code></td><td>The time of the Tween when the event occurred.</td></tr>     *     <tr><td><code>position</code></td><td>The value of the property controlled by the Tween, when the event occurred.</td></tr>     *  </table>     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event      * @eventType motionStart
		 */
		public static const MOTION_START : String = 'motionStart';
		/**
		 * Indicates that the Tween has been stopped     * with an explicit call to <code>Tween.stop()</code>.     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code> property.</td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *     <tr><td><code>time</code></td><td>The time of the Tween when the event occurred.</td></tr>     *     <tr><td><code>position</code></td><td>The value of the property controlled by the Tween, when the event occurred.</td></tr>     *  </table>     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event         * @eventType motionStop
		 */
		public static const MOTION_STOP : String = 'motionStop';
		/**
		 * Indicates that the Tween has reached the end and finished.      *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code> property.</td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *     <tr><td><code>time</code></td><td>The time of the Tween when the event occurred.</td></tr>     *     <tr><td><code>position</code></td><td>The value of the property controlled by the Tween, when the event occurred.</td></tr>     *  </table>     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event         * @eventType motionFinish
		 */
		public static const MOTION_FINISH : String = 'motionFinish';
		/**
		 * Indicates that the Tween has changed and the screen has been updated.     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code> property.</td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *     <tr><td><code>time</code></td><td>The time of the Tween when the event occurred.</td></tr>     *     <tr><td><code>position</code></td><td>The value of the property controlled by the Tween, when the event occurred.</td></tr>     *  </table>     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event         * @eventType motionChange
		 */
		public static const MOTION_CHANGE : String = 'motionChange';
		/**
		 * Indicates that the Tween has resumed playing after being paused.     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code> property.</td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *     <tr><td><code>time</code></td><td>The time of the Tween when the event occurred.</td></tr>     *     <tr><td><code>position</code></td><td>The value of the property controlled by the Tween, when the event occurred.</td></tr>     *  </table>     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event         * @eventType motionResume
		 */
		public static const MOTION_RESUME : String = 'motionResume';
		/**
		 * Indicates that the Tween has restarted playing from the beginning in looping mode.     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code> property.</td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *     <tr><td><code>time</code></td><td>The time of the Tween when the event occurred.</td></tr>     *     <tr><td><code>position</code></td><td>The value of the property controlled by the Tween, when the event occurred.</td></tr>     *  </table>     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event        * @eventType motionLoop
		 */
		public static const MOTION_LOOP : String = 'motionLoop';
		/**
		 * The time of the Tween when the event occurred.     *     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event
		 */
		public var time : Number;
		/**
		 * The value of the property controlled by the Tween, when the event occurred.     *     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event
		 */
		public var position : Number;

		/**
		 *  Constructor function for a TweenEvent object.	 *	 *  @param type The event type; indicates the action that caused the event.	 *  @param time The time within the duration of the animation.     *  @param position The frame of the tween.	 *  @param bubbles Specifies whether the event can bubble up the display list hierarchy.	 *  @param cancelable Specifies whether the behavior associated with the event can be prevented.     *     * @playerversion Flash 9     * @langversion 3.0     * @keyword Tween event
		 */
		public function TweenEvent (type:String, time:Number, position:Number, bubbles:Boolean = false, cancelable:Boolean = false);
		/**
		 *  @private
		 */
		public function clone () : Event;
	}
}
