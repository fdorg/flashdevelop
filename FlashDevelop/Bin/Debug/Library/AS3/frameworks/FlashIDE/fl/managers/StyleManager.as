package fl.managers
{
	import fl.core.UIComponent;
	import flash.display.Sprite;
	import flash.text.TextFormat;
	import flash.utils.Dictionary;
	import flash.utils.getDefinitionByName;
	import flash.utils.getQualifiedClassName;
	import flash.utils.getQualifiedSuperclassName;

	/**
	 * The StyleManager class provides static methods that can be used to get and 	 * set styles for a component instance, an entire component type, or all user 	 * interface components in a Flash document. Styles are defined as values that      * affect the display of a component, including padding, text formats, and skins.	 *	 * @includeExample examples/StyleManagerExample.as	 *	 * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class StyleManager
	{
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static var _instance : StyleManager;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var styleToClassesHash : Object;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var classToInstancesDict : Dictionary;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var classToStylesDict : Dictionary;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var classToDefaultStylesDict : Dictionary;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private var globalStyles : Object;

		/**
		 * Creates a new StyleManager object.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function StyleManager ();
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function getInstance ();
		/**
		 * Registers a component instance with the style manager. After a component instance is		 * instantiated, it can register with the style manager to be notified of changes 		 * in style. Component instances can register to receive notice of style changes that are		 * component-based or global in nature.         *		 * @param instance The component instance to be registered for style         * management.         *         * @internal Do you guys have a code snippet/test case/sample you could give us for this? (pdehaan(at)adobe.com)         * Adobe: [LM] Although this method is public, it is all handled internally by UIComponent.  Each component registers itself when it instantiates.         * @internal Should this then be (at)private in the docs?         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function registerInstance (instance:UIComponent) : void;
		/**
		 * @private		 * 		 * Sets an inherited style on a component.		 *         * @param instance The component object on which to set the inherited style.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function setSharedStyles (instance:UIComponent) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function getSharedStyle (instance:UIComponent, name:String) : Object;
		/**
		 * Gets a style that exists on a specific component.		 *		 * @param component The name of the component instance on which to find the		 *        requested style.		 *		 * @param name The name of the style to be retrieved.		 *		 * @return The requested style from the specified component. This function returns <code>null</code>          * if the specified style is not found.         *         * @see #clearComponentStyle()         * @see #getStyle()         * @see #setComponentStyle()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getComponentStyle (component:Object, name:String) : Object;
		/**
		 * Removes a style from the specified component.		 *		 * @param component The name of the component from which the style is to be removed.		 *         * @param name The name of the style to be removed.         *         * @see #clearStyle()         * @see #getComponentStyle()         * @see #setComponentStyle()         *          * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function clearComponentStyle (component:Object, name:String) : void;
		/**
		 * Sets a style on all instances of a component type, for example, on all instances of a 		 * Button component, or on all instances of a ComboBox component. 		 *		 * @param component The type of component, for example, Button or ComboBox.  This parameter also accepts		 * a component instance or class that can be used to identify all instances of a component type.		 *		 * @param name The name of the style to be set.		 *		 * @param style The style object that describes the style that is to be set.         *         * @see #clearComponentStyle()         * @see #getComponentStyle()         * @see #setStyle()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function setComponentStyle (component:Object, name:String, style:Object) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function getClassDef (component:Object) : Class;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function invalidateStyle (name:String) : void;
		/**
		 * @private (protected)         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private static function invalidateComponentStyle (componentClass:Class, name:String) : void;
		/**
		 * Sets a global style for all user interface components in a document.		 *		 * @param name A String value that names the style to be set.		 *		 * @param style The style object to be set. The value of this property depends on the 		 * style that the user sets. For example, if the style is set to "textFormat", the style 		 * property should be set to a TextFormat object. A mismatch between the style name and		 * the value of the style property may cause the component to behave incorrectly.         *         * @see #clearStyle()         * @see #getStyle()         * @see #setComponentStyle()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function setStyle (name:String, style:Object) : void;
		/**
		 * Removes a global style from all user interface components in a document.		 *         * @param name The name of the global style to be removed.         *         * @see #clearComponentStyle()         * @see #getStyle()         * @see #setStyle()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function clearStyle (name:String) : void;
		/**
		 * Gets a global style by name.		 *		 * @param name The name of the style to be retrieved.		 *		 * @return The value of the global style that was retrieved.         *		 * @internal "that was removed" - doesn't sound right. Do you guys have a code snippet/test 		 *         case/sample you could give us for this? (rberry(at)adobe.com)         * Adobe: [LM] Correct - description was wrong.  Code sample would be simple: {var textFormat:TextFormat = StyleManager.getStyle("textFormat") as TextFormat;}         *         * @see #clearStyle()         * @see #getComponentStyle()         * @see #setStyle()         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public static function getStyle (name:String) : Object;
	}
}
