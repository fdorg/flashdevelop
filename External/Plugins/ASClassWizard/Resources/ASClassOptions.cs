﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;

namespace ASClassWizard.Resources
{
    public class AS3ClassOptions
    {
        public bool createInheritedMethods;
        public bool createConstructor;
        public string Package;
        public string superClass;
        public List<string> interfaces;
        public bool isPublic;
        public bool isDynamic;
        public bool isFinal;
        public string Language;
        // TODO slavara: move to HaxeClassOptions
        public string Template;

        public AS3ClassOptions(
            string language,
            string package,
            string super_class, 
            List<string> Interfaces, 
            bool is_public, 
            bool is_dynamic, 
            bool is_final, 
            bool create_inherited, 
            bool create_constructor)
        {
            Language = language;
            Package = package;
            createConstructor = create_constructor;
            createInheritedMethods = create_inherited;
            superClass = super_class;
            interfaces = Interfaces;
            isPublic = is_public;
            isDynamic = is_dynamic;
            isFinal = is_final;
        }
    }
}
