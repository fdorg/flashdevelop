package fl.video
{
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.events.*;

	/**
	 * XML examples from above without xml entitiy substitution:	 *	 * <smil>	 *     <head>	 *         <meta base="rtmp://myserver/mypgm/" />	 *         <layout>	 *             <root-layout width="240" height="180" />	 *         </layout>	 *     </head>	 *     <body>	 *         <switch>	 *             <ref src="myvideo_cable.flv" system-bitrate="128000" dur="3:00.1"/>	 *             <video src="myvideo_isdn.flv" system-bitrate="56000" dur="3:00.1"/>	 *             <video src="myvideo_mdm.flv" dur="3:00.1"/>	 *         </switch>	 *     </body>	 * </smil>	 *	 * <smil>	 *     <head>	 *         <layout>	 *             <root-layout width="240" height="180" />	 *         </layout>	 *     </head>	 *     <body>	 *         <video src="http://myserver/myvideo.flv" dur="3:00.1"/>	 *     </body>	 *	 * Precise subset of SMIL supported (more readable format):	 *	 * * smil tag - top level tag	 *     o head tag	 *         + meta tag	 *             # Only base attribute supported	 *             * Two instances are supported for Flash Media Server (FMS).  First is primary server, second is backup.	 *         + layout tag	 *             # Only first instance is used, rest ignored.	 *             # root-layout tag	 *                 * Only width and height attributes supported.	 *                 * Width and height only supported in absolute pixel values .	 *     o body tag	 *         + Only one tag allowed in body (either switch, video or ref)	 *         + switch tag supported	 *         + video tag supported	 *              # At top level and within switch tag.	 *              # Only src, system-bitrate and dur attributes supported.	 *              # system-bitrate attribute only supported within switch tag.	 *              # dur attribute we only support full clock format (e.g. 00:03:00.1) and partial clock format (e.g. 03:00.1).	 *         + ref tag - synonym for video tag     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class SMILManager
	{
		private var _owner : INCManager;
		local var xml : XML;
		local var xmlLoader : URLLoader;
		local var baseURLAttr : Array;
		local var width : int;
		local var height : int;
		local var videoTags : Array;
		private var _url : String;

		/**
		 * constructor         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function SMILManager (owner:INCManager);
		/**
		 * <p>Starts download of XML file.  Will be parsed and based		 * on that we will decide how to connect.</p>		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function connectXML (url:String) : Boolean;
		/**
		 * <p>Append version parameter to URL.</p>		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function fixURL (origURL:String) : String;
		/**
		 * <p>Handles load of XML.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function xmlLoadEventHandler (e:Event) : void;
		/**
		 * parse head node of smil		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseHead (parentNode:XML) : void;
		/**
		 * parse layout node of smil		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseLayout (parentNode:XML) : void;
		/**
		 * parse body node of smil		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseBody (parentNode:XML) : void;
		/**
		 * parse switch node of smil		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseSwitch (parentNode:XML) : void;
		/**
		 * parse video or ref node of smil.  Returns object with		 * attribute info.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseVideo (node:XML) : Object;
		/**
		 * parse time in hh:mm:ss.s or mm:ss.s format.		 * Also accepts a bare number of seconds with		 * no colons.  Returns a number of seconds.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function parseTime (timeStr:String) : Number;
		/**
		 * checks for extra, unrecognized elements of the given kind		 * in parentNode and throws VideoError if any are found.		 * Ignores any nodes with different nodeKind().  Takes the		 * list of recognized elements as a parameter.		 *         * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function checkForIllegalNodes (parentNode:XML, kind:String, legalNodes:Array) : void;
	}
}
