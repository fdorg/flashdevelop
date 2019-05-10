package;
class Issue2764_1 {
	var _onDrawComplete : Void->Void;
	public function draw(onComplete : Void->Void) : Void {
		if (onComplete != null)
			_onDrawComplete = onComplete;
	}
}