﻿package test;

class Test {
	/**
	 * Documentation
	 */
	static function main() {
		var myTree = Node(Leaf("foo"), Node(Leaf("bar"), Leaf("foobar")));
		switch(myTree) {
		case Leaf(_):
		{
			trace("0");
		}
		case Node(_, Leaf(_)):
			trace("1");
		case Node(_, Node(Leaf("bar"), _)):
			trace("2");
		case _:
			trace("3");
		}

		if (true) {
			trace ("test");
			trace("hello");
		}
		else if (true)
		trace("hello");
		else {
		trace("hi");
		}

#if !debug
		trace("ok");
#elseif (debug_level > 3)
		trace(3);
#else
		trace("debug level too low");
#end

		var myWrap = [1, 2, 3];
		for (elt in myWrap) {
			trace(elt);
		}
	}
}

typedef Test = {
	a : Node;
	b : Int;
	c : String;
}