This library project has been set up to allow you to embed code and assets in a way that
is reusable by other projects.

All your code should be placed in "src".  After a successful "Build Project", intrinsic classes
will be generated in the "include" directory as a post-build step.

Additionally, the class root will be added automatically (found at Project Properties...
Compiler Options...IncludePackages) and you should make sure to pack any other namespaces 
you create, as IncludePackages is not recursive.
