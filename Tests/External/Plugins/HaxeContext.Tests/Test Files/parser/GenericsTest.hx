package;

class Test<T>
{
	public function test1(expected:T, actual:T):T
	{
	}
	
	public function test2<K>(expected:K, actual:K):K
	{
	}
}

class TestConstraint<T:Iterable<String>>
{
	public function test1(expected:T, actual:T):T
	{
	}
	
	public function test2<K:Iterable<String>>(expected:K, actual:K):K
	{
	}
}

class TestMultiple<T:(Iterable<String>, Measurable)>
{
	public function test1(expected:T, actual:T):T
	{
	}
	
	public function test2<K:(Iterable<String>, Measurable)>(expected:K, actual:K):K
	{
	}
}

class TestFullConstraint<T:Measurable, Z:(Iterable<String>, Measurable)>
{
	public function test1(expected:T, actual:Z):T
	{
	}
	
	public function test2<K:Measurable, V:(Iterable<String>, Measurable)>(expected:K, actual:V):K
	{
	}
}