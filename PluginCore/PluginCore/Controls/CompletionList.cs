using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Managers;
using PluginCore.Helpers;
using ScintillaNet;

namespace PluginCore.Controls
{

	public static class CompletionList
    {
        static public event InsertedTextHandler OnInsert;
        static public event InsertedTextHandler OnCancel;

		/// <summary>
        /// Properties of the class 
        /// </summary> 
        private static CompletionListControl completionList;
		
		#region State Properties

	    internal static Boolean listUp
	    {
            get { return completionList.listUp; }
	    }

        /// <summary>
        /// Set to 0 after calling .Show to keep the completion list active 
        /// when the text was erased completely (using backspace)
        /// </summary>
        public static Int32 MinWordLength;

		#endregion
		
		#region Control Creation
		
        /// <summary>
        /// Creates the control 
        /// </summary> 
		public static void CreateControl(IMainForm mainForm)
        {
            completionList = new CompletionListControl(mainForm);
            completionList.OnCancel += OnCancel;
            completionList.OnInsert += OnInsert;
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
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable) return;
            completionList.Show(itemList, autoHide);
		}

        /// <summary>
        /// Shows the completion list
        /// </summary>
        static public void Show(List<ICompletionListItem> itemList, bool autoHide)
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (!doc.IsEditable) return;
            ScintillaControl sci = doc.SciControl;
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
            return ReplaceText(sci, "", trigger);
		}

        /// <summary>
        /// 
        /// </summary> 
		static public bool ReplaceText(ScintillaControl sci, String tail, char trigger)
        {
            return completionList.ReplaceText(sci, tail, trigger);
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
            completionList.OnChar(sci, value);
        }

        static public bool HandleKeys(ScintillaControl sci, Keys key)
        {
            return completionList.HandleKeys(sci, key);
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

	}

}
