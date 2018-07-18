package;

class Main {
	var test:EReg;
	
	private function addComponent(c:Component, e:Dynamic):Entity return null;
	
	public function new() {
		var p = null;
		addComponent(p = new Player(), test = ~/,/);
	}
}

class Player extends Component {
	public function new() {}
}

class Component {}
class Entity {}