intrinsic class TextFormat
{
	var font:String;
	var size:Number;
	var color:Number;
	var url:String;
	var target:String;
	var bold:Boolean;
	var italic:Boolean;
	var underline:Boolean;
	var align:String;
	var leftMargin:Number;
	var rightMargin:Number;
	var indent:Number;
	var leading:Number;
	var blockIndent:Number;
	var tabStops:Array;
	var bullet:Boolean;
	function TextFormat(font:String,size:Number,textColor:Number,
                    	bold:Boolean,italic:Boolean,underline:Boolean,
                    	url:String,window:String,align:String,
                    	leftMargin:Number,rightMargin:Number,indent:Number,leading:Number);
	function getTextExtent(text:String):Object;
}


