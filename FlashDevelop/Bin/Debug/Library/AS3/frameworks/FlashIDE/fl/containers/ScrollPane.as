package fl.containers
{
	import fl.containers.BaseScrollPane;
	import fl.controls.ScrollBar;
	import fl.controls.ScrollPolicy;
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ScrollEvent;
	import fl.managers.IFocusManagerComponent;
	import flash.display.DisplayObject;
	import flash.display.Loader;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.events.ProgressEvent;
	import flash.events.SecurityErrorEvent;
	import flash.events.IOErrorEvent;
	import flash.events.HTTPStatusEvent;
	import flash.geom.Rectangle;
	import flash.net.URLRequest;
	import flash.system.ApplicationDomain;
	import flash.system.LoaderContext;
	import flash.ui.Keyboard;

	/**
	 * @copy BaseScrollPane#event:scroll     *     * @includeExample examples/ScrollPane.scroll.1.as -noswf     *     * @eventType fl.events.ScrollEvent.SCROLL     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="scroll", type="fl.events.ScrollEvent")] 
	/**
	 * Dispatched while content is loading.     *     * @eventType flash.events.ProgressEvent.PROGRESS     *     * @includeExample examples/ScrollPane.percentLoaded.1.as -noswf     *     * @see #event:complete     * @see #bytesLoaded     * @see #bytesTotal     * @see #percentLoaded     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="progress", type="flash.events.ProgressEvent")] 
	/**
	 * Dispatched when content has finished loading.     *     * @includeExample examples/ScrollPane.complete.1.as -noswf     *     * @eventType flash.events.Event.COMPLETE     *      * @see #event:progress     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event(name="complete", type="flash.events.Event")] 
	/**
	 * Dispatched when the properties and methods of a loaded SWF file are accessible.      * The following conditions must exist for this event to be dispatched:      * <ul>     *     <li>All the properties and methods that are associated with the loaded object,	 *         as well as those that are associated with the component, must be accessible.</li>     *     <li>The constructors for all child objects must have completed.</li>     * </ul>     *     * @eventType flash.events.Event.INIT     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("init", type="flash.events.Event")] 
	/**
	 * Dispatched after an input or output error occurs.     *     * @includeExample examples/UILoader.ioError.1.as -noswf     *     * @eventType flash.events.IOErrorEvent.IO_ERROR     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("ioError", type="flash.events.IOErrorEvent")] 
	/**
	 * Dispatched after a network operation starts.     *     * @eventType flash.events.Event.OPEN     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("open", type="flash.events.Event")] 
	/**
	 * Dispatched after a security error occurs while content is loading.     *     * @eventType flash.events.SecurityErrorEvent.SECURITY_ERROR     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("securityError", type="flash.events.SecurityErrorEvent")] 
	/**
	 * Dispatched when content is loading. This event is dispatched regardless of     * whether the load operation was triggered by an auto-load process or an explicit call to the      * <code>load()</code> method.     *     * @includeExample examples/UILoader.progress.1.as -noswf     * 	 * @eventType flash.events.ProgressEvent.PROGRESS     *     * @see #event:complete     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("progress", type="flash.events.ProgressEvent")] 
	/**
	 * The skin that shows when the scroll pane is disabled.     *     * @default ScrollPane_disabledSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="disabledSkin", type="Class")] 
	/**
	 * The default skin shown on the scroll pane.     *     * @default ScrollPane_upSkin     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="upSkin", type="Class")] 
	/**
	 * The amount of padding to put around the content in the scroll pane, in pixels.     *     * @default 0     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Style(name="contentPadding", type="Number", format="Length")] 

	/**
	 * The ScrollPane component displays display objects and JPEG, GIF, and PNG files,     * as well as SWF files, in a scrollable area. You can use a scroll pane to      * limit the screen area that is occupied by these media types.     * The scroll pane can display content that is loaded from a local     * disk or from the Internet. You can set this content while     * authoring and, at run time, by using ActionScript. After the scroll     * pane has focus, if its content has valid tab stops, those     * markers receive focus. After the last tab stop in the content,     * focus moves to the next component. The vertical and horizontal     * scroll bars in the scroll pane do not receive focus.	 *	 * <p><strong>Note:</strong> When content is being loaded from a different 	 * domain or <em>sandbox</em>, the properties of the content may be inaccessible 	 * for security reasons. For more information about how domain security 	 * affects the load process, see the Loader class.</p>	 *	 * <p><strong>Note:</strong> When loading very large image files into a ScrollPane object,	 * it may be necessary to listen for the <code>complete</code> event and then resize the	 * ScrollPane using the <code>setSize()</code> method. See the <code>complete</code>	 * event example.</p>	 *	 * @see flash.display.Loader Loader	 *     * @includeExample examples/ScrollPaneExample.as -noswf     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ScrollPane extends BaseScrollPane implements IFocusManagerComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _source : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _scrollDrag : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentClip : Sprite;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var loader : Loader;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var xOffset : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var yOffset : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var scrollDragHPos : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var scrollDragVPos : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var currentContent : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * Gets or sets a value that indicates whether scrolling occurs when a         * user drags on content within the scroll pane. A value of <code>true</code>         * indicates that scrolling occurs when a user drags on the content; a value         * of <code>false</code> indicates that it does not.         *         * @default false         *         * @includeExample examples/ScrollPane.scrollDrag.1.as -noswf         *         * @see #event:scroll         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scrollDrag () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scrollDrag (value:Boolean) : void;
		/**
		 * Gets a number between 0 and 100 indicating what percentage of the content is loaded.         * If you are loading assets from your library, and not externally loaded content,          * the <code>percentLoaded</code> property is set to 0.         *         * @default 0         *         * @includeExample examples/ScrollPane.percentLoaded.1.as -noswf         *         * @see #bytesLoaded         * @see #bytesTotal         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get percentLoaded () : Number;
		/**
		 * Gets the count of bytes of content that have been loaded.         * When this property equals the value of <code>bytesTotal</code>,         * all the bytes are loaded.           *         * @default 0         *         * @see #bytesTotal         * @see #percentLoaded         *         * @includeExample examples/ScrollPane.bytesLoaded.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesLoaded () : Number;
		/**
		 * Gets the count of bytes of content to be loaded.         *         * @default 0         *         * @includeExample examples/ScrollPane.percentLoaded.1.as -noswf         *         * @see #bytesLoaded         * @see #percentLoaded         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesTotal () : Number;
		/**
		 * Gets a reference to the content loaded into the scroll pane.         *         * @default null         *         * @includeExample examples/ScrollPane.content.1.as -noswf         * @includeExample examples/ScrollPane.content.2.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get content () : DisplayObject;
		/**
		 * Gets or sets an absolute or relative URL that identifies the 		 * location of the SWF or image file to load, the class name 		 * of a movie clip in the library, a reference to a display object,		 * or a instance name of a movie clip on the same level as the component.		 * 		 * <p>Valid image file formats include GIF, PNG, and JPEG. To load an 		 * asset by using a URLRequest object, use the <code>load()</code> 		 * method.</p>         *         * @default null         *         * @includeExample examples/ScrollPane.source.1.as -noswf         * @includeExample examples/ScrollPane.source.2.as -noswf         *         * @see #load()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set source (value:Object) : void;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *         * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf         *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new ScrollPane component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function ScrollPane ();
		/**
		 * Reloads the contents of the scroll pane.          *         * <p> This method does not redraw the scroll bar. To reset the          * scroll bar, use the <code>update()</code> method.</p>         *         * @includeExample examples/ScrollPane.refreshPane.1.as -noswf         *         * @see #update()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function refreshPane () : void;
		/**
		 * Refreshes the scroll bar properties based on the width         * and height of the content.  This is useful if the content         * of the ScrollPane changes during run time.         *         * @includeExample examples/ScrollPane.update.1.as -noswf         *         * @see #refreshPane()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function update () : void;
		/**
		 * The request parameter of this method accepts only a URLRequest object 		 * whose <code>source</code> property contains a string, a class, or a 		 * URLRequest object.		 * 		 * By default, the LoaderContext object uses the current domain as the 		 * application domain. To specify a different application domain value, 		 * to check a policy file, or to change the security domain, initialize 		 * a new LoaderContext object and pass it to this method.		 *		 * @param request The URLRequest object to use to load an image into the scroll pane.		 * @param context The LoaderContext object that sets the context of the load operation.		 *         * @see #source         * @see fl.containers.UILoader#load() UILoader.load()         * @see flash.net.URLRequest         * @see flash.system.ApplicationDomain         * @see flash.system.LoaderContext         *         * @includeExample examples/ScrollPane.load.1.as -noswf         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function load (request:URLRequest, context:LoaderContext = null) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setVerticalScrollPosition (scrollPos:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setHorizontalScrollPosition (scrollPos:Number, fireEvent:Boolean = false) : void;
		/**
		 * @private (protected)
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function onContentLoad (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function passEvent (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initLoader () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleScroll (event:ScrollEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleError (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleInit (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function clearLoadEvents () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function doDrag (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function doStartDrag (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function endDrag (event:MouseEvent) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function setScrollDrag () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawBackground () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function clearContent () : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function keyDownHandler (event:KeyboardEvent) : void;
		/**
		 * @private
		 */
		protected function calculateAvailableHeight () : Number;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function configUI () : void;
	}
}
