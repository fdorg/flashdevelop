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
            String[] layouts = Directory.GetFiles(this.GetLayoutsDir(), "*.fdl");
            for (Int32 i = 0; i < layouts.Length; i++)
            {
                String label = Path.GetFileNameWithoutExtension(layouts[i]);
                SelectorNode item = new SelectorNode(label, layouts[i]);
                selector.Nodes.Add(item);
            }

        }

        /// <summary>
        /// Gets the layouts directory
        /// </summary>
        private String GetLayoutsDir()
        {
            String userPath = Settings.Instance.CustomLayoutPath;
            if (Directory.Exists(userPath)) return userPath;
            else
            {
                String path = Path.Combine(this.GetDataDir(), "Layouts");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// Gets the plugin data directory
        /// </summary>
        private String GetDataDir()
        {
            return Path.Combine(PathHelper.DataDir, "LayoutManager");
        }


    }

}

