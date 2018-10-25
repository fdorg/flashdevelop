package;

class Main {
	var test:Player;
	
	private function addComponent(c:Component, e:Entity):Entity return null;
	
	public function new() {
		addComponent(test = new Player(), new Entity());
	}
}

class Player extends Component {
	public function new() {}
}

class Component {}
class Entity {}