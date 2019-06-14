package;
import flash.display.Sprite;
import flash.events.Event;
public class Main extends Sprite {
	public static function main(s:Sprite) {
		s.addEventListener(Event.ADDED, handleAdded);
	}
	
	private static function handleAdded(e:Event):Void {
		
	}
}