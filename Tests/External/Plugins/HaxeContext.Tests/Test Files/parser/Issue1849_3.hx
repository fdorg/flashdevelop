package;

interface AsyncConnection implements IStruct<AsyncConnection, AsyncConnection> {

	function resolve( name : String ) : AsyncConnection;
	function call( params : Array<Dynamic>, ?result : Dynamic -> Void ) : Void;
	function setErrorHandler( error : Dynamic -> Void ) : Void;

}

interface IStruct<K, V> {}