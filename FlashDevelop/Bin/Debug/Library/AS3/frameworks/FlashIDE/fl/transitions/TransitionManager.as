package fl.transitions
{
	import flash.events.EventDispatcher;
	import flash.display.*;
	import flash.geom.Rectangle;
	import flash.events.Event;

	/**
	 * The TransitionManager class defines animation effects. It allows you to apply one of ten  * animation effects to movie clips. When creating custom components, you can use the  * TransitionManager class to apply animation effects to movie clips in your component's  * visual interface. The transition effects in fl.transitions.easing are defined as a set of * transition classes that all extend the base class fl.transitions.Transition. You apply  * transitions through an instance of a TransitionManager only; you do not instantiate them  * directly. The TransitionManager class implements animation events.  * <p>You can create a TransitionManager instance in two ways:</p> * <ul><li>Call the <code>TransitionManager.start()</code> method. This is the simplest and recommended way to create a TransitionManager instance.</li> * <li>Use the <code>new</code> operator. You then designate the transition properties  * and start the transition effect in a second step by calling the  * <code>TransitionManager.startTransition()</code> method.</li></ul> * @playerversion Flash 9 * @langversion 3.0 * @keyword Transition     * @see #start() TransitionManager.start() * @see #TransitionManager() TransitionManager constructor function * @see #startTransition() TransitionManager.startTransition() * @see fl.transitions.Tween * @see fl.transitions.easing
	 */
	public class TransitionManager extends EventDispatcher
	{
		/**
		 * @private
		 */
		private static var IDCount : int;
		/**
		 * @private
		 */
		public var type : Object;
		/**
		 * @private
		 */
		public var className : String;
		/**
		 * @private
		 */
		private var _content : MovieClip;
		/**
		 * @private
		 */
		private var _transitions : Object;
		/**
		 * @private
		 */
		public var _innerBounds : Rectangle;
		/**
		 * @private
		 */
		public var _outerBounds : Rectangle;
		/**
		 * @private
		 */
		public var _width : Number;
		/**
		 * @private
		 */
		public var _height : Number;
		/**
		 * @private
		 */
		private var _contentAppearance : Object;
		/**
		 * @private
		 */
		private var _visualPropList : Object;
		/**
		 * @private
		 */
		private var _triggerEvent : String;

		/**
		 * The movie clip instance to which TransitionManager is to apply a transition. * @playerversion Flash 9 * @langversion 3.0 * @keyword content, Transition
		 */
		public function set content (c:MovieClip) : void;
		public function get content () : MovieClip;
		/**
		 * @private this was changed from get transitions() to get transitionsList() because hitting compile error
		 */
		public function get transitionsList () : Object;
		/**
		 * @private
		 */
		public function get numTransitions () : Number;
		/**
		 * @private
		 */
		public function get numInTransitions () : Number;
		/**
		 * @private
		 */
		public function get numOutTransitions () : Number;
		/**
		 * An object that contains the saved visual properties of the content (target movie clip)  * to which the transitions will be applied.  * @playerversion Flash 9 * @langversion 3.0 * @keyword contentAppearance, Transition
		 */
		public function get contentAppearance () : Object;

		/**
		 * Creates a new TransitionManager instance, designates the target object, applies a transition, and starts the transition. Specifically, calling this method creates an instance of the TransitionManager class if one does not already exist, creates an instance of the specified transition class designated in the <code>transParams</code> parameter, and then starts the transition. The transition is applied to the movie clip that is designated in the <code>content</code> parameter.  * <p>For example:</p>  * <listing>  * import fl.transitions.~~;  * import fl.transitions.easing.~~;  *     * TransitionManager.start(myMovieClip, {type:Zoom, direction:Transition.IN, duration:1, easing:Bounce.easeOut});  * </listing>  * @param content The MovieClip object to which to apply the transition effect.  * @param transParams A collection of parameters that are passed within an object. The   * <code>transParams</code> object should contain a <code>type</code> parameter that indicates  * the transition effect class to be applied, followed by <code>direction</code>, <code>duration</code>,  * and <code>easing</code> parameters. In addition, you must include any parameters required by that   * transition effect class. For example, the fl.transitions.Iris transition effect class requires additional  * <code>startPoint</code> and <code>shape</code> parameters. So, in addition to the  * <code>type</code>, <code>duration</code>, and <code>easing</code> parameters that every  * transition requires, you would also add (to the <code>transParams</code> object) the   * <code>startPoint</code> and <code>shape</code> parameters that the fl.transitions.Iris   * effect requires.   * @return The Transition instance.  * @playerversion Flash 9  * @langversion 3.0  * @keyword Transition
		 */
		public static function start (content:MovieClip, transParams:Object) : Transition;
		/**
		 * Constructor function for creating a new TransitionManager instance. However, the <code>TransitionManager.start()</code> method is a more efficient way of creating and implementing a TransitionManager instance. If you do use the TransitionManager constructor function to create an instance, use the <code>new</code> operator then designate the transition properties and start the transition effect in a second step by calling the <code>TransitionManager.startTransition()</code> method.  * <p>For example:</p>  * <listing>  * import fl.transitions.~~;  * import fl.transitions.easing.~~;  *       * var myTransitionManager:TransitionManager = new TransitionManager(myMovieClip);  * myTransitionManager.startTransition({type:Zoom, direction:Transition.IN, duration:1, easing:Bounce.easeOut});  * </listing>  * @param content The MovieClip object to which to apply the transition effect.  * @playerversion Flash 9  * @langversion 3.0  * @keyword Transition   * @see #start() TransitionManager.start()  * @see #startTransition() TransitionManager.startTransition()
		 */
		function TransitionManager (content:MovieClip);
		/**
		 * Creates a transition instance and starts it. If a matching transition already exists,  * the existing transition is removed and a new transition is created and started. This method  * is used in conjunction with the constructor function.  * <p>For example:</p>  * <listing>  * import fl.transitions.~~;  * import fl.transitions.easing.~~;  *         * var myTransitionManager:TransitionManager = new TransitionManager(myMovieClip);  * myTransitionManager.startTransition({type:Zoom, direction:Transition.IN, duration:1, easing:Bounce.easeOut});  * </listing>  * <p>Alternatively, you can use the <code>TransitionManager.start()</code> method, which is  * a more efficient way of implementing a transition effect. </p>  * @param transParams A collection of parameters that are passed within an object. The   * <code>transParams</code> object should contain a <code>type</code> parameter that indicates  * the transition effect class to be applied, followed by direction, duration, and easing   * parameters. In addition, you must include any parameters required by that transition effect   * class. For example, the fl.transitions.Iris transition effect class requires additional  * <code>startPoint</code> and <code>shape</code> parameters. So, in addition to the  * <code>type</code>, <code>duration</code>, and <code>easing</code> parameters that every  * transition requires, you would also add (to the <code>transParams</code> object) the   * <code>startPoint</code> and <code>shape</code> parameters that the fl.transitions.Iris   * effect requires.   * @return The Transition instance.  * @playerversion Flash 9  * @langversion 3.0  * @keyword Transition   * @see #start() TransitionManager.start()  * @see #TransitionManager() TransitionManager constructor function
		 */
		public function startTransition (transParams:Object) : Transition;
		/**
		 * @private
		 */
		public function addTransition (trans:Transition) : Transition;
		/**
		 * @private
		 */
		public function removeTransition (trans:Transition) : Boolean;
		/**
		 * @private
		 */
		public function findTransition (transParams:Object) : Transition;
		/**
		 * @private
		 */
		public function removeAllTransitions () : void;
		/**
		 * @private
		 */
		public function saveContentAppearance () : void;
		/**
		 * @private
		 */
		public function restoreContentAppearance () : void;
		/**
		 * @private
		 */
		function transitionInDone (e:Object) : void;
		/**
		 * @private
		 */
		function transitionOutDone (e:Object) : void;
	}
}
