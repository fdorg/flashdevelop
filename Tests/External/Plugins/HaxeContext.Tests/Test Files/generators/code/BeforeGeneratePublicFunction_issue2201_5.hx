package;

class Main {
	
	private function addComponent(c:Component, e:Dynamic, c2:Component):Entity return null;
	
	public function new() {
		var p = null;
		addComponent(p = new Player(), te$(EntryPoint)st, p = new Player());
	}
}

class Player extends Component {
	public function new() {}
}

class Component {}
class Entity {}