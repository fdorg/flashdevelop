package;

class TestObjectConstraint<T:({}, Measurable)>
{
	public function test1(expected:T, actual:T):T
	{
	}
	
	public function test2<K:({}, Measurable)>(expected:K, actual:K):K
	{
	}
}

class TestFullConstraint<T:({}, Measurable), Z:(Iterable<String>, Measurable)>
{
	public function test1(expected:T, actual:Z):T
	{
	}
	
	public function test2<K:({}, Measurable), V:(Iterable<String>, Measurable)>(expected:K, actual:V):K
	{
	}
}

class TestTypeDefConstraint<T:({ function new():Void; }, Measurable)>
{
	public function test1(expected:T, actual:T):T
	{
	}
	
	public function test2<K:({ function new():Void; }, Measurable)>(expected:K, actual:K):K
	{
	}
}