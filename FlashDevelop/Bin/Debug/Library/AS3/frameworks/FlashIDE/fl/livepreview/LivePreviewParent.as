package fl.livepreview
{
	import flash.display.*;
	import flash.external.*;
	import flash.utils.*;

	/**
	 * The LivePreviewParent class provides the timeline for a SWC file 	 * or for a compiled clip that is being exported when ActionScript 3.0 	 * is selected.     *	 * <p>When a property is set on a component instance or when a component 	 * instance is resized on the Stage, Flash makes calls to the methods of 	 * this class, which in turn call methods in your component code to set 	 * the properties and to resize the component.</p>	 *	 * <p>In cases where your component must implement a specific action when 	 * it is in live preview mode, use the following code to test for live preview 	 * mode:</p>	 *	 * <listing>var isLivePreview:Boolean = (parent != null &amp;&amp; getQualifiedClassName(parent) == "fl.livepreview::LivePreviewParent");</listing>	 *	 * <p>The LivePreviewParent class supports the definition of a <code>setSize()</code> 	 * method that uses <code>width</code> and <code>height</code> values to resize 	 * a component. If you do not define a <code>setSize()</code> method, this object 	 * sets the <code>width</code> and <code>height</code> properties individually.</p>	 *	 * <p>You can also use this class to create a custom live preview SWF file without 	 * creating a SWC file; however, it is probably easier to create a component live 	 * preview file by:</p>	 * <ul>	 * <li>Exporting your component as a SWC file.</li>	 * <li>Changing the .swc file extension to .zip.</li>	 * <li>Extracting the SWF file within the ZIP file.</li>	 * </ul> 	 * <p>To create a component live preview file in this way, follow these steps:</p>	 * <ol>	 * <li>Create a new Flash document.</li>	 * <li>Set its document class to fl.livepreview.LivePreviewParent.</li> 	 * <li>Drag your component to the Stage and position it to x and y coordinates of 0.</li>	 * <li>Check to ensure that the component parameters remain at their default settings.	 * This should be the case if you drag the component from the Library panel or from the 	 * Components panel.</li>	 * <li>Select Modify &gt; Document from the main menu and, for the Match option, click Contents.</li>	 * <li>Click OK.</li>	 * <li>Publish the file to see the resulting SWF file as a custom live preview	 * SWF file.</li>     * <li>Right-click the asset in the Library panel and select Component Definition from the context menu.</li>     * <li>The Component Definition dialog box allows you to specify a custom live preview 	 * SWF file for a component.</li>	 * </ol>	 *	 * <p>In some cases, you may want to have a custom live preview SWF file that is	 * completely different from your component. See the live preview of the fl.containers.UILoader	 * component for such an example. This live preview does not use the properties of UILoader, 	 * nor does it implement getter and setter functions for these properties. It does, however,	 * implement a <code>setSize()</code> method that uses <code>width</code> and <code>height</code>	 * parameters to draw the component at the new size.</p>	 *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class LivePreviewParent extends MovieClip
	{
		/**
		 * The component instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public var myInstance : DisplayObject;

		/**
		 * Initializes the scale and align modes of the Stage, sets the 		 * <code>myInstance</code> property, resizes <code>myInstance</code> to		 * the proper size and uses the ExternalInterface class to expose 		 * functions to Flash.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function LivePreviewParent ();
		/**
		 * Resizes the component instance on the Stage to the specified		 * dimensions, either by calling a user-defined method, or by 		 * separately setting the <code>width</code> and <code>height</code> 		 * properties.		 *		 * <p>This method is called by Flash Player.</p>		 *		 * @param width The new width for the <code>myInstance</code> instance.		 * @param height The new height for the <code>myInstance</code> instance.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function onResize (width:Number, height:Number) : void;
		/**
		 * Updates the properties of the component instance.  		 * This method is called by Flash Player when there 		 * is a change in the value of a property. This method		 * updates all component properties, whether or not 		 * they were changed.		 *		 * @param updateArray An array of parameter names and values.         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		public function onUpdate (...updateArray:Array) : void;
		/**
		 * @private         *         * @langversion 3.0         * @playerversion Flash 9.0.28.0
		 */
		private function updateCollection (collDesc:Object, index:String) : void;
	}
}
