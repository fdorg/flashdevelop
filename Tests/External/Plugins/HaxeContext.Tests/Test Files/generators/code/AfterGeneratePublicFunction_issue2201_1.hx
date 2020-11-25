package;

class Main {
	var test:Player;
	
	private function addComponent(c:Component):Entity return null;
	
	public function new() {
		addComponent(test = new Player()); // declare private variable on `test`
	}
}

class Player extends Component {
	public function new() {}
}

class Component {}
class Entity {}