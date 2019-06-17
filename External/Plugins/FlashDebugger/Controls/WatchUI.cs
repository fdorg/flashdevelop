﻿using System;
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
            AutoKeyHandling = true;
            treeControl = new DataTreeControl(true);
            treeControl.Tree.BorderStyle = BorderStyle.None;
            treeControl.Resize += TreeControlResize;
            treeControl.Tree.Font = PluginBase.Settings.DefaultFont;
            treeControl.Dock = DockStyle.Fill;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeControl);

            this.watchManager = watchManager;
            this.watchManager.ExpressionAdded += WatchManager_ExpressionAdded;
            this.watchManager.ExpressionRemoved += WatchManager_ExpressionRemoved;
            this.watchManager.ExpressionReplaced += WatchManager_ExpressionReplaced;
            this.watchManager.ExpressionsCleared += WatchManager_ExpressionsCleared;
            this.watchManager.ExpressionsLoaded += WatchManager_ExpressionsLoaded;
        }

        private void TreeControlResize(object sender, EventArgs e)
        {
            int w = treeControl.Width / 2;
            treeControl.Tree.Columns[0].Width = w;
            treeControl.Tree.Columns[1].Width = w - 8;
        }

        private void WatchManager_ExpressionAdded(object sender, WatchExpressionArgs e)
        {
            treeControl.Nodes.Insert(e.Position, GetExpressionNode(e.Expression));
            UpdateElements();
        }

        private void WatchManager_ExpressionRemoved(object sender, WatchExpressionArgs e)
        {
            UpdateElements();
        }

        private void WatchManager_ExpressionReplaced(object sender, WatchExpressionReplaceArgs e)
        {
            treeControl.Nodes[e.Position] = GetExpressionNode(e.NewExpression);
        }

        private void WatchManager_ExpressionsCleared(object sender, EventArgs e)
        {
            treeControl.Nodes.Clear();
        }

        private void WatchManager_ExpressionsLoaded(object sender, EventArgs e)
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
