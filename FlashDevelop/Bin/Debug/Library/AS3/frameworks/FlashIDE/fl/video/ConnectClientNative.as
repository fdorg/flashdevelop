package fl.video
{
	import flash.net.NetConnection;

	/**
	 * <p>Holds client-side functions for remote procedure calls (rpc)	 * from the FMS during initial connection for <code>NCManager2</code>.	 * One of these objects is created and passed to the <code>NetConnection.client</code>	 * property.</p>	 *     * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ConnectClientNative extends ConnectClient
	{
		public function ConnectClientNative (owner:NCManager, nc:NetConnection, connIndex:uint = 0);
		public function _onbwdone (...rest) : void;
		public function _onbwcheck (...rest) : *;
	}
}
