package;
public class Foo {
	@:isVar public var foo(get, $(EntryPoint)set):Int;
	function get_foo() return foo;
	function set_foo(v) return foo = v;
}