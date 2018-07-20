using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FDBuild")]
[assembly: AssemblyDescription("Command-Line compiler for FlashDevelop project files.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(PluginCore.DistroConfig.DISTRIBUTION_COMPANY)]
[assembly: AssemblyProduct("FDBuild")]
[assembly: AssemblyCopyright(PluginCore.DistroConfig.DISTRIBUTION_COPYRIGHT)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This is text that goes after "<program-name/> [options]" in help output.
[assembly: Mono.UsageComplement("<.fd project file>")]

// Attributes visible in "<program-name/> -V"
[assembly: Mono.About("")]
[assembly: Mono.Author(PluginCore.DistroConfig.DISTRIBUTION_COMPANY)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
