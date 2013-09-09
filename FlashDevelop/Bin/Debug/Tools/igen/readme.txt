IGen 1.0.1 - Intrinsic Class Generator for ActionScript 2
Copyright (c) 2005-2007 Philippe Elsass

1. Usage:

	igen [-clean] <source> <destination>

All the classes in the <source> folder and subfolders will be converted to
intrinsic classes in the <destination> folder. Only classes that have changed
since they were last parsed will be regenerated.

If the -clean option is set, classes in the <destination> which have no
match in the <source> folder will be removed. Empty folders will be removed.

IGen supports multi-byte encodings and keeps javadoc comments (/** ... */).


2. Usage in FlashDevelop:

IGen can easily be used as a post-build command in FlashDevelop projects to
automatically update intrinsic classes of a library project.

Example:

	$(ToolsDir)\igen\igen.exe -clean classes include
