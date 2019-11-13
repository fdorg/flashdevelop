package;
class Foo {
	public function new(sprite:Sprite) {
		var children = new Array<Sprite>();
		{
			var child = sprite.owner.firstChild;
			while (child != null) {
				var sprite = child.getSprite();
				if (sprite != null) {
					children.push(sprite);
				}
				
				child = child.next;
				child;$(EntryPoint)
			}
			
			children.reverse();
		}
	}
}