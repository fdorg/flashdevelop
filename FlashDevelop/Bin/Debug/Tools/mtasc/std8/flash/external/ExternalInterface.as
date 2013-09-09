intrinsic class flash.external.ExternalInterface {

	static var available : Boolean;
	static function addCallback( methodName : String, instance : Object, method : Function) : Boolean;
	static function call( methodName : String ) : Object;

}