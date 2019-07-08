using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using PluginCore.Helpers;

namespace LayoutManager.Controls
{
    public class LayoutSelectorEditor : ObjectSelectorEditor
    {
        /// <summary>
        /// 
        /// </summary>
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
            string userPath = Settings.Instance.CustomLayoutPath;
            if (Directory.Exists(userPath)) return userPath;
            string path = Path.Combine(GetDataDir(), "Layouts");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets the plugin data directory
        /// </summary>
        private string GetDataDir() => Path.Combine(PathHelper.DataDir, "LayoutManager");
    }
}
