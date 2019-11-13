package;
class EFoo {
	public function new(element:AudioElement) {
		var promise = untyped __js__('
			element.play();
		');
		var p:Dynamic = promise;
	}
}