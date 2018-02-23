using System;
using System.Collections.Generic;
using System.Drawing;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Generators
{
    internal class CodeGenerator : ASGenerator
    {
        protected override void ContextualGenerator(ScintillaControl sci, int position, ASResult expr, List<ICompletionListItem> options)
        {
            var context = ASContext.Context;
            if (context.CurrentClass.Flags.HasFlag(FlagType.Enum | FlagType.TypeDef) || context.CurrentClass.Flags.HasFlag(FlagType.Interface))
            {
                if (contextToken != null && expr.Member == null && !context.IsImported(expr.Type ?? ClassModel.VoidClass, sci.CurrentLine)) CheckAutoImport(expr, options);
                return;
            }
            base.ContextualGenerator(sci, position, expr, options);
        }

        protected override bool CanShowConvertToConst(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !ASContext.Context.CodeComplete.IsStringInterpolationStyle(sci, position) 
                && base.CanShowConvertToConst(sci, position, expr, found);
        }

        protected override bool CanShowImplementInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return expr.Context.Separator != "=" && base.CanShowImplementInterfaceList(sci, position, expr, found);
        }

        protected override bool CanShowGenerateConstructorAndToString(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var flags = found.InClass.Flags;
            return !flags.HasFlag(FlagType.Enum)
                && !flags.HasFlag(FlagType.TypeDef)
                && base.CanShowGenerateConstructorAndToString(sci, position, expr, found);
        }

        protected override bool HandleOverrideCompletion(bool autoHide)
        {
            var flags = ASContext.Context.CurrentClass.Flags;
            return !flags.HasFlag(FlagType.Abstract)
                && !flags.HasFlag(FlagType.Interface)
                && !flags.HasFlag(FlagType.TypeDef)
                && base.HandleOverrideCompletion(autoHide);
        }
    }

    class GeneratorItem : ICompletionListItem
    {
        readonly Action action;

        public GeneratorItem(string label, Action action)
        {
            Label = label;
            this.action = action;
        }

        public string Label { get; }

        public string Value
        {
            get
            {
                action.Invoke();
                return null;
            }
        }

        public string Description => TextHelper.GetString("ASCompletion.Info.GeneratorTemplate");

        public Bitmap Icon => (Bitmap) ASContext.Panel.GetIcon(34);
    }
}
