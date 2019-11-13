package fl.video
{
	/**
	 * The VideoScaleMode class provides constant values to use for the 	 * <code>FLVPlayback.scaleMode</code> and	 * <code>VideoPlayer.scaleMode</code> properties.	 * 	 * @see FLVPlayback#scaleMode         * @see VideoPlayer#scaleMode         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
	 */
	public class VideoScaleMode
	{
		/**
		 * Specifies that the video be constrained within the     * rectangle determined by the <code>registrationX</code>, <code>registrationY</code>,     * <code>registrationWidth</code>, and <code>registrationHeight</code> properties but that its     * original aspect ratio be preserved.     * 	 * <p>For example, if <code>registrationWidth = 100</code> and  	 * <code>registrationHeight = 100</code>, if <code>registrationX = 200</code> and 	 * <code>registrationY = 200</code>, and if an FLV file is loaded with a <code>width</code> of 648 and	 * a <code>height</code> of 480 (for an approximate aspect ratio of 4:3), and if 	 * <code>align = VideoAlign.CENTER</code> and <code>scaleMode =	 * MAINTAIN_ASPECT_RATIO</code>, you end up with <code>width =	 * 100</code>, and <code>height = (100 ~~ 480 / 648) = 74</code>, <code>x = 10</code>, and 	 * <code>y = (10 + ((100 - 74) / 2)) = 23</code>.</p>          *          * @langversion 3.0          * @playerversion Flash 9.0.28.0
		 */
		public static const MAINTAIN_ASPECT_RATIO : String = "maintainAspectRatio";
		/**
		 * Specifies that the video be displayed at exactly the	 * height and width of the source video.	 * 	 * <p>For example, if <code>registrationWidth = 100</code> and	 * <code>registrationHeight = 100</code>, if <code>registrationX = 200</code> and 	 * <code>registrationY = 200</code>, and if an FLV file is loaded with a <code>width</code> of 648 and	 * a <code>height</code> of 480 (for an approximate aspect ratio of 4:3), and if 	 * <code>align = VideoAlign.CENTER</code> and <code>scaleMode = NO_SCALE</code>, 	 * you end up with <code>width = 648</code>, <code>height = 480</code>, <code>x = (200 +	 * ((100 - 648) / 2)) = -74</code>, and <code>y = (200 + ((100 - 480) / 2)) =	 * 10</code>.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const NO_SCALE : String = "noScale";
		/**
		 * Specifies that the video be displayed at the	 * height and width specified by the <code>registrationHeight</code>	 * or <code>height</code> and <code>registrationWidth</code> or <code>width</code>	 * properties.	 * 	 * <p>For example, if <code>registrationWidth = width = 100</code> and	 * <code>registrationHeight = height = 100</code>, if <code>registrationX = 200</code> and 	 * <code>registrationY = 200</code>, and if an FLV file is loaded with a <code>width</code> of 648 and	 * a <code>height</code> of 480 (for an approximate aspect ratio of 4:3), and if 	 * <code>align = VideoAlign.CENTER</code> and <code>scaleMode = EXACT_FIT</code>, 	 * you end up with <code>width = 100</code>, <code>height = 100</code>, <code>x = 200</code>, 	 * and <code>y = 200</code>.</p>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static const EXACT_FIT : String = "exactFit";

	}
}
