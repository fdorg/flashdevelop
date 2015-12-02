# AppMan entries

If you want to add your plugin, theme or extension to AppMan, you need to do a pull request to the appman.xml file adding a new entry inside the FD5 comments. Requirements for the offered plugins, themes or extensions are:

* Code needs to be opensource and reviewable in a public repository
* The item needs to be packaged into a FDZ file and provide a MD5 checksum for verification
* The FDZ needs to extract the files to the user app data directory

# Coding style

##### Naming should be in English and clear

```
private Int32 memberProperty = 0;
```

##### Use camelCase for private members and uppercase for public properties, methods and types:

```
private Int32 memberProperty = 0;

public String MemberName = "MemberName";

public Boolean IsMember()
{
	return true;
}
```

##### Use types without explicit path:

```	
private void OnFormActivate(Object sender, /*System.*/EventArgs e)
{
	// Do something...
}
```

##### Do not use extensive extra empty lines and keep the code clean, not like:

```
// Comment...	
private Int32 memberMethod(Int32 value)
{
	
	
	
	// Comment for something...
	
	
	if (value > 2))
	{
		
		//this.oldCode = 0;
		
		
		// Random notes here
		
		
		
		return -1;
	}
	
	
	
	// Something here too...
	else return value;
	
	
	
	
	//Little bit here too...
}
```

##### Use brackets and parenthesis for easier readability:

```
if ((val1 > val2) && (val1 > val3))
{
	if (val2 > val3)
	{
		doThing();
	}
}
```

##### Do not nest expressions without brackets:

```
if (val1 > val2 && val1 > val3)

	if (val2 > val3)
	
		doThing();

```

##### Use can use one liners to shorten the code:

```
if (val1 > val2 && val1 > val3)
{
	if (val2 > val3) doThing();
}
```

##### Use explicit types:

```
Int32 myValue = 0;
Point[] myPoints = new Point[]
{
	new Point(1, 1),
	new Point(2, 2)
}
```

##### Code example:

```
import System;

namespace MyNameSpace
{
	class MyClass
	{
		// Comment here...
		private Int32 memberProperty = 0;
		private Int32 memberProperty2 = 1;
		private Int32 memberProperty3 = 2;
		
		// Comment here...
		public String MemberName = "MemberName";
		
		// Comment here...
		public static Boolean IsMemberProperty = false;
		
		// Comment here...
		public const Int32 CONSTANT = 1;
		
		/// <summary>
		/// Comment here...
		/// </summary> 
		public Boolean IsMember()
		{
			return true;
		}
		
		/// <summary>
		/// Comment here...
		/// </summary> 
		public void MemberMethod(Int32 value)
		{
			Int32 temp = MyClass.CONSTANT;
			if (value > 2))
			{
				this.memberProperty2 = temp;
				this.memberProperty3 = temp;
			}
			else this.memberProperty3 = value;
		}
		
		/// <summary>
		/// Comment here...
		/// </summary> 
		private Int32 MemberMethodEx(Int32 value)
		{
			Int32 temp = MyClass.CONSTANT;
			this.memberProperty3 = temp;
			switch (value)
			{
				case 1: return 1;
				case 2:
				{
					return -1;
				}
				default: return value;
			}
		}
		
		/// <summary>
		/// Comment here...
		/// </summary> 
		private void OnFormActivate(Object sender, EventArgs e)
		{
			this.MemberMethod(null, null);
		}

	}
	
}
```

##### More generally, be consistent and keep the code clean!
