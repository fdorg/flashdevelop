using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Managers;
using PluginCore.Helpers;
using ScintillaNet;

namespace PluginCore.Controls
{
    public delegate void InsertedTextHandler(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item);

    public class CompletionList
    {
        public static event InsertedTextHandler OnInsert;
        public static event InsertedTextHandler OnCancel;

        /// <summary>
        /// Properties of the class 
        /// </summary> 
        private static System.Timers.Timer tempo;
        private static System.Timers.Timer tempoTip;
        private static ListBox completionList;
        
        #region State Properties

        private static bool disableSmartMatch;
        private static ICompletionListItem currentItem;
        private static IList<ICompletionListItem> allItems;
        private static bool exactMatchInList;
        private static bool smartMatchInList;
        private static bool autoHideList;
        private static bool noAutoInsert;
        private static bool isActive;
        internal static bool listUp;
        private static bool fullList;
        private static int startPos;
        private static int currentPos;
        private static int lastIndex;
        private static string currentWord;
        private static string word;
        private static bool needResize;
        private static string widestLabel;
        private static long showTime;
        private static ICompletionListItem defaultItem;

        /// <summary>
        /// Set to 0 after calling .Show to keep the completion list active 
        /// when the text was erased completely (using backspace)
        /// </summary>
        public static int MinWordLength;

        #endregion
        
        #region Control Creation
        
        /// <summary>
        /// Creates the control 
        /// </summary> 
        public static void CreateControl(IMainForm mainForm)
        {
            tempo = new System.Timers.Timer();
            tempo.SynchronizingObject = (Form)mainForm;
            tempo.Elapsed += DisplayList;
            tempo.AutoReset = false;
            tempoTip = new System.Timers.Timer();
            tempoTip.SynchronizingObject = (Form)mainForm;
            tempoTip.Elapsed += UpdateTip;
            tempoTip.AutoReset = false;
            tempoTip.Interval = 800;
            
            completionList = new ListBox();
            completionList.Font = new Font(PluginBase.Settings.DefaultFont, FontStyle.Regular);
            completionList.Visible = false;
            completionList.Location = new Point(400,200);
            completionList.ItemHeight = completionList.Font.Height + 2;
            completionList.Size = new Size(180, 100);
            completionList.DrawMode = DrawMode.OwnerDrawFixed;
            completionList.DrawItem += CLDrawListItem;
            completionList.Click += CLClick;
            completionList.DoubleClick += CLDoubleClick;
            mainForm.Controls.Add(completionList);
        }
        
        #endregion
        
        #region Public List Properties

        /// <summary>
        /// Is the control active? 
        /// </summary> 
        public static bool Active => isActive;

        /// <summary>
        /// 
        /// </summary>
        public static bool HasMouseIn
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
        public static string SelectedLabel => (completionList?.SelectedItem as ICompletionListItem)?.Label;

        #endregion
        
        #region CompletionList Methods
        
        /// <summary>
        /// Checks if the position is valid
        /// </summary> 
        public static bool CheckPosition(int position) => position == currentPos;

        /// <summary>
        /// Shows the completion list
        /// </summary> 
        public static void Show(IList<ICompletionListItem> itemList, bool autoHide, string select)
        {
            if (!string.IsNullOrEmpty(select))
            {
                int maxLen = 0;
                foreach (var item in itemList)
                    if (item.Label.Length > maxLen)
                        maxLen = item.Label.Length;
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
        public static void Show(IList<ICompletionListItem> itemList, bool autoHide)
        {
            var doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable) return;
            var sci = doc.SciControl;
            try
            {
                if ((itemList == null) || (itemList.Count == 0))
                {
                    if (isActive) Hide();
                    return;
                }
                if (sci == null) 
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
            if (sci.SelectionStart == sci.SelectionEnd)
                startPos = sci.CurrentPos - word.Length;
            else 
                startPos = sci.SelectionStart;
            currentPos = sci.SelectionEnd; // sci.CurrentPos;
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
            UITools.Manager.LockControl(sci);
            faded = false;
        }

        /// <summary>
        /// Set default selected item in completion list
        /// </summary>
        public static void SelectItem(string name)
        {
            var pname = !name.Contains('.') ? "." + name : null;
            ICompletionListItem found = null;
            foreach (ICompletionListItem item in completionList.Items)
            {
                if (item.Label == name)
                {
                    defaultItem = item;
                    completionList.SelectedItem = item;
                    return;
                }
                if (pname != null && item.Label.EndsWithOrdinal(pname)) found = item;
            }
            if (found != null)
            {
                defaultItem = found;
                completionList.SelectedItem = found;
            }
        }

        /// <summary>
        /// Require that completion items are explicitly inserted (Enter, Tab, mouse-click)
        /// </summary>
        public static void DisableAutoInsertion()
        {
            noAutoInsert = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DisplayList(object sender, System.Timers.ElapsedEventArgs e)
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable) return;
            ScintillaControl sci = doc.SciControl;
            ListBox cl = completionList;
            if (cl.Items.Count == 0) return;

            // measure control
            if (needResize && !string.IsNullOrEmpty(widestLabel))
            {
                needResize = false;
                Graphics g = cl.CreateGraphics();
                SizeF size = g.MeasureString(widestLabel, cl.Font);
                cl.Width = (int)Math.Min(Math.Max(size.Width + 40, 100), ScaleHelper.Scale(400)) + ScaleHelper.Scale(10);
            }
            int newHeight = Math.Min(cl.Items.Count, 10) * cl.ItemHeight + 4;
            if (newHeight != cl.Height) cl.Height = newHeight;
            // place control
            Point coord = new Point(sci.PointXFromPosition(startPos), sci.PointYFromPosition(startPos));
            listUp = UITools.CallTip.CallTipActive || (coord.Y+cl.Height > sci.Height);
            coord = sci.PointToScreen(coord);
            coord = ((Form)PluginBase.MainForm).PointToClient(coord);
            cl.Left = coord.X-20 + sci.Left;
            if (listUp) cl.Top = coord.Y-cl.Height;
            else cl.Top = coord.Y + UITools.Manager.LineHeight(sci);
            // Keep on control area
            if (cl.Right > ((Form)PluginBase.MainForm).ClientRectangle.Right)
            {
                cl.Left = ((Form)PluginBase.MainForm).ClientRectangle.Right - cl.Width;
            }
            if (!cl.Visible)
            {
                Redraw();
                cl.Show();
                cl.BringToFront();
                if (UITools.CallTip.CallTipActive) UITools.CallTip.PositionControl(sci);
            }
        }

        static void Redraw()
        {
            Color back = PluginBase.MainForm.GetThemeColor("CompletionList.BackColor");
            completionList.BackColor = back == Color.Empty ? SystemColors.Window : back;
        }

        /// <summary>
        /// Hide completion list
        /// </summary>  
        public static void Hide()
        {
            if (completionList != null && isActive) 
            {
                tempo.Enabled = false;
                isActive = false;
                fullList = false;
                faded = false;
                completionList.Visible = false;
                if (completionList.Items.Count > 0) completionList.Items.Clear();
                currentItem = null;
                allItems = null;
                UITools.Tip.Hide();
                if (!UITools.CallTip.CallTipActive) UITools.Manager.UnlockControl();
            }
        }

        /// <summary>
        /// Cancel completion list with event
        /// </summary>  
        public static void Hide(char trigger)
        {
            if (completionList != null && isActive)
            {
                Hide();
                if (OnCancel != null)
                {
                    ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                    if (!doc.IsEditable) return;
                    OnCancel(doc.SciControl, currentPos, currentWord, trigger, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary> 
        static public void SelectWordInList(string tail)
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable)
            {
                Hide();
                return;
            }
            ScintillaControl sci = doc.SciControl;
            currentWord = tail;
            currentPos += tail.Length;
            sci.SetSel(currentPos, currentPos);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CLDrawListItem(object sender, DrawItemEventArgs e)
        {
            ICompletionListItem item = completionList.Items[e.Index] as ICompletionListItem;
            e.DrawBackground();
            Color fore = PluginBase.MainForm.GetThemeColor("CompletionList.ForeColor", SystemColors.WindowText);
            Color sel = PluginBase.MainForm.GetThemeColor("CompletionList.SelectedTextColor", SystemColors.HighlightText);
            bool selected = (e.State & DrawItemState.Selected) > 0;
            Brush textBrush = (selected) ? new SolidBrush(sel) : new SolidBrush(fore);
            Brush packageBrush = new SolidBrush(PluginBase.MainForm.GetThemeColor("CompletionList.PackageColor", Color.Gray));
            Rectangle tbounds = new Rectangle(ScaleHelper.Scale(18), e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
            if (item != null)
            {
                Graphics g = e.Graphics;
                float newHeight = e.Bounds.Height - 2;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(item.Icon, 1, e.Bounds.Top + ((e.Bounds.Height - newHeight) / 2), newHeight, newHeight);
                int p = item.Label.LastIndexOf('.');
                if (p > 0 && !selected && !(item is ICompletionListSpecialItem))
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
                UITools.Tip.Hide();
                currentItem = item;
                tempoTip.Stop();
                tempoTip.Start();
            }
        }

        /// <summary>
        /// Display item information in tooltip
        /// </summary> 
        public static void UpdateTip(object sender, System.Timers.ElapsedEventArgs e)
        {
            tempoTip.Stop();
            if (currentItem == null || faded)
                return;

            UITools.Tip.SetText(currentItem.Description ?? "", false);
            UITools.Tip.Redraw(false);

            int rightWidth = ((Form)PluginBase.MainForm).ClientRectangle.Right - completionList.Right - 10;
            int leftWidth = completionList.Left;

            Point posTarget = new Point(completionList.Right, completionList.Top);
            int widthTarget = rightWidth;
            if (rightWidth < 220 && leftWidth > 220)
            {
                widthTarget = leftWidth;
                posTarget = new Point(0, completionList.Top);
            }

            UITools.Tip.Location = posTarget;
            UITools.Tip.AutoSize(widthTarget, 500);

            if (widthTarget == leftWidth)
                UITools.Tip.Location = new Point(completionList.Left - UITools.Tip.Size.Width, posTarget.Y);

            UITools.Tip.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CLClick(object sender, EventArgs e)
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable)
            {
                Hide();
                return;
            }
            doc.SciControl.Focus();
        }

        /// <summary>
        /// 
        /// </summary> 
        private static void CLDoubleClick(object sender, EventArgs e)
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable)
            {
                Hide();
                return;
            }
            ScintillaControl sci = doc.SciControl;
            sci.Focus();
            ReplaceText(sci, '\0');
        }

        /// <summary>
        /// Filter the completion list with the letter typed
        /// </summary> 
        public static void FindWordStartingWith(string word)
        {
            if (word == null) word = "";
            int len = word.Length;
            int maxLen = 0;
            int lastScore = 0;
            /// <summary>
            /// FILTER ITEMS
            /// </summary>
            if (PluginBase.MainForm.Settings.AutoFilterList || fullList)
            {
                IList<ICompletionListItem> found;
                if (len == 0) 
                {
                    found = allItems;
                    lastIndex = 0;
                    exactMatchInList = false;
                    smartMatchInList = true;
                }
                else
                {
                    var temp = new List<ItemMatch>(allItems.Count);
                    var n = allItems.Count;
                    var i = 0;
                    lastScore = 99;
                    exactMatchInList = false;
                    smartMatchInList = false;
                    while (i < n)
                    {
                        var item = allItems[i];
                        // compare item's label with the searched word
                        var score = SmartMatch(item.Label, word, len);
                        if (score > 0)
                        {
                            // first match found
                            if (!smartMatchInList || score < lastScore)
                            {
                                lastScore = score;
                                lastIndex = temp.Count;
                                smartMatchInList = true;
                                exactMatchInList = score < 5 && word == CompletionList.word;
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
                    foreach (var item in found)
                    {
                        completionList.Items.Add(item);
                        if (item.Label.Length > maxLen)
                        {
                            widestLabel = item.Label;
                            maxLen = widestLabel.Length;
                        }
                    }
                    var topIndex = lastIndex;
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
            /// <summary>
            /// NO FILTER
            /// </summary>
            else
            {
                int n = completionList.Items.Count;
                while (lastIndex < n)
                {
                    var item = completionList.Items[lastIndex] as ICompletionListItem;
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

        private static int TestDefaultItem(int index, string word, int len)
        {
            if (defaultItem != null && completionList.Items.Contains(defaultItem))
            {
                int score = (len == 0) ? 1 : SmartMatch(defaultItem.Label, word, len);
                if (score > 0 && score < 6) return completionList.Items.IndexOf(defaultItem);
            }
            return index;
        }

        public static int SmartMatch(string label, string word, int len)
        {
            if (label.Length < len) return 0;

            // simple matching
            if (disableSmartMatch)
            {
                if (label.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                {
                    if (label.StartsWithOrdinal(word)) return 1;
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
                    p2 = label.IndexOfOrdinal(word);
                    if (p2 >= 0)
                    {
                        int p3 = label.LastIndexOfOrdinal("." + word); // in qualified type name
                        if (p3 > 0)
                        {
                            if (p3 == label.LastIndexOf('.'))
                            {
                                if (label.EndsWithOrdinal("." + word)) return 1;
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
            int firstChar = label.IndexOf(word[0].ToString(), StringComparison.OrdinalIgnoreCase);
            int i = 1;
            p = firstChar;
            while (i < len && p >= 0)
            {
                p = label.IndexOf(word[i++].ToString(), p + 1, StringComparison.OrdinalIgnoreCase);
            }
            return (p > 0) ? 7 : 0;
        }

        static int IsAbbreviation(string label, string word)
        {
            int len = word.Length;
            int i = 1;
            char c = word[0];
            int p2;
            int score = 0;
            if (label[0] == c) { p2 = 0; score = 1; }
            else if (!label.Contains('.'))
            {
                p2 = label.IndexOf(c);
                if (p2 < 0) return 0;
                score = 3;
            }
            else 
            {
                p2 = label.IndexOfOrdinal("." + c);
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
                var p = p2;
                c = word[i++];
                if (char.IsUpper(c)) p2 = label.IndexOfOrdinal(c.ToString(), p + 1);
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
                if (label == word || label.EndsWithOrdinal("." + word)) return 1;
                return score;
            }
            else return score + 2;
        }

        /// <summary>
        /// 
        /// </summary> 
        public static bool ReplaceText(ScintillaControl sci, char trigger) => ReplaceText(sci, "", trigger);

        /// <summary>
        /// 
        /// </summary> 
        public static bool ReplaceText(ScintillaControl sci, string tail, char trigger)
        {
            sci.BeginUndoAction();
            try
            {
                string triggers = PluginBase.Settings.InsertionTriggers ?? "";
                if (triggers.Length > 0 && !Regex.Unescape(triggers).Contains(trigger)) return false;

                ICompletionListItem item = null;
                if (completionList.SelectedIndex >= 0)
                {
                    item = completionList.Items[completionList.SelectedIndex] as ICompletionListItem;
                }
                Hide();
                if (item == null) return false;
                string replace = item.Value;
                if (replace != null)
                {
                    sci.SetSel(startPos, sci.CurrentPos);
                    if (word != null && tail.Length > 0)
                    {
                        if (replace.StartsWith(word, StringComparison.OrdinalIgnoreCase) && replace.IndexOfOrdinal(tail) >= word.Length)
                        {
                            replace = replace.Substring(0, replace.IndexOfOrdinal(tail));
                        }
                    }
                    sci.ReplaceSel(replace);
                    OnInsert?.Invoke(sci, startPos, replace, trigger, item);
                    if (tail.Length > 0) sci.ReplaceSel(tail);
                }
                return true;
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
        
        #endregion
        
        #region Event Handling
        
        public static IntPtr GetHandle()
        {
            return completionList.Handle;
        }

        public static void OnChar(ScintillaControl sci, int value)
        {
            char c = (char)value;
            string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
            if (characterClass.IndexOf(c) >= 0)
            {
                word += c;
                currentPos++;
                FindWordStartingWith(word);
                return;
            }
            else if (noAutoInsert)
            {
                Hide('\0');
                // handle this char
                UITools.Manager.SendChar(sci, value);
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
                    ReplaceText(sci, c.ToString(), c);
                }
                // handle this char
                UITools.Manager.SendChar(sci, value);
            }
        }

        public static bool HandleKeys(ScintillaControl sci, Keys key)
        {
            int index;
            switch (key)
            {
                case Keys.Back:
                    var wordLength = word.Length;
                    if (wordLength > 0 && wordLength >= MinWordLength)
                    {
                        word = word.Substring(0, wordLength - 1);
                        currentPos = sci.CurrentPos - 1;
                        lastIndex = 0;
                        FindWordStartingWith(word);
                    }
                    else Hide((char)8);
                    return false;
                    
                case Keys.Enter:
                    if (noAutoInsert || !ReplaceText(sci, '\n'))
                    {
                        Hide();
                        return false;
                    }
                    return true;

                case Keys.Tab:
                    if (!ReplaceText(sci, '\t'))
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
                    if (!completionList.Visible)
                    {
                        Hide();
                        if (key == Keys.Up) sci.LineUp(); 
                        else sci.CharLeft();
                        return false;
                    }
                    // go up the list
                    if (completionList.SelectedIndex > 0)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex-1;
                        completionList.SelectedIndex = index;
                    }
                    // wrap
                    else if (PluginBase.MainForm.Settings.WrapList)
                    {
                        RefreshTip();
                        index = completionList.Items.Count-1;
                        completionList.SelectedIndex = index;
                    }
                    break;

                case Keys.Down:
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!completionList.Visible)
                    {
                        Hide();
                        if (key == Keys.Down) sci.LineDown(); 
                        else sci.CharRight();
                        return false;
                    }
                    // go down the list
                    if (completionList.SelectedIndex < completionList.Items.Count-1)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex+1;
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
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!completionList.Visible)
                    {
                        Hide();
                        sci.PageUp();
                        return false;
                    }
                    // go up the list
                    if (completionList.SelectedIndex > 0)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex-completionList.Height/completionList.ItemHeight;
                        if (index < 0) index = 0;
                        completionList.SelectedIndex = index;
                    }
                    break;

                case Keys.PageDown:
                    noAutoInsert = false;
                    // the list was hidden and it should not appear
                    if (!completionList.Visible)
                    {
                        Hide();
                        sci.PageDown();
                        return false;
                    }
                    // go down the list
                    if (completionList.SelectedIndex < completionList.Items.Count-1)
                    {
                        RefreshTip();
                        index = completionList.SelectedIndex+completionList.Height/completionList.ItemHeight;
                        if (index > completionList.Items.Count-1) index = completionList.Items.Count-1;
                        completionList.SelectedIndex = index;
                    }
                    break;
                
                case (Keys.Control | Keys.Space):
                    break;
                
                case Keys.Left:
                    sci.CharLeft();
                    Hide();
                    break;

                case Keys.Right:
                    sci.CharRight();
                    Hide();
                    break;

                default:
                    Hide();
                    return false;
            }
            return true;
        }

        private static void RefreshTip()
        {
            UITools.Tip.Hide();
            tempoTip.Enabled = false;
        }
        
        #endregion

        #region Controls fading on Control key

        private static bool faded;

        internal static void FadeOut()
        {
            if (faded) return;
            faded = true;
            UITools.Tip.Hide();
            completionList.Visible = false;
        }

        internal static void FadeIn()
        {
            if (!faded) return;
            faded = false;
            completionList.Visible = true;
        }

        #endregion

    }

    struct ItemMatch
    {
        public readonly int Score;
        public readonly ICompletionListItem Item;

        public ItemMatch(int score, ICompletionListItem item)
        {
            Score = score;
            Item = item;
        }
    }

}
