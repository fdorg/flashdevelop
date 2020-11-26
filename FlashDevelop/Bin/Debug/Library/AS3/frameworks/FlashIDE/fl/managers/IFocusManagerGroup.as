package fl.managers
{
	/**
	 * The IFocusManagerGroup interface provides properties that are used 	 * to manage a set of components of which only one can be selected	 * at a time. Components that are part of such a component set 	 * must implement this interface.     *	 * <p>A radio button, for example, must implement the IFocusManagerGroup interface	 * because only one radio button can be selected from a group of radio button	 * components at a time. The focus manager ensures that focus is not given to a 	 * radio button that is not selected when focus moves in response to the Tab key.</p>	 * 	 * @see FocusManager     * @see IFocusManagerComponent     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public interface IFocusManagerGroup
	{
		/**
		 * Gets or sets the name of the group of components to which this component belongs.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get groupName () : String;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set groupName (value:String) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether this component is selected. 		 * A value of <code>true</code> indicates that the component is selected; 		 * a value of <code>false</code> indicates that it is not selected.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set selected (value:Boolean) : void;

	}
}
