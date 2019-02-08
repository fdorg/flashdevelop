package;

typedef Alias = Array<Int>;

typedef Iterable<T> = {
  function iterator() : Iterator<T>;
}

typedef TypeWithOptionalA = {
  var age : Int;
  var name : String;
  @:optional var phoneNumber : String;
}

typedef TypeWithOptionalB = {
    ?optionalString: String,
    requiredInt: Int
}

typedef SingleLine = { x : Int, y : Int }

typedef NormalDef = {
    var aliases:Array<String>;
    var processFunction:Dynamic;
}

typedef ShortDef = {
    aliases:Array<String>,
    processFunction:Dynamic
}

typedef WindowSettings = {
	>ShortDef,
	var level(default, never):Null<Int>;
}