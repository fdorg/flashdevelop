package;
import flash.display.Sprite;
class Issue2493_1 {
	
	var s(default, set) = new Sprite();
	
	function set_s(value:Sprite):Sprite {
		return s = value;
	}
}