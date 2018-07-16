package fl.video
{
	import flash.net.NetConnection;

	/**
	 * <p>Holds client-side functions for remote procedure calls (rpc)	 * from the FMS during initial connection.  One of these objects	 * is created and passed to the <code>NetConnection.client</code>	 * property.</p>	 *     * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ConnectClient
	{
		public var owner : NCManager;
		public var nc : NetConnection;
		public var connIndex : uint;
		public var pending : Boolean;

		public function ConnectClient (owner:NCManager, nc:NetConnection, connIndex:uint = 0);
		public function close () : void;
		public function onBWDone (...rest) : void;
		public function onBWCheck (...rest) : Number;
	}
}
