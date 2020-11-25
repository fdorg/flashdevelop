package {
	import flash.display.BitmapData;
	import flash.geom.Rectangle
	public class Main {
		public function Main() {
			var rectangle:Rectangle = new Rectangle(100, 100, 0, 0)
			var bitmapData:BitmapData = new BitmapData(rectangle.width, rectangle.height);
		}
	}
}