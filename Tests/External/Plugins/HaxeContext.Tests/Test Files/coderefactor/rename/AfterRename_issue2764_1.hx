package;
class Issue2764_1 {
	var newName : Void->Void;
	public function draw(onComplete : Void->Void) : Void {
		if (onComplete != null)
			newName = onComplete;
	}
}