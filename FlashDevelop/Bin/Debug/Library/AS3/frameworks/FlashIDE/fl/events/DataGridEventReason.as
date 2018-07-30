package fl.events
{
	/**
	 *  The DataGridEventReason class defines constants that are used for the  	 *  values of the <code>reason</code> property of the DataGridEvent object 	 *  when the value of the <code>type</code> property is <code>itemEditEnd</code>.     *      * @see fl.controls.DataGrid     * @see DataGridEvent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class DataGridEventReason
	{
		/**
		 *  The user canceled editing and does not want to save the edited 		 *  data. 		 *		 *  <p>Even if you call the <code>preventDefault()</code> method 		 *  from your event listener for the <code>itemEditEnd</code> event, 		 *  Flash still calls the <code>destroyItemEditor()</code> editor to close the editor.</p>         *         * @see flash.events.Event#preventDefault() Event.preventDefault()         * @see fl.controls.DataGrid#destroyItemEditor() DataGrid.destroyItemEditor()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const CANCELLED : String = "cancelled";
		/**
		 *  The list component lost focus, was scrolled, or is in a state where 		 *  editing is not allowed. 		 *		 *  <p>Even if you call the <code>preventDefault()</code> method from your event 		 *  listener for the <code>itemEditEnd</code> event, Flash still calls the 		 *  <code>destroyItemEditor()</code> editor to close the editor.</p>         *         * @see flash.events.Event#preventDefault() Event.preventDefault()         * @see fl.controls.DataGrid#destroyItemEditor() DataGrid.destroyItemEditor()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const OTHER : String = "other";
		/**
		 *  The user moved focus to a new column in the same row.		 *		 *  <p>Your event listener can include logic that permits a change in focus		 *  or that stops it. For example, if an event listener detects that the user		 *  entered an invalid value, the event listener can make a call to the 		 *  <code>preventDefault()</code> method to stop focus from being moved to a  		 *  new item. The item editor remains open and the user continues to edit the		 *  current item. To close the editor that is associated with the current item,		 *  call the <code>destroyItemEditor()</code> method.</p>         *         * @see #NEW_ROW         * @see flash.events.Event#preventDefault() Event.preventDefault()         * @see fl.controls.DataGrid#destroyItemEditor() DataGrid.destroyItemEditor()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const NEW_COLUMN : String = "newColumn";
		/**
		 *  Indicates that the user moved focus to a new row.		 * 		 *  <p>Your event listener can include logic that permits a change in focus		 *  or that stops it. For example, if an event listener detects that the user		 *  entered an invalid value, the event listener can make a call to the 		 *  <code>preventDefault()</code> method to stop focus from being moved to a  		 *  new row. The item editor remains open and the user continues to edit the		 *  current item. To close the editor that is associated with the current item,		 *  call the <code>destroyItemEditor()</code> method.</p>         *         * @see #NEW_COLUMN         * @see flash.events.Event#preventDefault() Event.preventDefault()         * @see fl.controls.DataGrid#destroyItemEditor() DataGrid.destroyItemEditor()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const NEW_ROW : String = "newRow";

	}
}
