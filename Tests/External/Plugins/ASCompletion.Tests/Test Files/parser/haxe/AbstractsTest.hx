package;

abstract AbstractInt(Int) {
  inline public function new(i:Int) {
    this = i;
  }
}

abstract MyAbstract(Int) from Int to Int {
  inline function new(i:Int) {
    this = i;
  }
}