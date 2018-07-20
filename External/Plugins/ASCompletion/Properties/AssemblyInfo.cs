// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using PluginCore;

// Information about this assembly is defined by the following attributes. 
// Change them to the information which is associated with the assembly you compile.
[assembly: AssemblyTitle("ASCompletion")]
[assembly: AssemblyDescription("ASCompletion Plugin For " + DistroConfig.DISTRIBUTION_NAME)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(DistroConfig.DISTRIBUTION_COMPANY)]
[assembly: AssemblyProduct("ASCompletion")]
[assembly: AssemblyCopyright(DistroConfig.DISTRIBUTION_COPYRIGHT)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[assembly: InternalsVisibleTo("ASCompletion.Tests")]
[assembly: InternalsVisibleTo("CodeRefactor.Tests")]
[assembly: InternalsVisibleTo("AS3Context.Tests")]
[assembly: InternalsVisibleTo("HaxeContext.Tests")]