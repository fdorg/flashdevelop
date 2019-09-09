// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;

namespace Mono.GetOptions
{
    using Mono;
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class OptionList
    {
        // Methods
        public OptionList(Options optionBundle)
        {
            this.optionBundle = null;
            appTitle = "Add a [assembly: AssemblyTitle(\"Here goes the application name\")] to your assembly";
            appCopyright = "Add a [assembly: AssemblyCopyright(\"(c)200n Here goes the copyright holder name\")] to your assembly";
            appDescription = "Add a [assembly: AssemblyDescription(\"Here goes the short description\")] to your assembly";
            appAboutDetails = "Add a [assembly: Mono.About(\"Here goes the short about details\")] to your assembly";
            appUsageComplement = "Add a [assembly: Mono.UsageComplement(\"Here goes the usage clause complement\")] to your assembly";
            list = new List<OptionDetails>();
            arguments = new List<string>();
            argumentsTail = new List<string>();
            argumentProcessor = null;
            HasSecondLevelHelp = false;
            bannerAlreadyShown = false;
            Initialize(optionBundle);
        }

        private void AddArgumentProcessor(MemberInfo memberInfo)
        {
            if (argumentProcessor != null)
            {
                throw new NotSupportedException("More than one argument processor method found");
            }
            if ((memberInfo.MemberType != MemberTypes.Method) || !(memberInfo is MethodInfo))
            {
                throw new NotSupportedException("Argument processor marked member isn't a method");
            }
            if (((MethodInfo) memberInfo).ReturnType.FullName != typeof(void).FullName)
            {
                throw new NotSupportedException("Argument processor method must return 'void'");
            }
            ParameterInfo[] infoArray1 = ((MethodInfo) memberInfo).GetParameters();
            if (((infoArray1 is null) || (infoArray1.Length != 1)) || (infoArray1[0].ParameterType.FullName != typeof(string).FullName))
            {
                throw new NotSupportedException("Argument processor method must have a string parameter");
            }
            argumentProcessor = (MethodInfo) memberInfo;
        }

        internal WhatToDoNext DoAbout()
        {
            ShowAbout();
            return WhatToDoNext.AbandonProgram;
        }

        internal WhatToDoNext DoHelp()
        {
            ShowHelp(false);
            return WhatToDoNext.AbandonProgram;
        }

        internal WhatToDoNext DoHelp2()
        {
            ShowHelp(true);
            return WhatToDoNext.AbandonProgram;
        }

        internal WhatToDoNext DoUsage()
        {
            ShowUsage();
            return WhatToDoNext.AbandonProgram;
        }

        public string[] ExpandResponseFiles(string[] args)
        {
            var list1 = new List<string>();
            var textArray1 = args;
            foreach (var text1 in textArray1)
            {
                if (text1.StartsWith("@", StringComparison.Ordinal))
                {
                    try
                    {
                        string text2;
                        StreamReader reader1 = new StreamReader(text1.Substring(1));
                        while ((text2 = reader1.ReadLine()) != null)
                        {
                            list1.AddRange(text2.Split());
                        }
                        reader1.Close();
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("Could not find response file: " + text1.Substring(1));
                    }
                    catch (Exception exception2)
                    {
                        Console.WriteLine("Error trying to read response file: " + text1.Substring(1));
                        Console.WriteLine(exception2.Message);
                    }
                }
                else
                {
                    list1.Add(text1);
                }
            }
            return list1.ToArray();
        }

        private object[] GetAssemblyAttributes(Type type) => entry.GetCustomAttributes(type, false);

        private string[] GetAssemblyAttributeStrings(Type type)
        {
            object[] objArray1 = GetAssemblyAttributes(type);
            if ((objArray1 is null) || (objArray1.Length == 0))
            {
                return new string[0];
            }
            int num1 = 0;
            string[] textArray1 = new string[objArray1.Length];
            object[] objArray2 = objArray1;
            foreach (var obj1 in objArray2)
            {
                textArray1[num1++] = obj1.ToString();
            }
            return textArray1;
        }

        private void GetAssemblyAttributeValue(Type type, ref string var)
        {
            object[] objArray1 = GetAssemblyAttributes(type);
            if (objArray1 != null && objArray1.Length > 0)
            {
                var = objArray1[0].ToString();
            }
        }

        private void GetAssemblyAttributeValue(Type type, string propertyName, ref string var)
        {
            object[] objArray1 = GetAssemblyAttributes(type);
            if (objArray1 != null && objArray1.Length > 0)
            {
                var = (string) type.InvokeMember(propertyName, BindingFlags.GetProperty | (BindingFlags.GetField | (BindingFlags.Public | BindingFlags.Instance)), null, objArray1[0], new object[0]);
            }
        }

        private static int IndexOfAny(string where, params char[] what) => where.IndexOfAny(what);

        private void Initialize(Options optionBundle)
        {
            entry = Assembly.GetEntryAssembly();
            appExeName = entry.GetName().Name;
            appVersion = entry.GetName().Version.ToString();
            this.optionBundle = optionBundle ?? throw new ArgumentNullException(nameof(optionBundle));
            parsingMode = optionBundle.ParsingMode;
            breakSingleDashManyLettersIntoManyOptions = optionBundle.BreakSingleDashManyLettersIntoManyOptions;
            endOptionProcessingWithDoubleDash = optionBundle.EndOptionProcessingWithDoubleDash;
            GetAssemblyAttributeValue(typeof(AssemblyTitleAttribute), "Title", ref appTitle);
            GetAssemblyAttributeValue(typeof(AssemblyCopyrightAttribute), "Copyright", ref appCopyright);
            GetAssemblyAttributeValue(typeof(AssemblyDescriptionAttribute), "Description", ref appDescription);
            GetAssemblyAttributeValue(typeof(AboutAttribute), ref appAboutDetails);
            GetAssemblyAttributeValue(typeof(UsageComplementAttribute), ref appUsageComplement);
            appAuthors = GetAssemblyAttributeStrings(typeof(AuthorAttribute));
            if (appAuthors.Length == 0)
            {
                appAuthors = new[] { "Add one or more [assembly: Mono.GetOptions.Author(\"Here goes the author name\")] to your assembly" } ;
            }
            MemberInfo[] infoArray1 = optionBundle.GetType().GetMembers();
            foreach (var info1 in infoArray1)
            {
                object[] objArray1 = info1.GetCustomAttributes(typeof(OptionAttribute), true);
                if (objArray1.Length > 0)
                {
                    OptionDetails details1 = new OptionDetails(info1, (OptionAttribute) objArray1[0], optionBundle);
                    list.Add(details1);
                    HasSecondLevelHelp = HasSecondLevelHelp || details1.SecondLevelHelp;
                }
                else
                {
                    objArray1 = info1.GetCustomAttributes(typeof(ArgumentProcessorAttribute), true);
                    if (objArray1.Length > 0)
                    {
                        AddArgumentProcessor(info1);
                    }
                }
            }
        }

        internal bool MaybeAnOption(string arg)
        {
            return (((parsingMode & OptionsParsingMode.Windows) > 0) && (arg[0] == '/')) || (((parsingMode & OptionsParsingMode.Linux) > 0) && (arg[0] == '-'));
        }

        public string[] NormalizeArgs(string[] args)
        {
            var flag1 = true;
            var list1 = new List<string>();
            var textArray1 = ExpandResponseFiles(args);
            foreach (var text1 in textArray1)
            {
                if (text1.Length > 0)
                {
                    if (flag1)
                    {
                        if (endOptionProcessingWithDoubleDash && (text1 == "--"))
                        {
                            flag1 = false;
                        }
                        else if (((parsingMode & OptionsParsingMode.Linux) > 0) && (text1[0] == '-') && ((text1.Length > 1) && (text1[1] != '-')) && breakSingleDashManyLettersIntoManyOptions)
                        {
                            CharEnumerator enumerator1 = text1.Substring(1).GetEnumerator();
                            while (true)
                            {
                                if (!enumerator1.MoveNext())
                                {
                                    break;
                                }
                                char ch1 = enumerator1.Current;
                                list1.Add("-" + ch1);
                            }
                        }
                        else
                        {
                            if (!MaybeAnOption(text1))
                            {
                                goto Label_014D;
                            }
                            char[] chArray1 = new[] { ':', '=' } ;
                            int num1 = IndexOfAny(text1, chArray1);
                            if (num1 < 0)
                            {
                                list1.Add(text1);
                                goto Label_0155;
                            }
                            list1.Add(text1.Substring(0, num1));
                            list1.Add(text1.Substring(num1 + 1));
                        }
                    }
                    else
                    {
                        argumentsTail.Add(text1);
                    }
                }
                goto Label_0155;
                Label_014D:
                list1.Add(text1);
                Label_0155:;
            }
            return list1.ToArray();
        }

        public string[] ProcessArgs(string[] args)
        {
            list.Sort();
            args = NormalizeArgs(args);
            try
            {
                int num1 = args.Length;
                for (int num2 = 0; num2 < num1; num2++)
                {
                    string text1;
                    string text2 = args[num2];
                    if ((num2 + 1) < num1)
                    {
                        text1 = args[num2 + 1];
                    }
                    else
                    {
                        text1 = null;
                    }
                    bool flag1 = false;
                    if ((text2.Length <= 1) || (!text2.StartsWith("-", StringComparison.Ordinal) && !text2.StartsWith("/", StringComparison.Ordinal)))
                    {
                        goto Label_00DA;
                    }
                    IEnumerator enumerator1 = list.GetEnumerator();
                Label_0078:
                    try
                    {
                        if (enumerator1.MoveNext())
                        {
                            OptionDetails details1 = (OptionDetails) enumerator1.Current;
                            OptionProcessingResult result1 = details1.ProcessArgument(text2, text1);
                            if (result1 == OptionProcessingResult.NotThisOption)
                            {
                                goto Label_0078;
                            }
                            flag1 = true;
                            if (result1 == OptionProcessingResult.OptionConsumedParameter)
                            {
                                num2++;
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable1 = enumerator1 as IDisposable;
                        disposable1?.Dispose();
                    }
                Label_00DA:
                    if (!flag1)
                    {
                        ProcessNonOption(text2);
                    }
                }
                IEnumerator enumerator2 = list.GetEnumerator();
            Label_0102:
                try
                {
                    if (enumerator2.MoveNext())
                    {
                        OptionDetails details2 = (OptionDetails) enumerator2.Current;
                        details2.TransferValues();
                        goto Label_0102;
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    disposable2?.Dispose();
                }
                IEnumerator enumerator3 = argumentsTail.GetEnumerator();
            Label_0151:
                try
                {
                    if (enumerator3.MoveNext())
                    {
                        string text3 = (string) enumerator3.Current;
                        ProcessNonOption(text3);
                        goto Label_0151;
                    }
                }
                finally
                {
                    IDisposable disposable3 = enumerator3 as IDisposable;
                    disposable3?.Dispose();
                }
                return arguments.ToArray();
            }
            catch (Exception exception1)
            {
                Console.WriteLine(exception1.ToString());
                Environment.Exit(1);
            }
            return null;
        }

        private void ProcessNonOption(string argument)
        {
            if (OptionDetails.Verbose)
            {
                Console.WriteLine("argument [" + argument + "]");
            }
            if (argumentProcessor is null)
            {
                arguments.Add(argument);
            }
            else
            {
                object[] objArray1 = { argument } ;
                argumentProcessor.Invoke(optionBundle, objArray1);
            }
        }

        private void ShowAbout()
        {
            ShowTitleLines();
            Console.WriteLine(appAboutDetails);
            StringBuilder builder1 = new StringBuilder("Authors: ");
            bool flag1 = true;
            string[] textArray1 = appAuthors;
            foreach (var text1 in textArray1)
            {
                if (flag1)
                {
                    flag1 = false;
                }
                else
                {
                    builder1.Append(", ");
                }
                builder1.Append(text1);
            }
            Console.WriteLine(builder1.ToString());
        }

        public void ShowBanner()
        {
            if (!bannerAlreadyShown)
            {
                string[] textArray1 = { appTitle, "  ", appVersion, " - ", appCopyright } ;
                Console.WriteLine(string.Concat(textArray1));
            }
            bannerAlreadyShown = true;
        }

        private void ShowHelp(bool showSecondLevelHelp)
        {
            ShowTitleLines();
            Console.WriteLine(Usage);
            Console.WriteLine("Options:");
            var list1 = new List<string>(list.Count);
            var num1 = 0;
            var enumerator1 = list.GetEnumerator();
        Label_003B:
            try
            {
                if (enumerator1.MoveNext())
                {
                    OptionDetails details1 = enumerator1.Current;
                    if (details1.SecondLevelHelp == showSecondLevelHelp)
                    {
                        string[] textArray1 = details1.ToString().Split('\n');
                        string[] textArray3 = textArray1;
                        foreach (var text1 in textArray3)
                        {
                            int num2 = text1.IndexOf('\t');
                            if (num2 > num1)
                            {
                                num1 = num2;
                            }
                            list1.Add(text1);
                        }
                    }
                    goto Label_003B;
                }
            }
            finally
            {
                IDisposable disposable1 = enumerator1;
                disposable1?.Dispose();
            }
            num1 += 2;
            IEnumerator enumerator2 = list1.GetEnumerator();
        Label_00E6:
            try
            {
                if (!enumerator2.MoveNext())
                {
                    return;
                }
                string text2 = (string) enumerator2.Current;
                string[] textArray2 = text2.Split('\t');
                Console.Write(textArray2[0].PadRight(num1));
                Console.WriteLine(textArray2[1]);
                if (textArray2.Length > 2)
                {
                    string text3 = new string(' ', num1);
                    for (int num3 = 2; num3 < textArray2.Length; num3++)
                    {
                        Console.Write(text3);
                        Console.WriteLine(textArray2[num3]);
                    }
                }
                goto Label_00E6;
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                disposable2?.Dispose();
            }
        }

        private void ShowTitleLines()
        {
            ShowBanner();
            Console.WriteLine(appDescription);
            Console.WriteLine();
        }

        private void ShowUsage()
        {
            Console.WriteLine(Usage);
            Console.Write("Short Options: ");
            IEnumerator enumerator1 = list.GetEnumerator();
        Label_0021:
            try
            {
                if (enumerator1.MoveNext())
                {
                    OptionDetails details1 = (OptionDetails) enumerator1.Current;
                    Console.Write(details1.ShortForm.Trim());
                    goto Label_0021;
                }
            }
            finally
            {
                IDisposable disposable1 = enumerator1 as IDisposable;
                disposable1?.Dispose();
            }
            Console.WriteLine();
        }

        private void ShowUsage(string errorMessage)
        {
            Console.WriteLine("ERROR: " + errorMessage.TrimEnd());
            ShowUsage();
        }


        // Properties
        public string AboutDetails => appAboutDetails;

        public string Usage => ("Usage: " + appExeName + " [options] " + appUsageComplement);


        // Fields
        private string appAboutDetails;
        private string[] appAuthors;
        private string appCopyright;
        private string appDescription;
        private string appExeName;
        private string appTitle;
        private string appUsageComplement;
        private string appVersion;
        private MethodInfo argumentProcessor;
        private readonly List<string> arguments;
        private readonly List<string> argumentsTail;
        private bool bannerAlreadyShown;
        private bool breakSingleDashManyLettersIntoManyOptions;
        private bool endOptionProcessingWithDoubleDash;
        private Assembly entry;
        private bool HasSecondLevelHelp;
        private readonly List<OptionDetails> list;
        private Options optionBundle;
        private OptionsParsingMode parsingMode;
    }
}