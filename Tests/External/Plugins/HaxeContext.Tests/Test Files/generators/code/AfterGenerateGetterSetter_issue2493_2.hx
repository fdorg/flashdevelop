package;
import flash.display.Sprite;
class Issue2493_1 {
	
	function get_s():Sprite {
		return s;
	}
	
	var s(get, null) = new Sprite();
}