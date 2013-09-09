intrinsic class XMLNode
{
	var attributes:Object;
	var childNodes:Array;
	var firstChild:XMLNode;
	var lastChild:XMLNode;
	var nextSibling:XMLNode;
	var nodeName:String;
	var nodeType:Number;
	var nodeValue:String;
	var parentNode:XMLNode;
	var previousSibling:XMLNode;

	function XMLNode(type:Number, value:String);

	function cloneNode(deep:Boolean):XMLNode;
	function removeNode():Void;
	function insertBefore(newChild:XMLNode,insertPoint:XMLNode):Void;
	function appendChild(newChild:XMLNode):Void;
	function hasChildNodes():Boolean;
	function toString():String;

	function addTreeNodeAt(index:Number, arg1:Object, arg2:Object):XMLNode;
	function addTreeNode(arg1:Object, arg2:Object):XMLNode;
	function getTreeNodeAt(index:Number):XMLNode;
	function removeTreeNodeAt(index:Number):XMLNode;
	function removeTreeNode():XMLNode;

}

