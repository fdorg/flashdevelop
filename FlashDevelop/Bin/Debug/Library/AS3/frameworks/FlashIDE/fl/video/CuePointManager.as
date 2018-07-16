package fl.video
{
	/**
	 * Dispatched when a cue point is reached.  Event Object has an     * <code>info</code> property that contains the <code>info</code> object received by the	 * <code>NetStream.onCuePoint</code> callback for FLV cue points or	 * the object passed into the ActionScript cue point APIs for ActionScript cue points.</p>     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("cuePoint")] 

	/**
	 * The CuePointManager class manages ActionScript cue points and enabling/disabling FLV	 * embedded cue points for the FLVPlayback class.	 *      * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class CuePointManager
	{
		private var _owner : FLVPlayback;
		local var _metadataLoaded : Boolean;
		local var _disabledCuePoints : Array;
		local var _disabledCuePointsByNameOnly : Object;
		local var _asCuePointIndex : int;
		local var _asCuePointTolerance : Number;
		local var _linearSearchTolerance : Number;
		local var _id : uint;
		static const DEFAULT_LINEAR_SEARCH_TOLERANCE : Number = 50;
		local var allCuePoints : Array;
		local var asCuePoints : Array;
		local var flvCuePoints : Array;
		local var navCuePoints : Array;
		local var eventCuePoints : Array;
		static var cuePointsReplace : Array;

		/**
		 * Has metadata been loaded.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get metadataLoaded () : Boolean;
		/**
		 * Set by FLVPlayback to update _asCuePointTolerance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set playheadUpdateInterval (aTime:Number) : void;
		/**
		 * Corresponds to _vp and _cpMgr array index in FLVPlayback.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get id () : uint;

		/**
		 * Constructor		 *         * @helpid 0         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function CuePointManager (owner:FLVPlayback, id:uint);
		/**
		 * Reset cue point lists.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function reset () : void;
		/**
		 * Add an ActionScript cue point.		 *		 * <p>It is legal to add multiple AS cue points with the same		 * name and time.  When removeASCuePoint is called with this		 * name and time, all will be removed.</p>		 *		 * @param timeOrCuePoint If Object, then object describing the cue		 * point.  Must have a name:String and time:Number (in seconds)		 * property.  May have a parameters:Object property that holds		 * name/value pairs.  May have type:String set to "actionscript",		 * if it is missing or set to something else it will be set		 * automatically.  If the Object does not conform to these		 * conventions, a <code>VideoError</code> will be thrown.		 *		 * <p>If Number, then time for new cue point to be added		 * and name parameter must follow.</p>         *		 * @param name Name for cuePoint if timeOrCuePoint parameter		 * is a Number.         *		 * @param parameters Parameters for cuePoint if		 * timeOrCuePoint parameter is a Number.         *		 * @return A copy of the cuePoint Object added.  The copy has the		 * following additional properties:		 *		 * <ul>		 *     <li><code>array</code> - the array of all AS cue points.  Treat		 *         this array as read only as adding, removing or editing objects		 *         within it can cause cue points to malfunction.</li>		 *     <li><code>index</code> - the index into the array for the		 *         returned cuepoint.</li>		 * </ul>		 * 		 * @throws VideoError if parameters are invalid		 * @see #removeASCuePoint()         * @see #getCuePoint()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function addASCuePoint (timeOrCuePoint:*, name:String = null, parameters:Object = null) : Object;
		/**
		 * Remove an ActionScript cue point from the currently		 * loaded FLV.  Only the name and time properties are used		 * from the cuePoint parameter to find the cue point to be		 * removed.		 *		 * <p>If multiple AS cue points match the search criteria, only		 * one will be removed.  To remove all, call this function		 * repeatedly in a loop with the same parameters until it returns		 * <code>null</code>.</p>		 *		 * @param timeNameOrCuePoint If string, name of cue point to		 * remove; remove first cue point with this name.  If number, time		 * of cue point to remove; remove first cue point at this time.		 * If Object, then object with name and time properties, remove		 * cue point with both this name and time.         *		 * @returns The cue point that was removed.  If there was no		 * matching cue point then <code>null</code> is returned.         *		 * @see #addASCuePoint()         * @see #getCuePoint()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeASCuePoint (timeNameOrCuePoint:*) : Object;
		/**
		 * <p>Enable or disable one or more FLV cue point.  Disabled cue		 * points are disabled for being dispatched as events and		 * navigating to them with <code>seekToPrevNavCuePoint()</code>,		 * <code>seekToNextNavCuePoint()</code> and		 * <code>seekToNavCuePoint()</code>.</p>		 *		 * <p>If this API is called just after setting the		 * <code>contentPath</code> property or if no FLV is loaded, then		 * the cue point will be enabled or disabled in the FLV to be		 * loaded.  Otherwise, it will be enabled or disabled in the		 * currently loaded FLV (even if it is called immediately before		 * setting the <code>contentPath</code> property to load another		 * FLV).</p>		 *		 * <p>Changes caused by calls to this function will not be		 * reflected in results returned from		 * <code>isFLVCuePointEnabled</code> until		 * <code>metadataLoaded</code> is true.</p>		 *		 * @param enabled whether to enable or disable FLV cue point		 * @param timeNameOrCuePoint If string, name of cue point to		 * enable/disable.  If number, time of cue point to		 * enable/disable.  If Object, then object with name and time		 * properties, enable/disable cue point that matches both name and		 * time.		 * @returns If <code>metadataLoaded</code> is true, returns number		 * of cue points whose enabled state was changed.  If		 * <code>metadataLoaded</code> is false, always returns -1.		 * @see #isFLVCuePointEnabled()         * @see #getCuePoint()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setFLVCuePointEnabled (enabled:Boolean, timeNameOrCuePoint:*) : int;
		/**
		 * removes enabled cue points from _disabledCuePoints         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function removeCuePoints (cuePointArray:Array, cuePoint:Object) : Number;
		/**
		 * Inserts cue points into array.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function insertCuePoint (insertIndex:int, cuePointArray:Array, cuePoint:Object) : Array;
		/**
		 * Returns false if FLV embedded cue point is disabled by		 * ActionScript.  Cue points are disabled via setting the		 * <code>cuePoints</code> property or by calling		 * <code>setFLVCuePointEnabled()</code>.		 *		 * <p>The return value from this function is only meaningful when		 * <code>metadata</code> is true.  It always returns false		 * when it is null.</p>		 *		 * @param timeNameOrCuePoint If string, name of cue point to		 * check; return false only if ALL cue points with this name are		 * disabled.  If number, time of cue point to check.  If Object,		 * then object with name and time properties, check cue point that		 * matches both name and time.		 * @returns false if cue point(s) is found and is disabled, true		 * either if no such cue point exists or if it is not disabled.		 * If time given is undefined, null or less than 0 then returns		 * false only if all cue points with this name are disabled.		 *		 * <p>The return value from this function is only meaningful when		 * <code>metadata</code> is true.  It always returns true when it		 * is false.</p>		 * @see #getCuePoint()         * @see #setFLVCuePointEnabled()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function isFLVCuePointEnabled (timeNameOrCuePoint:*) : Boolean;
		/**
		 * Called by FLVPlayback on "playheadUpdate" event         * to throw "cuePoint" events when appropriate.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function dispatchASCuePoints () : void;
		/**
		 * When our place in the stream is changed, this is called		 * to reset our index into actionscript cue point array.		 * Another method is used when AS cue points are added         * are removed.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function resetASCuePointIndex (time:Number) : void;
		/**
		 * Called by FLVPlayback "metadataReceived" event handler to process flv         * embedded cue points array.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function processFLVCuePoints (metadataCuePoints:Array) : void;
		/**
		 * <p>Process Array passed into FLVPlayback cuePoints property.		 * Array actually holds name value pairs.  Each cue point starts		 * with 5 pairs: t,time,n,name,t,type,d,disabled,p,numparams.		 * time is a Number in milliseconds (e.g. 3000 = 3 seconds), name		 * is a String, type is a Number (0 = event, 1 = navigation, 2 =		 * actionscript), disabled is a Number (0 for false, 1 for true)		 * and numparams is a Number.  After this, there are numparams		 * name/value pairs which could be any simple type.</p>		 *		 * <p>Note that all Strings are escaped with html/xml entities for		 * ampersand (&amp;), double quote (&quot;), single quote (&#39;)		 * and comma (&#44;), so must be unescaped.</p>		 *         * @see FLVPlayback#cuePoints         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function processCuePointsProperty (cuePoints:Array) : void;
		/**
		 * Used by processCuePointsProperty         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function addOrDisable (disable:Boolean, cuePoint:Object) : void;
		/**
		 * Used by processCuePointsProperty         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function unescape (origStr:String) : String;
		/**
		 * Search for a cue point in an array sorted by time.  See		 * closeIsOK parameter for search rules.		 *		 * @param cuePointArray array to search		 * @param closeIsOK If true, the behavior differs depending on the		 * parameters passed in:		 * 		 * <ul>		 *		 * <li>If name is null or undefined, then if the specific time is		 * not found then the closest time earlier than that is returned.		 * If there is no cue point earlier than time, the first cue point		 * is returned.</li>		 *		 * <li>If time is null, undefined or less than 0 then the first		 * cue point with the given name is returned.</li>		 *		 * <li>If time and name are both defined then the closest cue		 * point, then if the specific time and name is not found then the		 * closest time earlier than that with that name is returned.  If		 * there is no cue point with that name and with an earlier time,		 * then the first cue point with that name is returned.  If there		 * is no cue point with that name, null is returned.</li>		 * 		 * <li>If time is null, undefined or less than 0 and name is null		 * or undefined, a VideoError is thrown.</li>		 * 		 * </ul>		 *		 * <p>If closeIsOK is false the behavior is:</p>		 *		 * <ul>		 *		 * <li>If name is null or undefined and there is a cue point with		 * exactly that time, it is returned.  Otherwise null is		 * returned.</li>		 *		 * <li>If time is null, undefined or less than 0 then the first		 * cue point with the given name is returned.</li>		 *		 * <li>If time and name are both defined and there is a cue point		 * with exactly that time and name, it is returned.  Otherwise null		 * is returned.</li>		 *		 * <li>If time is null, undefined or less than 0 and name is null		 * or undefined, a VideoError is thrown.</li>		 * 		 * </ul>		 * @param time search criteria		 * @param name search criteria		 * @param start index of first item to be searched, used for		 * recursive implementation, defaults to 0 if undefined		 * @param len length of array to search, used for recursive		 * implementation, defaults to cuePointArray.length if undefined		 * @returns index for cue point in given array or -1 if no match found		 * @throws VideoError if time and/or name parameters are bad         * @see #cuePointCompare()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getCuePointIndex (cuePointArray:Array, closeIsOK:Boolean, time:Number = NaN, name:String = null, start:int = -1, len:int = -1) : int;
		/**
		 * Given a name, array and index, returns the next cue point in		 * that array after given index with the same name.  Returns null		 * if no cue point after that one with that name.  Throws		 * VideoError if argument is invalid.		 *         * @returns index for cue point in given array or -1 if no match found.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getNextCuePointIndexWithName (name:String, array:Array, index:int) : int;
		/**
		 * Takes two cue point Objects and returns -1 if first sorts		 * before second, 1 if second sorts before first and 0 if they are		 * equal.  First compares times with millisecond precision.  If         * they match, compares name if name parameter is not <code>null</code> or <code>undefined</code>.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static function cuePointCompare (time:Number, name:String, cuePoint:Object) : int;
		/**
		 * <p>Search for a cue point in the given array at the given time		 * and/or with given name.</p>		 *		 * @param closeIsOK If true, the behavior differs depending on the		 * parameters passed in:		 * 		 * <ul>		 *		 * <li>If name is null or undefined, then if the specific time is		 * not found then the closest time earlier than that is returned.		 * If there is no cue point earlier than time, the first cue point		 * is returned.</li>		 *		 * <li>If time is null, undefined or less than 0 then the first		 * cue point with the given name is returned.</li>		 *		 * <li>If time and name are both defined then the closest cue		 * point, then if the specific time and name is not found then the		 * closest time earlier than that with that name is returned.  If		 * there is no cue point with that name and with an earlier time,		 * then the first cue point with that name is returned.  If there		 * is no cue point with that name, null is returned.</li>		 * 		 * <li>If time is null, undefined or less than 0 and name is null		 * or undefined, a VideoError is thrown.</li>		 * 		 * </ul>		 *		 * <p>If closeIsOK is false the behavior is:</p>		 *		 * <ul>		 *		 * <li>If name is null or undefined and there is a cue point with		 * exactly that time, it is returned.  Otherwise null is		 * returned.</li>		 *		 * <li>If time is null, undefined or less than 0 then the first		 * cue point with the given name is returned.</li>		 *		 * <li>If time and name are both defined and there is a cue point		 * with exactly that time and name, it is returned.  Otherwise null		 * is returned.</li>		 *		 * <li>If time is null, undefined or less than 0 and name is null		 * or undefined, a VideoError is thrown.</li>		 * 		 * </ul>		 * @param timeOrCuePoint If String, then name for search.  If		 * Number, then time for search.  If Object, then cuepoint object		 * containing time and/or name parameters for search.		 * @returns <code>null</code> if no match was found, otherwise		 * copy of cuePoint object with additional properties:		 *		 * <ul>		 * 		 * <li><code>array</code> - the array that was searched.  Treat		 * this array as read only as adding, removing or editing objects		 * within it can cause cue points to malfunction.</li>		 *		 * <li><code>index</code> - the index into the array for the		 * returned cuepoint.</li>		 *		 * </ul>         * @see #getCuePointIndex()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getCuePoint (cuePointArray:Array, closeIsOK:Boolean, timeNameOrCuePoint:*) : Object;
		/**
		 * <p>Given a cue point object returned from getCuePoint (needs		 * the index and array properties added to those cue points),		 * returns the next cue point in that array after that one with		 * the same name.  Returns null if no cue point after that one		 * with that name.  Throws VideoError if argument is invalid.</p>		 *		 * @returns <code>null</code> if no match was found, otherwise		 * copy of cuePoint object with additional properties:		 *		 * <ul>		 * 		 * <li><code>array</code> - the array that was searched.  Treat		 * this array as read only as adding, removing or editing objects		 * within it can cause cue points to malfunction.</li>		 *		 * <li><code>index</code> - the index into the array for the		 * returned cuepoint.</li>		 *         * </ul>         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		function getNextCuePointWithName (cuePoint:Object) : Object;
		/**
		 * Used to make copies of cue point objects.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		static function deepCopyObject (obj:Object, recurseLevel:uint = 0) : Object;
	}
}
