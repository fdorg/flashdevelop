package fl.video
{
	import flash.display.*;

	/**
	 * @private     *     * @langversion 3.0     * @playerversion Flash 9.0.28.0
	 */
	public class ControlData
	{
		public var uiMgr : UIManager;
		public var index : int;
		public var ctrl : DisplayObject;
		public var owner : DisplayObject;
		public var enabled : Boolean;
		public var avatar : DisplayObject;
		public var state : uint;
		public var state_mc : Array;
		public var disabled_mc : DisplayObject;
		public var currentState_mc : DisplayObject;
		public var origX : Number;
		public var origY : Number;
		public var origScaleX : Number;
		public var origScaleY : Number;
		public var origWidth : Number;
		public var origHeight : Number;
		public var leftMargin : Number;
		public var rightMargin : Number;
		public var topMargin : Number;
		public var bottomMargin : Number;
		public var handle_mc : Sprite;
		public var percentage : Number;
		public var isDragging : Boolean;
		public var hit_mc : Sprite;
		public var progress_mc : DisplayObject;
		public var fullness_mc : DisplayObject;
		public var fill_mc : DisplayObject;
		public var mask_mc : DisplayObject;
		public var cachedFocusRect : Boolean;
		public var captureFocus : Boolean;

		public function ControlData (uiMgr:UIManager, ctrl:DisplayObject, owner:DisplayObject, index:int);
	}
}
