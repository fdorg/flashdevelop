package;
import flash.display.Sprite;
import flash.events.Event;
public class Main extends Sprite {
	public function new() {
		addEventListener(Event.ADDED, handleAdded$(EntryPoint));
	}
}