package fl.data
{
	/**
	 * The TileListCollectionItem class defines a single item in an inspectable	 * property that represents a data provider. A TileListCollectionItem object	 * is a collection list item that contains only <code>label</code> and	 * <code>source</code> properties, and is primarily used in the TileList	 * component.	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public dynamic class TileListCollectionItem
	{
		/**
		 * The <code>label</code> property of the object.		 *         * The default value is <code>label(<em>n</em>)</code>, where <em>n</em> is the ordinal index.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var label : String;
		/**
		 * The <code>source</code> property of the object. This can be the path or a class		 * name of the image that is displayed in the image cell of the TileList.		 *         * @default null         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var source : String;

		/**
		 * Creates a new TileListCollectionItem object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function TileListCollectionItem ();
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function toString () : String;
	}
}
