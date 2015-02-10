using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree.NodeControls;
using FlashDebugger.Controls.DataTree;
using PluginCore;
using PluginCore.Localization;
using flash.tools.debugger;
using flash.tools.debugger.expression;

namespace FlashDebugger.Controls
{
    public class WatchUI : DockPanelControl
    {
        private DataTreeControl treeControl;
        private List<String> watches;

        public WatchUI()
        {
            watches = new List<string>();
            treeControl = new DataTreeControl(true);
            this.treeControl.Tree.BorderStyle = BorderStyle.None;
            this.treeControl.Resize += new EventHandler(this.TreeControlResize);
            this.treeControl.Tree.Font = PluginBase.Settings.DefaultFont;
            this.treeControl.Dock = DockStyle.Fill;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeControl);
        }

        private void TreeControlResize(Object sender, EventArgs e)
        {
            Int32 w = this.treeControl.Width / 2;
            this.treeControl.Tree.Columns[0].Width = w;
            this.treeControl.Tree.Columns[1].Width = w - 8;
        }

        public bool AddElement(String item)
        {
            if (watches.Contains(item)) return false;
            watches.Add(item);
            UpdateElements();
            return true;
        }
        
        public void RemoveElement(string item)
        {
            watches.Remove(item);
            UpdateElements();
        }

        public void RemoveElement(int itemN)
        {
            if (itemN < watches.Count) RemoveElement(watches[itemN]);
        }

        public bool ReplaceElement(string oldItem, string newItem)
        {
            if (watches.Contains(newItem)) return false;
            int itemN = watches.IndexOf(oldItem);
            if (itemN == -1)
                AddElement(newItem);
            else
            {
                watches[itemN] = newItem;
                UpdateElements();
            }

            return true;
        }

        public void Clear()
        {
            watches.Clear();
            treeControl.Nodes.Clear();
        }

        public void UpdateElements()
        {
            treeControl.Tree.BeginUpdate();
            treeControl.SaveState();

            treeControl.Nodes.Clear();
            foreach (String item in watches)
            {
                DataNode node; // todo, introduce new Node types.
                try
                {
                    IASTBuilder builder = new ASTBuilder(false);
                    ValueExp exp = builder.parse(new java.io.StringReader(item));
                    var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                    var obj = exp.evaluate(ctx);
                    if (obj is Variable)
                        node = new VariableNode((Variable)obj);
                    else if (obj is Value)
                        node = new ValueNode(item, (Value)obj);
                    else
                        node = new ScalarNode(item, obj.toString());
                    node.Tag = item;
                }
                catch (Exception ex)
                {
                    node = new ErrorNode(item, ex);
                }
                node.Text = item;
                treeControl.AddNode(node);
            }

            treeControl.AddNode(new ValueNode(TextHelper.GetString("Label.AddExpression")));
            treeControl.RestoreState();
            treeControl.Tree.EndUpdate();
            treeControl.Enabled = true;
        }

    }
}
