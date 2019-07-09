package tink.http;

import tink.io.StreamParser;
import tink.url.Query;
import haxe.crypto.Base64;
import haxe.io.Bytes;

using tink.CoreApi;
using StringTools;

abstract ReadonlyMap<K, V>(Map<K, V>) from Map<K, V> {

  @:arrayAccess public inline function get(key:K):V
    return this.get(key);

  @:arrayAccess public inline function exists(key:K):Bool
    return this.exists(key);

  public inline function iterator():Iterator<V>
    return this.iterator();

  public inline function keys():Iterator<K>
    return this.keys();

}

typedef Extensions = ReadonlyMap<String, String>;

class ContentType {
  public var fullType(get, never):String;
    inline function get_fullType()
      return '$type/$subtype';
      
  public var type(default, null):String = '*';
  public var subtype(default, null):String = '*';
  public var extensions(default, null):Extensions;
  public var raw(default, null):String; // TODO: figure out why can't make this private (php)
  
  function new() { 
    extensions = new Map();
  }
  
  public function toString()
    return raw;
  
  static public function ofString(s:String):ContentType {
    var ret = new ContentType();
    
    ret.raw = s;
    var parsed = (s:HeaderValue).parse();
    var value = parsed[0].value;
    switch value.indexOf('/') {
      case -1:
        ret.type = value;
      case pos:
        ret.type = value.substring(0, pos);
        ret.subtype = value.substring(pos + 1);
    }
    ret.extensions = parsed[0].extensions;
    
    return ret;
  }
}

class Header {

  var fields:Array<HeaderField>;
  
  public function new(?fields)
    this.fields = switch fields {
      case null: [];
      case v: v;
    }
    
  /**
   *  Get all headers of the given name
   *  @param name - Header name to retrieve
   *  @return Array of headers of the given name
   */
  public function get(name:HeaderName)
    return [for (f in fields) if (f.name == name) f.value];
  
  /**
   *  Get a header
   *  @param name - Header name to retrieve
   *  @return `Success(header)` if there is exactly one entry of the given header name, `Failure(err)` otherwise
   */
  public function byName(name:HeaderName)
    return switch get(name) {
      case []:
        Failure(new Error(UnprocessableEntity, headerNotFound(name)));
      case [v]:
        Success(v);
      case v: 
        Failure(new Error(UnprocessableEntity, 'Multiple entries for $name header'));
    }
    
  /**
   *  Get the content type header
   */
  public function contentType() 
    return byName(CONTENT_TYPE).map(ContentType.ofString);
    
  public inline function iterator()
    return fields.iterator();
    
  /**
   *  Clone this header with additional header fields
   *  @param fields - Header fields to be added
   *  @return Header with additional fields
   */
  public function concat(fields:Array<HeaderField>):Header
    return new Header(this.fields.concat(fields));
  
  /**
   *  Get content length. Assumes zero if content-length header is missing
   */
  public function getContentLength()
    return switch byName(CONTENT_LENGTH) {
      case Success(Std.parseInt(_) => null): Failure(new Error(UnprocessableEntity, 'Invalid Content-Length Header'));
      case Success(Std.parseInt(_) => v): Success(v);
      case Failure(e): Failure(e);
    }
    
  public function accepts(type:String) {
    var prefix = type.split('/')[0];
    return byName(ACCEPT).map(function(v) {
      for(entry in v.parse()) {
        if(entry.value == '*/*' || entry.value == type) return true;
        switch entry.value.split('/') {
          case [p, '*'] if(prefix == p): return true;
          default: // continue
        }
      }
      return false;
    });
  }

  private var LINEBREAK(get, never):String; 
    inline function get_LINEBREAK() return '\r\n';

  public function toString() 
    return [for (f in fields) f.toString()].join(LINEBREAK) + LINEBREAK + LINEBREAK;
    
  inline function headerNotFound(name)
    return 'No $name header found';
}

typedef HeaderValueComponent = {
  var value(default, never):String;
  var extension(default, null):Extensions;
}

abstract HeaderValue(String) from String to String {
  
  public function getExtension():Map<String, String>
    return parse()[0].extensions;
  
  /**
   *  Parse the value of this header in to `{value:String, extensions:Map<String, String>}` form
   */
  public function parse()
    return parseWith(function(_, params) return [for(p in params) p.name => switch p.value.toString() {
      case quoted if (quoted.charCodeAt(0) == '"'.code): quoted.substr(1, quoted.length - 2);//TODO: find out how exactly escaping and what not works
      case v: v;
    }]);
    
  /**
   *  Parse the value of this header, using the given function to parse the extensions
   *  @param parseExtension - function to parse the extension
   */
  public function parseWith<T>(parseExtension:String->Iterator<QueryStringParam>->T)
    return [for(v in this.split(',')) {
      v = v.trim();
      var i = switch v.indexOf(';') {
        case -1: v.length;
        case i: i;
      }
      var value = v.substr(0, i);
      {
        value: value,
        extensions: parseExtension(value, Query.parseString(v, ';', i + 1)),
      }
    }];
    
  public static function basicAuth(username:String, password:String)
    return 'Basic ' + Base64.encode(Bytes.ofString('$username:$password')).toString();
  
  static var DAYS = 'Sun,Mon,Tue,Wen,Thu,Fri,Sat'.split(',');
  static var MONTHS = 'Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec'.split(',');
  @:from static public function ofDate(d:Date):HeaderValue
    return DateTools.format(d, DAYS[d.getDay()] + ", %d " + MONTHS[d.getMonth()] + " %Y %H:%M:%S GMT");
  
  @:from static public function ofInt(i:Int):HeaderValue
    return Std.string(i);

  
}

@:enum abstract HeaderName(String) to String {
  
  var REFERER                          = 'referer';
  var HOST                             = 'host';

  var SET_COOKIE                       = 'set-cookie';
  var COOKIE                           = 'cookie';

  var CONTENT_TYPE                     = 'content-type';
  var CONTENT_LENGTH                   = 'content-length';
  var CONTENT_DISPOSITION              = 'content-disposition';

  var ACCEPT                           = 'accept';
  var ACCEPT_ENCODING                  = 'accept-encoding';

  var TRANSFER_ENCODING                = 'transfer-encoding';

  var LOCATION                         = 'location';
  var AUTHORIZATION                    = 'authorization';

  var ORIGIN                           = 'origin';
  var VARY                             = 'vary';
  
  var CACHE_CONTROL                    = 'cache-control';
  var EXPIRES                          = 'expires';

  var ACCESS_CONTROL_REQUEST_METHOD    = 'access-control-request-method';
  var ACCESS_CONTROL_REQUEST_HEADERS   = 'access-control-request-headers';
  var ACCESS_CONTROL_ALLOW_ORIGIN      = 'access-control-allow-origin';
  var ACCESS_CONTROL_ALLOW_CREDENTIALS = 'access-control-allow-credentials';
  var ACCESS_CONTROL_EXPOSE_HEADERS    = 'access-control-expose-headers';
  var ACCESS_CONTROL_MAX_AGE           = 'access-control-max-age';
  var ACCESS_CONTROL_ALLOW_METHODS     = 'access-control-allow-methods';
  var ACCESS_CONTROL_ALLOW_HEADERS     = 'access-control-allow-headers';
  
  inline function new(s) this = s;
  
  @:from static inline function ofString(s:String)
    return new HeaderName(s.toLowerCase());
}

#if tink_json
@:jsonParse(function(json) return tink.http.Header.HeaderField.ofString(json))
@:jsonStringify(function(field:tink.http.Header.HeaderField) return field.toString())
#end
class HeaderField extends NamedWith<HeaderName, HeaderValue> {
  
  public function toString() 
    return 
      if (value is null) name 
      else '$name: $value';//urlencode?
    
  static public function ofString(s:String)
    return switch s.indexOf(':') {
      case -1: 
        new HeaderField(s, null);
      case v: 
        var name = s.substr(0, v);
        new HeaderField(name, s.substr(v + 1).trim()); //urldecode?
    }
  
  /**
   * Constructs a Set-Cookie header. Please note that cookies are HttpOnly by default. 
   * You can opt out of that behavior by setting `options.scriptable` to true.
   */  
  static public function setCookie(key:String, value:String, ?options: { ?expires: Date, ?domain: String, ?path: String, ?secure: Bool, ?scriptable: Bool}) {
    
    if (options is null) 
      options = { };
      
    var buf = new StringBuf();
    
    inline function addPair(name, value) {
      if(value is null) return;
      buf.add("; ");
      buf.add(name);
      buf.add(value);
    }
    
    buf.add(key.urlEncode() + '=' + value.urlEncode());
    
    if (options.expires != null) 
      addPair("expires=", (options.expires:HeaderValue));
    
    addPair("domain=", options.domain);
    addPair("path=", options.path);
    
    if (options.secure) addPair("secure", "");
    if (options.scriptable != true) addPair("HttpOnly", "");
    
    return new HeaderField(SET_COOKIE, buf.toString());
  }
}

class HeaderParser<T> extends BytewiseParser<T> {
  var header:T;
  var fields:Array<HeaderField>;
  var buf:StringBuf;
  var last:Int = -1;
  
  var makeHeader:String->Array<HeaderField>->Outcome<T, Error>;
  
  public function new(makeHeader) {
    this.buf = new StringBuf();
    this.makeHeader = makeHeader;
  }
  
  static var INVALID = Failed(new Error(UnprocessableEntity, 'Invalid HTTP header'));  
        
  override function read(c:Int):ParseStep<T> 
    return
      switch [last, c] {
        case [_, -1]:
          
          nextLine();

        case ['\r'.code, '\n'.code]:
          
          nextLine();
            
        case ['\r'.code, '\r'.code]:
          
          buf.addChar(last);
          Progressed;
          
        case ['\r'.code, other]:
          
          buf.addChar(last);
          buf.addChar(other);
          last = -1;
          Progressed;
          
        case [_, '\r'.code]:
          
          last = '\r'.code;
          Progressed;
          
        case [_, other]:
          
          last = other;
          buf.addChar(other);
          Progressed;
      }
      
  function nextLine() {
    var line = buf.toString();
    
    buf = new StringBuf();
    last = -1;
    
    return
      switch line {
        case '':
          if (header is null)
            Progressed;
          else
            Done(header);
        default:
          if (header is null)
            switch makeHeader(line, fields = []) {
              case Success(null):
                Done(this.header = null);
              case Success(v): 
                this.header = v;
                Progressed;
              case Failure(e):
                Failed(e);
            }
          else {
            fields.push(HeaderField.ofString(line));
            Progressed;
          }
      }      
    }
  
}