namespace Mono.GetOptions
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class OptionDetails : IComparable
    {
        // Methods
        static OptionDetails()
        {
            OptionDetails.Verbose = false;
        }

        public OptionDetails(System.Reflection.MemberInfo memberInfo, OptionAttribute option, Options optionBundle)
        {
            this.paramName = null;
            this.optionHelp = null;
            this.ShortForm = ("" + option.ShortForm).Trim();
            if (option.LongForm == null)
            {
                this.LongForm = string.Empty;
            }
            else
            {
                this.LongForm = (option.LongForm != string.Empty) ? option.LongForm : memberInfo.Name;
            }
            this.AlternateForm = option.AlternateForm;
            this.ShortDescription = this.ExtractParamName(option.ShortDescription);
            this.Occurs = 0;
            this.OptionBundle = optionBundle;
            this.BooleanOption = false;
            this.MemberInfo = memberInfo;
            this.NeedsParameter = false;
            this.Values = null;
            this.MaxOccurs = 1;
            this.VBCStyleBoolean = option.VBCStyleBoolean;
            this.SecondLevelHelp = option.SecondLevelHelp;
            this.ParameterType = OptionDetails.TypeOfMember(memberInfo);
            if (this.ParameterType != null)
            {
                if (this.ParameterType.FullName != "System.Boolean")
                {
                    if (this.LongForm.IndexOf(':') >= 0)
                    {
                        throw new InvalidOperationException("Options with an embedded colon (':') in their visible name must be boolean!!! [" + this.MemberInfo.ToString() + " isn't]");
                    }
                    this.NeedsParameter = true;
                    if (option.MaxOccurs == 1)
                    {
                        return;
                    }
                    if (this.ParameterType.IsArray)
                    {
                        this.Values = new ArrayList();
                        this.MaxOccurs = option.MaxOccurs;
                        return;
                    }
                    if ((this.MemberInfo is MethodInfo) || (this.MemberInfo is PropertyInfo))
                    {
                        this.MaxOccurs = option.MaxOccurs;
                        return;
                    }
                    object[] objArray1 = new object[] { "MaxOccurs set to non default value (", option.MaxOccurs, ") for a [", this.MemberInfo.ToString(), "] option" } ;
                    throw new InvalidOperationException(string.Concat(objArray1));
                }
                this.BooleanOption = true;
                if (option.MaxOccurs != 1)
                {
                    if ((this.MemberInfo is MethodInfo) || (this.MemberInfo is PropertyInfo))
                    {
                        this.MaxOccurs = option.MaxOccurs;
                    }
                    else
                    {
                        object[] objArray2 = new object[] { "MaxOccurs set to non default value (", option.MaxOccurs, ") for a [", this.MemberInfo.ToString(), "] option" } ;
                        throw new InvalidOperationException(string.Concat(objArray2));
                    }
                }
            }
        }

        private void DoIt(bool setValue)
        {
            if (!this.NeedsParameter)
            {
                this.Occurred(1);
                if (OptionDetails.Verbose)
                {
                    Console.WriteLine("<" + this.LongForm + "> set to [true]");
                }
                if (this.MemberInfo is FieldInfo)
                {
                    ((FieldInfo) this.MemberInfo).SetValue(this.OptionBundle, setValue);
                }
                else if (this.MemberInfo is PropertyInfo)
                {
                    ((PropertyInfo) this.MemberInfo).SetValue(this.OptionBundle, setValue, null);
                }
                else if (((WhatToDoNext) ((MethodInfo) this.MemberInfo).Invoke(this.OptionBundle, null)) == WhatToDoNext.AbandonProgram)
                {
                    Environment.Exit(1);
                }
            }
        }

        private void DoIt(string parameterValue)
        {
            if (parameterValue == null)
            {
                parameterValue = "";
            }
            char[] chArray1 = new char[] { ',' } ;
            string[] textArray1 = parameterValue.Split(chArray1);
            this.Occurred(textArray1.Length);
            string[] textArray2 = textArray1;
            for (int num1 = 0; num1 < textArray2.Length; num1++)
            {
                string text1 = textArray2[num1];
                object obj1 = null;
                if (OptionDetails.Verbose)
                {
                    string[] textArray3 = new string[] { "<", this.LongForm, "> set to [", text1, "]" } ;
                    Console.WriteLine(string.Concat(textArray3));
                }
                if ((this.Values != null) && (text1 != null))
                {
                    try
                    {
                        obj1 = Convert.ChangeType(text1, this.ParameterType.GetElementType());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(string.Format("The value '{0}' is not convertible to the appropriate type '{1}' for the {2} option", text1, this.ParameterType.GetElementType().Name, this.DefaultForm));
                    }
                    this.Values.Add(obj1);
                }
                else
                {
                    if (text1 != null)
                    {
                        try
                        {
                            obj1 = Convert.ChangeType(text1, this.ParameterType);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(string.Format("The value '{0}' is not convertible to the appropriate type '{1}' for the {2} option", text1, this.ParameterType.Name, this.DefaultForm));
                            goto Label_01B1;
                        }
                    }
                    if (this.MemberInfo is FieldInfo)
                    {
                        ((FieldInfo) this.MemberInfo).SetValue(this.OptionBundle, obj1);
                    }
                    else if (this.MemberInfo is PropertyInfo)
                    {
                        ((PropertyInfo) this.MemberInfo).SetValue(this.OptionBundle, obj1, null);
                    }
                    else
                    {
                        object[] objArray1 = new object[] { obj1 } ;
                        if (((WhatToDoNext) ((MethodInfo) this.MemberInfo).Invoke(this.OptionBundle, objArray1)) == WhatToDoNext.AbandonProgram)
                        {
                            Environment.Exit(1);
                        }
                    }
                }
            Label_01B1:;
            }
        }

        private string ExtractParamName(string shortDescription)
        {
            int num1 = shortDescription.IndexOf('{');
            if (num1 < 0)
            {
                this.paramName = "PARAM";
                return shortDescription;
            }
            int num2 = shortDescription.IndexOf('}');
            if (num2 < num1)
            {
                num2 = shortDescription.Length + 1;
            }
            this.paramName = shortDescription.Substring(num1 + 1, (num2 - num1) - 1);
            shortDescription = shortDescription.Substring(0, num1) + this.paramName + shortDescription.Substring(num2 + 1);
            return shortDescription;
        }

        private bool IsThisOption(string arg)
        {
            if ((arg == null) || (arg == string.Empty))
            {
                return false;
            }
            char[] chArray1 = new char[] { '-', '/' } ;
            arg = arg.TrimStart(chArray1);
            if (this.VBCStyleBoolean)
            {
                char[] chArray2 = new char[] { '-', '+' } ;
                arg = arg.TrimEnd(chArray2);
            }
            return (((arg == this.ShortForm) || (arg == this.LongForm)) || (arg == this.AlternateForm));
        }

        private void Occurred(int howMany)
        {
            this.Occurs += howMany;
            if ((this.MaxOccurs > 0) && (this.Occurs > this.MaxOccurs))
            {
                object[] objArray1 = new object[] { "Option ", this.ShortForm, " can be used at most ", this.MaxOccurs, " times" } ;
                throw new IndexOutOfRangeException(string.Concat(objArray1));
            }
        }

        public OptionProcessingResult ProcessArgument(string arg, string nextArg)
        {
            if (this.IsThisOption(arg))
            {
                if (!this.NeedsParameter)
                {
                    if (this.VBCStyleBoolean && arg.EndsWith("-", StringComparison.Ordinal))
                    {
                        this.DoIt(false);
                    }
                    else
                    {
                        this.DoIt(true);
                    }
                    return OptionProcessingResult.OptionAlone;
                }
                this.DoIt(nextArg);
                return OptionProcessingResult.OptionConsumedParameter;
            }
            if (this.IsThisOption(arg + ":" + nextArg))
            {
                this.DoIt(true);
                return OptionProcessingResult.OptionConsumedParameter;
            }
            return OptionProcessingResult.NotThisOption;
        }

        int IComparable.CompareTo(object other)
        {
            return this.Key.CompareTo(((OptionDetails) other).Key);
        }

        public override string ToString()
        {
            if (this.optionHelp == null)
            {
                string text1;
                string text2;
                bool flag1 = !string.IsNullOrEmpty(this.LongForm);
                if (this.OptionBundle.ParsingMode == OptionsParsingMode.Windows)
                {
                    text2 = "/";
                    text1 = "/";
                }
                else
                {
                    text2 = "-";
                    text1 = this.linuxLongPrefix;
                }
                this.optionHelp = "  ";
                this.optionHelp = this.optionHelp + ((this.ShortForm == string.Empty) ? "   " : (text2 + this.ShortForm + " "));
                this.optionHelp = this.optionHelp + (!flag1 ? "" : (text1 + this.LongForm));
                if (this.NeedsParameter)
                {
                    if (flag1)
                    {
                        this.optionHelp = this.optionHelp + ":";
                    }
                    this.optionHelp = this.optionHelp + this.ParamName;
                }
                else if (this.BooleanOption && this.VBCStyleBoolean)
                {
                    this.optionHelp = this.optionHelp + "[+|-]";
                }
                this.optionHelp = this.optionHelp + ("\t" + this.ShortDescription);
                if (!string.IsNullOrEmpty(this.AlternateForm))
                {
                    this.optionHelp = this.optionHelp + (" [short form: " + text2 + this.AlternateForm + "]");
                }
            }
            return this.optionHelp;
        }

        public void TransferValues()
        {
            if (this.Values != null)
            {
                if (this.MemberInfo is FieldInfo)
                {
                    ((FieldInfo) this.MemberInfo).SetValue(this.OptionBundle, this.Values.ToArray(this.ParameterType.GetElementType()));
                }
                else if (this.MemberInfo is PropertyInfo)
                {
                    ((PropertyInfo) this.MemberInfo).SetValue(this.OptionBundle, this.Values.ToArray(this.ParameterType.GetElementType()), null);
                }
                else
                {
                    object[] objArray1 = new object[] { this.Values.ToArray(this.ParameterType.GetElementType()) } ;
                    if (((WhatToDoNext) ((MethodInfo) this.MemberInfo).Invoke(this.OptionBundle, objArray1)) == WhatToDoNext.AbandonProgram)
                    {
                        Environment.Exit(1);
                    }
                }
            }
        }

        private static Type TypeOfMember(System.Reflection.MemberInfo memberInfo)
        {
            if ((memberInfo.MemberType == MemberTypes.Field) && (memberInfo is FieldInfo))
            {
                return ((FieldInfo) memberInfo).FieldType;
            }
            if ((memberInfo.MemberType == MemberTypes.Property) && (memberInfo is PropertyInfo))
            {
                return ((PropertyInfo) memberInfo).PropertyType;
            }
            if ((memberInfo.MemberType == MemberTypes.Method) && (memberInfo is MethodInfo))
            {
                if (((MethodInfo) memberInfo).ReturnType.FullName != typeof(WhatToDoNext).FullName)
                {
                    throw new NotSupportedException("Option method must return '" + typeof(WhatToDoNext).FullName + "'");
                }
                ParameterInfo[] infoArray1 = ((MethodInfo) memberInfo).GetParameters();
                if ((infoArray1 != null) && (infoArray1.Length != 0))
                {
                    return infoArray1[0].ParameterType;
                }
                return null;
            }
            throw new NotSupportedException("'" + memberInfo.MemberType + "' memberType is not supported");
        }


        // Properties
        public string DefaultForm
        {
            get
            {
                string text1 = "-";
                string text2 = this.linuxLongPrefix;
                if (this.parsingMode == OptionsParsingMode.Windows)
                {
                    text1 = "/";
                    text2 = "/";
                }
                if (this.ShortForm != string.Empty)
                {
                    return (text1 + this.ShortForm);
                }
                return (text2 + this.LongForm);
            }
        }

        internal string Key
        {
            get
            {
                return (this.LongForm + " " + this.ShortForm);
            }
        }

        private string linuxLongPrefix
        {
            get
            {
                return (((this.parsingMode & OptionsParsingMode.GNU_DoubleDash) != OptionsParsingMode.GNU_DoubleDash) ? "-" : "--");
            }
        }

        public string ParamName
        {
            get
            {
                return this.paramName;
            }
        }

        private OptionsParsingMode parsingMode
        {
            get
            {
                return this.OptionBundle.ParsingMode;
            }
        }


        // Fields
        public string AlternateForm;
        public bool BooleanOption;
        public string LongForm;
        public int MaxOccurs;
        public System.Reflection.MemberInfo MemberInfo;
        public bool NeedsParameter;
        public int Occurs;
        public Options OptionBundle;
        private string optionHelp;
        public Type ParameterType;
        public string paramName;
        public bool SecondLevelHelp;
        public string ShortDescription;
        public string ShortForm;
        public ArrayList Values;
        public bool VBCStyleBoolean;
        public static bool Verbose;
    }
}

