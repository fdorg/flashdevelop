package {
	public class Main {}
}

class Foo extends Object {
	AS3 function hasOwnProperty(V:* = null):Boolean {
		return super.hasOwnProperty(V);
	}
}

class Object {
	AS3 function hasOwnProperty (V:*=null) : Boolean {}
}