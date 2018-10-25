package fl.events
{
	import flash.events.Event;

	/**
	 * The ListEvent class defines events for list-based components including the List, DataGrid, TileList, and ComboBox components. 	 * These events include the following:	 * <ul>	 * <li><code>ListEvent.ITEM_CLICK</code>: dispatched after the user clicks the mouse over an item in the component.</li>	 * <li><code>ListEvent.ITEM_DOUBLE_CLICK</code>: dispatched after the user clicks the mouse twice in rapid succession 	 *                                  over an item in the component.</li>	 * <li><code>ListEvent.ITEM_ROLL_OUT</code>: dispatched after the user rolls the mouse pointer out of an item in the component.</li>	 * <li><code>ListEvent.ITEM_ROLL_OVER</code>: dispatched after the user rolls the mouse pointer over an item in the component.</li>	 * </ul>	 *     * @see fl.controls.List List     * @see fl.controls.SelectableList SelectableList     *     * @includeExample examples/ListEventExample.as     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ListEvent extends Event
	{
		/**
		 * Defines the value of the <code>type</code> property of an  		 * <code>itemRollOut</code> event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *    <tr>         *      <th>Property</th>         *      <th>Value</th>         *    </tr>		 *    <tr>         *      <td><code>bubbles</code></td>         *      <td><code>false</code></td></tr>		 *    <tr><td><code>cancelable</code></td><td><code>false</code>; there is		 *          no default behavior to cancel.</td></tr>			 * 	  <tr><td><code>columnIndex</code></td><td>The zero-based index of the column that		 * 			contains the renderer.</td></tr>		 *    <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener.</td></tr>		 * 	  <tr><td><code>index</code></td><td>The zero-based index in the DataProvider		 * 			that contains the renderer.</td></tr>		 * 	  <tr><td><code>item</code></td><td>A reference to the data that belongs to the renderer.</td></tr>		 * 	  <tr><td><code>rowIndex</code></td><td>The zero-based index of the row that		 * 	  		contains the renderer.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>         *  </table>         *         * @eventType itemRollOut         *         * @see #ITEM_ROLL_OVER         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ITEM_ROLL_OUT : String = "itemRollOut";
		/**
		 * Defines the value of the <code>type</code> property of an <code>itemRollOver</code> 		 * event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>false</code>; there is 		 *          no default behavior to cancel.</td></tr>			 * 	  <tr><td><code>columnIndex</code></td><td>The zero-based index of the column that		 * 			contains the renderer.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener.</td></tr>		 * 	  <tr><td><code>index</code></td><td>The zero-based index in the DataProvider		 * 			that contains the renderer.</td></tr>		 * 	  <tr><td><code>item</code></td><td>A reference to the data that belongs to the renderer.</td></tr>		 * 	  <tr><td><code>rowIndex</code></td><td>The zero-based index of the row that		 * 	  		contains the renderer.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>         *         * @eventType itemRollOver		 *         * @see #ITEM_ROLL_OUT         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ITEM_ROLL_OVER : String = "itemRollOver";
		/**
		 * Defines the value of the <code>type</code> property of an <code>itemClick</code> 		 * event object. 		 *          * <p>This event has the following properties:</p>         *  <table class="innertable" width="100%">         *     <tr><th>Property</th><th>Value</th></tr>         *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>         *     <tr><td><code>cancelable</code></td><td><code>true</code></td></tr>           *    <tr><td><code>columnIndex</code></td><td>The zero-based index of the column that         *          contains the renderer.</td></tr>         *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener.</td></tr>         *    <tr><td><code>index</code></td><td>The zero-based index in the DataProvider         *          that contains the renderer.</td></tr>         *    <tr><td><code>item</code></td><td>A reference to the data that belongs to the renderer.         *          </td></tr>         *    <tr><td><code>rowIndex</code></td><td>The zero-based index of the row that         *          contains the renderer.</td></tr>         *    <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>         *          property to access the object that is listening for the event.</td></tr>         *  </table>         *         * @eventType itemClick         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ITEM_CLICK : String = "itemClick";
		/**
		 * Defines the value of the <code>type</code> property of an <code>itemDoubleClick</code> 		 * event object. 		 * 		 * <p>This event has the following properties:</p>		 *  <table class="innertable" width="100%">		 *     <tr><th>Property</th><th>Value</th></tr>		 *     <tr><td><code>bubbles</code></td><td><code>false</code></td></tr>		 *     <tr><td><code>cancelable</code></td><td><code>true</code></td></tr>			 * 	  <tr><td><code>columnIndex</code></td><td>The zero-based index of the column that		 * 			contains the renderer.</td></tr>		 *     <tr><td><code>currentTarget</code></td><td>The object that is actively processing          *          the event object with an event listener.</td></tr>		 * 	  <tr><td><code>index</code></td><td>The zero-based index in the DataProvider		 * 			that contains the renderer.</td></tr>		 * 	  <tr><td><code>item</code></td><td>A reference to the data that belongs to the renderer.		 * 	  		</td></tr>		 * 	  <tr><td><code>rowIndex</code></td><td>The zero-based index of the row that		 * 	  		contains the renderer.</td></tr>		 * 	  <tr><td><code>target</code></td><td>The object that dispatched the event. The target is          *           not always the object listening for the event. Use the <code>currentTarget</code>		 * 			property to access the object that is listening for the event.</td></tr>		 *  </table>         *         * @eventType itemDoubleClick         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const ITEM_DOUBLE_CLICK : String = "itemDoubleClick";
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _rowIndex : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _columnIndex : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _index : int;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _item : Object;

		/**
		 * Gets the row index of the item that is associated with this event.		 *          * @see #columnIndex         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get rowIndex () : Object;
		/**
		 * Gets the column index of the item that is associated with this event.		 *          * @see #rowIndex         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get columnIndex () : int;
		/**
		 * Gets the zero-based index of the cell that contains the renderer.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get index () : int;
		/**
		 * Gets the data that belongs to the current cell renderer.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get item () : Object;

		/**
		 * Creates a new ListEvent object with the specified parameters. 		 *          * @param type The event type; this value identifies the action that caused the event.         *         * @param bubbles Indicates whether the event can bubble up the display list hierarchy.         *         * @param cancelable Indicates whether the behavior associated with the event can be		 *        prevented. 		 *          * @param columnIndex The zero-based index of the column that contains the renderer or visual		 *        representation of the data in the column.          *         * @param rowIndex The zero-based index of the row that contains the renderer or visual		 *        representation of the data in the row.          *         * @param index The zero-based index of the item in the DataProvider.          *         * @param item A reference to the data that belongs to the renderer.          *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ListEvent (type:String, bubbles:Boolean = false, cancelable:Boolean = false, columnIndex:int = -1, rowIndex:int = -1, index:int = -1, item:Object = null);
		/**
		 * Returns a string that contains all the properties of the ListEvent object. The string		 * is in the following format:		 * 		 * <p>[<code>ListEvent type=<em>value</em> bubbles=<em>value</em>		 * 	cancelable=<em>value</em> columnIndex=<em>value</em>		 * 	rowIndex=<em>value</em></code>]</p>		 *         * @return A string representation of the ListEvent object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
		/**
		 * Creates a copy of the ListEvent object and sets the value of each parameter to match		 * the original.		 *         * @return A new ListEvent object with parameter values that match those of the original.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function clone () : Event;
	}
}
