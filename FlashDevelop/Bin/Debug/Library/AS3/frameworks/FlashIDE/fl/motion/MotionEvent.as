package fl.motion
{
	import flash.events.Event;

	/**
	 * The MotionEvent class represents events that are broadcast by the fl.motion.Animator class. * Use these events with an event listener to initiate custom functions. For example, if you  * have an Animator instance named <code>abox_animator</code>, Flash Player executes the following * <code>trace</code> statement when the animation is complete: * <listing> * import fl.motion.MotionEvent; * abox_animator.addEventListener(MotionEvent.MOTION_END,afterMotion); * function afterMotion(e:MotionEvent) { *    trace("animation complete!"); * } * </listing> * @playerversion Flash 9.0.28.0 * @langversion 3.0 * @keyword MotionEvent, Copy Motion as ActionScript     * @see ../../motionXSD.html Motion XML Elements    * @see fl.motion.Animator * @see flash.events.IEventDispatcher
	 */
	public class MotionEvent extends Event
	{
		/**
		 * Indicates that the Motion instance has started playing.      *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code>. </td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *  </table>     * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator#event:motionStart     *      * @eventType motionStart
		 */
		public static const MOTION_START : String = 'motionStart';
		/**
		 * Indicates that the motion has stopped,      * whether by an explicit call to <code>Animator.stop()</code> or <code>Animator.end()</code>,     * or by reaching the end of the Motion instance.     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code>. </td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *  </table>          * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator#event:motionEnd     *      * @eventType motionEnd
		 */
		public static const MOTION_END : String = 'motionEnd';
		/**
		 * Indicates that the Motion instance has changed and the screen has been updated.     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code>. </td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *  </table>          * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator#event:motionUpdate     *      * @eventType motionUpdate
		 */
		public static const MOTION_UPDATE : String = 'motionUpdate';
		/**
		 * Indicates that the Animator instance's <code>time</code> value has changed,      * but the screen has not yet been updated (Flash Player has not dispatched the <code>motionUpdate</code> event).     *  <p>The properties of the event object have the following values:</p>     *  <table class="innertable">     *     <tr><th>Property</th><th>Value</th></tr>     *     <tr><td><code>bubbles</code></td><td>false</td></tr>     *     <tr><td><code>cancelable</code></td><td>false</td></tr>     *     <tr><td><code>currentTarget</code></td><td>The object that defines the      *       event listener that handles the event. For example, if you use      *       <code>myButton.addEventListener()</code> to register an event listener,      *       <code>myButton</code> is the value of the <code>currentTarget</code>. </td></tr>     *     <tr><td><code>target</code></td><td>The object that dispatched the event;      *       it is not always the object listening for the event.      *       Use the <code>currentTarget</code> property to always access the      *       object listening for the event.</td></tr>     *  </table>          * @playerversion Flash 9.0.28.0     * @langversion 3.0     * @see fl.motion.Animator#event:timeChange     *      * @eventType timeChange
		 */
		public static const TIME_CHANGE : String = 'timeChange';

		/**
		 *  Constructor.	 *	 *  @param type The event type; indicates the action that caused the event.	 *	 *  @param bubbles Specifies whether the event can bubble up the display list hierarchy.	 *	 *  @param cancelable Specifies whether the behavior associated with the event can be prevented.     * @playerversion Flash 9.0.28.0     * @langversion 3.0     *
		 */
		public function MotionEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false);
		/**
		 *  @private
		 */
		public function clone () : Event;
	}
}
