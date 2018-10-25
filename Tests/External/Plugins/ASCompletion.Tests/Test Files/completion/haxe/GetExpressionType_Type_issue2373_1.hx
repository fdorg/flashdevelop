package;
@:enum abstract Issue2373Foo(Int) from Int to Int {
	var One$(EntryPoint) = 1;
	var Two = 1;
}
@:enum abstract Issue2373Bar(Int) from Int to Int {
	var One = 1;
	var Two = 1;
}