package fl.data
{
	/**
	 * The SimpleCollectionItem class defines a single item in an inspectable	 * property that represents a data provider. A SimpleCollectionItem object	 * is a collection list item that contains only <code>label</code> and	 * <code>data</code> properties--for example, a ComboBox or List component.     *     * @internal Is this revised description correct?	 * Adobe: [LM} Yes, its ok.     *	 * @includeExample examples/SimpleCollectionItemExample.as	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public dynamic class SimpleCollectionItem
	{
		/**
		 * The <code>label</code> property of the object.		 *         * The default value is <code>label(<em>n</em>)</code>, where <em>n</em> is the ordinal index.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var label : String;
		/**
		 * The <code>data</code> property of the object.		 *         * @default null         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var data : String;

		/**
		 * Creates a new SimpleCollectionItem object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SimpleCollectionItem ();
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
	}
}
