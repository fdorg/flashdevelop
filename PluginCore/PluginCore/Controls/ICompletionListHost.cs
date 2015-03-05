using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    /* Possible properties/methods of interest:
     *      - suppressedKeys: collection of extra key combinations that would be consumed by the completionList
     *      - OnListShowing/OnListHidden: method to know when the list is going to show or is hidden, better than getting a reference to the list and listen for events
     *      - AfterCompletionCommit/BeforeCompletionCommit
     */
    public interface ICompletionListHost
    {

        event EventHandler LostFocus;
        event EventHandler PositionChanged;
        event EventHandler SizeChanged;
        event KeyEventHandler KeyDown;
        // Hacky event... needed for MethodCallTip where we need to get the new state after the key has been sent
        // A better approach, and the way some IDEs work, would require MethodCallTip to be more "active" having more knowledge about the written function data, as well as a timer for some operations
        event KeyEventHandler KeyPosted;
        event KeyPressEventHandler KeyPress;
        event MouseEventHandler MouseDown;
        
        Control Owner { get; }
        string SelectedText { get; set; }
        int SelectionEnd { get; set; }
        int SelectionStart { get; set; }
        int CurrentPos { get; }
        bool IsEditable { get; }

        int GetLineFromCharIndex(int pos);
        Point GetPositionFromCharIndex(int pos);
        int GetLineHeight();
        void SetSelection(int start, int end);

        void BeginUndoAction();
        void EndUndoAction();

    }

}
