﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ASCompletion.Completion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Completion
{
    internal class CodeGenerator : IContextualGenerator
    {
        public bool ContextualGenerator(ScintillaControl sci, List<ICompletionListItem> options, ASResult expr)
        {
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
