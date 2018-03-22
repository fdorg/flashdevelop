package;
class Foo {
	public function new() {
		var str : String = '<e attr="attribute">element1</e><e>element2</e><e>element3</e>';
		
		var xml : Xml = Xml.parse(str);
		
		var elements : Iterator<Xml> = xml.elements();
		for (e in elements) {
			e;$(EntryPoint)
		}
	}
}