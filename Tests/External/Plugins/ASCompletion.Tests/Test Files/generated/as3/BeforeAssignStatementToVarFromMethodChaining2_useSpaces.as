package {
	class Main {
		public function Main() {
			new String("")
            .split("").concat("abc")
            .concat([
                "a", // commentline
                "b",
                "c"
            ])
			/**
			 * commentdoc
			 */
			.concat(new Array().concat(
				["a", "b", "c"]
			))
			.concat(new Array().concat(
				[
					"a", "b", "c"
				]
			))
            .join("")
            
            .charAt(0);$(EntryPoint)
		}
	}
}