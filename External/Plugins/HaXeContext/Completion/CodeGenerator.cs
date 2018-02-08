using System;
using System.Collections.Generic;
using System.Drawing;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Completion
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

        protected override bool CanShowImplementInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return expr.Context.Separator != "=" && base.CanShowImplementInterfaceList(sci, position, expr, found);
        }

        protected override bool CanShowGenerateConstructorAndToString(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return base.CanShowGenerateConstructorAndToString(sci, position, expr, found)
                && !found.InClass.Flags.HasFlag(FlagType.Enum);
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
