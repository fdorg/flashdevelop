﻿using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using PluginCore;

// Information about this assembly is defined by the following attributes. 
// Change them to the information which is associated with the assembly you compile.
[assembly: AssemblyTitle("PluginCore")]
[assembly: AssemblyDescription("PluginCore For " + DistroConfig.DISTRIBUTION_NAME)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(DistroConfig.DISTRIBUTION_COMPANY)]
[assembly: AssemblyProduct("PluginCore")]
[assembly: AssemblyCopyright(DistroConfig.DISTRIBUTION_COPYRIGHT)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]
[assembly: InternalsVisibleTo("PluginCore.Tests")]