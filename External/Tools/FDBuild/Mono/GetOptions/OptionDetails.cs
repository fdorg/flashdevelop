namespace Mono.GetOptions
{
    using System;
    using System.Collections;
    using System.Reflection;

    class OptionDetails : IComparable
    {
        // Methods
        static OptionDetails()
        {
            Verbose = false;
        }

        public OptionDetails(MemberInfo memberInfo, OptionAttribute option, Options optionBundle)
        {
            paramName = null;
            optionHelp = null;
            ShortForm = ("" + option.ShortForm).Trim();
            if (option.LongForm is null)
            {
                LongForm = string.Empty;
            }
            else
            {
                LongForm = option.LongForm.Length != 0 ? option.LongForm : memberInfo.Name;
            }
            AlternateForm = option.AlternateForm;
            ShortDescription = ExtractParamName(option.ShortDescription);
            Occurs = 0;
            OptionBundle = optionBundle;
            BooleanOption = false;
            MemberInfo = memberInfo;
            NeedsParameter = false;
            Values = null;
            MaxOccurs = 1;
            VBCStyleBoolean = option.VBCStyleBoolean;
            SecondLevelHelp = option.SecondLevelHelp;
            ParameterType = TypeOfMember(memberInfo);
            if (ParameterType != null)
            {
                if (ParameterType.FullName != "System.Boolean")
                {
                    if (LongForm.IndexOf(':') >= 0)
                    {
                        throw new InvalidOperationException("Options with an embedded colon (':') in their visible name must be boolean!!! [" + MemberInfo + " isn't]");
                    }
                    NeedsParameter = true;
                    if (option.MaxOccurs == 1)
                    {
                        return;
                    }
                    if (ParameterType.IsArray)
                    {
                        Values = new ArrayList();
                        MaxOccurs = option.MaxOccurs;
                        return;
                    }
                    if (MemberInfo is MethodInfo || MemberInfo is PropertyInfo)
                    {
                        MaxOccurs = option.MaxOccurs;
                        return;
                    }
                    object[] objArray1 = { "MaxOccurs set to non default value (", option.MaxOccurs, ") for a [", MemberInfo.ToString(), "] option" } ;
                    throw new InvalidOperationException(string.Concat(objArray1));
                }
                BooleanOption = true;
                if (option.MaxOccurs != 1)
                {
                    if (MemberInfo is MethodInfo || MemberInfo is PropertyInfo)
                    {
                        MaxOccurs = option.MaxOccurs;
                    }
                    else
                    {
                        object[] objArray2 = { "MaxOccurs set to non default value (", option.MaxOccurs, ") for a [", MemberInfo.ToString(), "] option" } ;
                        throw new InvalidOperationException(string.Concat(objArray2));
                    }
                }
            }
        }

        private void DoIt(bool setValue)
        {
            if (!NeedsParameter)
            {
                Occurred(1);
                if (Verbose)
                {
                    Console.WriteLine("<" + LongForm + "> set to [true]");
                }
                if (MemberInfo is FieldInfo info)
                {
                    info.SetValue(OptionBundle, setValue);
                }
                else if (MemberInfo is PropertyInfo propertyInfo)
                {
                    propertyInfo.SetValue(OptionBundle, setValue, null);
                }
                else if ((WhatToDoNext) ((MethodInfo) MemberInfo).Invoke(OptionBundle, null) == WhatToDoNext.AbandonProgram)
                {
                    Environment.Exit(1);
                }
            }
        }

        private void DoIt(string parameterValue)
        {
            parameterValue ??= "";
            char[] chArray1 = { ',' } ;
            string[] textArray1 = parameterValue.Split(chArray1);
            Occurred(textArray1.Length);
            string[] textArray2 = textArray1;
            foreach (var text1 in textArray2)
            {
                object obj1 = null;
                if (Verbose)
                {
                    string[] textArray3 = { "<", LongForm, "> set to [", text1, "]" } ;
                    Console.WriteLine(string.Concat(textArray3));
                }
                if (Values != null && text1 != null)
                {
                    try
                    {
                        obj1 = Convert.ChangeType(text1, ParameterType.GetElementType());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(
                            $"The value '{text1}' is not convertible to the appropriate type '{ParameterType.GetElementType().Name}' for the {DefaultForm} option");
                    }
                    Values.Add(obj1);
                }
                else
                {
                    if (text1 != null)
                    {
                        try
                        {
                            obj1 = Convert.ChangeType(text1, ParameterType);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(
                                $"The value '{text1}' is not convertible to the appropriate type '{ParameterType.Name}' for the {DefaultForm} option");
                            goto Label_01B1;
                        }
                    }
                    if (MemberInfo is FieldInfo fieldInfo)
                    {
                        fieldInfo.SetValue(OptionBundle, obj1);
                    }
                    else if (MemberInfo is PropertyInfo propertyInfo)
                    {
                        propertyInfo.SetValue(OptionBundle, obj1, null);
                    }
                    else
                    {
                        object[] objArray1 = { obj1 } ;
                        if ((WhatToDoNext) ((MethodInfo) MemberInfo).Invoke(OptionBundle, objArray1) == WhatToDoNext.AbandonProgram)
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
                paramName = "PARAM";
                return shortDescription;
            }
            int num2 = shortDescription.IndexOf('}');
            if (num2 < num1)
            {
                num2 = shortDescription.Length + 1;
            }
            paramName = shortDescription.Substring(num1 + 1, num2 - num1 - 1);
            shortDescription = shortDescription.Substring(0, num1) + paramName + shortDescription.Substring(num2 + 1);
            return shortDescription;
        }

        bool IsThisOption(string arg)
        {
            if (string.IsNullOrEmpty(arg)) return false;
            char[] chArray1 = { '-', '/' } ;
            arg = arg.TrimStart(chArray1);
            if (VBCStyleBoolean)
            {
                char[] chArray2 = { '-', '+' } ;
                arg = arg.TrimEnd(chArray2);
            }
            return arg == ShortForm || arg == LongForm || arg == AlternateForm;
        }

        private void Occurred(int howMany)
        {
            Occurs += howMany;
            if (MaxOccurs > 0 && Occurs > MaxOccurs)
            {
                object[] objArray1 = { "Option ", ShortForm, " can be used at most ", MaxOccurs, " times" } ;
                throw new IndexOutOfRangeException(string.Concat(objArray1));
            }
        }

        public OptionProcessingResult ProcessArgument(string arg, string nextArg)
        {
            if (IsThisOption(arg))
            {
                if (!NeedsParameter)
                {
                    if (VBCStyleBoolean && arg.EndsWith("-", StringComparison.Ordinal))
                    {
                        DoIt(false);
                    }
                    else
                    {
                        DoIt(true);
                    }
                    return OptionProcessingResult.OptionAlone;
                }
                DoIt(nextArg);
                return OptionProcessingResult.OptionConsumedParameter;
            }
            if (IsThisOption(arg + ":" + nextArg))
            {
                DoIt(true);
                return OptionProcessingResult.OptionConsumedParameter;
            }
            return OptionProcessingResult.NotThisOption;
        }

        int IComparable.CompareTo(object other) => Key.CompareTo(((OptionDetails) other).Key);

        public override string ToString()
        {
            if (optionHelp is null)
            {
                string text1;
                string text2;
                bool flag1 = !string.IsNullOrEmpty(LongForm);
                if (OptionBundle.ParsingMode == OptionsParsingMode.Windows)
                {
                    text2 = "/";
                    text1 = "/";
                }
                else
                {
                    text2 = "-";
                    text1 = linuxLongPrefix;
                }
                optionHelp = "  ";
                optionHelp += ShortForm == string.Empty ? "   " : text2 + ShortForm + " ";
                optionHelp += !flag1 ? "" : text1 + LongForm;
                if (NeedsParameter)
                {
                    if (flag1)
                    {
                        optionHelp += ":";
                    }
                    optionHelp += ParamName;
                }
                else if (BooleanOption && VBCStyleBoolean)
                {
                    optionHelp += "[+|-]";
                }
                optionHelp += "\t" + ShortDescription;
                if (!string.IsNullOrEmpty(AlternateForm))
                {
                    optionHelp += " [short form: " + text2 + AlternateForm + "]";
                }
            }
            return optionHelp;
        }

        public void TransferValues()
        {
            if (Values is null) return;
            if (MemberInfo is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(OptionBundle, Values.ToArray(ParameterType.GetElementType()));
            }
            else if (MemberInfo is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(OptionBundle, Values.ToArray(ParameterType.GetElementType()), null);
            }
            else
            {
                object[] objArray1 = { Values.ToArray(ParameterType.GetElementType()) } ;
                if ((WhatToDoNext) ((MethodInfo) MemberInfo).Invoke(OptionBundle, objArray1) == WhatToDoNext.AbandonProgram)
                {
                    Environment.Exit(1);
                }
            }
        }

        private static Type TypeOfMember(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Field && memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }
            if (memberInfo.MemberType == MemberTypes.Property && memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }
            if (memberInfo.MemberType == MemberTypes.Method && memberInfo is MethodInfo methodInfo)
            {
                if (methodInfo.ReturnType.FullName != typeof(WhatToDoNext).FullName)
                {
                    throw new NotSupportedException("Option method must return '" + typeof(WhatToDoNext).FullName + "'");
                }
                var infoArray1 = methodInfo.GetParameters();
                return infoArray1.Length != 0 ? infoArray1[0].ParameterType : null;
            }
            throw new NotSupportedException("'" + memberInfo.MemberType + "' memberType is not supported");
        }

        // Properties
        public string DefaultForm
        {
            get
            {
                string text1 = "-";
                string text2 = linuxLongPrefix;
                if (parsingMode == OptionsParsingMode.Windows)
                {
                    text1 = "/";
                    text2 = "/";
                }
                if (ShortForm != string.Empty)
                {
                    return text1 + ShortForm;
                }
                return text2 + LongForm;
            }
        }

        internal string Key => LongForm + " " + ShortForm;

        private string linuxLongPrefix => (parsingMode & OptionsParsingMode.GNU_DoubleDash) != OptionsParsingMode.GNU_DoubleDash ? "-" : "--";

        public string ParamName => paramName;

        private OptionsParsingMode parsingMode => OptionBundle.ParsingMode;

        // Fields
        public string AlternateForm;
        public bool BooleanOption;
        public string LongForm;
        public int MaxOccurs;
        public MemberInfo MemberInfo;
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