intrinsic class System.IME {
	static var ALPHANUMERIC_FULL : String;
	static var ALPHANUMERIC_HALF : String;
	static var CHINESE : String;
	static var JAPANESE_HIRAGANA : String;
	static var JAPANESE_KATAKANA_FULL : String;
	static var JAPANESE_KATAKANA_HALF : String;
	static var KOREAN : String;
	static var UNKNOWN : String;

	static function getEnabled() : Boolean;
	static function setEnabled(enabled:Boolean) : Boolean;
	static function getConversionMode() : String;
	static function setConversionMode(mode:String) : Boolean;
	static function setCompositionString (composition:String) : Boolean;
	static function doConversion() : Boolean;
	static function addListener(listener:Object) : Void;
	static function removeListener(listener:Object) : Boolean;
}
