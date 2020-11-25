/* Same license as Node.js

	Maintainer: Dion Amago, dion@transition9.com

	Copied from Ritchie Turner's (blackdog@cloudshift.cl) repo due to a lack of contact with Ritchie and a desire to
	keep node.js libraries updated

	From Ritchie's docs:
	"Node.js 0.8 api without haXe embellishments so that other apis may be implemented
	on top without being hindered by design choices here.
	Domain not added."
*/

package js;

typedef NodeListener = Dynamic;
typedef NodeErr = Null<String>;

/*
	 emits: newListener
 */
typedef NodeEventEmitter = {
	function addListener(event:String,fn:NodeListener):Dynamic;
	function on(event:String,fn:NodeListener):Dynamic;
	function once(event:String,fn:NodeListener):Void;
	function removeListener(event:String,listener:NodeListener):Void;
	function removeAllListeners(event:String):Void;
	function listeners(event:String):Array<NodeListener>;
	function setMaxListeners(m:Int):Void;
	function emit(event:String,?arg1:Dynamic,?arg2:Dynamic,?arg3:Dynamic):Void;
}

typedef NodeWatchOpt = {
	var persistent:Bool;
	@:optional
	var interval:Int;
};

typedef NodeExecOpt = {
	var encoding:String;
	var timeout:Int;
	var maxBuffer:Int;
	var killSignal:String;
	var env:Dynamic;
	var cwd:String;
}

typedef NodeSpawnOpt = {
	var cwd:String;
	var env:Dynamic;
	var customFds:Array<Int>;
	var setsid:Bool;
}

/* note:can't spec multiple optional args, so adding an arbitrary 3 */
typedef NodeConsole = {
	function log(s:String,?a1:Dynamic,?a2:Dynamic,?a3:Dynamic):Void;
	function info(s:String,?a1:Dynamic,?a2:Dynamic,?a3:Dynamic):Void;
	function warn(s:String,?a1:Dynamic,?a2:Dynamic,?a3:Dynamic):Void;
	function error(s:String,?a1:Dynamic,?a2:Dynamic,?a3:Dynamic):Void;
	function time(label:String):Void;
	function timeEnd(label:String):Void;
	function dir(obj:Dynamic):Void;
	function trace():Void;
	function assert():Void;
}

typedef NodePath = {
	function join(?p1:String,?p2:String,?p3:String):String;
	function normalize(p:String):String;
	function resolve(?from:Array<String>,to:String):String;
	function dirname(p:String):String;
	function basename(p:String,?ext:String):String;
	function extname(p:String):String;
	var sep :String;
	var delimiter :String;
}

typedef NodeUrlObj = {
	var href:String;
	var host:String;
	var protocol:String;
	var auth:String;
	var hostname:String;
	var port:String;
	var pathname:String;
	var search:String;
	var query:Dynamic;
	var hash:String;
}

typedef NodeUrl = {
	function parse(p:String,?andQueryString:Bool):NodeUrlObj;
	function format(o:NodeUrlObj):String;
	function resolve(from:Array<String>,to:String):String;
}

typedef NodeQueryString = {
	function parse(s:String,?sep:String,?eq:String,?options:{maxKeys:Int}):Dynamic;
	function escape(s:String):String;
	function unescape(s:String):String;
	function stringify(obj:Dynamic,?sep:String,?eq:String):String;
}

@:native("Buffer") extern class NodeBuffer implements ArrayAccess<Int> {

	@:overload(function(str:String,?enc:String):Void {})
	@:overload(function(arr:Array<Int>):Void {})
	function new(size:Int):Void;

	var length(default,null) : Int;
	var INSPECT_MAX_BYTES:Int;

	function copy(targetBuffer:NodeBuffer,targetStart:Int,sourceStart:Int,sourceEnd:Int):Void;
	function slice(start:Int,end:Int):NodeBuffer;
	function write(s:String,?offset:Int,?length:Int,?enc:String):Int;
	function toString(enc:String,?start:Int,?end:Int):String;
	function fill(value:Float,offset:Int,?end:Int):Void;
	static function isBuffer(o:Dynamic):Bool;
	static function byteLength(s:String,?enc:String):Int;
	static function concat(list:Array<NodeBuffer>, ?totalLength:Float):NodeBuffer;

	function readUInt8(offset:Int,?noAssert:Bool):Int;
	function readUInt16LE(offset:Int,?noAssert:Bool):Int;
	function readUInt16BE(offset:Int,?noAssert:Bool):Int;
	function readUInt32LE(offset:Int,?noAssert:Bool):Int;
	function readUInt32BE(offset:Int,?noAssert:Bool):Int;

	function readInt8(offset:Int,?noAssert:Bool):Int;
	function readInt16LE(offset:Int,?noAssert:Bool):Int;
	function readInt16BE(offset:Int,?noAssert:Bool):Int;
	function readInt32LE(offset:Int,?noAssert:Bool):Int;
	function readInt32BE(offset:Int,?noAssert:Bool):Int;

	function readFloatLE(offset:Int,?noAssert:Bool):Float;
	function readFloatBE(offset:Int,?noAssert:Bool):Float;
	function readDoubleLE(offset:Int,?noAssert:Bool):Float; // is this right?
	function readDoubleBE(offset:Int,?noAssert:Bool):Float; // is this right?

	function writeUInt8(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeUInt16LE(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeUInt16BE(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeUInt32LE(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeUInt32BE(value:Int,offset:Int,?noAssert:Bool):Void;

	function writeInt8(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeInt16LE(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeInt16BE(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeInt32LE(value:Int,offset:Int,?noAssert:Bool):Void;
	function writeInt32BE(value:Int,offset:Int,?noAssert:Bool):Void;

	function writeFloatLE(value:Float,offset:Int,?noAssert:Bool):Void;
	function writeFloatBE(value:Float,offset:Int,?noAssert:Bool):Void;
	function writeDoubleLE(value:Float,offset:Int,?noAssert:Bool):Void; // is this right?
	function writeDoubleBE(value:Float,offset:Int,?noAssert:Bool):Void; // is this right?
}

typedef NodeScript = {
	function runInThisContext():Dynamic;
	function runInNewContext(?sandbox:Dynamic):Void;
}

typedef NodeVM = {
	function runInThisContext(code:String,?fileName:String):Dynamic;
	function runInNewContext(?sandbox:Dynamic):Void;
	function createScript(code:Dynamic,?fileName:String):NodeScript;
}

typedef ReadStreamOpt = {
	flags:String,
	encoding:String,
	fd:Null<Int>,
	mode:Int,
	bufferSize:Int,
	?start:Int,
	?end:Int
}

typedef WriteStreamOpt = {
	var flags:String;
	var encoding:String;
	var mode:Int;
}

/*
	 Emits:
	 data,end,error,close
*/
typedef NodeReadStream = { > NodeEventEmitter,
	var readable:Bool;
	function pause():Void;
	function resume():Void;
	function destroy():Void;
	function destroySoon():Void;
	function setEncoding(enc:String):Void;
	function pipe(dest:NodeWriteStream,?opts:{end:Bool}):Void;
}

/*
	 Emits:
	 drain,error,close,pipe
*/
typedef NodeWriteStream = { > NodeEventEmitter,
	var writeable:Bool;
	@:overload(function(chunk:NodeBuffer):Bool {})
	function write(d:String,?enc:String,?fd:Int):Bool;
	@:overload(function(b:NodeBuffer):Void {})
	function end(?s:String,?enc:String):Void;
	function destroy():Void;
	function destroySoon():Void;
}

typedef NodeNetworkInterface = {
	var address :String;
	var family :String;
	var internal :Bool;
}

typedef NodeOs = {
	function hostname():String;
	function type():String;
	function release():String;
	function uptime():Int;
	function loadavg():Array<Float>;
	function totalmem():Int;
	function freemem():Int;
	function cpus():Int;
	function platform():String;
	function arch():String;
	function networkInterfaces():Dynamic;
}


typedef NodeJsDate = {
		function getTime():Int;
		function toDateString():String;
		function toUTCString():String;
}

typedef NodeStat = {
	var dev:Int;
	var ino:Int;
	var mode:Int;
	var nlink:Int;
	var uid:Int;
	var gid:Int;
	var rdev:Int;
	var size:Int;
	var blkSize:Int;
	var blocks:Int;
	var atime:NodeJsDate;
	var mtime:NodeJsDate;
	var ctime:NodeJsDate;

	function isFile():Bool;
	function isDirectory():Bool;
	function isBlockDevice():Bool;
	function isCharacterDevice():Bool;
	function isSymbolicLink():Bool;
	function isFIFO():Bool;
	function isSocket():Bool;
}

/*
	Emits: error,change
 */
typedef NodeFSWatcher = { > NodeEventEmitter,
	 function close():Void;
}

typedef NodeFsFileOptions = {
	@:optional var encoding :String;
	@:optional var flag :String;
}

typedef NodeFS = {
	function rename(from:String,to:String,cb:NodeErr->Void):Void;
	function renameSync(from:String,to:String):Void;

	function stat(path:String,cb:NodeErr->NodeStat->Void):Void;
	function statSync(path:String):NodeStat;

	function lstat(path:Dynamic,cb:NodeErr->NodeStat->Void):Void;
	function lstatSync(path:String):NodeStat;

	function fstat(fd:Int,cb:NodeErr->NodeStat->Void):Void;
	function fstatSync(fd:Int):NodeStat;

	function link(srcPath:String,dstPath:String,cb:NodeErr->Void):Void;
	function linkSync(srcPath:String,dstPath:String):Void;

	function unlink(path:String,cn:NodeErr->Void):Void;
	function unlinkSync(path:String):Void;

	function symlink(linkData:Dynamic,path:String,?type:String,?cb:NodeErr->Void):Void;
	function symlinkSync(linkData:Dynamic,path:String,?type:String):Void;

	function readlink(path:String,cb:NodeErr->String->Void):Void;
	function readlinkSync(path:String):String;

	function realpath(path:String,cb:NodeErr->String->Void):Void;
	function realpathSync(path:String):String;

	function chmod(path:String,mode:Int,cb:NodeErr->Void):Void;
	function chmodSync(path:String,?mode:Int):Void;

	function fchmod(fd:Int,mode:Int,cb:NodeErr->Void):Void;
	function fchmodSync(fd:Int,?mode:Int):Void;

	function chown(path:String,uid:Int,gid:Int,cb:NodeErr->Void):Void ;
	function chownSync(path:String,uid:Int,gid:Int):Void;

	function fchown(fd:Int,uid:Int,gid:Int,cb:NodeErr->Void):Void ;
	function fchownSync(fd:Int,uid:Int,gid:Int):Void;

	function rmdir(path:String,cb:NodeErr->Void):Void;
	function rmdirSync(path:String):Void;

	function mkdir(path:String,?mode:Int,?cb:NodeErr->Void):Void;
	function mkdirSync(path:String,?mode:Int):Void;

	function readdir(path:String,cb:NodeErr->Array<String>->Void):Void;
	function readdirSync(path:String):Array<String>;

	function close(fd:Int,cb:NodeErr->Void):Void;
	function closeSync(fd:Int):Void;

	function open(path:String,flags:String,?mode:Int,cb:NodeErr->Int->Void):Void;

	function openSync(path:String,flags:String,?mode:Int):Int;

	function write(fd:Int,bufOrStr:Dynamic,offset:Int,length:Int,position:Null<Int>,?cb:NodeErr->Int->Void):Void;
	function writeSync(fd:Int,bufOrStr:Dynamic,offset:Int,length:Int,position:Null<Int>):Int;

	function read(fd:Int,buffer:NodeBuffer,offset:Int,length:Int,position:Int,cb:NodeErr->Int->NodeBuffer->Void):Void;
	function readSync(fd:Int,buffer:NodeBuffer,offset:Int,length:Int,position:Int):Int;

	function truncate(fd:Int,len:Int,cb:NodeErr->Void):Void;
	function truncateSync(fd:Int,len:Int):NodeErr;

	@:overload(function(path:String,cb:NodeErr->String->Void):Void {})
	function readFile(path:String,?options:NodeFsFileOptions,cb:NodeErr->Dynamic->Void):Void;
	function readFileSync(path:String,?options:NodeFsFileOptions):Dynamic;

	@:overload(function(fileName:String,data:NodeBuffer,cb:NodeErr->Void):Void {})
	function writeFile(fileName:String,contents:String,?enc:String,cb:NodeErr->Void):Void;
	@:overload(function(fileName:String,data:NodeBuffer):Void {})
	function writeFileSync(fileName:String,contents:String,?enc:String):Void;

	@:overload(function(fileName:String,data:NodeBuffer,cb:NodeErr->Void):Void {})
	function appendFile(fileName:String,contents:String,?enc:String,cb:NodeErr->Void):Void;

	@:overload(function(fileName:String,data:NodeBuffer):Void {})
	function appendFileSync(fileName:String,contents:String,?enc:String):Void;


	function utimes(path:String,atime:Dynamic,mtime:Dynamic,cb:NodeErr->Void):Void;
	function utimeSync(path:String,atime:Dynamic,mtime:Dynamic):Void;

	function futimes(fd:Int,atime:Dynamic,mtime:Dynamic,cb:NodeErr->Void):Void;
	function futimeSync(fd:Int,atime:Dynamic,mtime:Dynamic):Void;

	function fsync(fd:Int,cb:NodeErr->Void):Void;
	function fsyncSync(fd:Int):Void;

	function watchFile(fileName:String,?options:NodeWatchOpt,listener:NodeStat->NodeStat->Void):Void;
	function unwatchFile(fileName:String):Void;
	function watch(fileName:String,?options:NodeWatchOpt,listener:String->String->Void):NodeFSWatcher;
	function createReadStream(path:String,?options:ReadStreamOpt):NodeReadStream;
	function createWriteStream(path:String,?options:WriteStreamOpt):NodeWriteStream;

	function exists(p:String,cb:Bool->Void):Void;
	function existsSync(p:String):Bool;
}

typedef NodeUtil = {
	function debug(s:String):Void;
	function inspect(o:Dynamic,?showHidden:Bool,?depth:Int):Void;
	function log(s:String):Void;
	function pump(rs:NodeReadStream,ws:NodeWriteStream,cb:Dynamic->Void):Void;
	function inherits(constructor:Dynamic,superConstructor:Dynamic):Void;
	function isArray(o:Dynamic):Bool;
	function isRegExp(o:Dynamic):Bool;
	function isDate(o:Dynamic):Bool;
	function isError(o:Dynamic):Bool;
	function format(out:String,?a1:Dynamic,?a2:Dynamic,?a3:Dynamic):Void; // should be arbitrary # of args
}

/*
	Emits:
	exit, uncaughtException + SIGNAL events (SIGINT etc)
 */
typedef NodeProcess = { > NodeEventEmitter,
	var stdout:NodeWriteStream;
	var stdin:NodeReadStream;
	var stderr:NodeWriteStream;
	var argv:Array<String>;
	var env:Dynamic;
	var pid:Int;
	var title:String;
	var arch:String;
	var platform:String;
	var installPrefix:String;
	var execPath:String;
	var version:String;
	var versions:Dynamic;

	function memoryUsage():{rss:Int,vsize:Int,heapUsed:Int,heapTotal:Int};
	function nextTick(fn:Void->Void):Void;
	function exit(code:Int):Void;
	function cwd():String;
	function getuid():Int;
	function getgid():Int;
	function setuid(u:Int):Void;
	function setgid(g:Int):Void;
	function umask(?m:Int):Int;
	function chdir(d:String):Void;
	function kill(pid:Int,?signal:String):Void;
	function uptime():Int;
	function abort():Void;
	function hrtime():Array<Int>;
}

/*
	Emits: exit,close
*/
typedef NodeChildProcess = { > NodeEventEmitter,
	var stdin:NodeWriteStream;
	var stdout:NodeReadStream;
	var stderr:NodeReadStream;
	var pid:Int;
	function kill(signal:String):Void;
}

/*
	Emits: message
*/
typedef NodeChildForkProcess = { > NodeChildProcess,
	 @:overload(function(o:Dynamic,?socket:NodeNetSocket):Void {})
	 function send(o:Dynamic,?server:NodeNetServer):Void;
}

typedef NodeChildProcessCommands = {
	function spawn(command: String,args: Array<String>,?options: Dynamic ) : NodeChildProcess;
	function exec(command: String,?options:Dynamic,cb: {code:Int}->String->String->Void ): NodeChildProcess;
	function execFile(command: String,?options:Dynamic,cb: {code:Int}->String->String->Void ): NodeChildProcess;
	function fork(path:String,?args:Dynamic,?options:Dynamic):NodeChildForkProcess;
}

typedef NodeClusterSettings = {
	var exec:String;
	var args:Array<String>;
	var silent:Bool;
}


/* emits: message, online,listening,disconnect,exit, setup */
typedef NodeWorker = { > NodeEventEmitter,
	var uniqueID:String; // indexes into cluster.workers
	var process:NodeChildProcess;
	var suicide:Bool;
	function send(message:Dynamic,?sendHandle:Dynamic):Void;
	function destroy():Void;
}

/* Emits: death,message, fork, online, listening	*/
typedef NodeCluster = { > NodeEventEmitter,
	var isMaster:Bool;
	var isWorker:Bool;
	var workers:Array<NodeWorker>;
	function fork(?env:Dynamic):NodeWorker;
	function send(o:Dynamic):Void;
	function setupMaster(?settings:NodeClusterSettings):Void;
	function disconnect(?cb:Void->Void):Void;
}


/* NET ............................................. */

/*
	 Emits:
	 connection
*/
typedef NodeNet = { > NodeEventEmitter,
	function createServer(?options:{allowHalfOpen:Bool},fn:NodeNetSocket->Void):NodeNetServer;
	@:overload(function(cs:String):NodeNetSocket {})
	function createConnection(port:Int,host:String):NodeNetSocket;
	@:overload(function(cs:String):NodeNetSocket { } )
	@:overload(function(port:Int, host:String, cb:Void->Void):NodeNetSocket {} )
	function connect(port:Int,host:String):NodeNetSocket;
	function isIP(input:String):Int; // 4 or 6
	function isIPv4(input:String):Bool;
	function isIPv6(input:String):Bool;
}

/*
	 Emits:
	 connection,close,error,listening
*/
typedef NodeNetServer = { > NodeEventEmitter,
	var maxConnections:Int;
	var connections:Int;

	@:overload(function(path:String,?cb:Void->Void):Void {})
	@:overload(function(fd:Int,?cb:Void->Void):Void {})
	function listen(port:Int,?host:String,?cb:Void->Void):Void;
	function close(cb:Void->Void):Void;
	function address():Void;
	function pause(msecs:Int):Void;
}

typedef NodeConnectionOpt = {
	port:Int,
	?host:String,
	?localAddress:String
}

/*

	Emits:
	connect,data,end,timeout,drain,error,close

	implements a duplex stream interface
*/
typedef NodeNetSocket = { > NodeEventEmitter,
	var remoteAddress:String;
	var remotePort:Int;
	var bufferSize:Int;
	var bytesRead:Int;
	var bytesWritten:Int;

	@:overload(function(path:String,?cb:Void->Void):Void {})
	@:overload(function(options:NodeConnectionOpt,connectionListener:Void->Void):Void {})
	function connect(port:Int,?host:String,?cb:Void->Void):Void;
	function setEncoding(enc:String):Void;
	function setSecure():Void;
	@:overload(function(data:Dynamic,?enc:String,?fileDesc:Int,?cb:Void->Void):Bool {})
	function write(data:Dynamic,?enc:String,?cb:Void->Void):Bool;
	function end(?data:Dynamic,?enc:String):Void;
	function destroy():Void;
	function pause():Void;
	function resume():Void;
	function setTimeout(timeout:Int,?cb:Void->Void):Void;
	function setNoDelay(?noDelay:Bool):Void;
	function setKeepAlive(enable:Bool,?delay:Int):Void;
	function address():{address:String,port:Int};
}

/* HTTP ............................................*/


/*
	 Emits:
	 data,end,close
 */
typedef NodeHttpServerReq = { > NodeReadStream,
	var method:String;
	var url:String;
	var headers:Dynamic;
	var trailers:Dynamic;
	var httpVersion:String;
	var connection:NodeNetSocket;
}

/*
 */
typedef NodeHttpServerResp = { > NodeWriteStream,
	var statusCode:Int;
	function writeContinue():Void;
	@:overload(function(statusCode:Int,?reasonPhrase:String,?headers:Dynamic):Void {})
	function writeHead(statusCode:Int,headers:Dynamic):Void;
	function setHeader(name:String,value:Dynamic):Void;
	function getHeader(name:String):Dynamic;
	function removeHeader(name:String):Void;
	function addTrailers(headers:Dynamic):Void;
}

/* Emits:
	 continue,response
*/
typedef NodeHttpClientReq = { > NodeWriteStream,
}

/* Emits:
	 data,end,close
*/
typedef NodeHttpClientResp = { > NodeReadStream,
	var statusCode:Int;
	var httpVersion:String;
	var headers:Dynamic;
	var client:NodeHttpClient;
}


typedef NodeHttpClient = { > NodeEventEmitter,
	function request(method:String,path:String,?headers:Dynamic):NodeHttpClientReq;
	function verifyPeer():Bool;
	function getPeerCertificate():NodePeerCert;
}

/*
	 Emits:
	 request,connection,checkContinue,connect,clientError,close
 */
typedef NodeHttpServer = { > NodeEventEmitter,
	@:overload(function(path:String,?cb:Void->Void):Void {})
	function listen(port:Int,?host:String,?cb:Void->Void):Void;
	function close(?cb:Void->Void):Void;
}