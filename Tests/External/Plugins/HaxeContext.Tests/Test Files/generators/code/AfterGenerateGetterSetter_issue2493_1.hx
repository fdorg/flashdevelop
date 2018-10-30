package;
import flash.display.Sprite;
class Issue2493_1 {
	
	@:isVar var s(get, set) = new Sprite();
	
	function get_s():Sprite {
		return s;
	}
	
	function set_s(value:Sprite):Sprite {
		return s = value;
	}
}