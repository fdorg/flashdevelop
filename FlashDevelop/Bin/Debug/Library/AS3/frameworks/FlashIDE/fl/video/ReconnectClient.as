package fl.video
{
	/**
	 * <p>Holds client-side functions for remote procedure calls (rpc)	 * from the FMS during reconnection.  One of these objects is created	 * and passed to the <code>NetConnection.client</code> property.</p>	 *     * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ReconnectClient
	{
		public var owner : NCManager;

		public function ReconnectClient (owner:NCManager);
		public function close () : void;
		public function onBWDone (...rest) : void;
	}
}
