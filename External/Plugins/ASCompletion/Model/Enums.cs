using System;

namespace ASCompletion.Model
{
    [Flags]
    public enum Visibility : uint
    {
        Default = 1 << 1,
        Public = 1 << 2,
        Internal = 1 << 3,
        Protected = 1 << 4,
        Private = 1 << 5,
    }

    [Flags]
    public enum FlagType : ulong
    {
        Package = 1 << 1,
        Import = 1 << 2,
        Namespace = 1 << 3,
        Access = 1 << 4,
        Module = 1 << 5,
        Class = 1 << 6,
        Interface = 1 << 7,
        Enum = 1 << 8,
        TypeDef = 1 << 9,
        Abstract = 1 << 10,
        Struct = 1 << 11,
        Delegate = 1 << 12,
        Extends = 1 << 13,
        Implements = 1 << 14,
        Using = 1 << 15,

        Native = 1 << 16,
        Intrinsic = 1 << 17,
        Extern = 1 << 18,
        Final = 1 << 19,
        Dynamic = 1 << 20, // flag misused for both 'dynamic' keyword (ok) and for non-static members (bad)
        Static = 1 << 21,
        Override = 1 << 22,

        Constant = 1 << 23,
        Variable = 1 << 24,
        Function = 1 << 25,
        Getter = 1 << 26,
        Setter = 1 << 27,
        HXProperty = 1 << 28,
        Constructor = 1 << 29,

        LocalVar = 1 << 30,
        ParameterVar = 1L << 31,
        AutomaticVar = 1L << 32,
        Inferred = 1L << 33,

        Declaration = 1L << 34,
        Template = 1L << 35,
        DocTemplate = 1L << 36,
        CodeTemplate = 1L << 37,

        User = 1L << 38,
    }

    public enum ASMetaKind
    {
        Unknown,
        Event,
        Style,
        Effect,
        Exclude,
        Include,
        DefaultProperty,
        MaxChildren,
        Inspectable
    }
}