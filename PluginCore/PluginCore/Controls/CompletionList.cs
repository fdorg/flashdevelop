// NOTE: We may well dump this static class, or mark it as deprecated, and create UITools.CompletionList, it would make the code look bit more organized and in line with other code in there

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ScintillaNet;

namespace PluginCore.Controls
{

    public delegate void InsertedTextHandler(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item);

    public static class CompletionList
    {
        static public event InsertedTextHandler OnInsert;
        static public event InsertedTextHandler OnCancel;

        /// <summary>
        /// Properties of the class 
        /// </summary> 
        internal static CompletionListControl completionList;
        
        #region State Properties

        internal static Boolean listUp
        {
            get { return completionList.listUp; }
            set { completionList.listUp = value; }
        }

        /// <summary>
        /// Set to 0 after calling .Show to keep the completion list active 
        /// when the text was erased completely (using backspace)
        /// </summary>
        public static Int32 MinWordLength
        {
            get { return completionList.MinWordLength; }
            set { completionList.MinWordLength = value; }
        }

        #endregion
        
        #region Control Creation
        
        /// <summary>
        /// Creates the control 
        /// </summary> 
        public static void CreateControl(IMainForm mainForm)
        {
            completionList = new CompletionListControl(new ScintillaHost());
            completionList.OnCancel += OnCancelHandler;
            completionList.OnInsert += OnInsertHandler;
        }

        #endregion
        
        #region Public List Properties

        /// <summary>
        /// Is the control active? 
        /// </summary> 
        public static Boolean Active
        {
            get { return completionList.Active; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean HasMouseIn
        {
            get
            {
                return completionList.HasMouseIn;
            }
        }

        /// <summary>
        /// Retrieves the currently selected label, or null if none selected
        /// </summary>
        public static string SelectedLabel
        {
            get
            {
                return completionList.SelectedLabel;
            }
        }
        
        #endregion
        
        #region CompletionList Methods
        
        /// <summary>
        /// Checks if the position is valid
        /// </summary> 
        public static Boolean CheckPosition(Int32 position)
        {
            return completionList.CheckPosition(position);
        }

        /// <summary>
        /// Shows the completion list
        /// </summary> 
        static public void Show(List<ICompletionListItem> itemList, Boolean autoHide, String select)
        {
            completionList.Show(itemList, autoHide, select);
        }

        /// <summary>
        /// Shows the completion list
        /// </summary>
        static public void Show(List<ICompletionListItem> itemList, bool autoHide)
        {
            completionList.Show(itemList, autoHide);
        }

        /// <summary>
        /// Set default selected item in completion list
        /// </summary>
        static public void SelectItem(String name)
        {
            completionList.SelectItem(name);
        }

        /// <summary>
        /// Require that completion items are explicitely inserted (Enter, Tab, mouse-click)
        /// </summary>
        public static void DisableAutoInsertion()
        {
            completionList.DisableAutoInsertion();
        }

        static public void Redraw()
        {
            completionList.Redraw();
        }

        /// <summary>
        /// Hide completion list
        /// </summary> 	
        static public void Hide()
        {
            completionList.Hide();
        }

        /// <summary>
        /// Cancel completion list with event
        /// </summary>  
        static public void Hide(char trigger)
        {
            completionList.Hide(trigger);
        }

        /// <summary>
        /// 
        /// </summary> 
        static public void SelectWordInList(String tail)
        {
            completionList.SelectWordInList(tail);
        }

        /// <summary>
        /// Display item information in tooltip
        /// </summary> 
        static public void UpdateTip(Object sender, System.Timers.ElapsedEventArgs e)
        {
            completionList.UpdateTip(sender, e);
        }

        static public int SmartMatch(string label, string word, int len)
        {
            return completionList.SmartMatch(label, word, len);
        }

        /// <summary>
        /// Filter the completion list with the letter typed
        /// </summary> 
        static public void FindWordStartingWith(String word)
        {
            completionList.FindWordStartingWith(word);
        }

        /// <summary>
        /// 
        /// </summary> 
        static public bool ReplaceText(ScintillaControl sci, char trigger)
        {
            return completionList.ReplaceText("", trigger);
        }

        /// <summary>
        /// 
        /// </summary> 
        static public bool ReplaceText(ScintillaControl sci, String tail, char trigger)
        {
            return completionList.ReplaceText(tail, trigger);
        }
        
        #endregion
        
        #region Event Handling
        
        /// <summary>
        /// 
        /// </summary> 
        static public IntPtr GetHandle()
        {
            return completionList.GetHandle();
        }

        static public void OnChar(ScintillaControl sci, int value)
        {
            // Note: If we refactor/remove this class, this could be called directly from UITools
            if (!completionList.OnChar((char)value))
                UITools.Manager.SendChar(sci, value);
        }

        static public bool HandleKeys(ScintillaControl sci, Keys key)
        {
            return completionList.HandleKeys(key);
        }

        private static void OnCancelHandler(Control sender, Int32 position, String text, Char trigger, ICompletionListItem item)
        {
            if (OnCancel != null)
                OnCancel((ScintillaControl)sender, position, text, trigger, item);
        }

        private static void OnInsertHandler(Control sender, Int32 position, String text, Char trigger, ICompletionListItem item)
        {
            if (OnInsert != null)
                OnInsert((ScintillaControl)sender, position, text, trigger, item);
        }

        #endregion

        #region Controls fading on Control key

        internal static void FadeOut()
        {
            completionList.FadeOut();
        }

        internal static void FadeIn()
        {
            completionList.FadeIn();
        }

        #endregion

        #region Default Completion List Host

        internal class ScintillaHost : ICompletionListHost
        {

            private List<Control> controlHierarchy = new List<Control>();

            private WeakReference sci = new WeakReference(null);
            internal ScintillaControl SciControl
            {
                get
                {
                    if (sci.Target == null)
                        return null;
                    
                    if (!sci.IsAlive)
                        return PluginBase.MainForm.CurrentDocument.SciControl;

                    return (ScintillaControl)sci.Target;
                }
                set
                {
                    if (sci.Target == value) return;
 
                    sci.Target = value;
                    ClearControlHierarchy();
                }
            }

            public event EventHandler LostFocus
            {
                add { Owner.LostFocus += value; }
                remove { Owner.LostFocus -= value; }
            }

            private EventHandler positionChanged;
            public event EventHandler PositionChanged
            {
                add
                {
                    if (positionChanged == null || positionChanged.GetInvocationList().Length == 0)
                    {
                        var sci = SciControl;
                        sci.Scroll += Scintilla_Scroll;
                        sci.Zoom += Scintilla_Zoom;

                        BuildControlHierarchy(sci);
                    }
                    positionChanged += value;
                }
                remove
                {
                    positionChanged -= value;
                    if (positionChanged == null || positionChanged.GetInvocationList().Length < 1)
                    {
                        var sci = SciControl;
                        sci.Scroll -= Scintilla_Scroll;
                        sci.Zoom -= Scintilla_Zoom;
                        ClearControlHierarchy();
                    }
                }
            }

            public event EventHandler SizeChanged
            {
                add { Owner.SizeChanged += value; }
                remove { Owner.SizeChanged -= value; }
            }

            public event KeyEventHandler KeyDown
            {
                add { Owner.KeyDown += value; }
                remove { Owner.KeyDown -= value; }
            }

            public event KeyEventHandler KeyPosted
            {
                add { SciControl.KeyPosted += value; }
                remove { SciControl.KeyPosted -= value; }
            }

            #pragma warning disable 0067
            public event KeyPressEventHandler KeyPress; // Unhandled for this one, although we could
            #pragma warning restore 0067

            public event MouseEventHandler MouseDown
            {
                add { Owner.MouseDown += value; }
                remove { Owner.MouseDown -= value; }
            }

            public Control Owner
            {
                get { return SciControl; }
            }

            public string SelectedText
            {
                get { return SciControl.SelText; }
                set { SciControl.ReplaceSel(value); }
            }

            public int SelectionEnd
            {
                get { return SciControl.SelectionEnd; }
                set { SciControl.SelectionStart = value; }
            }

            public int SelectionStart
            {
                get { return SciControl.SelectionStart; }
                set { SciControl.SelectionStart = value; }
            }

            public int CurrentPos
            {
                get { return SciControl.CurrentPos; }
            }

            public bool IsEditable
            {
                get { return PluginBase.MainForm.CurrentDocument.IsEditable && SciControl != null; }
            }

            public int GetLineHeight()
            {
                return UITools.Manager.LineHeight(SciControl);
            }

            public int GetLineFromCharIndex(int pos)
            {
                return SciControl.LineFromPosition(pos);
            }

            public Point GetPositionFromCharIndex(int pos)
            {
                var sci = SciControl;
                return new Point(sci.PointXFromPosition(pos), sci.PointYFromPosition(pos));
            }

            public void SetSelection(int start, int end)
            {
                SciControl.SetSel(start, end);
            }

            public void BeginUndoAction()
            {
                SciControl.BeginUndoAction();
            }

            public void EndUndoAction()
            {
                SciControl.EndUndoAction();
            }

            private void BuildControlHierarchy(Control current)
            {
                while (current != null)
                {
                    current.LocationChanged += Control_LocationChanged;
                    current.ParentChanged += Control_ParentChanged;
                    controlHierarchy.Add(current);
                    current = current.Parent;
                }
            }

            private void ClearControlHierarchy()
            {
                foreach (var control in controlHierarchy)
                {
                    control.LocationChanged -= Control_LocationChanged;
                    control.ParentChanged -= Control_ParentChanged;
                }
                controlHierarchy.Clear();
            }

            private void Control_LocationChanged(object sender, EventArgs e)
            {
                if (positionChanged != null)
                    positionChanged(sender, e);
            }

            private void Control_ParentChanged(object sender, EventArgs e)
            {
                ClearControlHierarchy();
                BuildControlHierarchy(SciControl);
                if (positionChanged != null)
                    positionChanged(sender, e);
            }

            private void Scintilla_Scroll(object sender, ScrollEventArgs e)
            {
                if (positionChanged != null)
                    positionChanged(sender, e);
            }

            private void Scintilla_Zoom(ScintillaControl sci)
            {
                if (positionChanged != null)
                    positionChanged(sci, EventArgs.Empty);
            }

        }
        
        #endregion
    }
}
