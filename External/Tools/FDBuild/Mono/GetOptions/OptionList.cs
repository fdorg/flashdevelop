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
            this.appTitle = "Add a [assembly: AssemblyTitle(\"Here goes the application name\")] to your assembly";
            this.appCopyright = "Add a [assembly: AssemblyCopyright(\"(c)200n Here goes the copyright holder name\")] to your assembly";
            this.appDescription = "Add a [assembly: AssemblyDescription(\"Here goes the short description\")] to your assembly";
            this.appAboutDetails = "Add a [assembly: Mono.About(\"Here goes the short about details\")] to your assembly";
            this.appUsageComplement = "Add a [assembly: Mono.UsageComplement(\"Here goes the usage clause complement\")] to your assembly";
            this.list = new ArrayList();
            this.arguments = new ArrayList();
            this.argumentsTail = new ArrayList();
            this.argumentProcessor = null;
            this.HasSecondLevelHelp = false;
            this.bannerAlreadyShown = false;
            this.Initialize(optionBundle);
        }

        private void AddArgumentProcessor(MemberInfo memberInfo)
        {
            if (this.argumentProcessor != null)
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
            if (((infoArray1 == null) || (infoArray1.Length != 1)) || (infoArray1[0].ParameterType.FullName != typeof(string).FullName))
            {
                throw new NotSupportedException("Argument processor method must have a string parameter");
            }
            this.argumentProcessor = (MethodInfo) memberInfo;
        }

        internal WhatToDoNext DoAbout()
        {
            this.ShowAbout();
            return WhatToDoNext.AbandonProgram;
        }

        internal WhatToDoNext DoHelp()
        {
            this.ShowHelp(false);
            return WhatToDoNext.AbandonProgram;
        }

        internal WhatToDoNext DoHelp2()
        {
            this.ShowHelp(true);
            return WhatToDoNext.AbandonProgram;
        }

        internal WhatToDoNext DoUsage()
        {
            this.ShowUsage();
            return WhatToDoNext.AbandonProgram;
        }

        public string[] ExpandResponseFiles(string[] args)
        {
            ArrayList list1 = new ArrayList();
            string[] textArray1 = args;
            for (int num1 = 0; num1 < textArray1.Length; num1++)
            {
                string text1 = textArray1[num1];
                if (text1.StartsWith("@", StringComparison.Ordinal))
                {
                    try
                    {
                        string text2;
                        StreamReader reader1 = new StreamReader(text1.Substring(1));
                        while ((text2 = reader1.ReadLine()) != null)
                        {
                            list1.AddRange(text2.Split(new char[0]));
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
            return (string[]) list1.ToArray(typeof(string));
        }

        private object[] GetAssemblyAttributes(Type type)
        {
            return this.entry.GetCustomAttributes(type, false);
        }

        private string[] GetAssemblyAttributeStrings(Type type)
        {
            object[] objArray1 = this.GetAssemblyAttributes(type);
            if ((objArray1 == null) || (objArray1.Length == 0))
            {
                return new string[0];
            }
            int num1 = 0;
            string[] textArray1 = new string[objArray1.Length];
            object[] objArray2 = objArray1;
            for (int num2 = 0; num2 < objArray2.Length; num2++)
            {
                object obj1 = objArray2[num2];
                textArray1[num1++] = obj1.ToString();
            }
            return textArray1;
        }

        private void GetAssemblyAttributeValue(Type type, ref string var)
        {
            object[] objArray1 = this.GetAssemblyAttributes(type);
            if ((objArray1 != null) && (objArray1.Length > 0))
            {
                var = objArray1[0].ToString();
            }
        }

        private void GetAssemblyAttributeValue(Type type, string propertyName, ref string var)
        {
            object[] objArray1 = this.GetAssemblyAttributes(type);
            if ((objArray1 != null) && (objArray1.Length > 0))
            {
                var = (string) type.InvokeMember(propertyName, BindingFlags.GetProperty | (BindingFlags.GetField | (BindingFlags.Public | BindingFlags.Instance)), null, objArray1[0], new object[0]);
            }
        }

        private static int IndexOfAny(string where, params char[] what)
        {
            return where.IndexOfAny(what);
        }

        private void Initialize(Options optionBundle)
        {
            if (optionBundle == null)
            {
                throw new ArgumentNullException("optionBundle");
            }
            this.entry = Assembly.GetEntryAssembly();
            this.appExeName = this.entry.GetName().Name;
            this.appVersion = this.entry.GetName().Version.ToString();
            this.optionBundle = optionBundle;
            this.parsingMode = optionBundle.ParsingMode;
            this.breakSingleDashManyLettersIntoManyOptions = optionBundle.BreakSingleDashManyLettersIntoManyOptions;
            this.endOptionProcessingWithDoubleDash = optionBundle.EndOptionProcessingWithDoubleDash;
            this.GetAssemblyAttributeValue(typeof(AssemblyTitleAttribute), "Title", ref this.appTitle);
            this.GetAssemblyAttributeValue(typeof(AssemblyCopyrightAttribute), "Copyright", ref this.appCopyright);
            this.GetAssemblyAttributeValue(typeof(AssemblyDescriptionAttribute), "Description", ref this.appDescription);
            this.GetAssemblyAttributeValue(typeof(AboutAttribute), ref this.appAboutDetails);
            this.GetAssemblyAttributeValue(typeof(UsageComplementAttribute), ref this.appUsageComplement);
            this.appAuthors = this.GetAssemblyAttributeStrings(typeof(AuthorAttribute));
            if (this.appAuthors.Length == 0)
            {
                this.appAuthors = new string[] { "Add one or more [assembly: Mono.GetOptions.Author(\"Here goes the author name\")] to your assembly" } ;
            }
            MemberInfo[] infoArray1 = optionBundle.GetType().GetMembers();
            for (int num1 = 0; num1 < infoArray1.Length; num1++)
            {
                MemberInfo info1 = infoArray1[num1];
                object[] objArray1 = info1.GetCustomAttributes(typeof(OptionAttribute), true);
                if ((objArray1 != null) && (objArray1.Length > 0))
                {
                    OptionDetails details1 = new OptionDetails(info1, (OptionAttribute) objArray1[0], optionBundle);
                    this.list.Add(details1);
                    this.HasSecondLevelHelp = this.HasSecondLevelHelp || details1.SecondLevelHelp;
                }
                else
                {
                    objArray1 = info1.GetCustomAttributes(typeof(ArgumentProcessorAttribute), true);
                    if ((objArray1 != null) && (objArray1.Length > 0))
                    {
                        this.AddArgumentProcessor(info1);
                    }
                }
            }
        }

        internal bool MaybeAnOption(string arg)
        {
            return (((this.parsingMode & OptionsParsingMode.Windows) > 0) && (arg[0] == '/')) || (((this.parsingMode & OptionsParsingMode.Linux) > 0) && (arg[0] == '-'));
        }

        public string[] NormalizeArgs(string[] args)
        {
            bool flag1 = true;
            ArrayList list1 = new ArrayList();
            string[] textArray1 = this.ExpandResponseFiles(args);
            for (int num2 = 0; num2 < textArray1.Length; num2++)
            {
                string text1 = textArray1[num2];
                if (text1.Length > 0)
                {
                    if (flag1)
                    {
                        if (this.endOptionProcessingWithDoubleDash && (text1 == "--"))
                        {
                            flag1 = false;
                        }
                        else if (((this.parsingMode & OptionsParsingMode.Linux) > 0) && (text1[0] == '-') && ((text1.Length > 1) && (text1[1] != '-')) && this.breakSingleDashManyLettersIntoManyOptions)
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
                            if (!this.MaybeAnOption(text1))
                            {
                                goto Label_014D;
                            }
                            char[] chArray1 = new char[] { ':', '=' } ;
                            int num1 = OptionList.IndexOfAny(text1, chArray1);
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
                        this.argumentsTail.Add(text1);
                    }
                }
                goto Label_0155;
            Label_014D:
                list1.Add(text1);
            Label_0155:;
            }
            return (string[]) list1.ToArray(typeof(string));
        }

        public string[] ProcessArgs(string[] args)
        {
            this.list.Sort();
            args = this.NormalizeArgs(args);
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
                    IEnumerator enumerator1 = this.list.GetEnumerator();
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
                        if (disposable1 != null)
                        {
                            disposable1.Dispose();
                        }
                    }
                Label_00DA:
                    if (!flag1)
                    {
                        this.ProcessNonOption(text2);
                    }
                }
                IEnumerator enumerator2 = this.list.GetEnumerator();
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
                    if (disposable2 != null)
                    {
                        disposable2.Dispose();
                    }
                }
                IEnumerator enumerator3 = this.argumentsTail.GetEnumerator();
            Label_0151:
                try
                {
                    if (enumerator3.MoveNext())
                    {
                        string text3 = (string) enumerator3.Current;
                        this.ProcessNonOption(text3);
                        goto Label_0151;
                    }
                }
                finally
                {
                    IDisposable disposable3 = enumerator3 as IDisposable;
                    if (disposable3 != null)
                    {
                        disposable3.Dispose();
                    }
                }
                return (string[]) this.arguments.ToArray(typeof(string));
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
            if (this.argumentProcessor == null)
            {
                this.arguments.Add(argument);
            }
            else
            {
                object[] objArray1 = new object[] { argument } ;
                this.argumentProcessor.Invoke(this.optionBundle, objArray1);
            }
        }

        private void ShowAbout()
        {
            this.ShowTitleLines();
            Console.WriteLine(this.appAboutDetails);
            StringBuilder builder1 = new StringBuilder("Authors: ");
            bool flag1 = true;
            string[] textArray1 = this.appAuthors;
            for (int num1 = 0; num1 < textArray1.Length; num1++)
            {
                string text1 = textArray1[num1];
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
            if (!this.bannerAlreadyShown)
            {
                string[] textArray1 = new string[] { this.appTitle, "  ", this.appVersion, " - ", this.appCopyright } ;
                Console.WriteLine(string.Concat(textArray1));
            }
            this.bannerAlreadyShown = true;
        }

        private void ShowHelp(bool showSecondLevelHelp)
        {
            this.ShowTitleLines();
            Console.WriteLine(this.Usage);
            Console.WriteLine("Options:");
            ArrayList list1 = new ArrayList(this.list.Count);
            int num1 = 0;
            IEnumerator enumerator1 = this.list.GetEnumerator();
        Label_003B:
            try
            {
                if (enumerator1.MoveNext())
                {
                    OptionDetails details1 = (OptionDetails) enumerator1.Current;
                    if (details1.SecondLevelHelp == showSecondLevelHelp)
                    {
                        char[] chArray1 = new char[] { '\n' } ;
                        string[] textArray1 = details1.ToString().Split(chArray1);
                        string[] textArray3 = textArray1;
                        for (int num4 = 0; num4 < textArray3.Length; num4++)
                        {
                            string text1 = textArray3[num4];
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
                IDisposable disposable1 = enumerator1 as IDisposable;
                if (disposable1 != null)
                {
                    disposable1.Dispose();
                }
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
                char[] chArray2 = new char[] { '\t' } ;
                string[] textArray2 = text2.Split(chArray2);
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
                if (disposable2 != null)
                {
                    disposable2.Dispose();
                }
            }
        }

        private void ShowTitleLines()
        {
            this.ShowBanner();
            Console.WriteLine(this.appDescription);
            Console.WriteLine();
        }

        private void ShowUsage()
        {
            Console.WriteLine(this.Usage);
            Console.Write("Short Options: ");
            IEnumerator enumerator1 = this.list.GetEnumerator();
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
                if (disposable1 != null)
                {
                    disposable1.Dispose();
                }
            }
            Console.WriteLine();
        }

        private void ShowUsage(string errorMessage)
        {
            Console.WriteLine("ERROR: " + errorMessage.TrimEnd());
            this.ShowUsage();
        }


        // Properties
        public string AboutDetails
        {
            get
            {
                return this.appAboutDetails;
            }
        }

        public string Usage
        {
            get
            {
                return ("Usage: " + this.appExeName + " [options] " + this.appUsageComplement);
            }
        }


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
        private ArrayList arguments;
        private ArrayList argumentsTail;
        private bool bannerAlreadyShown;
        private bool breakSingleDashManyLettersIntoManyOptions;
        private bool endOptionProcessingWithDoubleDash;
        private Assembly entry;
        private bool HasSecondLevelHelp;
        private ArrayList list;
        private Options optionBundle;
        private OptionsParsingMode parsingMode;
    }
}

