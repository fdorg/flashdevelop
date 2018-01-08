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
        public override bool ContextualGenerator(ScintillaControl sci, List<ICompletionListItem> options, ASResult expr)
        {
            var context = ASContext.Context;
            if (context.CurrentClass.Flags.HasFlag(FlagType.Interface)
                && (expr.Member == null || expr.Member.Flags.HasFlag(FlagType.Variable)))
            {
                return true;
            }
            if (context.CurrentClass.Flags.HasFlag(FlagType.Enum | FlagType.TypeDef))
            {
                if (contextToken != null && expr.Member == null)
                {
                    var type = expr.Type ?? ClassModel.VoidClass;
                    if (!context.IsImported(type, sci.CurrentLine)) CheckAutoImport(expr, options);
                }
                return true;
            }
            return false;
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
