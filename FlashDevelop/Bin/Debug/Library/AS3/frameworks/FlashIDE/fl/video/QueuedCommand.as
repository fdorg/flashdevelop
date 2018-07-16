package fl.video
{
	/**
	 * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class QueuedCommand
	{
		public static const PLAY : uint = 0;
		public static const LOAD : uint = 1;
		public static const PAUSE : uint = 2;
		public static const STOP : uint = 3;
		public static const SEEK : uint = 4;
		public static const PLAY_WHEN_ENOUGH : uint = 5;
		public var type : uint;
		public var url : String;
		public var isLive : Boolean;
		public var time : Number;

		public function QueuedCommand (type:uint, url:String, isLive:Boolean, time:Number);
	}
}
