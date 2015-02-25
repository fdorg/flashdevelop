// NOTE: We may well dump this static class, or mark it as deprecated, and create UITools.CompletionList, it would make the code look bit more organized and in line with some other controls

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

        // TODO: Get hold of current Scintilla control through UITools and don't call CurrentDocument.SciControl everytime
        private class ScintillaHost : ICompletionListHost
        {

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
                    var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    sci.Scroll += Scintilla_Scroll;
                    sci.Zoom += Scintilla_Zoom;
                    positionChanged += value;
                }
                remove
                {
                    var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    sci.Scroll -= Scintilla_Scroll;
                    sci.Zoom -= Scintilla_Zoom;
                    positionChanged -= value;
                }
            }

            public event KeyEventHandler KeyDown
            {
                add { Owner.KeyDown += value; }
                remove { Owner.KeyDown -= value; }
            }

            public event KeyPressEventHandler KeyPress; // Unhandled for this one
            public event MouseEventHandler MouseDown
            {
                add { Owner.MouseDown += value; }
                remove { Owner.MouseDown -= value; }
            }

            public Control Owner
            {
                get { return PluginBase.MainForm.CurrentDocument.SciControl; }
            }

            public string SelectedText
            {
                get { return PluginBase.MainForm.CurrentDocument.SciControl.SelText; }
                set { PluginBase.MainForm.CurrentDocument.SciControl.ReplaceSel(value); }
            }

            public int SelectionEnd
            {
                get { return PluginBase.MainForm.CurrentDocument.SciControl.SelectionEnd; }
                set { PluginBase.MainForm.CurrentDocument.SciControl.SelectionStart = value; }
            }

            public int SelectionStart
            {
                get { return PluginBase.MainForm.CurrentDocument.SciControl.SelectionStart; }
                set { PluginBase.MainForm.CurrentDocument.SciControl.SelectionStart = value; }
            }

            public int CurrentPos
            {
                get { return PluginBase.MainForm.CurrentDocument.SciControl.CurrentPos; }
            }

            public bool IsEditable
            {
                get { return PluginBase.MainForm.CurrentDocument.IsEditable && PluginBase.MainForm.CurrentDocument.SciControl != null; }
            }

            public int GetLineHeight()
            {
                var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                return UITools.Manager.LineHeight(sci);
            }

            public Point GetPositionFromCharIndex(int pos)
            {
                var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                return new Point(sci.PointXFromPosition(pos), sci.PointYFromPosition(pos));
            }

            public void SetSelection(int start, int end)
            {
                var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                sci.SetSel(start, end);
            }

            public void BeginUndoAction()
            {
                PluginBase.MainForm.CurrentDocument.SciControl.BeginUndoAction();
            }

            public void EndUndoAction()
            {
                PluginBase.MainForm.CurrentDocument.SciControl.EndUndoAction();
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
	}
}
