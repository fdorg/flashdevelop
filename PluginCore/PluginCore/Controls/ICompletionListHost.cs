using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    /* Possible properties/methods of interest:
     *      - suppressedKeys: collection of extra key combinations that would be consumed by the completionList
     *      - OnListShowing/OnListHidden: method to know when the list is going to show or is hidden, better to get a reference to the list and listen for events
     *      - AfterCompletionCommit/BeforeCompletionCommit
     */
    public interface ICompletionListHost
    {

        event EventHandler LostFocus;
        event EventHandler PositionChanged;
        event KeyEventHandler KeyDown;
        event KeyPressEventHandler KeyPress;
        event MouseEventHandler MouseDown;
        
        Control Owner { get; }
        string SelectedText { get; set; }
        int SelectionEnd { get; set; }
        int SelectionStart { get; set; }
        int CurrentPos { get; }
        bool IsEditable { get; }

        Point GetPositionFromCharIndex(int pos);
        int GetLineHeight();
        void SetSelection(int start, int end);

        void BeginUndoAction();
        void EndUndoAction();

    }
}
