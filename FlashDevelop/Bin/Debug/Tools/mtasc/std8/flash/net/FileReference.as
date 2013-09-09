intrinsic class flash.net.FileReference {

	var creator : String;
	var creationDate : Date;
	var modificationDate : Date;
	var size : Number;
	var type : String;
	var name : String;

	function FileReference();

	function browse( typeList : Array ) : Boolean;
	function upload( url : String ) : Boolean;
	function download( url : String, defaultName : String ) : Boolean;
	function cancel() : Void;

	function addListener( listener : Object ) : Void;
	function removeListener( listener : Object ) : Boolean;

}