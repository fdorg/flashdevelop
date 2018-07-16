package fl.video
{
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.events.*;

	/**
	 * <p>Handles downloading and parsing Timed Text (TT) xml format	 * for fl.video.FLVPlayback. See fl.video.FLVPlaybackCaptioning	 * for more info on the subset of Timed Text supported.</p>	 * 	 * <p>When the timed text is parsed it is turned into ActionScript	 * cue points which are added to the FLVPlayback instance's list.	 * The cue points use the naming convention of beginning with the	 * prefix "fl.video.caption.", so users should not create any cue points	 * with names like this unless they want them used for captioning,	 * and users who are doing general cue point handling should	 * ignore cue points with names like this.</p>	 * 	 * <p>The ActionScript cue points created have the following	 * attributes:</p>	 * 	 * <ul>	 * 	 *   <li>name: starts with prefix "fl.video.caption." followed by	 *   a version number ("2.0" for this release) and then has some	 *   arbitrary string after that.  In practice the string is	 *   simply a series of positive integers incremented each time to	 *   keep each name unique.  It is not necessary that each name be	 *   unique.</li>	 * 	 *   <li>time: the time when the caption should display.</li>	 * 	 *   <li>parameters:</li>	 *	 *   <ul>	 *	 *     <li>text:String - html formatted text for the caption.	 *     This text is passed directly to the TextField.htmlText	 *     property. </li>	 * 	 *     <li>endTime:Number - the time when the caption should	 *     disappear.  if this is NaN, then the caption will display	 *     until the flv completes, i.e. the FLVPlayback instance	 *     dispatches the <code>VideoEvent.COMPLETE</code> event.</li>	 * 	 *     <li>url:String - this is the URL which the timed text xml	 *     was loaded from.  This is used so that if multiple urls are	 *     loaded, for example if the English captions are loaded	 *     initially, but then the user switches to the German	 *     captions, the already created captions for English will be	 *     ignored.  If captions are discovered with url undefined,	 *     they will always be displayed (this is to support creating	 *     caption cue points via other means, see below).</li>	 * 	 *     <li>backgroundColor:uint - sets TextField.backgroundColor</li>	 * 	 *     <li>backgroundColorAlpha:Boolean - true if backgroundColor	 *     has alpha of 0%, the only alpha we respect (other than	 *     100%).  Sets TextField.background = !backgroundColor.</li>	 * 	 *     <li>wrapOption:Boolean - sets TextField.wordWrap</li>	 * 	 *   </ul>	 * 	 * </ul>	 * 	 * <p>If a user created his or her own cue points that stick to	 * this standard, whether they be AS cue points or cue points	 * embedded in the FLV, they would also work with the	 * FLVPlaybackCaptioning component.</p>	 * 	 * @see fl.video.FLVPlaybackCaptioning     * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class TimedTextManager
	{
		local var owner : FLVPlaybackCaptioning;
		private var flvPlayback : FLVPlayback;
		private var videoPlayerIndex : uint;
		private var limitFormatting : Boolean;
		public var xml : XML;
		public var xmlLoader : URLLoader;
		private var _url : String;
		local var nameCounter : uint;
		local var lastCuePoint : Object;
		local var styleStack : Array;
		local var definedStyles : Object;
		local var styleCounter : uint;
		local var whiteSpacePreserved : Boolean;
		local var fontTagOpened : Object;
		local var italicTagOpen : Boolean;
		local var boldTagOpen : Boolean;
		static var CAPTION_LEVEL_ATTRS : Array;
		local var xmlNamespace : Namespace;
		local var xmlns : Namespace;
		local var tts : Namespace;
		local var ttp : Namespace;

		/**
		 * constructor
		 */
		public function TimedTextManager (owner:FLVPlaybackCaptioning);
		/**
		 * <p>Starts download of XML file.  Will be parsed and based		 * on that we will decide how to connect.</p>		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function load (url:String) : void;
		/**
		 * <p>Handles load of XML.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function xmlLoadEventHandler (e:Event) : void;
		/**
		 * parse head node of tt		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseHead (parentNode:XML) : void;
		/**
		 * parse styling node of tt		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseStyling (parentNode:XML) : void;
		/**
		 * parse body node of tt		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseBody (parentNode:XML) : void;
		function parseParagraph (parentNode:XML) : void;
		function parseSpan (parentNode:XML, cuePoint:Object) : String;
		function openFontTag () : String;
		function closeFontTags () : String;
		function parseStyleAttribute (xmlNode:XML, styleObj:Object) : void;
		/**
		 * Extracts supported style attributes (tts:... attributes		 * in namespace http://www.w3.org/2006/04/ttaf1#styling)         * from an XMLList of attributes for a tag element.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseTTSAttributes (xmlNode:XML, styleObject:Object) : void;
		function getStyle () : Object;
		function pushStyle (styleObj:Object) : void;
		function popStyle () : void;
		/**
		 * copies attributes from one style object to another         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function copyStyleObjectProps (tgt:Object, src:Object) : void;
		/**
		 * parses color         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseColor (colorStr:String) : Object;
		/**
		 * parses fontSize         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseFontSize (sizeStr:String) : String;
		/**
		 * parses fontFamily         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseFontFamily (familyStr:String) : String;
		/**
		 * parse time in hh:mm:ss.s or mm:ss.s format.		 * Also accepts a bare number of seconds with		 * no colons.  Returns a number of seconds.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseTimeAttribute (parentNode:XML, attr:String, req:Boolean) : Number;
		/**
		 * checks for extra, unrecognized elements of the given kind		 * in parentNode and throws VideoError if any are found.		 * Ignores any nodes with different nodeKind().  Takes the		 * list of recognized elements as a parameter.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function checkForIllegalElements (parentNode:XML, legalNodes:Object) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function fixCaptionText (inText:String) : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function unicodeEscapeReplace (match:String, first:String, second:String, index:int, all:String) : String;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getSpaceAttribute (node:XML) : String;
	}
}
