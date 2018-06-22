package {
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.IEventDispatcher;
	public class Main extends Sprite {
		public function Main() {
			new Sprite().addEventListener(Event.ADDED, handleAdded);
		}
		
		private function handleAdded(e:Event):void {
			IEventDispatcher(e.currentTarget).removeEventListener(Event.ADDED, handleAdded);
			
		}
	}
}