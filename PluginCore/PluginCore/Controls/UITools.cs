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
        static private UITools manager;

        static public UITools Manager
        {
            get {
                if (manager == null)
                {
                    manager = new UITools();
                }
                return manager; 
            }
        }

        static public RichToolTip Tip
        {
            get { return manager.simpleTip; }
        }

        static public MethodCallTip CallTip
        {
            get { return manager.callTip; }
        }

        static public CompletionListControl CompletionList
        {
            get { return Controls.CompletionList.completionList; }
        }

        static public void Init()
        {
            if (manager == null)
            {
                manager = new UITools();
            }
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
        public bool ShowDetails
        {
            get { return showDetails; }
            set { showDetails = value; }
        }

        private EventType eventMask = 
            EventType.Keys | 
            EventType.FileSave | 
            EventType.Command | 
            EventType.FileSwitch;

        private RichToolTip simpleTip;
        private MethodCallTip callTip;

        private bool ignoreKeys;
        private bool showDetails;

        private UITools()
        {
            showDetails = PluginBase.Settings.ShowDetails;
            //
            // CONTROLS
            //
            try
            {
                Controls.CompletionList.CreateControl(PluginBase.MainForm);
                simpleTip = CompletionList.Tip;
                callTip = CompletionList.CallTip;
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
            EventManager.AddEventHandler(this, eventMask);
        }
        #endregion

        private WeakReference lockedSciControl;
        private Point lastMousePos = new Point(0,0);

        #region SciControls & MainForm Events

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
                    string cmd = (e as DataEvent).Action;
                    if (cmd.IndexOfOrdinal("ProjectManager") > 0
                        || cmd.IndexOfOrdinal("Changed") > 0
                        || cmd.IndexOfOrdinal("Context") > 0
                        || cmd.IndexOfOrdinal("ClassPath") > 0
                        || cmd.IndexOfOrdinal("Watcher") > 0
                        || cmd.IndexOfOrdinal("Get") > 0
                        || cmd.IndexOfOrdinal("Set") > 0
                        || cmd.IndexOfOrdinal("SDK") > 0)
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
            sci.MouseDwellTime = PluginBase.MainForm.Settings.HoverDelay;
            sci.DwellStart += new DwellStartHandler(HandleDwellStart);
            sci.DwellEnd += new DwellEndHandler(HandleDwellEnd);
            sci.CharAdded += new ScintillaNet.CharAddedHandler(OnChar);
            sci.UpdateUI += new UpdateUIHandler(OnUIRefresh);
            sci.TextInserted += new TextInsertedHandler(OnTextInserted);
            sci.TextDeleted += new TextDeletedHandler(OnTextDeleted);
            sci.GotFocus += OnGotFocus;
        }

        /// <summary>
        /// Notify all listeners that document markers were changed
        /// </summary>
        public void MarkerChanged(ScintillaControl sender, int line)
        {
            if (OnMarkerChanged != null) OnMarkerChanged(sender, line);
        }

        private void HandleDwellStart(ScintillaControl sci, int position)
        {
            if (OnMouseHover == null || sci == null || DisableEvents) return;
            try
            {
                // check mouse over the editor
                if ((position < 0) || simpleTip.Visible || CompletionList.HasMouseIn) return;
                Point mousePos = (PluginBase.MainForm as Form).PointToClient(Cursor.Position);
                if (mousePos.X == lastMousePos.X && mousePos.Y == lastMousePos.Y)
                    return;

                lastMousePos = mousePos;
                Rectangle bounds = GetWindowBounds(sci);
                if (!bounds.Contains(mousePos)) return;

                // check no panel is over the editor
                // 2015/02 CompletionList changes: Is this check currently needed? If a panel is over the editor HandleDwellStart doesn't seem to fire
                DockPanel panel = PluginBase.MainForm.DockPanel;
                DockContentCollection panels = panel.Contents;
                foreach (DockContent content in panels)
                {
                    if (content.IsHidden || content.Bounds.Height == 0 || content.Bounds.Width == 0
                        || content.GetType().ToString() == "FlashDevelop.Docking.TabbedDocument") 
                        continue;
                    bounds = GetWindowBounds(content);
                    if (bounds.Contains(mousePos))
                        return;
                }
                if (OnMouseHover != null) OnMouseHover(sci, position);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                // disable this feature completely
                OnMouseHover = null;
            }
        }

        private Rectangle GetWindowBounds(Control ctrl)
        {
            while (ctrl.Parent != null && !(ctrl is DockWindow)) ctrl = ctrl.Parent;
            if (ctrl != null) return ctrl.Bounds;
            else return new Rectangle();
        }

        private Point GetMousePosIn(Control ctrl)
        {
            Point ctrlPos = ctrl.PointToScreen(new Point());
            Point pos = Cursor.Position;
            return new Point(pos.X - ctrlPos.X, pos.Y - ctrlPos.Y);
        }

        private void HandleDwellEnd(ScintillaControl sci, int position)
        {
            // NOTE: simpleTip should only be hidden if a certain movement threshold is exceeded (x <> current word, y <> word + tip pos & height) or HandleDwellStart is fired again
            // This would allow the user to select the tip text
            if (!CompletionList.Active)
                simpleTip.Hide();
            if (OnMouseHoverEnd != null) OnMouseHoverEnd(sci, position);
        }

        #endregion
        
        #region Scintilla Hook
        
        public bool PreFilterMessage(ref Message m)
        {
            if (Tip.Focused || CallTip.Focused) return false;

            if (m.Msg == Win32.WM_MOUSEWHEEL) // capture all MouseWheel events 
            {
                if (Win32.ShouldUseWin32())
                {
                    Win32.SendMessage(CompletionList.GetHandle(), m.Msg, (Int32)m.WParam, (Int32)m.LParam);
                    return true;
                }
                else return false;
            }
            else if (m.Msg == Win32.WM_KEYDOWN)
            {
                if ((int)m.WParam == 17) // Ctrl
                {
                    if (CompletionList.Active) CompletionList.FadeOut();
                    if (callTip.CallTipActive) callTip.FadeOut();
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
            if (CompletionList.Active || CallTip.CallTipActive)
                return;
            Application.RemoveMessageFilter(this);
            if (lockedSciControl != null && lockedSciControl.IsAlive)
            {
                ScintillaControl sci = (ScintillaControl)lockedSciControl.Target;
                sci.IgnoreAllKeys = false;
            }
            lockedSciControl = null;
        }

        private void OnUIRefresh(ScintillaControl sci)
        {
            Form mainForm = PluginBase.MainForm as Form;
            if (mainForm.InvokeRequired)
            {
                mainForm.BeginInvoke((MethodInvoker)delegate { this.OnUIRefresh(sci); });
                return;
            }
            if (sci != null && sci.IsFocus)
            {
                int position = sci.CurrentPos;
                if (CompletionList.Active && CompletionList.CheckPosition(position)) return;
                if (callTip.CallTipActive && callTip.CheckPosition(position)) return;
            }
            CompletionList.Hide();
            callTip.Hide();
            simpleTip.Hide();
        }
        
        private void OnTextInserted(ScintillaControl sci, int position, int length, int linesAdded)
        {
            if (OnTextChanged != null && !DisableEvents) 
                OnTextChanged(sci, position, length, linesAdded);
        }
        private void OnTextDeleted(ScintillaControl sci, int position, int length, int linesAdded)
        {
            if (OnTextChanged != null && !DisableEvents) 
                OnTextChanged(sci, position, -length, linesAdded);
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            var sci = (ScintillaControl)sender;
            ((CompletionList.ScintillaHost)CompletionList.Host).SciControl = sci;
            var language = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage);
            if (language != null)   // Should we provide some custom string otherwise?
                CompletionList.CharacterClass = language.characterclass.Characters;
        }

        private void OnChar(ScintillaControl sci, int value)
        {
            if (sci == null || DisableEvents) return;
            if (!CompletionList.Active && !callTip.CallTipActive)
            {
                SendChar(sci, value);
                return;
            }
            //if (lockedSciControl != null && lockedSciControl.IsAlive) sci = (ScintillaControl)lockedSciControl.Target;
            //else
            //{
            //    callTip.Hide();
            //    CompletionList.Hide();
            //    SendChar(sci, value);
            //    return;
            //}
            
            if (callTip.CallTipActive) callTip.OnChar(value);
            if (CompletionList.Active) Controls.CompletionList.OnChar(sci, value);
            else SendChar(sci, value);
        }

        public void SendChar(ScintillaControl sci, int value)
        {
            if (OnCharAdded != null) OnCharAdded(sci, value);	
        }
        
        private bool HandleKeys(Keys key)
        {
            // UITools is currently broadcasting a shortcut, ignore!
            if (ignoreKeys || DisableEvents) return false;
            
            // list/tip shortcut dispatching
            if ((key == (Keys.Control | Keys.Space)) || (key == (Keys.Shift | Keys.Control | Keys.Space)))
            {
                ignoreKeys = true;
                KeyEvent ke = new KeyEvent(EventType.Keys, key);
                EventManager.DispatchEvent(this, ke);
                ignoreKeys = false;
                // if not handled - show snippets
                if (!ke.Handled && PluginBase.MainForm.CurrentDocument.IsEditable && PluginBase.MainForm.CurrentDocument.SciControl.ContainsFocus 
                    && !PluginBase.MainForm.CurrentDocument.SciControl.IsSelectionRectangle)
                {
                    PluginBase.MainForm.CallCommand("InsertSnippet", "null");
                    ke.Handled = true;
                }
                
                return ke.Handled;
            }

            // toggle "long-description" for the hover tooltip
            if (key == Keys.F1 && Tip.Visible && !CompletionList.Active && !CallTip.Visible)
            {
                showDetails = !showDetails;
                simpleTip.UpdateTip();
                return true;
            }

            return false;
        }
        
        
        /// <summary>
        /// Compute current editor line height
        /// </summary>
        public int LineHeight(ScintillaControl sci)
        {
            if (sci == null) return 0;
            // evaluate the font size
            Font tempFont = new Font(sci.Font.Name, sci.Font.Size+sci.ZoomLevel);
            Graphics g = sci.CreateGraphics();
            SizeF textSize = g.MeasureString("S", tempFont);
            return (int)Math.Ceiling(textSize.Height);
        }

        #endregion
    }
}
