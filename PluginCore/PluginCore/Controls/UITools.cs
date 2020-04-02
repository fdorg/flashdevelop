using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Managers;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;

namespace PluginCore.Controls
{
    
    public class UITools : IMessageFilter, IEventHandler
    {
        public delegate void CharAddedHandler(ScintillaControl sender, int value);
        public delegate void TextChangedHandler(ScintillaControl sender, int position, int length, int linesAdded);
        public delegate void MouseHoverHandler(ScintillaControl sender, int position);
        public delegate void LineEventHandler(ScintillaControl sender, int line);

        #region Singleton Instance

        static UITools manager;

        public static UITools Manager => manager ??= new UITools();

        public static CodeTip CodeTip => manager.codeTip;

        public static RichToolTip Tip => manager.simpleTip;

        public static RichToolTip ErrorTip => manager.errorTip;

        public static MethodCallTip CallTip => manager.callTip;

        public static void Init()
        {
            if (manager is null) manager = new UITools();
        }
        #endregion

        #region Initialization

        public event MouseHoverHandler OnMouseHover;
        public event MouseHoverHandler OnMouseHoverEnd;
        public event CharAddedHandler OnCharAdded;
        public event TextChangedHandler OnTextChanged;
        public event LineEventHandler OnMarkerChanged;

        public bool DisableEvents;

        /// <summary>
        /// Option: show detailed information in tips.
        /// </summary>
        /// <remarks>
        /// Default value is defined in the main settings.
        /// State is switched using F1 key when a tip is visible.
        /// </remarks>
        public bool ShowDetails { get; set; }

        const EventType eventMask = EventType.Keys | EventType.FileSave | EventType.Command | EventType.FileSwitch;

        readonly CodeTip codeTip;
        readonly RichToolTip simpleTip;
        readonly MethodCallTip callTip;
        readonly RichToolTip errorTip;

        bool ignoreKeys;

        UITools()
        {
            ShowDetails = PluginBase.Settings.ShowDetails;
            //
            // CONTROLS
            //
            try
            {
                CompletionList.CreateControl(PluginBase.MainForm);
                codeTip = new CodeTip(PluginBase.MainForm);
                simpleTip = new RichToolTip(PluginBase.MainForm);
                callTip = new MethodCallTip(PluginBase.MainForm);
                errorTip = new RichToolTip(PluginBase.MainForm);
            }
            catch(Exception ex)
            {
                ErrorManager.ShowError(/*"Error while creating editor controls.",*/ ex);
            }
            //
            // Events
            //
            PluginBase.MainForm.IgnoredKeys.Add(Keys.Space | Keys.Control); // complete member
            PluginBase.MainForm.IgnoredKeys.Add(Keys.Space | Keys.Control | Keys.Shift); // complete method
            PluginBase.MainForm.DockPanel.ActivePaneChanged += DockPanel_ActivePaneChanged;
            EventManager.AddEventHandler(this, eventMask);
        }
        #endregion

        WeakReference lockedSciControl;
        Point lastMousePos = new Point(0,0);

        #region SciControls & MainForm Events

        void DockPanel_ActivePaneChanged(object sender, EventArgs e)
        {
            var panel = PluginBase.MainForm.DockPanel;
            if (panel.ActivePane != null && panel.ActivePane != panel.ActiveDocumentPane)
            {
                OnUIRefresh(null);
            }
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Keys:
                    e.Handled = HandleKeys(((KeyEvent)e).Value);
                    return;
                    
                case EventType.FileSave:
                    MessageBar.HideWarning();
                    return;

                case EventType.Command:
                    string cmd = ((DataEvent)e).Action;
                    // EventType.Command handlind should quite probably disappear when merging the "Decoupled CompletionList". This is too hacky and error-prone...
                    if (cmd.IndexOfOrdinal("ProjectManager") > 0
                        || cmd.IndexOfOrdinal("Changed") > 0
                        || cmd.IndexOfOrdinal("Context") > 0
                        || cmd.IndexOfOrdinal("ClassPath") > 0
                        || cmd.IndexOfOrdinal("Watcher") > 0
                        || cmd.IndexOfOrdinal("Get") > 0
                        || cmd.IndexOfOrdinal("Set") > 0
                        || cmd.IndexOfOrdinal("SDK") > 0
                        || cmd == "ASCompletion.FileModelUpdated"
                        || cmd == "ASCompletion.PathExplorerFinished"
                        || cmd == "ASCompletion.ContextualGenerator.AddOptions"
                        || cmd == "ASCompletion.DotCompletion"
                        || cmd == "ResultsPanel.ClearResults"
                        || cmd.IndexOfOrdinal("LintingManager.") == 0)
                        return; // ignore notifications
                    break;
            }
            // most of the time, an event should hide the list
            OnUIRefresh(null);
        }
        
        /// <summary>
        /// Reserved to MainForm
        /// </summary>
        public void ListenTo(ScintillaControl sci)
        {
            // hook scintilla events
            sci.MouseDwellTime = PluginBase.Settings.HoverDelay;
            sci.DwellStart += HandleDwellStart;
            sci.DwellEnd += HandleDwellEnd;
            sci.CharAdded += OnChar;
            sci.UpdateUI += OnUIRefresh;
            sci.TextInserted += OnTextInserted;
            sci.TextDeleted += OnTextDeleted;
        }

        /// <summary>
        /// Notify all listeners that document markers were changed
        /// </summary>
        public void MarkerChanged(ScintillaControl sender, int line) => OnMarkerChanged?.Invoke(sender, line);

        void HandleDwellStart(ScintillaControl sci, int position, int x, int y)
        {
            if (OnMouseHover is null || sci is null || DisableEvents) return;
            try
            {
                // check mouse over the editor
                if ((position < 0) || simpleTip.Visible || errorTip.Visible || CompletionList.HasMouseIn) return;
                Point mousePos = ((Form) PluginBase.MainForm).PointToClient(Cursor.Position);
                if (mousePos.X == lastMousePos.X && mousePos.Y == lastMousePos.Y)
                    return;

                lastMousePos = mousePos;
                Rectangle bounds = GetWindowBounds(sci);
                if (!bounds.Contains(mousePos)) return;

                // check no panel is over the editor
                var panel = PluginBase.MainForm.DockPanel;
                var panels = panel.Contents;
                foreach (DockContent content in panels)
                {
                    if (content.IsHidden || content.Bounds.Height == 0 || content.Bounds.Width == 0
                        || content.GetType().ToString() == "FlashDevelop.Docking.TabbedDocument") 
                        continue;
                    bounds = GetWindowBounds(content);
                    if (bounds.Contains(mousePos))
                        return;
                }
                OnMouseHover?.Invoke(sci, position);
                if (errorTip.Visible)
                {
                    //move simpleTip up to not overlap error tip
                    simpleTip.Location = new Point(simpleTip.Location.X, simpleTip.Location.Y - errorTip.Size.Height);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                // disable this feature completely
                OnMouseHover = null;
            }
        }

        Rectangle GetWindowBounds(Control ctrl)
        {
            while (ctrl.Parent != null && !(ctrl is DockWindow)) ctrl = ctrl.Parent;
            return ctrl.Bounds;
        }

        Point GetMousePosIn(Control ctrl)
        {
            Point ctrlPos = ctrl.PointToScreen(new Point());
            Point pos = Cursor.Position;
            return new Point(pos.X - ctrlPos.X, pos.Y - ctrlPos.Y);
        }

        void HandleDwellEnd(ScintillaControl sci, int position, int x, int y)
        {
            simpleTip.Hide();
            errorTip.Hide();
            OnMouseHoverEnd?.Invoke(sci, position);
        }

        #endregion
        
        #region Scintilla Hook
        
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == Win32.WM_MOUSEWHEEL) // capture all MouseWheel events 
            {
                if (!callTip.CallTipActive || !callTip.Focused)
                {
                    if (Win32.ShouldUseWin32())
                    {
                        Win32.SendMessage(CompletionList.GetHandle(), m.Msg, (int)m.WParam, (int)m.LParam);
                        return true;
                    }
                    return false;
                }
                return false;
            }

            if (m.Msg == Win32.WM_KEYDOWN)
            {
                if ((int)m.WParam == 17) // Ctrl
                {
                    if (CompletionList.Active) CompletionList.FadeOut();
                    if (callTip.CallTipActive && !callTip.Focused) callTip.FadeOut();
                }
            }
            else if (m.Msg == Win32.WM_KEYUP)
            {
                if ((int)m.WParam == 17 || (int)m.WParam == 18) // Ctrl / AltGr
                {
                    if (CompletionList.Active) CompletionList.FadeIn();
                    if (callTip.CallTipActive) callTip.FadeIn();
                }
            }
            return false;
        }
        
        public void LockControl(ScintillaControl sci)
        {
            if (lockedSciControl != null && lockedSciControl.IsAlive && lockedSciControl.Target == sci)
                return;
            UnlockControl();
            sci.IgnoreAllKeys = true;
            lockedSciControl = new WeakReference(sci);
            Application.AddMessageFilter(this);
        }

        public void UnlockControl()
        {
            if (CompletionList.Active || CallTip.CallTipActive) return;
            Application.RemoveMessageFilter(this);
            if (lockedSciControl != null && lockedSciControl.IsAlive)
            {
                var sci = (ScintillaControl)lockedSciControl.Target;
                sci.IgnoreAllKeys = false;
            }
            lockedSciControl = null;
        }

        void OnUIRefresh(ScintillaControl sci)
        {
            var mainForm = (Form) PluginBase.MainForm;
            if (mainForm.InvokeRequired)
            {
                mainForm.BeginInvoke((MethodInvoker)delegate { OnUIRefresh(sci); });
                return;
            }
            if (sci != null && sci.IsFocus)
            {
                int position = sci.SelectionEnd;
                if (CompletionList.Active && CompletionList.CheckPosition(position)) return;
                if (callTip.CallTipActive && callTip.CheckPosition(position)) return;
            }
            codeTip.Hide();
            callTip.Hide();
            CompletionList.Hide();
            simpleTip.Hide();
            errorTip.Hide();
        }

        void OnTextInserted(ScintillaControl sci, int position, int length, int linesAdded)
        {
            if (!DisableEvents) OnTextChanged?.Invoke(sci, position, length, linesAdded);
        }

        void OnTextDeleted(ScintillaControl sci, int position, int length, int linesAdded)
        {
            if (!DisableEvents) OnTextChanged?.Invoke(sci, position, -length, linesAdded);
        }

        void OnChar(ScintillaControl sci, int value)
        {
            if (sci is null || DisableEvents) return;
            if (!CompletionList.Active && !callTip.CallTipActive)
            {
                SendChar(sci, value);
                return;
            }
            if (lockedSciControl != null && lockedSciControl.IsAlive) sci = (ScintillaControl)lockedSciControl.Target;
            else
            {
                codeTip.Hide();
                callTip.Hide();
                CompletionList.Hide();
                SendChar(sci, value);
                return;
            }
            
            if (callTip.CallTipActive) callTip.OnChar(sci);
            if (CompletionList.Active) CompletionList.OnChar(sci, value);
            else SendChar(sci, value);
        }

        public void SendChar(ScintillaControl sci, int value) => OnCharAdded?.Invoke(sci, value);

        bool HandleKeys(Keys key)
        {
            // UITools is currently broadcasting a shortcut, ignore!
            if (ignoreKeys || DisableEvents) return false;
            
            // list/tip shortcut dispatching
            if ((key == (Keys.Control | Keys.Space)) || (key == (Keys.Shift | Keys.Control | Keys.Space)))
            {
                /*if (CompletionList.Active || callTip.CallTipActive)
                {
                    UnlockControl();
                    CompletionList.Hide();
                    codeTip.Hide();
                    callTip.Hide();
                }*/
                // offer to handle the shortcut
                ignoreKeys = true;
                KeyEvent ke = new KeyEvent(EventType.Keys, key);
                EventManager.DispatchEvent(this, ke);
                ignoreKeys = false;
                // if not handled - show snippets
                if (!ke.Handled
                    && PluginBase.MainForm.CurrentDocument.SciControl is { } scintilla
                    && !scintilla.IsSelectionRectangle)
                {
                    PluginBase.MainForm.CallCommand("InsertSnippet", "null");
                }
                return true;
            }

            // toggle "long-description" for the hover tooltip
            if (key == Keys.F1 && Tip.Visible && !CompletionList.Active)
            {
                ShowDetails = !ShowDetails;
                simpleTip.UpdateTip(PluginBase.MainForm.CurrentDocument.SciControl);
                return true;
            }

            // are we currently displaying something?
            if (!CompletionList.Active && !callTip.CallTipActive) return false;
            
            // hide if pressing Esc or Ctrl+Key combination
            if (lockedSciControl is null || !lockedSciControl.IsAlive || key == Keys.Escape
                || ((Control.ModifierKeys & Keys.Control) != 0 && Control.ModifierKeys != (Keys.Control|Keys.Alt)) )
            {
                if (key == (Keys.Control | Keys.C) || key == (Keys.Control | Keys.A))
                    return false; // let text copy in tip
                UnlockControl();
                CompletionList.Hide((char)27);
                codeTip.Hide();
                callTip.Hide();
                return false;
            }
            // chars
            string ks = key.ToString();
            if (ks.Length == 1 || (ks.EndsWithOrdinal(", Shift") && ks.IndexOf(',') == 1) || ks.StartsWithOrdinal("NumPad"))
            {
                return false;
            }
            var sci = (ScintillaControl)lockedSciControl.Target;
            // toggle "long-description"
            if (key == Keys.F1)
            {
                ShowDetails = !ShowDetails;
                if (callTip.CallTipActive) callTip.UpdateTip(sci);
                else CompletionList.UpdateTip(null, null);
                return true;
            }
            
            // switches
            if ((key & Keys.ShiftKey) == Keys.ShiftKey || (key & Keys.ControlKey) == Keys.ControlKey || (key & Keys.Menu) == Keys.Menu)
            {
                return false;
            }

            // handle special keys
            var handled = false;
            if (callTip.CallTipActive) handled |= callTip.HandleKeys(sci, key);
            if (CompletionList.Active) handled |= CompletionList.HandleKeys(sci, key);
            return handled;
        }
        
        
        /// <summary>
        /// Compute current editor line height
        /// </summary>
        public int LineHeight(ScintillaControl sci)
        {
            if (sci is null) return 0;
            // evaluate the font size
            using var tempFont = new Font(sci.Font.Name, sci.Font.Size+sci.ZoomLevel);
            var g = sci.CreateGraphics();
            var textSize = g.MeasureString("S", tempFont);
            return (int)Math.Ceiling(textSize.Height);
        }

        #endregion
    }
}