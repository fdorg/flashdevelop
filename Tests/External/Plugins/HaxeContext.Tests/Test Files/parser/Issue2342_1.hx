package;
typedef NodeHttpServer = { > NodeEventEmitter,                      
        @:overload(function(path:String,?cb:Void->Void):Void {})
        function listen(port:Int,?host:String,?cb:Void->Void):Void;
        function close(?cb:Void->Void):Void;
}      