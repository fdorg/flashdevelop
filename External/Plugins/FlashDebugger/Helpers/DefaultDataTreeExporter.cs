// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Controls.Tree;
using FlashDebugger.Controls;
using FlashDebugger.Controls.DataTree;
using PluginCore;
using PluginCore.Localization;

namespace FlashDebugger.Helpers
{
    public class DefaultDataTreeExporter : IDataTreeExporter
    {
        static readonly string[] as3DisabledProps = { "stage", "parent", "root", "loaderInfo", "_nativeWindow", "nativeWindow" };

        public int CopyTreeMaxRecursion { get; set; }

        public int CopyTreeMaxChars { get; set; }

        public string GetTreeAsText(ValueNode dataNode, string levelSep, DataTreeControl control, int levelLimit)
        {
            var sb = new StringBuilder();
            // ensure expanded
            control.ListChildItems(dataNode);
            // add children nodes
            GetTreeItemsAsText(new List<Node> { dataNode }, new HashSet<string>(), levelSep, 0, sb, control, levelLimit);
            return sb.ToString();
        }

        void GetTreeItemsAsText(IList<Node> dataNodes, ISet<string> visited, string levelSep, int level, StringBuilder sb, DataTreeControl control, int levelLimit)
        {
            // per node
            int len = dataNodes.Count;
            for (int c = 0; c < len; c++)
            {
                var child = (ValueNode)dataNodes[c];

                // skip if unwanted item
                if (!IsWantedChild(child)) continue;

                // ensure expanded
                if (!child.IsLeaf) control.ListChildItems(child);

                // add node
                AppendTimes(sb, levelSep, level);

                // stop recursion if too long
                if (sb.Length > CopyTreeMaxChars)
                {
                    sb.AppendLine("......");
                    return;
                }

                if (child.IsPrimitive)
                {
                    sb.Append(child.Text + " : " + child.Value + " ");
                }
                else
                {
                    sb.Append(child.Text + " : " + child.ClassPath + " ");
                }

                // recurse for children .. but skip if unwanted items
                if (child.Nodes.Count > 0 && IsWantedParent(child))
                {

                    // opening brace
                    sb.AppendLine("{");

                    string childId = child.Id;

                    // add error if too many levels of recursion
                    if (level <= levelLimit || levelLimit == 0)
                    {

                        // check if encountered before
                        var isNew = childId == "" || !visited.Contains(childId);
                        if (!isNew)
                        {
                            // error
                            AppendTimes(sb, levelSep, level + 1);
                            sb.AppendLine(string.Format(TextHelper.GetString("TreeExporter.DuplicatedObject"), child.Value));
                        }
                        else if (level > CopyTreeMaxRecursion)
                        {
                            // error
                            AppendTimes(sb, levelSep, level + 1);
                            sb.AppendLine(TextHelper.GetString("TreeExporter.MaxDepthReached"));
                        }
                        else
                        {
                            // add to list
                            if (childId != "")
                                visited.Add(childId);

                            // children
                            try
                            {
                                GetTreeItemsAsText(child.Nodes, visited, levelSep, level + 1, sb, control, levelLimit);
                            }
                            catch { }
                        }
                    }

                    // closing brace
                    AppendTimes(sb, levelSep, level);
                    sb.AppendLine("}");
                }
                else
                {
                    sb.AppendLine();
                }
            }
        }

        static bool IsWantedParent(Node parent)
        {
            try
            {
                // if is an empty array []
                // then skip it from opening and closing { } the output
                if (parent.Nodes.Count == 1 || parent.Nodes.Count == 2)
                {
                    if (parent is ValueNode pNode && pNode.ClassPath == "Array")
                    {
                        var child1 = (DataNode)parent.Nodes[0];
                        if (child1.Text == "[static]" || child1.Text == "length")
                        {
                            return false;
                        }
                    }
                }

                // catch "cannot cast Aga.TreeNode to DataNode" 
                // TODO : fix this instead of just catching it
            }
            catch { }
            return true;
        }

        static bool IsWantedChild(Node child)
        {
            try
            {
                // skip if static
                if (child.Text == "[static]") return false;
                var parent = child.Parent as ValueNode;
                if (parent?.ClassPath != null)
                {
                    // if is an array []
                    // skip [static] and "length" properties
                    if (parent.ClassPath == "Array")
                    {
                        if (child.Text == "length")
                        {
                            return false;
                        }
                    }

                    // if is an AS3 display object,
                    // don't go upward (stage, parent)
                    if (parent.ClassPath.StartsWithOrdinal("flash.display.") || parent.ClassPath == "Main")
                    {
                        if (as3DisabledProps.Contains(child.Text))
                        {
                            return false;
                        }
                    }

                    // if is an AS2 display object,
                    // don't go upward (_parent)
                    if (parent.ClassPath == "MovieClip" || parent.ClassPath == "Video")
                    {
                        if (child.Text == "_parent")
                        {
                            return false;
                        }
                    }

                }

                // catch "cannot cast Aga.TreeNode to DataNode" 
                // TODO : fix this instead of just catching it
            }
            catch { }
            return true;
        }

        static void AppendTimes(StringBuilder sb, string append, int times)
        {
            for (int t = 0; t < times; t++)
            {
                sb.Append(append);
            }
        }
    }
}