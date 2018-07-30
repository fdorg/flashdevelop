package;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.IEventDispatcher;
public class Main extends Sprite {
	public function new() {
		addEventListener(Event.ADDED, handleAdded);
	}
	
	function handleAdded(e:Event):Void {
		cast(e.currentTarget, IEventDispatcher).removeEventListener(Event.ADDED, handleAdded);
		
	}
}