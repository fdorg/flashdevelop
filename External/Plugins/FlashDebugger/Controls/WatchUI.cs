using System;
using System.Windows.Forms;
using FlashDebugger.Controls.DataTree;
using FlashDebugger.Debugger;
using PluginCore;
using PluginCore.Localization;
using flash.tools.debugger;
using flash.tools.debugger.expression;

namespace FlashDebugger.Controls
{
    public class WatchUI : DockPanelControl
    {
        private DataTreeControl treeControl;
        private WatchManager watchManager;

        public WatchUI(WatchManager watchManager)
        {
            this.AutoKeyHandling = true;
            this.treeControl = new DataTreeControl(true);
            this.treeControl.Tree.BorderStyle = BorderStyle.None;
            this.treeControl.Resize += new EventHandler(this.TreeControlResize);
            this.treeControl.Tree.Font = PluginBase.Settings.DefaultFont;
            this.treeControl.Dock = DockStyle.Fill;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeControl);

            this.watchManager = watchManager;
            this.watchManager.ExpressionAdded += WatchManager_ExpressionAdded;
            this.watchManager.ExpressionRemoved += WatchManager_ExpressionRemoved;
            this.watchManager.ExpressionReplaced += WatchManager_ExpressionReplaced;
            this.watchManager.ExpressionsCleared += WatchManager_ExpressionsCleared;
            this.watchManager.ExpressionsLoaded += WatchManager_ExpressionsLoaded;
        }

        private void TreeControlResize(Object sender, EventArgs e)
        {
            Int32 w = this.treeControl.Width / 2;
            this.treeControl.Tree.Columns[0].Width = w;
            this.treeControl.Tree.Columns[1].Width = w - 8;
        }

        private void WatchManager_ExpressionAdded(Object sender, WatchExpressionArgs e)
        {
            treeControl.Nodes.Insert(e.Position, GetExpressionNode(e.Expression));
            UpdateElements();
        }

        private void WatchManager_ExpressionRemoved(Object sender, WatchExpressionArgs e)
        {
            UpdateElements();
        }

        private void WatchManager_ExpressionReplaced(Object sender, WatchExpressionReplaceArgs e)
        {
            treeControl.Nodes[e.Position] = GetExpressionNode(e.NewExpression);
        }

        private void WatchManager_ExpressionsCleared(Object sender, EventArgs e)
        {
            treeControl.Nodes.Clear();
        }

        private void WatchManager_ExpressionsLoaded(Object sender, EventArgs e)
        {
            UpdateElements();
        }

        public bool AddElement(string item)
        {
            return watchManager.Add(item);
        }
        
        public void RemoveElement(string item)
        {
            watchManager.Remove(item);
        }

        public void RemoveElement(int itemN)
        {
            watchManager.RemoveAt(itemN);
        }

        public bool ReplaceElement(string oldItem, string newItem)
        {
            return watchManager.Replace(oldItem, newItem);
        }

        public void Clear()
        {
            watchManager.ClearAll();
        }
        
        public void UpdateElements()
        {
            treeControl.Tree.BeginUpdate();
            treeControl.SaveState();
            treeControl.Nodes.Clear();
            var watches = watchManager.Watches;
            foreach (string item in watches)
            {
                treeControl.AddNode(GetExpressionNode(item));
            }
            treeControl.AddNode(new ValueNode(TextHelper.GetString("Label.AddExpression")));
            treeControl.RestoreState();
            treeControl.Tree.EndUpdate();
            treeControl.Enabled = true;
        }

        private DataNode GetExpressionNode(string item)
        {
            DataNode node;
            try
            {
                if (!PluginMain.debugManager.FlashInterface.isDebuggerStarted || !PluginMain.debugManager.FlashInterface.isDebuggerSuspended )
                {
                    return new ErrorNode(item, new Exception(""));
                }

                IASTBuilder builder = new ASTBuilder(false);
                ValueExp exp = builder.parse(new java.io.StringReader(item));
                var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                var obj = exp.evaluate(ctx);
                if (obj is Variable)
                {
                    node = new VariableNode((Variable)obj)
                    {
                        HideClassId = PluginMain.settingObject.HideClassIds,
                        HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                    };
                }
                else if (obj is Value)
                {
                    node = new ValueNode(item, (Value)obj)
                    {
                        HideClassId = PluginMain.settingObject.HideClassIds,
                        HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                    };
                }
                else node = new ScalarNode(item, obj.toString());
                node.Tag = item;
            }
            catch (Exception ex)
            {
                node = new ErrorNode(item, ex);
            }
            node.Text = item;
            return node;
        }

    }

}
