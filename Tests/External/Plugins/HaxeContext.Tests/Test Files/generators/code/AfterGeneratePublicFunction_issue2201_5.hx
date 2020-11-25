package;

class Main {
	var test:Dynamic;
	
	private function addComponent(c:Component, e:Dynamic, c2:Component):Entity return null;
	
	public function new() {
		var p = null;
		addComponent(p = new Player(), test, p = new Player());
	}
}

class Player extends Component {
	public function new() {}
}

class Component {}
class Entity {}