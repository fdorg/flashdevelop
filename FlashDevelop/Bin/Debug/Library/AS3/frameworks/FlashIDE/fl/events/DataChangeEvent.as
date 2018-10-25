package fl.events
{
	import flash.events.Event;

	/**
	 * The DataChangeEvent class defines the event that is dispatched when the data 	 * that is associated with a component changes. This event is used by the List, 	 * DataGrid, TileList, and ComboBox components.	 *	 * <p>This class provides the following event:</p>	 * <ul>	 *     <li><code>DataChangeEvent.DATA_CHANGE</code>: dispatched when the component data changes.</li>	 * </ul>     *	 * @includeExample examples/DataChangeEventExample.as	 *     * @see DataChangeType     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class DataChangeEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code> property of a <code>dataChange</code>		 * event object. 		 *		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 * 	   <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>		 *     <tr><td><code>changeType</code></td><td>Identifies the type of change that was made.</td></tr>		 *	   <tr><td><code>currentTarget</code></td><td>The object that is actively processing 		 * 			the event object with an event listener.</td></tr>		 *     <tr><td><code>endIndex</code></td><td>Identifies the index of the last changed item.</td></tr>		 *     <tr><td><code>items</code></td><td>An array that lists the items that were changed.</td></tr>		 *     <tr><td><code>startIndex</code></td><td>Identifies the index of the first changed item.</td></tr>    	 *     <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>         *         * @eventType dataChange         *         * @see #PRE_DATA_CHANGE         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const DATA_CHANGE : String = "dataChange";
		/**
		 * Defines the value of the <code>type</code> property of a <code>preDataChange</code>		 * event object. This event object is dispatched before a change is made to component data.		 *		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 * 	   <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is no default behavior to cancel.</td></tr>		 * 	   <tr><td><code>changeType</code></td><td>Identifies the type of change to be made.</td></tr>		 *	   <tr><td><code>currentTarget</code></td><td>The object that is actively processing 		 * 			the event object with an event listener.</td></tr>		 *     <tr><td><code>endIndex</code></td><td>Identifies the index of the last item to be		 * 			changed.</td></tr>		 *     <tr><td><code>items</code></td><td>An array that lists the items to be changed.</td></tr>		 *     <tr><td><code>startIndex</code></td><td>Identifies the index of the first item to be		 *         changed.</td></tr>	     *     <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *          not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>		 *          * @eventType preDataChange         *         * @see #DATA_CHANGE         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const PRE_DATA_CHANGE : String = "preDataChange";
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _startIndex : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _endIndex : uint;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _changeType : String;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _items : Array;

		/**
		 * Gets the type of the change that triggered the event. The DataChangeType class 		 * defines the possible values for this property.         *         * @see DataChangeType         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get changeType () : String;
		/**
		 * Gets an array that contains the changed items.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get items () : Array;
		/**
		 * Gets the index of the first changed item in the array of items 		 * that were changed.         *         * @see #endIndex         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0		 *
		 */
		public function get startIndex () : uint;
		/**
		 * Gets the index of the last changed item in the array of items		 * that were changed.         *         * @see #startIndex         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get endIndex () : uint;

		/**
		 * Creates a new DataChangeEvent object with the specified parameters.		 *		 * @param eventType The type of change event.		 *		 * @param changeType The type of change that was made. The DataChangeType class defines the possible values for		 *        this parameter.		 *		 * @param items A list of items that were changed.		 * 		 * @param startIndex The index of the first item that was changed.         *         * @param endIndex The index of the last item that was changed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function DataChangeEvent (eventType:String, changeType:String, items:Array, startIndex:int = -1, endIndex:int = -1);
		/**
		 * Returns a string that contains all the properties of the DataChangeEvent object. The		 * string is in the following format:		 * 		 * <p>[<code>DataChangeEvent type=<em>value</em> changeType=<em>value</em> 		 * startIndex=<em>value</em> endIndex=<em>value</em>		 * bubbles=<em>value</em> cancelable=<em>value</em></code>]</p>		 *         * @return A string that contains all the properties of the DataChangeEvent object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
		/**
		 * Creates a copy of the DataEvent object and sets the value of each parameter to match		 * that of the original.		 *		 * @return A new DataChangeEvent object with property values that match those of the         *         original.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
