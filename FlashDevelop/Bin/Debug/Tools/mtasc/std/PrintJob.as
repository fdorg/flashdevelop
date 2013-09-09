intrinsic class PrintJob
{
	function start():Boolean;
	function addPage(target:Object, printArea:Object, options:Object, frameNum:Number):Boolean;
	function send():Void;

	var paperWidth:Number;
	var paperHeight:Number;
	var pageWidth:Number;
	var pageHeight:Number;
	var orientation:String;
}
