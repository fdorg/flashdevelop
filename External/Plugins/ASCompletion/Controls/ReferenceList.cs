using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using Timer = System.Timers.Timer;

namespace ASCompletion.Controls
{
    class ReferenceList
    {
        const int MinWidth = 150;

        static readonly Timer fadingTimer;
        static readonly ListView listView;

        static ReferenceList()
        {
            listView = new ListView
            {
                Visible = false,
                Width = 300,
                Height = 200,
                Columns = {""},
                View = View.Details,
                ShowGroups = true,
                MultiSelect = false,
                FullRowSelect = false,
                HeaderStyle = ColumnHeaderStyle.None
            };

            listView.Groups.Add("implementors", TextHelper.GetString("Label.ImplementedBy"));
            listView.Groups.Add("implements", TextHelper.GetString("Label.Implements"));
            listView.Groups.Add("overriders", TextHelper.GetString("Label.OverriddenBy"));
            listView.Groups.Add("overrides", TextHelper.GetString("Label.Overrides"));

            fadingTimer = new Timer
            {
                Interval = 800,
                SynchronizingObject = (Form) PluginBase.MainForm
            };
            
            listView.MouseEnter += ListView_MouseEnter;
            listView.MouseLeave += ListView_MouseLeave;
            listView.DoubleClick += ListView_DoubleClick;
            listView.LostFocus += ListView_LostFocus;
            listView.KeyPress += ListView_KeyPress;

            fadingTimer.Elapsed += FadingTimer_Elapsed;

            PluginBase.MainForm.Controls.Add(listView);
        }

        internal static void Show(IEnumerable<Reference> implementors, IEnumerable<Reference> implemented, IEnumerable<Reference> overriders, IEnumerable<Reference> overridden)
        {
            HideList();

            var implementorsGroup = listView.Groups["implementors"];
            var implementedGroup = listView.Groups["implements"];
            var overridersGroup = listView.Groups["overriders"];
            var overriddenGroup = listView.Groups["overrides"];

            AddItems(implementorsGroup, implementors);
            AddItems(implementedGroup, implemented);
            AddItems(overridersGroup, overriders);
            AddItems(overriddenGroup, overridden);

            var p = ((Form)PluginBase.MainForm).PointToClient(Cursor.Position);
            p.Offset(-2, -2);
            listView.Location = p;

            listView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.Columns[0].Width = Math.Max(MinWidth, listView.Columns[0].Width);
            listView.Width = listView.Columns[0].Width + 30;

            listView.BringToFront();
            listView.Show();
            listView.Focus();

            if (listView.Items.Count > 0)
                listView.Height = Math.Min(listView.Items[listView.Items.Count - 1].Bounds.Bottom + 10, 500);

            fadingTimer.Start();
            ListView_MouseEnter(null, null);
        }

        internal static IEnumerable<Reference> ConvertCache(MemberModel member, IEnumerable<ClassModel> list)
        {
            return list.Select(m => new Reference
            {
                File = m.InFile.FileName,
                Line = m.Members.Search(member.Name).LineFrom,
                Type = m.QualifiedName
            });
        }

        internal static IEnumerable<Reference> ConvertClassCache(IEnumerable<ClassModel> list)
        {
            return list.Select(m => new Reference
            {
                File = m.InFile.FileName,
                Line = m.LineFrom,
                Type = m.QualifiedName
            });
        }

        #region Event Listeners

        static void ListView_LostFocus(object sender, EventArgs e) => HideList();

        static void ListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ListView_DoubleClick(null, null);
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                HideList();
            }

        }

        static void ListView_MouseLeave(object sender, EventArgs e) => fadingTimer.Start();

        static void ListView_MouseEnter(object sender, EventArgs e) => fadingTimer.Stop();

        static void FadingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!listView.RectangleToScreen(new Rectangle(0, 0, listView.Width, listView.Height)).Contains(Control.MousePosition))
                HideList();
        }

        static void ListView_DoubleClick(object sender, EventArgs e)
        {
            var selection = listView.Items[listView.SelectedIndices[0]];
            var reference = (Reference)selection.Tag;

            HideList();
            if (PluginBase.MainForm.CurrentDocument?.SciControl is { } sci)
                ASComplete.SaveLastLookupPosition(sci);
            PluginBase.MainForm.OpenEditableDocument(reference.File, false);
            PluginBase.MainForm.CurrentDocument?.SciControl.GotoLine(reference.Line);
        }
        #endregion

        static void HideList()
        {
            listView.Hide();
            listView.Items.Clear();
            fadingTimer.Stop();
        }

        static void AddItems(ListViewGroup group, IEnumerable<Reference> items)
        {
            foreach (var r in items)
            {
                var item = new ListViewItem
                {
                    Text = r.Type,
                    Tag = r,
                    Group = group
                };
                listView.Items.Add(item);
            }
            
        }
    }

    class Reference
    {
        public string Type;
        public string File;
        public int Line;
    }
}
