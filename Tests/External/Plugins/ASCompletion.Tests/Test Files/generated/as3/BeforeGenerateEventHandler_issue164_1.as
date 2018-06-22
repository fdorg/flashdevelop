package {
	import flash.display.Sprite;
	import flash.events.Event;
	public class Main extends Sprite {
		public function Main() {
			new Sprite().addEventListener(Event.ADDED, handleAdded$(EntryPoint));
		}
	}
}