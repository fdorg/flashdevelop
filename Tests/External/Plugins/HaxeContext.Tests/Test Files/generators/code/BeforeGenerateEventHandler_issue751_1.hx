package;
import js.Browser;
class Main {
	public function main() {
		var a = Browser.document.createAnchorElement();
		a.addEventListener("click", myClick$(EntryPoint)Handler);
	}
}