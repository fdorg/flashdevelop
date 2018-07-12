package fl.video
{
	/**
	 * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class VideoPlayerState
	{
		public var owner : VideoPlayer;
		public var index : int;
		public var url : String;
		public var isLive : Boolean;
		public var isLiveSet : Boolean;
		public var totalTime : Number;
		public var totalTimeSet : Boolean;
		public var autoPlay : Boolean;
		public var isWaiting : Boolean;
		public var prevState : String;
		public var minProgressPercent : Number;
		public var preSeekTime : Number;
		public var cmdQueue : Array;

		public function VideoPlayerState (owner:VideoPlayer, index:int);
	}
}
