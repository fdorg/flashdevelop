package fl.containers
{
	import fl.core.InvalidationType;
	import fl.core.UIComponent;
	import fl.events.ComponentEvent;
	import flash.display.DisplayObject;
	import flash.display.Graphics;
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.display.Shape;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.ProgressEvent;
	import flash.events.SecurityErrorEvent;
	import flash.events.IOErrorEvent;
	import flash.events.HTTPStatusEvent;
	import flash.net.URLRequest;
	import flash.system.ApplicationDomain;
	import flash.system.LoaderContext;
	import flash.utils.ByteArray;

	/**
	 * Dispatched when content loading is complete. This event is dispatched      * regardless of whether the load operation was triggered by an auto-load process or an explicit      * call to the <code>load()</code> method.     *	 * @eventType flash.events.Event.COMPLETE     *     * @includeExample examples/UILoader.complete.1.as -noswf     *     * @see #event:progress     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("complete", type="flash.events.Event")] 
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
	 * Dispatched when content is loading. This event is dispatched regardless of     * whether the load operation was triggered by an auto-load process or an explicit call to the      * <code>load()</code> method.     *     * @includeExample examples/UILoader.progress.1.as -noswf     * 	 * @eventType flash.events.ProgressEvent.PROGRESS     *     * @see #event:complete     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("progress", type="flash.events.ProgressEvent")] 
	/**
	 * Dispatched after the component is resized.     *     * @includeExample examples/UILoader.resize.1.as -noswf     *     * @eventType fl.events.ComponentEvent.RESIZE     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("resize", type="fl.events.ComponentEvent")] 
	/**
	 * Dispatched after a security error occurs while content is loading.     *     * @eventType flash.events.SecurityErrorEvent.SECURITY_ERROR     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	[Event("securityError", type="flash.events.SecurityErrorEvent")] 

	/**
	 * The UILoader class makes it possible to set content to load and to then 	 * monitor the loading operation at run time. This class also handles the resizing 	 * of the loaded content. If loading content from a different domain (sandbox), 	 * security implications may mean content properties are inaccessible. Please	 * see the Loader class for more information.	 *	 * <p>Using ActionScript to set a property of the UILoader class 	 * overrides the parameter of the same name that is set in the Property 	 * inspector or Component inspector.</p>	 *	 * <p>This component wraps flash.display.Loader. The Loader class handles all the actual	 * loading; the UILoader just provides a visual display for the Loader object.</p>	 *	 * <p><strong>Note:</strong> When content is being loaded from a different 	 * domain or <em>sandbox</em>, the properties of the content may be inaccessible 	 * for security reasons. For more information about how domain security 	 * affects the load process, see the Loader class.</p>	 *	 * @see flash.display.Loader Loader	 *	 * @includeExample examples/UILoaderExample.as -noswf     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class UILoader extends UIComponent
	{
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _scaleContent : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _autoLoad : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentInited : Boolean;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _source : Object;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var loader : Loader;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var _maintainAspectRatio : Boolean;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected var contentClip : Sprite;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var defaultStyles : Object;

		/**
		 * Gets or sets a value that indicates whether the UILoader instance automatically 		 * loads the specified content. A value of <code>true</code> indicates that the UILoader		 * automatically loads the content; a value of <code>false</code> indicates that content		 * is not loaded until the <code>load()</code> method is called.         *         * @default true         *         * @includeExample examples/UILoader.autoLoad.1.as -noswf         *         * @see #load()         * @see #source         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get autoLoad () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set autoLoad (value:Boolean) : void;
		/**
		 * Gets or sets a value that indicates whether to automatically scale the image to 		 * the size of the UILoader instance. A value of <code>true</code> indicates that the		 * image is automatically scaled to the size of the UILoader instance; a value of 		 * <code>false</code> indicates that the loaded content is automatically scaled		 * to its default size.         *         * @default true         *		 * @includeExample examples/UILoader.scaleContent.1.as -noswf		 *         * @see #maintainAspectRatio         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get scaleContent () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set scaleContent (value:Boolean) : void;
		/**
		 * Gets or sets a value that indicates whether to maintain		 * the aspect ratio that was used in the original image or to resize		 * the image at the curent width and height of the UILoader component.		 * A value of <code>true</code> indicates that the original aspect ratio		 * is to be maintained; a value of <code>false</code> indicates that the 		 * loaded content should be resized to the current dimensions of the UILoader.		 *		 * <p>To use this property, you must set the <code>scaleContent</code> property to 		 * <code>false</code>; otherwise, this property is ignored.</p> 		 *          * @default true         *         * @includeExample examples/UILoader.maintainAspectRatio.1.as -noswf         *         * @see #scaleContent         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get maintainAspectRatio () : Boolean;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set maintainAspectRatio (value:Boolean) : void;
		/**
		 * @copy ScrollPane#bytesLoaded         *         * @default 0         *         * @includeExample examples/UILoader.progress.1.as -noswf         *         * @see #bytesTotal         * @see #percentLoaded         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesLoaded () : uint;
		/**
		 * @copy ScrollPane#bytesTotal         *         * @default 0         *         * @includeExample examples/UILoader.progress.1.as -noswf         *         * @see #bytesLoaded         * @see #percentLoaded         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get bytesTotal () : uint;
		/**
		 * Contains the root display object of the SWF file or image file (a JPEG, PNG, or GIF format file)          * that was loaded by using the <code>load()</code> method or setting the <code>source</code> property.         * The value is <code>undefined</code> until the load begins. Set the properties for the content 		 * within an event handler function for the <code>complete</code> event.         *         * @default null		 *		 * @includeExample examples/UILoader.content.1.as -noswf		 *         * @see #event:complete         * @see #load()         * @see #source         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get content () : DisplayObject;
		/**
		 * @copy fl.containers.ScrollPane#source         *         * @default null         * 		 * @includeExample examples/UILoader.source.1.as -noswf		 *         * @see #autoLoad         * @see #load()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get source () : Object;
		/**
		 * @private (setter)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function set source (value:Object) : void;
		/**
		 * @copy ScrollPane#percentLoaded         *         * @default 0         *         * @includeExample examples/UILoader.progress.1.as -noswf         *          * @see #bytesLoaded         * @see #bytesTotal         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function get percentLoaded () : Number;

		/**
		 * @copy fl.core.UIComponent#getStyleDefinition()         *         * @see fl.core.UIComponent#getStyle() UIComponent.getStyle()         * @see fl.core.UIComponent#setStyle() UIComponent.setStyle()         * @see fl.managers.StyleManager StyleManager         *		 * @includeExample ../core/examples/UIComponent.getStyleDefinition.1.as -noswf		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyleDefinition () : Object;
		/**
		 * Creates a new UILoader component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function UILoader ();
		/**
		 * Resizes the component to the requested size. If the <code>scaleContent</code>		 * property is set to <code>true</code>, the UILoader is not resized.         *         * @param w The width of the component, in pixels.         * @param h The height of the component, in pixels.		 *         * @includeExample examples/UILoader.setSize.1.as -noswf         *         * @see #scaleContent         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function setSize (w:Number, h:Number) : void;
		/**
		 * Loads binary data that is stored in a ByteArray object.         *         * @param bytes A ByteArray object that contains a file in one of the formats supported 		 *              by the Loader class: SWF, GIF, JPEG, or PNG.         *         * @param context Only the <code>applicationDomain</code> property of the LoaderContext 		 *                object applies; the <code>checkPolicyFile</code> and <code>securityDomain</code> 		 *                properties of the LoaderContext object do not apply.         *         * @throws ArgumentError The <code>length</code> property of the ByteArray object is 0.         *         * @throws IllegalOperationError The <code>checkPolicyFile</code> or <code>securityDomain</code>          *         property of the <code>context</code> parameter is non-null.         *         * @throws SecurityError The <code>applicationDomain</code> property of the          *         <code>context</code> property that was provided is from a domain that is not allowed.         *		 * @includeExample examples/UILoader.loadBytes.1.as -noswf		 *         * @see #source         * @see flash.display.Loader#loadBytes() Loader.loadBytes()         * @see flash.system.LoaderContext LoaderContext		 *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function loadBytes (bytes:ByteArray, context:LoaderContext = null) : void;
		/**
		 * Loads the specified content or, if no content is specified, loads the 		 * content from the location identified by the <code>source</code> property. 		 *		 * By default, the LoaderContext object uses the current domain as the 		 * application domain. To specify a different application domain, to check 		 * a policy file, or to change the security domain, initialize a new LoaderContext 		 * object and pass it to this method.		 *		 * <p>By default, the context property uses the current domain. 		 * To specify a different ApplicationDomain, check a policy file, or 		 * change the SecurityDomain, pass a new LoaderContext object.</p>		 *		 * @param request The URLRequest object that identifies the location from 		 *                which to load the content. If this value is not specified, 		 *                the current value of the <code>source</code> property is 		 *                used as the content location.		 * @param context The LoaderContext object that sets the context of the load operation.		 * @param context A LoaderContext object used to specify the context of the load .		 *         * @includeExample examples/UILoader.load.1.as -noswf         * @includeExample examples/UILoader.load.2.as -noswf         *		 * @see #autoLoad         * @see #close()         * @see #source         * @see #unload()         * @see flash.system.ApplicationDomain         * @see flash.system.LoaderContext         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function load (request:URLRequest = null, context:LoaderContext = null) : void;
		/**
		 * Removes a child of this UILoader object that was loaded by using 		 * either the <code>load()</code> method or the <code>source</code> 		 * property.         *         * @includeExample examples/UILoader.unload.1.as -noswf         *         * @see #load()		 * @see #source         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function unload () : void;
		/**
		 * Cancels a <code>load()</code> method operation that is currently in progress for the          * Loader instance. The <code>load()</code> method can also be called from the <code>source</code> parameter.         *		 * @throws Error The URLStream object does not have an open stream.         *         * @includeExample examples/UILoader.close.1.as -noswf         *         * @see #load()		 * @see #source         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function close () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function _unload (throwError:Boolean = false) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function initLoader () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function handleComplete (event:Event) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function passEvent (event:Event) : void;
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
		protected function draw () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function drawLayout () : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		protected function sizeContent (target:DisplayObject, contentWidth:Number, contentHeight:Number, targetWidth:Number, targetHeight:Number) : void;
		/**
		 * @private (protected)
		 */
		protected function configUI () : void;
	}
}
