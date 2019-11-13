package;

/**
 * Some typedef custom comments
 */
typedef TypedefTest
{
	///
	/// Java Style comments
	///
	var age : Int;
}

/**
 * Some enum custom comments
 */
enum EnumTest
{
	/**
	 * Enum element comments
	 */
    Foo;
}

/**
 * Some abstract custom comments
 */
abstract AbstractInt(Int) {
	///
	/// Java Style comments
	///
	inline public function new(i:Int) {
		this = i;
	}
}