using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Managers;
using PluginCore.Helpers;
using ScintillaNet;

// TODO: Remove all direct references to ScintillaControl

namespace PluginCore.Controls
{
    public delegate void CompletionListInsertedTextHandler(Control sender, int position, string text, char trigger, ICompletionListItem item);

    public class CompletionListControl : IMessageFilter
    {
        public event CompletionListInsertedTextHandler OnInsert;
        public event CompletionListInsertedTextHandler OnCancel;
        public event EventHandler OnShowing;
        public event EventHandler OnHidden;
        
        /// <summary>
        /// Properties of the class 
        /// </summary> 
        private System.Timers.Timer tempo;
        private System.Timers.Timer tempoTip;
        private System.Windows.Forms.ListBox completionList;
        private System.Windows.Forms.ToolStripControlHost listContainer;
        private System.Windows.Forms.ToolStripDropDown listHost;

        #region State Properties

        private  bool disableSmartMatch;
        private  ICompletionListItem currentItem;
        private  List<ICompletionListItem> allItems;
        private  bool exactMatchInList;
        private  bool smartMatchInList;
        private  bool autoHideList;
        private  bool noAutoInsert;
        private  bool isActive;
        internal bool listUp;
        private  bool fullList;
        private  int startPos;
        private  int currentPos;
        private  int lastIndex;
        private  string currentWord;
        private  string word;
        private  bool needResize;
        private  string widestLabel;
        private  long showTime;
        private  ICompletionListItem defaultItem;

        private  ICompletionListHost host;
        private  RichToolTip tip;
        private  MethodCallTip callTip; // Used only by the main completion list so far, would it be better to control this case in another way? like the cl, we'd like to show this everywhere possible

        /// <summary>
        /// Set to 0 after calling .Show to keep the completion list active 
        /// when the text was erased completely (using backspace)
        /// </summary>
        public int MinWordLength;

        #endregion

        #region Control Creation

        /// <summary>
        /// Creates the control 
        /// </summary> 
        public CompletionListControl(ICompletionListHost target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.host = target;

            listHost = new ToolStripDropDown();
            listHost.Padding = Padding.Empty;
            listHost.Margin = Padding.Empty;
            listHost.AutoClose = false;
            listHost.DropShadowEnabled = false;
            listHost.AutoSize = false;
            listHost.Size = new Size(180, 100);

            tempo = new System.Timers.Timer();
            tempo.SynchronizingObject = (Form)PluginBase.MainForm;
            tempo.Elapsed += new System.Timers.ElapsedEventHandler(DisplayList);
            tempo.AutoReset = false;
            tempoTip = new System.Timers.Timer();
            tempoTip.SynchronizingObject = (Form)PluginBase.MainForm;
            tempoTip.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTip);
            tempoTip.AutoReset = false;
            tempoTip.Interval = 800;

            completionList = new ListBoxEx();
            completionList.Font = new System.Drawing.Font(PluginBase.Settings.DefaultFont, FontStyle.Regular);
            completionList.ItemHeight = completionList.Font.Height + 2;
            completionList.DrawMode = DrawMode.OwnerDrawFixed;
            completionList.DrawItem += new DrawItemEventHandler(CLDrawListItem);
            completionList.Click += new EventHandler(CLClick);
            completionList.DoubleClick += new EventHandler(CLDoubleClick);

            listContainer = new ToolStripControlHost(completionList);
            listContainer.AutoToolTip = false;
            listContainer.AutoSize = false;
            listContainer.Margin = Padding.Empty;
            listContainer.Padding = Padding.Empty;
            
            listHost.Items.Add(listContainer);
        }

        #endregion

        #region Public List Properties

        /// <summary>
        /// Is the control active? 
        /// </summary> 
        public bool Active
        {
            get { return isActive; }
        }

        /// <summary>
        /// Gets if the mouse is currently inside the completion list control
        /// </summary>
        public bool HasMouseIn
        {
            get
            {
                if (!isActive || completionList == null) return false;
                return completionList.ClientRectangle.Contains(completionList.PointToClient(Control.MousePosition));
            }
        }

        /// <summary>
        /// Retrieves the currently selected label, or null if none selected
        /// </summary>
        public string SelectedLabel
        {
            get
            {
                if (completionList == null) return null;
                ICompletionListItem selected = completionList.SelectedItem as ICompletionListItem;
                return (selected == null) ? null : selected.Label;
            }
        }

        /// <summary>
        /// Gets the target of the current completion list control
        /// </summary>
        public ICompletionListHost Host
        {
            get { return host; }
        }

        /// <summary>
        /// Gets the help tip associated with the completion list
        /// </summary>
        public RichToolTip Tip
        {
            get
            {
                if (tip == null)
                    tip = new RichToolTip(host.Owner);
                return tip;
            }
            internal set { tip = value; }
        }

        /// <summary>
        /// Gets the method call tip associated with the completion list
        /// </summary>
        public MethodCallTip CallTip
        {
            get
            {
                if (callTip == null)
                    callTip = new MethodCallTip(PluginBase.MainForm);
                return callTip;
            }
            internal set { callTip = value; }
        }

        #endregion

        #region CompletionList Methods

        /// <summary>
        /// Checks if the position is valid
        /// </summary> 
        public bool CheckPosition(int position)
        {
            return position == currentPos;
        }

        /// <summary>
        /// Shows the completion list
        /// </summary> 
        public void Show(List<ICompletionListItem> itemList, bool autoHide, string select)
        {
            if (!string.IsNullOrEmpty(select))
            {
                int maxLen = 0;
                foreach (ICompletionListItem item in itemList)
                    if (item.Label.Length > maxLen) maxLen = item.Label.Length;
                maxLen = Math.Min(256, maxLen);
                if (select.Length > maxLen) select = select.Substring(0, maxLen);
                currentWord = select;
            }
            else currentWord = null;
            Show(itemList, autoHide);
        }

        /// <summary>
        /// Shows the completion list
        /// </summary>
        public void Show(List<ICompletionListItem> itemList, bool autoHide)
        {
            try
            {
                if (!host.IsEditable)
                {
                    if (isActive) Hide();
                    return;
                }
                if ((itemList == null) || (itemList.Count == 0))
                {
                    if (isActive) Hide();
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
            // state
            allItems = itemList;
            autoHideList = autoHide;
            noAutoInsert = false;
            word = "";
            if (currentWord != null)
            {
                word = currentWord;
                currentWord = null;
            }
            MinWordLength = 1;
            fullList = (word.Length == 0) || !autoHide || !PluginBase.MainForm.Settings.AutoFilterList;
            lastIndex = 0;
            exactMatchInList = false;
            if (host.SelectionStart == host.SelectionEnd)
                startPos = host.CurrentPos - word.Length;
            else
                startPos = host.SelectionStart;
            currentPos = host.SelectionEnd; // sci.CurrentPos;
            defaultItem = null;
            // populate list
            needResize = true;
            tempo.Enabled = autoHide && (PluginBase.MainForm.Settings.DisplayDelay > 0);
            if (tempo.Enabled) tempo.Interval = PluginBase.MainForm.Settings.DisplayDelay;
            FindWordStartingWith(word);
            // state
            isActive = true;
            tempoTip.Enabled = false;
            showTime = DateTime.Now.Ticks;
            disableSmartMatch = noAutoInsert || PluginBase.MainForm.Settings.DisableSmartMatch;
        }

        /// <summary>
        /// Set default selected item in completion list
        /// </summary>
        public void SelectItem(string name)
        {
            int p = name.IndexOf('<');
            if (p > 1) name = name.Substring(0, p) + "<T>";
            string pname = (name.IndexOf('.') < 0) ? "." + name : null;
            ICompletionListItem found = null;
            foreach (ICompletionListItem item in completionList.Items)
            {
                if (item.Label == name)
                {
                    defaultItem = item;
                    completionList.SelectedItem = item;
                    return;
                }
                if (pname != null && item.Label.EndsWith(pname)) found = item;
            }
            if (found != null)
            {
                defaultItem = found;
                completionList.SelectedItem = found;
            }
        }

        /// <summary>
        /// Require that completion items are explicitely inserted (Enter, Tab, mouse-click)
        /// </summary>
        public void DisableAutoInsertion()
        {
            noAutoInsert = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayList(Object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!host.IsEditable) return;
            ListBox cl = completionList;
            if (cl.Items.Count == 0) return;

            // measure control
            var listSize = new Size();
            if (needResize && !string.IsNullOrEmpty(widestLabel))
            {
                needResize = false;
                Graphics g = cl.CreateGraphics();
                SizeF size = g.MeasureString(widestLabel, cl.Font);
                listSize.Width = (int)Math.Min(Math.Max(size.Width + 40, 100), 400) + ScaleHelper.Scale(10);
            }
            else listSize.Width = cl.Width;
            int newHeight = Math.Min(cl.Items.Count, 10) * cl.ItemHeight + 4;
            listSize.Height = newHeight != cl.Height ? newHeight : cl.Height;
            cl.Size = listContainer.Size = listHost.Size = listSize;
            // place control
            UpdatePosition();
        }

        public void Redraw()
        {
            Color back = PluginBase.MainForm.GetThemeColor("CompletionList.BackColor");
            completionList.BackColor = back == Color.Empty ? System.Drawing.SystemColors.Window : back;
        }

        /// <summary>
        /// Hide completion list
        /// </summary> 	
        public void Hide()
        {
            if (completionList != null && isActive)
            {
                RemoveHandlers();
                tempo.Enabled = false;
                isActive = false;
                fullList = false;
                bool visible = listHost.Visible;
                listHost.Close();
                if (completionList.Items.Count > 0) completionList.Items.Clear();
                currentItem = null;
                allItems = null;
                Tip.Hide();
                if (visible && OnHidden != null) OnHidden(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Cancel completion list with event
        /// </summary> 	
        public void Hide(char trigger)
        {
            if (completionList != null && isActive)
            {
                Hide();
                if (OnCancel != null)
                {
                    if (!host.IsEditable) return;
                    OnCancel(host.Owner, currentPos, currentWord, trigger, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary> 
        public void SelectWordInList(string tail)
        {
            if (!host.IsEditable)
            {
                Hide();
                return;
            }
            currentWord = tail;
            currentPos += tail.Length;
            host.SetSelection(currentPos, currentPos);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CLDrawListItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            ICompletionListItem item = completionList.Items[e.Index] as ICompletionListItem;
            e.DrawBackground();
            Color fore = PluginBase.MainForm.GetThemeColor("CompletionList.ForeColor");
            bool selected = (e.State & DrawItemState.Selected) > 0;
            Brush textBrush = (selected) ? SystemBrushes.HighlightText : fore == Color.Empty ? SystemBrushes.WindowText : new SolidBrush(fore);
            Brush packageBrush = Brushes.Gray;
            Rectangle tbounds = new Rectangle(ScaleHelper.Scale(18), e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
            if (item != null)
            {
                Graphics g = e.Graphics;
                float newHeight = e.Bounds.Height - 2;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(item.Icon, 1, e.Bounds.Top + ((e.Bounds.Height - newHeight) / 2), newHeight, newHeight);
                int p = item.Label.LastIndexOf('.');
                if (p > 0 && !selected)
                {
                    string package = item.Label.Substring(0, p + 1);
                    g.DrawString(package, e.Font, packageBrush, tbounds, StringFormat.GenericDefault);
                    int left = tbounds.Left + DrawHelper.MeasureDisplayStringWidth(e.Graphics, package, e.Font) - 2;
                    if (left < tbounds.Right) g.DrawString(item.Label.Substring(p + 1), e.Font, textBrush, left, tbounds.Top, StringFormat.GenericDefault);
                }
                else g.DrawString(item.Label, e.Font, textBrush, tbounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
            if ((item != null) && ((e.State & DrawItemState.Selected) > 0))
            {
                Tip.Hide();
                currentItem = item;
                tempoTip.Stop();
                tempoTip.Start();
            }
        }

        /// <summary>
        /// Display item information in tooltip
        /// </summary> 
        public void UpdateTip(Object sender, System.Timers.ElapsedEventArgs e)
        {
            tempoTip.Stop();
            if (currentItem == null || listHost.Opacity != 1)
                return;

            Tip.SetText(currentItem.Description ?? "", false);
            Tip.Redraw(false);

            var screen = Screen.FromControl(listHost);
            int rightWidth = screen.WorkingArea.Right - listHost.Right - 10;
            int leftWidth = listHost.Left;

            Point posTarget = new Point(listHost.Right, listHost.Top);
            int widthTarget = rightWidth;
            if (rightWidth < 220 && leftWidth > 220)
            {
                widthTarget = leftWidth;
                posTarget.X = 0;
            }

            Tip.Location = posTarget;
            Tip.AutoSize(widthTarget, 500);

            if (widthTarget == leftWidth)
                Tip.Location = new Point(listHost.Left - Tip.Size.Width, posTarget.Y);

            Tip.Show();
        }

        private void UpdatePosition()
        {
            Point coord = host.GetPositionFromCharIndex(startPos);
            // Check for completion list outside of control view
            if (coord.X < 0 || coord.X > host.Owner.Width || coord.Y < 0 || coord.Y > host.Owner.Height)
            {
                Hide();
                return;
            }
            coord = host.Owner.PointToScreen(coord);
            coord.X += host.Owner.Left;
            var screen = Screen.FromHandle(host.Owner.Handle);
            listUp = CallTip.CallTipActive || (coord.Y + listHost.Height > screen.WorkingArea.Bottom && coord.Y - listHost.Height > screen.WorkingArea.Top);
            if (listUp) coord.Y -= listHost.Height;
            else coord.Y += host.GetLineHeight();
            // Keep on screen area
            if (coord.X + listHost.Width > screen.WorkingArea.Right)
            {
                coord.X = screen.WorkingArea.Right - listHost.Width;
            }

            if (listHost.Visible)
                listHost.Show(coord);
            else
            {
                Redraw();
                if (OnShowing != null) OnShowing(this, EventArgs.Empty);
                listHost.Opacity = 1;
                listHost.Show(coord);
                if (CallTip.CallTipActive) CallTip.PositionControl(((ScintillaControl)host.Owner));
                AddHandlers();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void CLClick(Object sender, System.EventArgs e)
        {
            if (!host.IsEditable)
                Hide();
        }

        /// <summary>
        /// 
        /// </summary> 
        private void CLDoubleClick(Object sender, System.EventArgs e)
        {
            if (!host.IsEditable)
            {
                Hide();
                return;
            }
            ReplaceText('\0');
        }

        /// <summary>
        /// Filter the completion list with the letter typed
        /// </summary> 
        public void FindWordStartingWith(string word)
        {
            if (word == null) word = "";
            int len = word.Length;
            int maxLen = 0;
            int lastScore = 0;
            // FILTER ITEMS
            if (PluginBase.MainForm.Settings.AutoFilterList || fullList)
            {
                List<ICompletionListItem> found;
                if (len == 0)
                {
                    found = allItems;
                    lastIndex = 0;
                    exactMatchInList = false;
                    smartMatchInList = true;
                }
                else
                {
                    List<ItemMatch> temp = new List<ItemMatch>(allItems.Count);
                    int n = allItems.Count;
                    int i = 0;
                    int score;
                    lastScore = 99;
                    ICompletionListItem item;
                    exactMatchInList = false;
                    smartMatchInList = false;
                    while (i < n)
                    {
                        item = allItems[i];
                        // compare item's label with the searched word
                        score = SmartMatch(item.Label, word, len);
                        if (score > 0)
                        {
                            // first match found
                            if (!smartMatchInList || score < lastScore)
                            {
                                lastScore = score;
                                lastIndex = temp.Count;
                                smartMatchInList = true;
                                exactMatchInList = score < 5 && word == this.word;
                            }
                            temp.Add(new ItemMatch(score, item));
                            if (item.Label.Length > maxLen)
                            {
                                widestLabel = item.Label;
                                maxLen = widestLabel.Length;
                            }
                        }
                        else if (fullList) temp.Add(new ItemMatch(0, item));
                        i++;
                    }
                    // filter
                    found = new List<ICompletionListItem>(temp.Count);
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (j == lastIndex) lastIndex = found.Count;
                        if (temp[j].Score - lastScore < 3) found.Add(temp[j].Item);
                    }
                }
                // no match?
                if (!smartMatchInList)
                {
                    if (autoHideList && PluginBase.MainForm.Settings.EnableAutoHide && (len == 0 || len > 255))
                    {
                        Hide('\0');
                    }
                    else
                    {
                        // smart match
                        if (word.Length > 0)
                        {
                            FindWordStartingWith(word.Substring(0, len - 1));
                        }
                        if (!smartMatchInList && autoHideList && PluginBase.MainForm.Settings.EnableAutoHide)
                        {
                            Hide('\0');
                        }
                    }
                    return;
                }
                fullList = false;
                // reset timer
                if (tempo.Enabled)
                {
                    tempo.Enabled = false;
                    tempo.Enabled = true;
                }
                // is update needed?
                if (completionList.Items.Count == found.Count)
                {
                    int n = completionList.Items.Count;
                    bool changed = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (completionList.Items[i] != found[i])
                        {
                            changed = true;
                            break;
                        }
                    }
                    if (!changed)
                    {
                        // preselected item
                        if (defaultItem != null)
                        {
                            if (lastScore > 3 || (lastScore > 2 && defaultItem.Label.StartsWith(word, StringComparison.OrdinalIgnoreCase)))
                            {
                                lastIndex = lastIndex = TestDefaultItem(lastIndex, word, len);
                            }
                        }
                        completionList.SelectedIndex = lastIndex;
                        return;
                    }
                }
                // update
                try
                {
                    completionList.BeginUpdate();
                    completionList.Items.Clear();
                    foreach (ICompletionListItem item in found)
                    {
                        completionList.Items.Add(item);
                        if (item.Label.Length > maxLen)
                        {
                            widestLabel = item.Label;
                            maxLen = widestLabel.Length;
                        }
                    }
                    int topIndex = lastIndex;
                    if (defaultItem != null)
                    {
                        if (lastScore > 3 || (lastScore > 2 && defaultItem.Label.StartsWith(word, StringComparison.OrdinalIgnoreCase)))
                        {
                            lastIndex = TestDefaultItem(lastIndex, word, len);
                        }
                    }
                    // select first item
                    completionList.TopIndex = topIndex;
                    completionList.SelectedIndex = lastIndex;
                }
                catch (Exception ex)
                {
                    Hide('\0');
                    ErrorManager.ShowError(/*"Completion list populate error.", */ ex);
                    return;
                }
                finally
                {
                    completionList.EndUpdate();
                }
                // update list
                if (!tempo.Enabled) DisplayList(null, null);
            }
            // NO FILTER
            else
            {
                int n = completionList.Items.Count;
                ICompletionListItem item;
                while (lastIndex < n)
                {
                    item = completionList.Items[lastIndex] as ICompletionListItem;
                    if (string.Compare(item.Label, 0, word, 0, len, true) == 0)
                    {
                        completionList.SelectedIndex = lastIndex;
                        completionList.TopIndex = lastIndex;
                        exactMatchInList = true;
                        return;
                    }
                    lastIndex++;
                }
                // no match
                if (autoHideList && PluginBase.MainForm.Settings.EnableAutoHide) Hide('\0');
                else exactMatchInList = false;
            }
        }

        private int TestDefaultItem(int index, string word, int len)
        {
            if (defaultItem != null && completionList.Items.Contains(defaultItem))
            {
                int score = (len == 0) ? 1 : SmartMatch(defaultItem.Label, word, len);
                if (score > 0 && score < 6) return completionList.Items.IndexOf(defaultItem);
            }
            return index;
        }

        public int SmartMatch(string label, string word, int len)
        {
            if (label.Length < len) return 0;

            // simple matching
            if (disableSmartMatch)
            {
                if (label.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                {
                    if (label.StartsWith(word)) return 1;
                    else return 5;
                }
                return 0;
            }
            
            // try abbreviation
            bool firstUpper = char.IsUpper(word[0]);
            if (firstUpper)
            {
                int abbr = IsAbbreviation(label, word);
                if (abbr > 0) return abbr;
            }

            int p = label.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            if (p >= 0)
            {
                int p2;
                if (firstUpper) // try case sensitive search
                {
                    p2 = label.IndexOf(word);
                    if (p2 >= 0)
                    {
                        int p3 = label.LastIndexOf("." + word); // in qualified type name
                        if (p3 > 0)
                        {
                            if (p3 == label.LastIndexOf('.'))
                            {
                                if (label.EndsWith("." + word)) return 1;
                                else return 3;
                            }
                            else return 4;
                        }
                    }
                    if (p2 == 0)
                    {
                        if (word == label) return 1;
                        else return 2;
                    }
                    else if (p2 > 0) return 4;
                }

                p2 = label.LastIndexOf("." + word, StringComparison.OrdinalIgnoreCase); // in qualified type name
                if (p2 > 0)
                {
                    if (p2 == label.LastIndexOf('.'))
                    {
                        if (label.EndsWith("." + word, StringComparison.OrdinalIgnoreCase)) return 2;
                        else return 4;
                    }
                    else return 5;
                }
                if (p == 0)
                {
                    if (label.Equals(word, StringComparison.OrdinalIgnoreCase))
                    {
                        if (label.Equals(word)) return 1;
                        else return 2;
                    }
                    else return 3;
                }
                else
                {
                    int p4 = label.IndexOf(':');
                    if (p4 > 0) return SmartMatch(label.Substring(p4 + 1), word, len);
                    return 5;
                }
            }

            // loose
            int n = label.Length;
            int firstChar = label.IndexOf(word[0].ToString(), StringComparison.OrdinalIgnoreCase);
            int i = 1;
            p = firstChar;
            while (i < len && p >= 0)
            {
                p = label.IndexOf(word[i++].ToString(), p + 1, StringComparison.OrdinalIgnoreCase);
            }
            return (p > 0) ? 7 : 0;
        }

        public int IsAbbreviation(string label, string word)
        {
            int len = word.Length;
            int i = 1;
            char c = word[0];
            int p;
            int p2;
            int score = 0;
            if (label[0] == c) { p2 = 0; score = 1; }
            else if (label.IndexOf('.') < 0)
            {
                p2 = label.IndexOf(c);
                if (p2 < 0) return 0;
                score = 3;
            }
            else
            {
                p2 = label.IndexOf("." + c);
                if (p2 >= 0) { score = 2; p2++; }
                else
                {
                    p2 = label.IndexOf(c);
                    if (p2 < 0) return 0;
                    score = 4;
                }
            }
            int dist = 0;

            while (i < len)
            {
                p = p2;
                c = word[i++];
                if (char.IsUpper(c)) p2 = label.IndexOf(c.ToString(), p + 1);
                else p2 = label.IndexOf(c.ToString(), p + 1, StringComparison.OrdinalIgnoreCase);
                if (p2 < 0) return 0;

                int ups = 0;
                for (int i2 = p + 1; i2 < p2; i2++)
                    if (label[i2] == '_') { ups = 0; }
                    else if (char.IsUpper(label[i2])) ups++;
                score += Math.Min(3, ups); // malus if skipped upper chars

                dist += p2 - p;
            }
            if (dist == len - 1)
            {
                if (label == word || label.EndsWith("." + word)) return 1;
                return score;
            }
            else return score + 2;
        }

        /// <summary>
        /// 
        /// </summary> 
        public bool ReplaceText(char trigger)
        {
            return ReplaceText("", trigger);
        }

        /// <summary>
        /// 
        /// </summary> 
        public bool ReplaceText(string tail, char trigger)
        {
            String triggers = PluginBase.Settings.InsertionTriggers ?? "";
            if (triggers.Length > 0 && Regex.Unescape(triggers).IndexOf(trigger) < 0) return false;

            try
            {
                ICompletionListItem item = null;
                if (completionList.SelectedIndex >= 0)
                {
                    item = completionList.Items[completionList.SelectedIndex] as ICompletionListItem;
                }
                Hide();
                if (item != null)
                {
                    String replace = item.Value;
                    if (replace != null)
                    {
                        if (word != null && tail.Length > 0)
                        {
                            if (replace.StartsWith(word, StringComparison.OrdinalIgnoreCase) && replace.IndexOf(tail) >= word.Length)
                            {
                                replace = replace.Substring(0, replace.IndexOf(tail));
                            }
                        }
                        host.BeginUndoAction();
                        host.SetSelection(startPos, host.CurrentPos);
                        host.SelectedText = replace;
                        if (OnInsert != null) OnInsert(host.Owner, startPos, replace, trigger, item);
                        if (tail.Length > 0) host.SelectedText = tail;
                    }
                    return true;
                }
                return false;
            }
            finally
            {
                host.EndUndoAction();
            }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// 
        /// </summary> 
        public IntPtr GetHandle()
        {
            return completionList.Handle;
        }

        private void AddHandlers()
        {
            Application.AddMessageFilter(this);
            host.LostFocus += Target_LostFocus;
            host.MouseDown += Target_MouseDown;
            host.KeyDown += Target_KeyDown;
            host.KeyPress += Target_KeyPress;
            host.PositionChanged += Target_PositionChanged;
        }

        private void RemoveHandlers()
        {
            Application.RemoveMessageFilter(this);
            host.LostFocus -= Target_LostFocus;
            host.MouseDown -= Target_MouseDown;
            host.KeyDown -= Target_KeyDown;
            host.KeyPress -= Target_KeyPress;
            host.PositionChanged -= Target_PositionChanged;
        }

        private void Target_LostFocus(object sender, EventArgs e)
        {
            if (!listHost.ContainsFocus && !Tip.Focused && !CallTip.Focused)
                Hide();
        }

        private void Target_MouseDown(object sender, MouseEventArgs e)
        {
            if (host.CurrentPos != currentPos)
                Hide();
        }

        private void Target_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = e.Handled = HandleKeys(e.KeyData);
        }

        private void Target_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
                OnChar(e.KeyChar);
        }

        private void Target_PositionChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        /// <summary>
        /// 
        /// </summary> 
        public bool OnChar(char c)
        {
            // TODO: Inject these values or get from host
            string characterClass = ScintillaControl.Configuration.GetLanguage(PluginBase.MainForm.CurrentDocument.SciControl.ConfigurationLanguage).characterclass.Characters;
            if (characterClass.IndexOf(c) >= 0)
            {
                word += c;
                currentPos++;
                FindWordStartingWith(word);
                return true;
            }
            else if (noAutoInsert)
            {
                Hide('\0');
                // handle this char
                return false;
            }
            else
            {
                // check for fast typing
                long millis = (DateTime.Now.Ticks - showTime) / 10000;
                if (!exactMatchInList && (word.Length > 0 || (millis < 400 && defaultItem == null)))
                {
                    Hide('\0');
                }
                else if (word.Length == 0 && (currentItem == null || currentItem == allItems[0]) && defaultItem == null)
                {
                    Hide('\0');
                }
                else if (word.Length > 0 || c == '.' || c == '(' || c == '[' || c == '<' || c == ',' || c == ';')
                {
                    ReplaceText(c.ToString(), c);
                }
                // handle this char
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary> 
        public bool HandleKeys(Keys key)
        {
            int index;
            switch (key)
            {
                case Keys.Back:
                    if (word.Length > MinWordLength)
                    {
                        word = word.Substring(0, word.Length - 1);
                        currentPos = host.CurrentPos - 1;
                        lastIndex = 0;
                        FindWordStartingWith(word);
                    }
                    else Hide((char)8);
                    return false;

                case Keys.Enter:
                    if (noAutoInsert || !ReplaceText('\n'))
                    {
                        Hide();
                        return false;
                    }
                    return true;

                case Keys.Tab:
                    if (!ReplaceText('\t'))
                    {
                        Hide();
                        return false;
                    }
                    return true;

                case Keys.Space:
                    if (noAutoInsert) Hide();
                    return false;

                case Keys.Up:
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!listHost.Visible)
                    {
                        Hide();
                        return false;
                    }
                    // go up the list
                    if (completionList.SelectedIndex > 0)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex - 1;
                        completionList.SelectedIndex = index;
                    }
                    // wrap
                    else if (PluginBase.MainForm.Settings.WrapList)
                    {
                        RefreshTip();
                        index = completionList.Items.Count - 1;
                        completionList.SelectedIndex = index;
                    }
                    break;

                case Keys.Down:
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!listHost.Visible)
                    {
                        Hide();
                        return false;
                    }
                    // go down the list
                    if (completionList.SelectedIndex < completionList.Items.Count - 1)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex + 1;
                        completionList.SelectedIndex = index;
                    }
                    // wrap
                    else if (PluginBase.MainForm.Settings.WrapList)
                    {
                        RefreshTip();
                        index = 0;
                        completionList.SelectedIndex = index;
                    }
                    break;

                case Keys.PageUp:
                /*case Keys.PageUp | Keys.Control:*/ // Used to navigate through documents
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!listHost.Visible)
                    {
                        Hide();
                        return false;
                    }
                    // go up the list
                    if (completionList.SelectedIndex > 0)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex - completionList.Height / completionList.ItemHeight;
                        if (index < 0) index = 0;
                        completionList.SelectedIndex = index;
                    }
                    break;

                case Keys.PageDown:
                /*case Keys.PageDown | Keys.Control:*/ // Used to navigate through documents
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!listHost.Visible)
                    {
                        Hide();
                        return false;
                    }
                    // go down the list
                    if (completionList.SelectedIndex < completionList.Items.Count - 1)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex + completionList.Height / completionList.ItemHeight;
                        if (index > completionList.Items.Count - 1) index = completionList.Items.Count - 1;
                        completionList.SelectedIndex = index;
                    }
                    break;

                case Keys.Home:
                    noAutoInsert = false;
                    // go down the list
                    if (completionList.SelectedIndex > 0)
                    {
                        RefreshTip();
                        index = 0;
                        completionList.SelectedIndex = index;
                    }

                    break;

                case Keys.End:
                    noAutoInsert = false;
                    // go down the list
                    if (completionList.SelectedIndex < completionList.Items.Count - 1)
                    {
                        RefreshTip();
                        index = completionList.Items.Count - 1;
                        completionList.SelectedIndex = index;
                    }

                    break;

                case (Keys.Control | Keys.Space):
                    break;

                case Keys.Left:
                case Keys.Right:
                    Hide();
                    return false;

                case Keys.Escape:
                    Hide((char) 27);
                    break;

                default:
                    Keys modifiers = key & Keys.Modifiers;
                    if (modifiers == Keys.Control)
                    {
                        key = key & Keys.KeyCode;
                        if (key > 0 && key != Keys.ControlKey && key != Keys.Down && key != Keys.Up)
                            Hide();
                    }
                    else if (modifiers == Keys.Shift)
                    {
                        key = key & Keys.KeyCode;
                        if (key == Keys.Down || key == Keys.Up || key == Keys.Left || key == Keys.Right ||
                            key == Keys.PageUp || key == Keys.PageDown || key == Keys.Home || key == Keys.End)
                            Hide();
                    }
                    else if (modifiers == (Keys.Shift | Keys.Control))
                    {
                        key = key & Keys.KeyCode;
                        if (key == Keys.Left || key == Keys.Right)
                            Hide();
                    }

                    return false;
            }
            return true;
        }

        private void RefreshTip()
        {
            Tip.Hide();
            tempoTip.Enabled = false;
        }

        #endregion

        #region Controls fading on Control key

        internal void FadeOut()
        {
            if (listHost.Opacity != 1) return;
            Tip.Hide();
            listHost.Opacity = 0;
        }

        internal void FadeIn()
        {
            if (listHost.Opacity == 1) return;
            listHost.Opacity = 1;
        }

        #endregion

        #region Global Hook

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == Win32.WM_MOUSEWHEEL) // capture all MouseWheel events 
            {
                if (Tip.Focused || CallTip.Focused) return false;
                if (Win32.ShouldUseWin32())
                {
                    Win32.SendMessage(completionList.Handle, m.Msg, (Int32)m.WParam, (Int32)m.LParam);
                    return true;
                }
            }
            else if (m.Msg == Win32.WM_KEYDOWN)
            {
                if (Tip.Focused || CallTip.Focused) return false;
                if ((int)m.WParam == 17) // Ctrl
                {
                    if (Active) FadeOut();
                    if (CallTip.CallTipActive) CallTip.FadeOut();
                }
                else if ((int) m.WParam == 112) // F1 - since it's by default set as a shortcut we are required to handle it at a lower level
                {
                    UITools.Manager.ShowDetails = !UITools.Manager.ShowDetails;
                    bool retVal = false;
                    if (Active)
                    {
                        UpdateTip(null, null);
                        retVal = true;
                    }
                    else
                    {
                        if (Tip.Visible)
                        {
                            Tip.UpdateTip(PluginBase.MainForm.CurrentDocument.SciControl);
                            retVal = true;
                        }
                        if (CallTip.Visible)
                        {
                            callTip.UpdateTip(PluginBase.MainForm.CurrentDocument.SciControl);
                            retVal = true;
                        }
                    }

                    return retVal;
                }
            }
            else if (m.Msg == Win32.WM_KEYUP)
            {
                if (Tip.Focused || CallTip.Focused) return false;
                if ((int)m.WParam == 17 || (int)m.WParam == 18) // Ctrl / AltGr
                {
                    if (Active) FadeIn();
                    if (CallTip.CallTipActive) CallTip.FadeIn();
                }
            }
            return false;
        }
        
        #endregion

        #region Unfocusable List

        // If by any chance this is not compatible with CrossOver, or we want some alternative 100% crossplatform compatible, a custom fully managed control that cannot be focused could be developed
        private class ListBoxEx : ListBox
        {
            protected override void DefWndProc(ref Message m)
            {
                const int WM_MOUSEACTIVATE = 0x21;
                const int WM_LBUTTONDOWN = 0x201;
                const int MA_NOACTIVATE = 0x0003;
                
                switch (m.Msg)
                {
                    case WM_MOUSEACTIVATE:
                        m.Result = (IntPtr)MA_NOACTIVATE;
                        return;
                    case WM_LBUTTONDOWN:
                        SelectedIndex = IndexFromPoint((short)(m.LParam.ToInt32() & 0xFFFF), (short)((m.LParam.ToInt32() & 0xFFFF0000) >> 16));
                        m.Result = IntPtr.Zero;
                        return;
                }
                base.DefWndProc(ref m);
            }
        }

        #endregion

    }

    struct ItemMatch
    {
        public int Score;
        public ICompletionListItem Item;

        public ItemMatch(int score, ICompletionListItem item)
        {
            Score = score;
            Item = item;
        }
    }

}
