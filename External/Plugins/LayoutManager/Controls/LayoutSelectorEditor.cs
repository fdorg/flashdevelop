using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using PluginCore.Helpers;

namespace LayoutManager.Controls
{
    public class LayoutSelectorEditor : ObjectSelectorEditor
    {
        protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
        {
            base.FillTreeWithData(selector, context, provider);
            selector.Nodes.Add(new SelectorNode("<None>", null));
            var layouts = Directory.GetFiles(GetLayoutsDir(), "*.fdl");
            foreach (var layout in layouts)
            {
                var label = Path.GetFileNameWithoutExtension(layout);
                selector.Nodes.Add(new SelectorNode(label, layout));
            }
        }

        /// <summary>
        /// Gets the layouts directory
        /// </summary>
        private string GetLayoutsDir()
        {
            var userPath = Settings.Instance.CustomLayoutPath;
            if (Directory.Exists(userPath)) return userPath;
            var path = Path.Combine(Path.Combine(PathHelper.DataDir, nameof(LayoutManager)), "Layouts");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}