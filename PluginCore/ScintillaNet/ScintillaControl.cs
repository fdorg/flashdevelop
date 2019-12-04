using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ScintillaNet.Configuration;
using ScintillaNet.Lexers;
using PluginCore.FRService;
using PluginCore.Managers;
using PluginCore.Controls;
using PluginCore;

namespace ScintillaNet
{
    public class ScintillaControl : Control, IEventHandler
    {
        bool saveBOM;
        Encoding encoding;
        readonly IntPtr directPointer;
        readonly Perform _sciFunction;
        bool hasHighlights;
        bool isBraceMatching = true;
        bool isHiliteSelected = true;
        bool useHighlightGuides = true;
        System.Timers.Timer highlightDelay;
        static readonly Dictionary<string, ShortcutOverride> shortcutOverrides = new Dictionary<string, ShortcutOverride>();
        Enums.SmartIndent smartIndent = Enums.SmartIndent.CPP;
        readonly Hashtable ignoredKeys = new Hashtable();
        string configLanguage = string.Empty;
        string fileName = string.Empty;
        int lastSelectionLength;
        int lastSelectionStart;
        int lastSelectionEnd;

        #region ScrollBars

        ScrollBarEx vScrollBar;
        ScrollBarEx hScrollBar;
        Control scrollerCorner;

        /// <summary>
        /// Is the vertical scroll bar visible?
        /// </summary>
        public bool IsVScrollBar
        {
            get
            {
                if (Controls.Contains(vScrollBar)) return vScrollBar.Visible;
                return SPerform(2281, 0, 0) != 0;
            }
            set
            {
                if (Controls.Contains(vScrollBar)) vScrollBar.Visible = value;
                else SPerform(2280, value ? 1 : 0, 0);
            }
        }

        /// <summary>
        /// Is the horizontal scroll bar visible? 
        /// </summary>
        public bool IsHScrollBar
        {
            get
            {
                if (Controls.Contains(hScrollBar)) return hScrollBar.Visible;
                return SPerform(2131, 0, 0) != 0;
            }
            set
            {
                if (Controls.Contains(hScrollBar)) hScrollBar.Visible = value;
                else SPerform(2130, value ? 1 : 0, 0);
            }
        }

        /// <summary>
        /// Handle the incoming theme events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type != EventType.ApplyTheme) return;
            var value = PluginBase.MainForm.GetThemeValue("ScrollBar.UseCustom");
            if (value == "True" || (value is null && PluginBase.MainForm.GetThemeColor("ScrollBar.ForeColor") != Color.Empty))
            {
                if (!Controls.Contains(vScrollBar)) AddScrollBars(this);
                UpdateScrollBarTheme(this);
            }
            else if (Controls.Contains(vScrollBar))
            {
                RemoveScrollBars(this);
            }
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        void UpdateScrollBarTheme(ScintillaControl sender)
        {
            PluginBase.MainForm.ThemeControls(sender.vScrollBar);
            PluginBase.MainForm.ThemeControls(sender.hScrollBar);
            // Apply settings so that old defaults work...
            sender.vScrollBar.ArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ArrowColor", sender.vScrollBar.ForeColor);
            sender.vScrollBar.HotArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotArrowColor", sender.vScrollBar.ForeColor);
            sender.vScrollBar.ActiveArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ActiveArrowColor", sender.vScrollBar.ActiveForeColor);
            sender.vScrollBar.HotForeColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotForeColor", sender.vScrollBar.ForeColor);
            sender.hScrollBar.ArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ArrowColor", sender.hScrollBar.ForeColor);
            sender.hScrollBar.HotArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotArrowColor", sender.hScrollBar.ForeColor);
            sender.hScrollBar.ActiveArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ActiveArrowColor", sender.hScrollBar.ActiveForeColor);
            sender.hScrollBar.HotForeColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotForeColor", sender.hScrollBar.ForeColor);
            sender.scrollerCorner.BackColor = PluginBase.MainForm.GetThemeColor("ScrollBar.BackColor", sender.vScrollBar.BackColor);
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        void InitScrollBars(ScintillaControl sender)
        {
            sender.vScrollBar = new ScrollBarEx();
            sender.vScrollBar.Width = ScrollBarEx.ScaleOddUp(17); // Should be odd for nice and crisp arrow points.
            sender.vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            sender.vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            sender.hScrollBar = new ScrollBarEx();
            sender.hScrollBar.Height = ScrollBarEx.ScaleOddUp(17); // Should be odd for nice and crisp arrow points.
            sender.hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            sender.hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            sender.scrollerCorner = new Control();
            sender.scrollerCorner.Width = sender.vScrollBar.Width;
            sender.scrollerCorner.Height = sender.hScrollBar.Height;
            var value = PluginBase.MainForm.GetThemeValue("ScrollBar.UseCustom");
            if (value == "True" || (value is null && PluginBase.MainForm.GetThemeColor("ScrollBar.ForeColor") != Color.Empty))
            {
                sender.AddScrollBars(sender);
                sender.UpdateScrollBarTheme(sender);
            }
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
        }

        /// <summary>
        /// Update the scrollbars on sci control ui update
        /// </summary>
        void OnScrollUpdate(ScintillaControl sender)
        {
            var vTotal = sender.LinesVisible;
            var vPage = sender.LinesOnScreen;
            var vMax = sender.EndAtLastLine == 0 ? (vTotal - 1) : (vTotal - vPage);
            sender.vScrollBar.Scroll -= sender.OnScrollBarScroll;
            sender.vScrollBar.Minimum = 0;
            sender.vScrollBar.Maximum = vMax;
            sender.vScrollBar.ViewPortSize = sender.vScrollBar.LargeChange = vPage;
            sender.vScrollBar.Value = sender.FirstVisibleLine;
            sender.vScrollBar.CurrentPosition = (vMax > 0) ? sender.VisibleFromDocLine(sender.CurrentLine) : -1;
            sender.vScrollBar.MaxCurrentPosition = vTotal - 1;
            sender.vScrollBar.Scroll += sender.OnScrollBarScroll;
            sender.vScrollBar.Enabled = vMax > 0;
            sender.hScrollBar.Scroll -= sender.OnScrollBarScroll;
            sender.hScrollBar.Minimum = 0;
            sender.hScrollBar.Maximum = sender.ScrollWidth;
            sender.hScrollBar.ViewPortSize = sender.hScrollBar.LargeChange = sender.Width;
            sender.hScrollBar.Value = sender.XOffset;
            sender.hScrollBar.Scroll += sender.OnScrollBarScroll;
        }

        /// <summary>
        /// Update the sci control on scrollbar scroll
        /// </summary>
        void OnScrollBarScroll(object sender, ScrollEventArgs e)
        {
            Painted -= OnScrollUpdate;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (e.OldValue != -1) FirstVisibleLine = e.NewValue;
            }
            else XOffset = hScrollBar.Value;
            Painted += OnScrollUpdate;
        }

        /// <summary>
        /// Add controls to container
        /// </summary>
        void AddScrollBars(ScintillaControl sender)
        {
            sender.IsVScrollBar = false; // Hide builtin
            sender.IsHScrollBar = false; // Hide builtin
            sender.vScrollBar.VisibleChanged += OnResize;
            sender.hScrollBar.VisibleChanged += OnResize;
            sender.vScrollBar.Scroll += sender.OnScrollBarScroll;
            sender.hScrollBar.Scroll += sender.OnScrollBarScroll;
            sender.Controls.Add(sender.hScrollBar);
            sender.Controls.Add(sender.vScrollBar);
            sender.Controls.Add(sender.scrollerCorner);
            sender.Painted += sender.OnScrollUpdate;
            sender.IsVScrollBar = sender.IsVScrollBar;
            sender.IsHScrollBar = sender.IsHScrollBar;
            sender.OnResize(null, null);
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        void RemoveScrollBars(ScintillaControl sender)
        {
            sender.vScrollBar.VisibleChanged -= OnResize;
            sender.hScrollBar.VisibleChanged -= OnResize;
            sender.vScrollBar.Scroll -= sender.OnScrollBarScroll;
            sender.hScrollBar.Scroll -= sender.OnScrollBarScroll;
            sender.Controls.Remove(sender.hScrollBar);
            sender.Controls.Remove(sender.vScrollBar);
            sender.Controls.Remove(sender.scrollerCorner);
            sender.Painted -= sender.OnScrollUpdate;
            sender.IsVScrollBar = sender.IsVScrollBar;
            sender.IsHScrollBar = sender.IsHScrollBar;
            sender.OnResize(null, null);
        }

        #endregion

        #region Scintilla Main

        public ScintillaControl() : this(IntPtr.Size == 4 ? "SciLexer.dll" : "SciLexer64.dll")
        {
            if (Win32.ShouldUseWin32()) DragAcceptFiles(Handle, 1);
        }

        public ScintillaControl(string fullPath)
        {
            if (Win32.ShouldUseWin32())
            {
                IntPtr lib = LoadLibrary(fullPath);
                HandleSci = CreateWindowEx(0, "Scintilla", "", WS_CHILD_VISIBLE_TABSTOP, 0, 0, Width, Height, Handle, 0, new IntPtr(0), null);
                directPointer = (IntPtr)SlowPerform(2185, 0, 0);
                IntPtr sciFunctionPointer = GetProcAddress(new HandleRef(null, lib), "Scintilla_DirectFunction");
                if (sciFunctionPointer == IntPtr.Zero) sciFunctionPointer = GetProcAddress(new HandleRef(null, lib), "_Scintilla_DirectFunction@16");
                if (sciFunctionPointer == IntPtr.Zero)
                {
                    string msg = "The Scintilla module has no export for the 'Scintilla_DirectFunction' procedure.";
                    throw new Win32Exception(msg, new Win32Exception(Marshal.GetLastWin32Error()));
                }
                _sciFunction = (Perform)Marshal.GetDelegateForFunctionPointer(sciFunctionPointer, typeof(Perform));
                directPointer = DirectPointer;
            }
            UpdateUI += OnUpdateUI;
            UpdateUI += OnBraceMatch;
            UpdateUI += OnCancelHighlight;
            DoubleClick += OnBlockSelect;
            CharAdded += OnSmartIndent;
            Resize += OnResize;
            InitScrollBars(this);
        }

        protected override void Dispose(bool disposing)
        {
            EventManager.RemoveEventHandler(this);
            highlightDelay?.Stop();
            base.Dispose(disposing);
        }

        public void OnResize(object sender, EventArgs e)
        {
            int vsbWidth = Controls.Contains(vScrollBar) && vScrollBar.Visible ? vScrollBar.Width : 0;
            int hsbHeight = Controls.Contains(hScrollBar) && hScrollBar.Visible ? hScrollBar.Height : 0;
            if (Win32.ShouldUseWin32())
            {
                SetWindowPos(HandleSci, 0, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - vsbWidth, ClientRectangle.Height - hsbHeight, 0);
            }
            if (Controls.Contains(vScrollBar))
            {
                vScrollBar.SetBounds(ClientRectangle.Width - vsbWidth, 0, vScrollBar.Width, ClientRectangle.Height - hsbHeight);
                hScrollBar.SetBounds(0, ClientRectangle.Height - hsbHeight, ClientRectangle.Width - vsbWidth, hScrollBar.Height);
                scrollerCorner.Visible = vScrollBar.Visible && hScrollBar.Visible;
                if (scrollerCorner.Visible)
                {
                    scrollerCorner.Location = new Point(vScrollBar.Location.X, hScrollBar.Location.Y);
                }
            }
        }

        #endregion

        #region Scintilla Event Members

        public event KeyHandler Key;
        public event ZoomHandler Zoom;
        public event FocusHandler FocusChanged;
        public event StyleNeededHandler StyleNeeded;
        public event CharAddedHandler CharAdded;
        public event SavePointReachedHandler SavePointReached;
        public event SavePointLeftHandler SavePointLeft;
        public event ModifyAttemptROHandler ModifyAttemptRO;
        public event UpdateUIHandler UpdateUI;
        public event ModifiedHandler Modified;
        public event MacroRecordHandler MacroRecord;
        public event MarginClickHandler MarginClick;
        public event NeedShownHandler NeedShown;
        public event PaintedHandler Painted;
        public event UserListSelectionHandler UserListSelection;
        public event URIDroppedHandler URIDropped;
        public event DwellStartHandler DwellStart;
        public event DwellEndHandler DwellEnd;
        public event HotSpotClickHandler HotSpotClick;
        public event HotSpotDoubleClickHandler HotSpotDoubleClick;
        public event CallTipClickHandler CallTipClick;
        public event AutoCSelectionHandler AutoCSelection;
        public event TextInsertedHandler TextInserted;
        public event TextDeletedHandler TextDeleted;
        public event FoldChangedHandler FoldChanged;
        public event UserPerformedHandler UserPerformed;
        public event UndoPerformedHandler UndoPerformed;
        public event RedoPerformedHandler RedoPerformed;
        public event LastStepInUndoRedoHandler LastStepInUndoRedo;
        public event MarkerChangedHandler MarkerChanged;
        public event BeforeInsertHandler BeforeInsert;
        public event BeforeDeleteHandler BeforeDelete;
        public event SmartIndentHandler SmartIndent;
        public new event StyleChangedHandler StyleChanged;
        public new event DoubleClickHandler DoubleClick;
        public event IndicatorClickHandler IndicatorClick;
        public event IndicatorReleaseHandler IndicatorRelease;
        public event AutoCCancelledHandler AutoCCancelled;
        public event AutoCCharDeletedHandler AutoCCharDeleted;
        public event UpdateSyncHandler UpdateSync;
        public event SelectionChangedHandler SelectionChanged;

        #endregion

        #region Scintilla Properties

        /// <summary>
        /// Gets the sci handle
        /// </summary>
        public IntPtr HandleSci { get; }

        /// <summary>
        /// Current used configuration
        /// </summary> 
        public static Scintilla Configuration { get; set; } = null;

        /// <summary>
        /// Indent view type
        /// </summary>
        public Enums.IndentView IndentView { get; set; } = Enums.IndentView.Real;

        /// <summary>
        /// Current configuration language
        /// </summary>
        public string ConfigurationLanguage
        {
            get => configLanguage;
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                SetLanguage(value);
            }
        }

        /// <summary>
        /// The file extension without the dot or an empty string if there is none
        /// </summary>
        public string GetFileExtension()
        {
            string extension = Path.GetExtension(FileName);
            if (!string.IsNullOrEmpty(extension))
                extension = extension.Substring(1); // remove dot
            return extension;
        }

        public void SaveExtensionToSyntaxConfig(string extension)
        {
            List<Language> languages = Configuration.GetLanguages();
            foreach (Language language in languages)
            {
                if (language.name == configLanguage)
                {
                    language.AddExtension(extension);
                    language.SaveExtensions();
                }
                // remove this extension from other syntax files to avoid conflicts
                else if (language.HasExtension(extension))
                {
                    language.RemoveExtension(extension);
                    language.SaveExtensions();
                }
            }

            foreach (var document in PluginBase.MainForm.Documents)
            {
                var sci = document.SciControl;
                if (sci.GetFileExtension() == extension)
                    sci.ConfigurationLanguage = ConfigurationLanguage;
            }
        }

        void SetLanguage(string value)
        {
            Language lang = Configuration.GetLanguage(value);
            if (lang is null) return;
            StyleClearAll();
            try
            {
                lang.lexer.key = (int)Enum.Parse(typeof(Enums.Lexer), lang.lexer.name, true);
            }
            catch { /* If not found, uses the lang.lexer.key directly. */ }
            configLanguage = value;
            Lexer = lang.lexer.key;
            if (lang.lexer.stylebits > 0) StyleBits = lang.lexer.stylebits;
            if (lang.editorstyle != null)
            {
                EdgeColour = lang.editorstyle.PrintMarginColor;
                CaretFore = lang.editorstyle.CaretForegroundColor;
                CaretLineBack = lang.editorstyle.CaretLineBackgroundColor;
                SetSelBack(true, lang.editorstyle.SelectionBackgroundColor);
                SetSelFore(true, lang.editorstyle.SelectionForegroundColor);
                SetFoldMarginHiColour(true, lang.editorstyle.MarginForegroundColor);
                SetFoldMarginColour(true, lang.editorstyle.MarginBackgroundColor);
                int markerForegroundColor = lang.editorstyle.MarkerForegroundColor;
                int markerBackgroundColor = lang.editorstyle.MarkerBackgroundColor;
                MarkerSetBack((int)Enums.MarkerOutline.Folder, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.Folder, markerForegroundColor);
                MarkerSetBack((int)Enums.MarkerOutline.FolderOpen, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.FolderOpen, markerForegroundColor);
                MarkerSetBack((int)Enums.MarkerOutline.FolderSub, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.FolderSub, markerForegroundColor);
                MarkerSetBack((int)Enums.MarkerOutline.FolderTail, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.FolderTail, markerForegroundColor);
                MarkerSetBack((int)Enums.MarkerOutline.FolderEnd, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.FolderEnd, markerForegroundColor);
                MarkerSetBack((int)Enums.MarkerOutline.FolderOpenMid, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.FolderOpenMid, markerForegroundColor);
                MarkerSetBack((int)Enums.MarkerOutline.FolderMidTail, markerBackgroundColor);
                MarkerSetFore((int)Enums.MarkerOutline.FolderMidTail, markerForegroundColor);
                MarkerSetBack(0, lang.editorstyle.BookmarkLineColor);
                MarkerSetBack(2, lang.editorstyle.ModifiedLineColor);
            }
            if (lang.characterclass != null)
            {
                WordChars(lang.characterclass.Characters);
            }
            var lexerType = ((Enums.Lexer) lang.lexer.key) switch
            {
                Enums.Lexer.PYTHON => typeof(PYTHON),
                Enums.Lexer.CPP => typeof(CPP),
                Enums.Lexer.HTML => typeof(HTML),
                Enums.Lexer.XML => typeof(XML),
                Enums.Lexer.PERL => typeof(PERL),
                Enums.Lexer.SQL => typeof(SQL),
                Enums.Lexer.VB => typeof(VB),
                Enums.Lexer.PROPERTIES => typeof(PROPERTIES),
                Enums.Lexer.ERRORLIST => typeof(ERRORLIST),
                Enums.Lexer.MAKEFILE => typeof(MAKEFILE),
                Enums.Lexer.BATCH => typeof(BATCH),
                Enums.Lexer.LATEX => typeof(LATEX),
                Enums.Lexer.LUA => typeof(LUA),
                Enums.Lexer.DIFF => typeof(DIFF),
                Enums.Lexer.CONF => typeof(CONF),
                Enums.Lexer.PASCAL => typeof(PASCAL),
                Enums.Lexer.AVE => typeof(AVE),
                Enums.Lexer.ADA => typeof(ADA),
                Enums.Lexer.LISP => typeof(LISP),
                Enums.Lexer.RUBY => typeof(RUBY),
                Enums.Lexer.EIFFEL => typeof(EIFFEL),
                Enums.Lexer.EIFFELKW => typeof(EIFFELKW),
                Enums.Lexer.TCL => typeof(TCL),
                Enums.Lexer.NNCRONTAB => typeof(NNCRONTAB),
                Enums.Lexer.BULLANT => typeof(BULLANT),
                Enums.Lexer.VBSCRIPT => typeof(VBSCRIPT),
                Enums.Lexer.BAAN => typeof(BAAN),
                Enums.Lexer.MATLAB => typeof(MATLAB),
                Enums.Lexer.SCRIPTOL => typeof(SCRIPTOL),
                Enums.Lexer.ASM => typeof(ASM),
                Enums.Lexer.FORTRAN => typeof(FORTRAN),
                Enums.Lexer.F77 => typeof(F77),
                Enums.Lexer.CSS => typeof(CSS),
                Enums.Lexer.POV => typeof(POV),
                Enums.Lexer.LOUT => typeof(LOUT),
                Enums.Lexer.ESCRIPT => typeof(ESCRIPT),
                Enums.Lexer.PS => typeof(PS),
                Enums.Lexer.NSIS => typeof(NSIS),
                Enums.Lexer.MMIXAL => typeof(MMIXAL),
                Enums.Lexer.LOT => typeof(LOT),
                Enums.Lexer.YAML => typeof(YAML),
                Enums.Lexer.TEX => typeof(TEX),
                Enums.Lexer.METAPOST => typeof(METAPOST),
                Enums.Lexer.POWERBASIC => typeof(POWERBASIC),
                Enums.Lexer.FORTH => typeof(FORTH),
                Enums.Lexer.ERLANG => typeof(ERLANG),
                Enums.Lexer.OCTAVE => typeof(OCTAVE),
                Enums.Lexer.MSSQL => typeof(MSSQL),
                Enums.Lexer.VERILOG => typeof(VERILOG),
                Enums.Lexer.KIX => typeof(KIX),
                Enums.Lexer.GUI4CLI => typeof(GUI4CLI),
                Enums.Lexer.SPECMAN => typeof(SPECMAN),
                Enums.Lexer.AU3 => typeof(AU3),
                Enums.Lexer.APDL => typeof(APDL),
                Enums.Lexer.BASH => typeof(BASH),
                Enums.Lexer.ASN1 => typeof(ASN1),
                Enums.Lexer.VHDL => typeof(VHDL),
                Enums.Lexer.CAML => typeof(CAML),
                Enums.Lexer.HASKELL => typeof(HASKELL),
                Enums.Lexer.TADS3 => typeof(TADS3),
                Enums.Lexer.REBOL => typeof(REBOL),
                Enums.Lexer.SMALLTALK => typeof(SMALLTALK),
                Enums.Lexer.FLAGSHIP => typeof(FLAGSHIP),
                Enums.Lexer.CSOUND => typeof(CSOUND),
                Enums.Lexer.INNOSETUP => typeof(INNOSETUP),
                Enums.Lexer.OPAL => typeof(OPAL),
                Enums.Lexer.SPICE => typeof(SPICE),
                Enums.Lexer.D => typeof(D),
                Enums.Lexer.CMAKE => typeof(CMAKE),
                Enums.Lexer.GAP => typeof(GAP),
                Enums.Lexer.PLM => typeof(PLM),
                Enums.Lexer.PROGRESS => typeof(PROGRESS),
                Enums.Lexer.ABAQUS => typeof(ABAQUS),
                Enums.Lexer.ASYMPTOTE => typeof(ASYMPTOTE),
                Enums.Lexer.R => typeof(R),
                Enums.Lexer.MAGIK => typeof(MAGIK),
                Enums.Lexer.POWERSHELL => typeof(POWERSHELL),
                Enums.Lexer.MYSQL => typeof(MYSQL),
                Enums.Lexer.PO => typeof(PO),
                Enums.Lexer.SORCUS => typeof(SORCUS),
                Enums.Lexer.POWERPRO => typeof(POWERPRO),
                Enums.Lexer.NIMROD => typeof(NIMROD),
                Enums.Lexer.SML => typeof(SML),
                _ => null,
            };
            foreach (var usestyle in lang.usestyles)
            {
                if (usestyle.key == 0) //name is defined instead of key
                {
                    try
                    {
                        usestyle.key = (int) Enum.Parse(lexerType, usestyle.name, true);
                    }
                    catch (Exception ex)
                    {
                        string info;
                        if (lexerType is null)
                        {
                            info = $"Lexer '{lang.lexer.name}' ({lang.lexer.key}) unknown.";
                            ErrorManager.ShowWarning(info, ex);
                            break;
                        }

                        info = $"Style '{usestyle.name}' in syntax file is not used by lexer '{lexerType.Name}'.";
                        ErrorManager.ShowWarning(info, ex);
                    }
                }
                // Set whitespace fore color to indentguide color
                if (usestyle.key == (int)Enums.StylesCommon.IndentGuide)
                {
                    SetWhitespaceFore(true, usestyle.ForegroundColor);
                }
                if (usestyle.HasForegroundColor) StyleSetFore(usestyle.key, usestyle.ForegroundColor);
                if (usestyle.HasBackgroundColor) StyleSetBack(usestyle.key, usestyle.BackgroundColor);
                if (usestyle.HasFontName) StyleSetFont(usestyle.key, usestyle.FontName);
                if (usestyle.HasFontSize) StyleSetSize(usestyle.key, usestyle.FontSize);
                if (usestyle.HasBold) StyleSetBold(usestyle.key, usestyle.IsBold);
                if (usestyle.HasItalics) StyleSetItalic(usestyle.key, usestyle.IsItalics);
                if (usestyle.HasEolFilled) StyleSetEOLFilled(usestyle.key, usestyle.IsEolFilled);
            }
            // Clear the keywords lists 
            for (int j = 0; j < 9; j++) KeyWords(j, "");
            foreach (var usekeyword in lang.usekeywords)
            {
                KeywordClass kc = Configuration.GetKeywordClass(usekeyword.cls);
                if (kc != null) KeyWords(usekeyword.key, kc.val);
            }

            UpdateSync?.Invoke(this);
        }

        /// <summary>
        /// Texts in the control
        /// </summary> 
        public override string Text
        {
            get => GetText(Length);
            set => SetText(value);
        }

        /// <summary>
        /// Filename of the editable document
        /// </summary> 
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Is the control focused?
        /// </summary> 
        public override bool Focused => IsFocus;

        /// <summary>
        /// Should we ignore recieved keys? 
        /// </summary> 
        public bool IgnoreAllKeys { get; set; } = false;

        /// <summary>
        /// Enables the selected word highlighting.
        /// </summary>
        public bool IsHiliteSelected
        {
            get => isHiliteSelected;
            set
            {
                isHiliteSelected = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Enables the brace matching from current position.
        /// </summary>
        public bool IsBraceMatching
        {
            get => isBraceMatching;
            set
            {
                isBraceMatching = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Enables the highlight guides from current position.
        /// </summary>
        public bool UseHighlightGuides
        {
            get => useHighlightGuides;
            set
            {
                useHighlightGuides = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Enables the Smart Indenter so that On enter, it indents the next line.
        /// </summary>
        public Enums.SmartIndent SmartIndentType
        {
            get => smartIndent;
            set
            {
                smartIndent = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Are white space characters currently visible?
        /// Returns one of Enums.WhiteSpace constants.
        /// </summary>
        public Enums.WhiteSpace ViewWhitespace
        {
            get => (Enums.WhiteSpace)ViewWS;
            set => ViewWS = (int)value;
        }

        /// <summary>
        /// Get or sets the background alpha of the caret line.
        /// </summary>
        public int CaretLineBackAlpha
        {
            get => SPerform(2471, 0, 0);
            set => SPerform(2470, value, 0);
        }

        /// <summary>
        /// Get or sets the background alpha of the caret line.
        /// </summary>
        public int CaretStyle
        {
            get => SPerform(2513, 0, 0);
            set => SPerform(2512, value, 0);
        }

        /// <summary>
        /// Sets or gets the indicator used for IndicatorFillRange and IndicatorClearRange
        /// </summary>
        public int CurrentIndicator
        {
            get => SPerform(2501, 0, 0);
            set => SPerform(2500, value, 0);
        }

        /// <summary>
        /// Gets or sets the number of entries in position cache.
        /// </summary>
        public int PositionCache
        {
            get => SPerform(2515, 0, 0);
            set => SPerform(2514, value, 0);
        }

        /// <summary>
        /// Get the current indicator value or sets the value used for IndicatorFillRange.
        /// </summary>
        public int IndicatorValue
        {
            get => SPerform(2503, 0, 0);
            set => SPerform(2502, value, 0);
        }

        /// <summary>
        /// Retrieve the current end of line mode - one of CRLF, CR, or LF.
        /// </summary>
        public Enums.EndOfLine EndOfLineMode
        {
            get => (Enums.EndOfLine)EOLMode;
            set => EOLMode = (int)value;
        }

        /// <summary>
        /// Length Method for : Retrieve the text of the line containing the caret.
        /// Returns the index of the caret on the line.
        /// </summary>
        public int CurLineSize => SPerform(2027, 0, 0) - 1;

        /// <summary>
        /// Length Method for : Retrieve the contents of a line.
        /// Returns the length of the line.
        /// </summary>
        public int LineSize => SPerform(2153, 0, 0);

        /// <summary>
        /// Length Method for : Retrieve the selected text.
        /// Return the length of the text.
        /// </summary>
        public int SelTextSize => SPerform(2161, 0, 0) - 1;

        /// <summary>
        /// Length Method for : Retrieve all the text in the document.
        /// Returns number of characters retrieved.
        /// </summary>
        public int TextSize => SPerform(2182, 0, 0);

        /// <summary>
        /// Are there any redoable actions in the undo history?
        /// </summary>
        public bool CanRedo => SPerform(2016, 0, 0) != 0;

        /// <summary>
        /// Is there an auto-completion list visible?
        /// </summary>
        public bool IsAutoCActive => SPerform(2102, 0, 0) != 0;

        /// <summary>
        /// Retrieve the position of the caret when the auto-completion list was displayed.
        /// </summary>
        public int AutoCPosStart => SPerform(2103, 0, 0);

        /// <summary>
        /// Will a paste succeed?
        /// </summary>
        public bool CanPaste => SPerform(2173, 0, 0) != 0;

        /// <summary>
        /// Are there any undoable actions in the undo history?
        /// </summary>
        public bool CanUndo => SPerform(2174, 0, 0) != 0;

        /// <summary>
        /// Is there an active call tip?
        /// </summary>
        public bool IsCallTipActive => SPerform(2202, 0, 0) != 0;

        /// <summary>
        /// Retrieve the position where the caret was before displaying the call tip.
        /// </summary>
        public int CallTipPosStart => SPerform(2203, 0, 0);

        /// <summary>
        /// Create a new document object.
        /// Starts with reference count of 1 and not selected into editor.
        /// </summary>
        public int CreateDocument => SPerform(2375, 0, 0);

        /// <summary>
        /// Get currently selected item position in the auto-completion list
        /// </summary>
        public int AutoCGetCurrent => SPerform(2445, 0, 0);

        /// <summary>
        /// Returns the number of characters in the document.
        /// </summary>
        public int Length => SPerform(2006, 0, 0);

        /// <summary>
        /// Enable/Disable convert-on-paste for line endings
        /// </summary>
        public bool PasteConvertEndings
        {
            get => SPerform(2468, 0, 0) != 0;
            set => SPerform(2467, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Returns the position of the caret.
        /// </summary>
        public int CurrentPos
        {
            get => SPerform(2008, 0, 0);
            set => SPerform(2141, value, 0);
        }

        /// <summary>
        /// Returns the character at the caret position.
        /// </summary>
        public char CurrentChar => (char)CharAt(CurrentPos);

        /// <summary>
        /// Returns the line containing the current position.
        /// </summary>
        public int CurrentLine => LineFromPosition(CurrentPos);

        /// <summary>
        /// Returns the position of the opposite end of the selection to the caret.
        /// </summary>
        public int AnchorPosition
        {
            get => SPerform(2009, 0, 0);
            set => SPerform(2026, value, 0);
        }

        /// <summary>
        /// Is undo history being collected?
        /// </summary>
        public bool IsUndoCollection
        {
            get => SPerform(2019, 0, 0) != 0;
            set => SPerform(2012, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Are white space characters currently visible?
        /// Returns one of SCWS_/// constants.
        /// </summary>
        public int ViewWS
        {
            get => SPerform(2020, 0, 0);
            set => SPerform(2021, value, 0);
        }

        /// <summary>
        /// Retrieve the position of the last correctly styled character.
        /// </summary>
        public int EndStyled => SPerform(2028, 0, 0);

        /// <summary>
        /// Retrieve the current end of line mode - one of CRLF, CR, or LF.
        /// </summary>
        public int EOLMode
        {
            get => SPerform(2030, 0, 0);
            set => SPerform(2031, value, 0);
        }

        /// <summary>
        /// Is drawing done first into a buffer or direct to the screen?
        /// </summary>
        public bool IsBufferedDraw
        {
            get => SPerform(2034, 0, 0) != 0;
            set => SPerform(2035, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve the visible size of a tab.
        /// </summary>
        public int TabWidth
        {
            get => SPerform(2121, 0, 0);
            set => SPerform(2036, value, 0);
        }

        /// <summary>
        /// Get the time in milliseconds that the caret is on and off.
        /// </summary>
        public int CaretPeriod
        {
            get => SPerform(2075, 0, 0);
            set => SPerform(2076, value, 0);
        }

        /// <summary>
        /// Retrieve number of bits in style bytes used to hold the lexical state.
        /// </summary>
        public int StyleBits
        {
            get => SPerform(2091, 0, 0);
            set => SPerform(2090, value, 0);
        }

        /// <summary>
        /// Retrieve the last line number that has line state.
        /// </summary>
        public int MaxLineState => SPerform(2094, 0, 0);

        /// <summary>
        /// Is the background of the line containing the caret in a different colour?
        /// </summary>
        public bool IsCaretLineVisible
        {
            get => SPerform(2095, 0, 0) != 0;
            set => SPerform(2096, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Get the colour of the background of the line containing the caret.
        /// </summary>
        public int CaretLineBack
        {
            get => SPerform(2097, 0, 0);
            set => SPerform(2098, value, 0);
        }

        /// <summary>
        /// Retrieve the auto-completion list separator character.
        /// </summary>
        public int AutoCSeparator
        {
            get => SPerform(2107, 0, 0);
            set => SPerform(2106, value, 0);
        }

        /// <summary>
        /// Retrieve whether auto-completion cancelled by backspacing before start.
        /// </summary>
        public bool IsAutoCGetCancelAtStart
        {
            get => SPerform(2111, 0, 0) != 0;
            set => SPerform(2110, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve whether a single item auto-completion list automatically choose the item.
        /// </summary>
        public bool IsAutoCGetChooseSingle
        {
            get => SPerform(2114, 0, 0) != 0;
            set => SPerform(2113, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve state of ignore case flag.
        /// </summary>
        public bool IsAutoCGetIgnoreCase
        {
            get => SPerform(2116, 0, 0) != 0;
            set => SPerform(2115, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve whether or not autocompletion is hidden automatically when nothing matches.
        /// </summary>
        public bool IsAutoCGetAutoHide
        {
            get => SPerform(2119, 0, 0) != 0;
            set => SPerform(2118, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve whether or not autocompletion deletes any word characters
        /// after the inserted text upon completion.
        /// </summary>
        public bool IsAutoCGetDropRestOfWord
        {
            get => SPerform(2271, 0, 0) != 0;
            set => SPerform(2270, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve the auto-completion list type-separator character.
        /// </summary>
        public int AutoCTypeSeparator
        {
            get => SPerform(2285, 0, 0);
            set => SPerform(2286, value, 0);
        }

        /// <summary>
        /// Retrieve indentation size.
        /// </summary>
        public int Indent
        {
            get => SPerform(2123, 0, 0);
            set => SPerform(2122, value, 0);
        }

        /// <summary>
        /// Retrieve whether tabs will be used in indentation.
        /// </summary>
        public bool IsUseTabs
        {
            get => SPerform(2125, 0, 0) != 0;
            set => SPerform(2124, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Are the indentation guides visible?
        /// </summary>
        public bool IsIndentationGuides
        {
            get => SPerform(2133, 0, 0) != 0;
            set => SPerform(2132, value ? (int)IndentView : 0, 0);
        }

        /// <summary>
        /// Get the highlighted indentation guide column.
        /// </summary>
        public int HighlightGuide
        {
            get => SPerform(2135, 0, 0);
            set => SPerform(2134, value, 0);
        }

        /// <summary>
        /// Get the code page used to interpret the bytes of the document as characters.
        /// </summary>
        public int CodePage
        {
            get => SPerform(2137, 0, 0);
            set => SPerform(2037, value, 0);
        }

        /// <summary>
        /// Get the foreground colour of the caret.
        /// </summary>
        public int CaretFore
        {
            get => SPerform(2138, 0, 0);
            set => SPerform(2069, value, 0);
        }

        /// <summary>
        /// In palette mode?
        /// </summary>
        public bool IsUsePalette
        {
            get => SPerform(2139, 0, 0) != 0;
            set => SPerform(2039, value ? 1 : 0, 0);
        }

        /// <summary>
        /// In read-only mode?
        /// </summary>
        public bool IsReadOnly
        {
            get => SPerform(2140, 0, 0) != 0;
            set => SPerform(2171, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Returns the position at the start of the selection.
        /// </summary>
        public int SelectionStart
        {
            get => SPerform(2143, 0, 0);
            set => SPerform(2142, value, 0);
        }

        /// <summary>
        /// Returns the position at the end of the selection.
        /// </summary>
        public int SelectionEnd
        {
            get => SPerform(2145, 0, 0);
            set => SPerform(2144, value, 0);
        }

        /// <summary>
        /// Returns true if the selection extends over more than one line.
        /// </summary>
        public bool IsSelectionMultiline => LineFromPosition(SelectionStart) != LineFromPosition(SelectionEnd);

        /// <summary>
        /// Returns the print magnification.
        /// </summary>
        public int PrintMagnification
        {
            get => SPerform(2147, 0, 0);
            set => SPerform(2146, value, 0);
        }

        /// <summary>
        /// Returns the print colour mode.
        /// </summary>
        public int PrintColourMode
        {
            get => SPerform(2149, 0, 0);
            set => SPerform(2148, value, 0);
        }

        /// <summary>
        /// Retrieve the display line at the top of the display.
        /// </summary>
        public int FirstVisibleLine
        {
            set => SPerform(2613, value, 0);
            get => SPerform(2152, 0, 0);
        }

        /// <summary>
        /// Returns the number of lines in the document. There is always at least one.
        /// </summary>
        public int LineCount => SPerform(2154, 0, 0);

        /// <summary>
        /// Returns the size in pixels of the left margin.
        /// </summary>
        public int MarginLeft
        {
            get => SPerform(2156, 0, 0);
            set => SPerform(2155, 0, value);
        }

        /// <summary>
        /// Returns the size in pixels of the right margin.
        /// </summary>
        public int MarginRight
        {
            get => SPerform(2158, 0, 0);
            set => SPerform(2157, 0, value);
        }

        /// <summary>
        /// Is the document different from when it was last saved?
        /// </summary>  
        public bool IsModify => SPerform(2159, 0, 0) != 0;

        /// <summary>
        /// Retrieve the number of characters in the document.
        /// </summary>
        public int TextLength => SPerform(2183, 0, 0);

        /// <summary>
        /// Retrieve a pointer to a function that processes messages for this Scintilla.
        /// </summary>
        public int DirectFunction => SPerform(2184, 0, 0);

        /// <summary>
        /// Retrieve a pointer value to use as the first argument when calling
        /// the function returned by GetDirectFunction.
        /// </summary>
        public IntPtr DirectPointer => (IntPtr)SPerform(2185, 0, 0);

        /// <summary>
        /// Returns true if overtype mode is active otherwise false is returned.
        /// </summary>
        public bool IsOvertype
        {
            get => SPerform(2187, 0, 0) != 0;
            set => SPerform(2186, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Returns the width of the insert mode caret.
        /// </summary>
        public int CaretWidth
        {
            get => SPerform(2189, 0, 0);
            set => SPerform(2188, value, 0);
        }

        /// <summary>
        /// Get the position that starts the target. 
        /// </summary>
        public int TargetStart
        {
            get => SPerform(2191, 0, 0);
            set => SPerform(2190, value, 0);
        }

        /// <summary>
        /// Get the position that ends the target.
        /// </summary>
        public int TargetEnd
        {
            get => SPerform(2193, 0, 0);
            set => SPerform(2192, value, 0);
        }

        /// <summary>
        /// Get the search flags used by SearchInTarget.
        /// </summary>
        public int SearchFlags
        {
            get => SPerform(2199, 0, 0);
            set => SPerform(2198, value, 0);
        }

        /// <summary>
        /// Does a tab pressed when caret is within indentation indent?
        /// </summary>
        public bool IsTabIndents
        {
            get => SPerform(2261, 0, 0) != 0;
            set => SPerform(2260, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Does a backspace pressed when caret is within indentation unindent?
        /// </summary>
        public bool IsBackSpaceUnIndents
        {
            get => SPerform(2263, 0, 0) != 0;
            set => SPerform(2262, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve the time the mouse must sit still to generate a mouse dwell event.
        /// </summary>
        public int MouseDwellTime
        {
            get => SPerform(2265, 0, 0);
            set => SPerform(2264, value, 0);
        }

        /// <summary>
        /// Retrieve whether text is word wrapped.
        /// </summary>
        public int WrapMode
        {
            get => SPerform(2269, 0, 0);
            set => SPerform(2268, value, 0);
        }

        /// <summary>
        /// Retrieve the display mode of visual flags for wrapped lines.
        /// </summary>
        public int WrapVisualFlags
        {
            get => SPerform(2461, 0, 0);
            set => SPerform(2460, value, 0);
        }

        /// <summary>
        /// Retrieve the location of visual flags for wrapped lines.
        /// </summary>
        public int WrapVisualFlagsLocation
        {
            get => SPerform(2463, 0, 0);
            set => SPerform(2462, value, 0);
        }

        /// <summary>
        /// Retrieve the start indent for wrapped lines.
        /// </summary>
        public int WrapStartIndent
        {
            get => SPerform(2465, 0, 0);
            set => SPerform(2464, value, 0);
        }

        /// <summary>
        /// Retrieve the degree of caching of layout information.
        /// </summary>
        public int LayoutCache
        {
            get => SPerform(2273, 0, 0);
            set => SPerform(2272, value, 0);
        }

        /// <summary>
        /// Retrieve the document width assumed for scrolling.
        /// </summary>
        public int ScrollWidth
        {
            get => SPerform(2275, 0, 0);
            set => SPerform(2274, value, 0);
        }

        /// <summary>
        /// Retrieve whether the maximum scroll position has the last
        /// line at the bottom of the view.
        /// </summary>
        public int EndAtLastLine
        {
            get => SPerform(2278, 0, 0);
            set => SPerform(2277, value, 0);
        }

        /// <summary>
        /// Is drawing done in two phases with backgrounds drawn before faoregrounds?
        /// </summary>
        public bool IsTwoPhaseDraw
        {
            get => SPerform(2283, 0, 0) != 0;
            set => SPerform(2284, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Are the end of line characters visible?
        /// </summary>
        public bool IsViewEOL
        {
            get => SPerform(2355, 0, 0) != 0;
            set => SPerform(2356, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Retrieve a pointer to the document object.
        /// </summary>
        public int DocPointer
        {
            get => SPerform(2357, 0, 0);
            set => SPerform(2358, 0, value);
        }

        /// <summary>
        /// Retrieve the column number which text should be kept within.
        /// </summary>
        public int EdgeColumn
        {
            get => SPerform(2360, 0, 0);
            set => SPerform(2361, value, 0);
        }

        /// <summary>
        /// Retrieve the edge highlight mode.
        /// </summary>
        public int EdgeMode
        {
            get => SPerform(2362, 0, 0);
            set => SPerform(2363, value, 0);
        }

        /// <summary>
        /// Retrieve the colour used in edge indication.
        /// </summary>
        public int EdgeColour
        {
            get => SPerform(2364, 0, 0);
            set => SPerform(2365, value, 0);
        }

        /// <summary>
        /// Retrieves the number of lines completely visible.
        /// </summary>
        public int LinesOnScreen => SPerform(2370, 0, 0);

        /// <summary>
        /// Is the selection rectangular? The alternative is the more common stream selection. 
        /// </summary>  
        public bool IsSelectionRectangle => SPerform(2372, 0, 0) != 0;

        /// <summary>
        /// Set the zoom level. This number of points is added to the size of all fonts.
        /// It may be positive to magnify or negative to reduce. Retrieve the zoom level.
        /// </summary>
        public int ZoomLevel
        {
            get => SPerform(2374, 0, 0);
            set => SPerform(2373, value, 0);
        }

        /// <summary>
        /// Get which document modification events are sent to the container.
        /// </summary>
        public int ModEventMask
        {
            get => SPerform(2378, 0, 0);
            set => SPerform(2359, value, 0);
        }

        /// <summary>
        /// Change internal focus flag. Get internal focus flag.
        /// </summary>
        public bool IsFocus
        {
            get => SPerform(2381, 0, 0) != 0;
            set => SPerform(2380, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Change error status - 0 = OK. Get error status.
        /// </summary>
        public int Status
        {
            get => SPerform(2383, 0, 0);
            set => SPerform(2382, value, 0);
        }

        /// <summary>
        /// Set whether the mouse is captured when its button is pressed. Get whether mouse gets captured.
        /// </summary>
        public bool IsMouseDownCaptures
        {
            get => SPerform(2385, 0, 0) != 0;
            set => SPerform(2384, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Sets the cursor to one of the SC_CURSOR/// values. Get cursor type.
        /// </summary>
        public int CursorType
        {
            get => SPerform(2387, 0, 0);
            set => SPerform(2386, value, 0);
        }

        /// <summary>
        /// Change the way control characters are displayed:
        /// If symbol is < 32, keep the drawn way, else, use the given character.
        /// Get the way control characters are displayed.
        /// </summary>
        public int ControlCharSymbol
        {
            get => SPerform(2389, 0, 0);
            set => SPerform(2388, value, 0);
        }

        /// <summary>
        /// Get and Set the xOffset (ie, horizontal scroll position).
        /// </summary>
        public int XOffset
        {
            get => SPerform(2398, 0, 0);
            set => SPerform(2397, value, 0);
        }

        /// <summary>
        /// Is printing line wrapped?
        /// </summary>
        public int PrintWrapMode
        {
            get => SPerform(2407, 0, 0);
            set => SPerform(2406, value, 0);
        }

        /// <summary>
        /// Get the mode of the current selection.
        /// </summary>
        public int SelectionMode
        {
            get => SPerform(2423, 0, 0);
            set => SPerform(2422, value, 0);
        }

        /// <summary>
        /// Retrieve the lexing language of the document.
        /// </summary>
        public int Lexer
        {
            get => SPerform(4002, 0, 0);
            set => SPerform(4001, value, 0);
        }

        /// <summary>
        /// Gets the EOL marker
        /// </summary>
        public string NewLineMarker
        {
            get
            {
                if (EOLMode == 1) return "\r";
                if (EOLMode == 2) return "\n";
                return "\r\n";
            }
        }

        /// <summary>
        /// Compact the document buffer and return a read-only pointer to the characters in the document.
        /// </summary>
        public int CharacterPointer => SPerform(2520, 0, 0);

        /// <summary>
        /// Always interpret keyboard input as Unicode
        /// </summary>
        public bool UnicodeKeys
        {
            get => SPerform(2522, 0, 0) != 0;
            set => SPerform(2521, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Set extra ascent for each line
        /// </summary>
        public int ExtraAscent
        {
            get => SPerform(2526, 0, 0);
            set => SPerform(2525, value, 0);
        }

        /// <summary>
        /// Set extra descent for each line
        /// </summary>
        public int ExtraDescent
        {
            get => SPerform(2528, 0, 0);
            set => SPerform(2527, value, 0);
        }

        /// <summary>
        /// Get the start of the range of style numbers used for margin text
        /// </summary>
        public int MarginStyleOffset
        {
            get => SPerform(2538, 0, 0);
            set => SPerform(2537, value, 0);
        }

        /// <summary>
        /// Get the start of the range of style numbers used for annotations
        /// </summary>
        public int AnnotationStyleOffset
        {
            get => SPerform(2551, 0, 0);
            set => SPerform(2550, value, 0);
        }

        /// <summary>
        /// Get the start of the range of style numbers used for annotations
        /// </summary>
        public bool AnnotationVisible
        {
            get => SPerform(2549, 0, 0) != 0;
            set => SPerform(2548, value ? 1 : 0, 0);
        }

        /// <summary>
        /// Sets whether the maximum width line displayed is used to set scroll width.
        /// </summary>
        public bool ScrollWidthTracking
        {
            get => SPerform(2517, 0, 0) != 0;
            set => SPerform(2516, value ? 1 : 0, 0);
        }

        #endregion

        #region Scintilla Methods

        /// <summary>
        /// Adds a new keys to ignore
        /// </summary> 
        public virtual void AddIgnoredKeys(Keys keys) => ignoredKeys.Add((int)keys, (int)keys);

        /// <summary>
        /// Removes the ignored keys
        /// </summary> 
        public virtual void RemoveIgnoredKeys(Keys keys) => ignoredKeys.Remove((int)keys);

        /// <summary>
        /// Clears the ignored keys container
        /// </summary> 
        public virtual void ClearIgnoredKeys() => ignoredKeys.Clear();

        /// <summary>
        /// Does the container have keys?
        /// </summary> 
        public virtual bool ContainsIgnoredKeys(Keys keys) => ignoredKeys.ContainsKey((int)keys);

        /// <summary>
        /// Sets the focus to the control
        /// </summary>
        public new bool Focus() => SetFocus(HandleSci) != IntPtr.Zero;

        /// <summary>
        /// Duplicate the selection. 
        /// If selection empty duplicate the line containing the caret.
        /// </summary>
        public void SelectionDuplicate() => SPerform(2469, 0, 0);

        /// <summary>
        /// Calls SelectionDuplicate and moves the selection
        /// to a convenient position afterwards.
        /// </summary>
        public void SmartSelectionDuplicate()
        {
            bool wholeLine = SelectionStart == SelectionEnd;
            int selectionLength = SelectionEnd - SelectionStart;
            SelectionDuplicate();
            if (wholeLine) LineDown();
            else
            {
                SelectionStart += selectionLength;
                SelectionEnd += selectionLength;
            }
        }

        /// <summary>
        /// Can the caret preferred x position only be changed by explicit movement commands?
        /// </summary>
        public bool GetCaretSticky() => SPerform(2457, 0, 0) != 0;

        /// <summary>
        /// Stop the caret preferred x position changing when the user types.
        /// </summary>
        public void SetCaretSticky(bool useSetting) => SPerform(2458, useSetting ? 1 : 0, 0);

        /// <summary>
        /// Switch between sticky and non-sticky: meant to be bound to a key.
        /// </summary>
        public void ToggleCaretSticky() => SPerform(2459, 0, 0);

        /// <summary>
        /// Retrieve the fold level of a line.
        /// </summary>
        public int GetFoldLevel(int line) => SPerform(2223, line, 0);

        /// <summary>
        /// Set the fold level of a line.
        /// This encodes an integer level along with flags indicating whether the
        /// line is a header and whether it is effectively white space.
        /// </summary>
        public void SetFoldLevel(int line, int level) => SPerform(2222, line, level);

        /// <summary>
        /// Find the last child line of a header line.
        /// </summary>
        public int LastChild(int line, int level) => SPerform(2224, line, level);

        /// <summary>
        /// Find the last child line of a header line. 
        /// </summary>
        public int LastChild(int line) => SPerform(2224, line, 0);

        /// <summary>
        /// Is a line visible?
        /// </summary>  
        public bool GetLineVisible(int line) => SPerform(2228, line, 0) != 0;

        /// <summary>
        /// Find the parent line of a child line.
        /// </summary>
        public int FoldParent(int line) => SPerform(2225, line, 0);

        /// <summary>
        /// Is a header line expanded?
        /// </summary>
        public bool FoldExpanded(int line) => SPerform(2230, line, 0) != 0;

        /// <summary>
        /// Show the children of a header line.
        /// </summary>
        public void FoldExpanded(int line, bool expanded) => SPerform(2229, line, expanded ? 1 : 0);

        /// <summary>
        /// Clear all the styles and make equivalent to the global default style.
        /// </summary>
        public void StyleClearAll() => SPerform(2050, 0, 0);

        /// <summary>
        /// Sets the foreground colour of a style.
        /// </summary>
        public void StyleSetFore(int style, int fore) => SPerform(2051, style, fore);

        /// <summary>
        /// Gets the foreground colour of a style.
        /// </summary>
        public int StyleGetFore(int style) => SPerform(2481, style, 0);

        /// <summary>
        /// Sets the background colour of a style.
        /// </summary>
        public void StyleSetBack(int style, int back) => SPerform(2052, style, back);

        /// <summary>
        /// Gets the background colour of a style
        /// </summary>
        public int StyleGetBack(int style) => SPerform(2482, style, 0);

        /// <summary>
        /// Sets a style to be bold or not.
        /// </summary>
        public void StyleSetBold(int style, bool bold) => SPerform(2053, style, bold ? 1 : 0);

        /// <summary>
        /// Gets whether a style is bold or not.
        /// </summary>
        public bool StyleGetBold(int style) => SPerform(2483, style, 0) != 0;

        /// <summary>
        /// Sets a style to be italic or not.
        /// </summary>
        public void StyleSetItalic(int style, bool italic)
        {
            SPerform(2054, style, italic ? 1 : 0);
        }

        /// <summary>
        /// Gets whether a style is italic or not.
        /// </summary>
        public bool StyleGetItalic(int style) => SPerform(2484, style, 0) != 0;

        /// <summary>
        /// Sets the size of characters of a style.
        /// </summary>
        public void StyleSetSize(int style, int sizePoints) => SPerform(2055, style, sizePoints);

        /// <summary>
        /// Gets the size of characters of a style.
        /// </summary>
        public int StyleGetSize(int style) => SPerform(2485, style, 0);

        /// <summary>
        /// Set the font of a style.
        /// </summary>
        public unsafe void StyleSetFont(int style, string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) fontName = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(fontName))
            {
                SPerform(2056, style, (uint)b);
            }
        }

        /// <summary>
        /// Get the font of a style.
        /// </summary>
        public unsafe string StyleGetFont(int style)
        {
            var size = SPerform(2486, style, 0);
            var buffer = new byte[size + 1];
            fixed (byte* b = buffer) SPerform(2486, style, (IntPtr) b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, size);
        }

        /// <summary>
        /// Set a style to have its end of line filled or not.
        /// </summary>
        public void StyleSetEOLFilled(int style, bool filled) => SPerform(2057, style, filled ? 1 : 0);

        /// <summary>
        /// Set a style to be underlined or not.
        /// </summary>
        public void StyleSetUnderline(int style, bool underline) => SPerform(2059, style, underline ? 1 : 0);

        /// <summary>
        /// Set a style to be mixed case, or to force upper or lower case.
        /// </summary>
        public void StyleSetCase(int style, int caseForce) => SPerform(2060, style, caseForce);

        /// <summary>
        /// Set the character set of the font in a style.
        /// </summary>
        public void StyleSetCharacterSet(int style, int characterSet) => SPerform(2066, style, characterSet);

        /// <summary>
        /// Set a style to be a hotspot or not.
        /// </summary>
        public void StyleSetHotSpot(int style, bool hotspot) => SPerform(2409, style, hotspot ? 1 : 0);

        /// <summary>
        /// Set a style to be visible or not.
        /// </summary>
        public void StyleSetVisible(int style, bool visible) => SPerform(2074, style, visible ? 1 : 0);

        /// <summary>
        /// Set the set of characters making up words for when moving or selecting by word.
        /// First sets defaults like SetCharsDefault.
        /// </summary>
        public unsafe void WordChars(string characters)
        {
            if (string.IsNullOrEmpty(characters)) characters = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(characters))
            {
                SPerform(2077, 0, (uint)b);
            }
        }

        /// <summary>
        /// Set a style to be changeable or not (read only).
        /// Experimental feature, currently buggy.
        /// </summary>
        public void StyleSetChangeable(int style, bool changeable) => SPerform(2099, style, changeable ? 1 : 0 );

        /// <summary>
        /// Define a set of characters that when typed will cause the autocompletion to
        /// choose the selected item.
        /// </summary>
        public unsafe void AutoCSetFillUps(string characterSet)
        {
            if (string.IsNullOrEmpty(characterSet)) characterSet = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(characterSet))
            {
                SPerform(2112, 0, (uint)b);
            }
        }

        /// <summary>
        /// Enable / Disable underlining active hotspots.
        /// </summary>
        public void HotspotActiveUnderline(bool useSetting) => SPerform(2412, useSetting ? 1 : 0, 0);

        /// <summary>
        /// Limit hotspots to single line so hotspots on two lines don't merge.
        /// </summary>
        public void HotspotSingleLine(bool useSetting) => SPerform(2421, useSetting ? 1 : 0, 0);

        /// <summary>
        /// Set a fore colour for active hotspots.
        /// </summary>
        public void HotspotActiveFore(bool useSetting, int fore) => SPerform(2410, useSetting ? 1 : 0, fore);

        /// <summary>
        /// Set a back colour for active hotspots.
        /// </summary>
        public void HotspotActiveBack(bool useSetting, int back) => SPerform(2411, useSetting ? 1 : 0, back);

        /// <summary>
        /// Retrieve the number of bits the current lexer needs for styling.
        /// </summary>
        public int GetStyleBitsNeeded() => SPerform(4011, 0, 0);

        /// <summary>
        /// Set up a value that may be used by a lexer for some optional feature.
        /// </summary>
        public unsafe void SetProperty(string key, string val)
        {
            if (string.IsNullOrEmpty(key)) key = "\0\0";
            if (string.IsNullOrEmpty(val)) val = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(val))
            {
                fixed (byte* b2 = Encoding.GetEncoding(CodePage).GetBytes(key))
                {
                    SPerform(4004, (int)b2, (uint)b);
                }
            }
        }

        /// <summary>
        /// Retrieve a "property" value previously set with SetProperty,
        /// interpreted as an int AFTER any "$()" variable replacement.
        /// </summary>
        public unsafe int GetPropertyInt(string key)
        {
            if (string.IsNullOrEmpty(key)) key = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(key))
            {
                return SPerform(4010, (int)b, 0);
            }
        }

        /// <summary>
        /// Set up the key words used by the lexer.
        /// </summary>
        public unsafe void KeyWords(int keywordSet, string keyWords)
        {
            if (string.IsNullOrEmpty(keyWords)) keyWords = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(keyWords))
            {
                SPerform(4005, keywordSet, (uint)b);
            }
        }

        /// <summary>
        /// Set the lexing language of the document based on string name.
        /// </summary>
        public unsafe void LexerLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) language = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(language))
            {
                SPerform(4006, 0, (uint)b);
            }
        }

        /// <summary>
        /// Retrieve the extra styling information for a line.
        /// </summary>
        public int GetLineState(int line) => SPerform(2093, line, 0);

        /// <summary>
        /// Used to hold extra styling information for each line.
        /// </summary>
        public void SetLineState(int line, int state) => SPerform(2092, line, state);

        /// <summary>
        /// Retrieve the number of columns that a line is indented.
        /// </summary>
        public int GetLineIndentation(int line) => SPerform(2127, line, 0);

        /// <summary>
        /// Change the indentation of a line to a number of columns.
        /// </summary>
        public void SetLineIndentation(int line, int indentSize) => SPerform(2126, line, indentSize);

        /// <summary>
        /// Retrieve the position before the first non indentation character on a line.
        /// </summary>
        public int LineIndentPosition(int line) => SPerform(2128, line, 0);

        /// <summary>
        /// Retrieve the column number of a position, taking tab width into account.
        /// </summary>
        public int Column(int pos) => SPerform(2129, pos, 0);

        /// <summary>
        /// Get the position after the last visible characters on a line.
        /// </summary>
        public int LineEndPosition(int line) => SPerform(2136, line, 0);

        /// <summary>
        /// Returns the character byte at the position.
        /// </summary>
        public int CharAt(int pos) => SPerform(2007, pos, 0);

        /// <summary>
        /// Returns the style byte at the position.
        /// </summary>
        public int StyleAt(int pos) => SPerform(2010, pos, 0);

        /// <summary>
        /// Retrieve the type of a margin.
        /// </summary>
        public int GetMarginTypeN(int margin) => SPerform(2241, margin, 0);

        /// <summary>
        /// Set a margin to be either numeric or symbolic.
        /// </summary>
        public void SetMarginTypeN(int margin, int marginType) => SPerform(2240, margin, marginType);

        /// <summary>
        /// Retrieve the width of a margin in pixels.
        /// </summary>
        public int GetMarginWidthN(int margin) => SPerform(2243, margin, 0);

        /// <summary>
        /// Set the width of a margin to a width expressed in pixels.
        /// </summary>
        public void SetMarginWidthN(int margin, int pixelWidth) => SPerform(2242, margin, pixelWidth);

        /// <summary>
        /// Retrieve the marker mask of a margin.
        /// </summary>
        public int GetMarginMaskN(int margin) => SPerform(2245, margin, 0);

        /// <summary>
        /// Set a mask that determines which markers are displayed in a margin.
        /// </summary>
        public void SetMarginMaskN(int margin, int mask) => SPerform(2244, margin, mask);

        /// <summary>
        /// Retrieve the mouse click sensitivity of a margin.
        /// </summary>
        public bool MarginSensitiveN(int margin) => SPerform(2247, margin, 0) != 0;

        /// <summary>
        /// Make a margin sensitive or insensitive to mouse clicks.
        /// </summary>
        public void MarginSensitiveN(int margin, bool sensitive) => SPerform(2246, margin, sensitive ? 1 : 0);

        /// <summary>
        /// Gets the cursor of a margin.
        /// </summary>
        public int GetMarginCursorN(int margin) => SPerform(2249, margin, 0);

        /// <summary>
        /// Set the cursor of a margin.
        /// </summary>
        public void SetMarginCursorN(int margin, int cursor) => SPerform(2248, margin, cursor);

        /// <summary>
        /// Retrieve the style of an indicator.
        /// </summary>
        public int GetIndicStyle(int indic) => SPerform(2081, indic, 0);

        /// <summary>
        /// Set an indicator to plain, squiggle or TT.
        /// </summary>
        public void SetIndicStyle(int indic, int style) => SPerform(2080, indic, style);

        /// <summary>
        /// Retrieve the foreground colour of an indicator.
        /// </summary>
        public int GetIndicFore(int indic) => SPerform(2083, indic, 0);

        /// <summary>
        /// Set the foreground colour of an indicator.
        /// </summary>
        public void SetIndicFore(int indic, int fore) => SPerform(2082, indic, fore);

        /// <summary>
        /// Add text to the document at current position.
        /// </summary>
        public unsafe void AddText(int length, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2001, length, (uint)b);
            }
        }

        /// <summary>
        /// Insert string at a position. 
        /// </summary>
        public unsafe void InsertText(int pos, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2003, pos, (uint)b);
            }
        }

        /// <summary>
        /// Convert all line endings in the document to one mode.
        /// </summary>
        public void ConvertEOLs(Enums.EndOfLine eolMode) => ConvertEOLs((int)eolMode);

        /// <summary>
        /// Set the symbol used for a particular marker number.
        /// </summary>
        public void MarkerDefine(int markerNumber, Enums.MarkerSymbol markerSymbol) => MarkerDefine(markerNumber, (int)markerSymbol);

        /// <summary>
        /// Set the character set of the font in a style.
        /// </summary>
        public void StyleSetCharacterSet(int style, Enums.CharacterSet characterSet) => StyleSetCharacterSet(style, (int)characterSet);

        /// <summary>
        /// Set a style to be mixed case, or to force upper or lower case.
        /// </summary>
        public void StyleSetCase(int style, Enums.CaseVisible caseForce) => StyleSetCase(style, (int)caseForce);

        /// <summary>
        /// Delete all text in the document.
        /// </summary>
        public void ClearAll() => SPerform(2004, 0, 0);

        /// <summary>
        /// Set all style bytes to 0, remove all folding information.
        /// </summary>
        public void ClearDocumentStyle() => SPerform(2005, 0, 0);

        /// <summary>
        /// Redoes the next action on the undo history.
        /// </summary>
        public void Redo() => SPerform(2011, 0, 0);

        /// <summary>
        /// Select all the text in the document.
        /// </summary>
        public void SelectAll() => SPerform(2013, 0, 0);

        /// <summary>
        /// Remember the current position in the undo history as the position
        /// at which the document was saved. 
        /// </summary>
        public void SetSavePoint() => SPerform(2014, 0, 0);

        /// <summary>
        /// Retrieve the line number at which a particular marker is located.
        /// </summary>
        public int MarkerLineFromHandle(int handle) => SPerform(2017, handle, 0);

        /// <summary>
        /// Delete a marker.
        /// </summary>
        public void MarkerDeleteHandle(int handle) => SPerform(2018, handle, 0);

        /// <summary>
        /// Find the position from a point within the window.
        /// </summary>
        public int PositionFromPoint(int x, int y) => SPerform(2022, x, y);

        /// <summary>
        /// Find the position from a point within the window but return
        /// INVALID_POSITION if not close to text.
        /// </summary>
        public int PositionFromPointClose(int x, int y) => SPerform(2023, x, y);

        /// <summary>
        /// Set caret to start of a line and ensure it is visible.
        /// </summary>
        public void GotoLine(int line) => SPerform(2024, line, 0);

        /// <summary>
        /// Set caret to a position and ensure it is visible.
        /// </summary>
        public void GotoPos(int pos) => SPerform(2025, pos, 0);

        /// <summary>
        /// Retrieve the text of the line containing the caret.
        /// Returns the index of the caret on the line.
        /// </summary>
        public unsafe string GetCurLine(int length)
        {
            length = Math.Min(length, SPerform(2027, 0, 0) - 1);
            var buffer = new byte[length + 1];
            fixed (byte* b = buffer) SPerform(2027, length + 1, (uint)b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, length);
        }

        /// <summary>
        /// Convert all line endings in the document to one mode.
        /// </summary>
        public void ConvertEOLs(int eolMode) => SPerform(2029, eolMode, 0);

        /// <summary>
        /// Set the current styling position to pos and the styling mask to mask.
        /// The styling mask can be used to protect some bits in each styling byte from modification.
        /// </summary>
        public void StartStyling(int pos, int mask) => SPerform(2032, pos, mask);

        /// <summary>
        /// Change style from current styling position for length characters to a style
        /// and move the current styling position to after this newly styled segment.
        /// </summary>
        public void SetStyling(int length, int style) => SPerform(2033, length, style);

        /// <summary>
        /// Set the symbol used for a particular marker number.
        /// </summary>
        public void MarkerDefine(int markerNumber, int markerSymbol) => SPerform(2040, markerNumber, markerSymbol);

        /// <summary>
        /// Set the foreground colour used for a particular marker number.
        /// </summary>
        public void MarkerSetFore(int markerNumber, int fore) => SPerform(2041, markerNumber, fore);

        /// <summary>
        /// Set the background colour used for a particular marker number.
        /// </summary>
        public void MarkerSetBack(int markerNumber, int back) => SPerform(2042, markerNumber, back);

        /// <summary>
        /// Add a marker to a line, returning an ID which can be used to find or delete the marker.
        /// </summary>
        public int MarkerAdd(int line, int markerNumber) => SPerform(2043, line, markerNumber);

        /// <summary>
        /// Delete a marker from a line.
        /// </summary>
        public void MarkerDelete(int line, int markerNumber) => SPerform(2044, line, markerNumber);

        /// <summary>
        /// Delete all markers with a particular number from all lines.
        /// </summary>
        public void MarkerDeleteAll(int markerNumber) => SPerform(2045, markerNumber, 0);

        /// <summary>
        /// Get a bit mask of all the markers set on a line.
        /// </summary>
        public int MarkerGet(int line) => SPerform(2046, line, 0);

        /// <summary>
        /// Find the next line after lineStart that includes a marker in mask.
        /// </summary>
        public int MarkerNext(int lineStart, int markerMask) => SPerform(2047, lineStart, (uint)markerMask);

        /// <summary>
        /// Find the previous line before lineStart that includes a marker in mask.
        /// </summary>
        public int MarkerPrevious(int lineStart, int markerMask) => SPerform(2048, lineStart, (uint)markerMask);

        /// <summary>
        /// Define a marker from a pixmap.
        /// </summary>
        public unsafe void MarkerDefinePixmap(int markerNumber, string pixmap)
        {
            if (string.IsNullOrEmpty(pixmap)) pixmap = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(pixmap))
            {
                SPerform(2049, markerNumber, (uint)b);
            }
        }

        /// <summary>
        /// Define a marker image from a bitmap. Supports alpha channel.
        /// </summary>
        public unsafe void MarkerDefineRGBAImage(int markerNumber, Bitmap image)
        {
            var rgba = RGBA.ConvertToRGBA(image);
            //SCI_RGBAIMAGESETWIDTH
            SPerform(2624, image.Width, 0);
            //SCI_RGBAIMAGESETHEIGHT
            SPerform(2625, image.Height, 0);
            fixed (byte* b = rgba)
            {
                //SCI_MARKERDEFINERGBAIMAGE
                SPerform(2626, markerNumber, (uint)b);
            }
        }

        /// <summary>
        /// Reset the default style to its state at startup
        /// </summary>
        public void StyleResetDefault() => SPerform(2058, 0, 0);

        /// <summary>
        /// Set the foreground colour of the selection and whether to use this setting.
        /// </summary>
        public void SetSelFore(bool useSetting, int fore) => SPerform(2067, useSetting ? 1 : 0, fore);

        /// <summary>
        /// Set the background colour of the selection and whether to use this setting.
        /// </summary>
        public void SetSelBack(bool useSetting, int back) => SPerform(2068, useSetting ? 1 : 0, back);

        /// <summary>
        /// When key+modifier combination km is pressed perform msg.
        /// </summary>
        public void AssignCmdKey(int km, int msg) => SPerform(2070, km, msg);

        /// <summary>
        /// When key+modifier combination km is pressed do nothing.
        /// </summary>
        public void ClearCmdKey(int km) => SPerform(2071, km, 0);

        /// <summary>
        /// Drop all key mappings.
        /// </summary>
        public void ClearAllCmdKeys() => SPerform(2072, 0, 0);

        /// <summary>
        /// Set the styles for a segment of the document.
        /// </summary>
        public unsafe void SetStylingEx(int length, string styles)
        {
            if (string.IsNullOrEmpty(styles)) styles = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(styles))
            {
                SPerform(2073, length, (uint)b);
            }
        }

        /// <summary>
        /// Start a sequence of actions that is undone and redone as a unit.
        /// May be nested.
        /// </summary>
        public void BeginUndoAction() => SPerform(2078, 0, 0);

        /// <summary>
        /// End a sequence of actions that is undone and redone as a unit.
        /// </summary>
        public void EndUndoAction() => SPerform(2079, 0, 0);

        /// <summary>
        /// Set the foreground colour of all whitespace and whether to use this setting.
        /// </summary>
        public void SetWhitespaceFore(bool useSetting, int fore) => SPerform(2084, useSetting ? 1 : 0, fore);

        /// <summary>
        /// Set the background colour of all whitespace and whether to use this setting.
        /// </summary>
        public void SetWhitespaceBack(bool useSetting, int back) => SPerform(2085, useSetting ? 1 : 0, back);

        /// <summary>
        /// Display a auto-completion list.
        /// The lenEntered parameter indicates how many characters before
        /// the caret should be used to provide context.
        /// </summary>
        public unsafe void AutoCShow(int lenEntered, string itemList)
        {
            if (string.IsNullOrEmpty(itemList)) itemList = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(itemList))
            {
                SPerform(2100, lenEntered, (uint)b);
            }
        }

        /// <summary>
        /// Remove the auto-completion list from the screen.
        /// </summary>
        public void AutoCCancel() => SPerform(2101, 0, 0);

        /// <summary>
        /// User has selected an item so remove the list and insert the selection.
        /// </summary>
        public void AutoCComplete() => SPerform(2104, 0, 0);

        /// <summary>
        /// Define a set of character that when typed cancel the auto-completion list.
        /// </summary>
        public unsafe void AutoCStops(string characterSet)
        {
            if (string.IsNullOrEmpty(characterSet)) characterSet = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(characterSet))
            {
                SPerform(2105, 0, (uint)b);
            }
        }

        /// <summary>
        /// Select the item in the auto-completion list that starts with a string.
        /// </summary>
        public unsafe void AutoCSelect(string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2108, 0, (uint)b);
            }
        }

        /// <summary>
        /// Display a list of strings and send notification when user chooses one.
        /// </summary>
        public unsafe void UserListShow(int listType, string itemList)
        {
            if (string.IsNullOrEmpty(itemList)) itemList = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(itemList))
            {
                SPerform(2117, listType, (uint)b);
            }
        }

        /// <summary>
        /// Register an XPM image for use in autocompletion lists.
        /// </summary>
        public unsafe void RegisterImage(int type, string xpmData)
        {
            if (string.IsNullOrEmpty(xpmData)) xpmData = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(xpmData))
            {
                SPerform(2405, type, (uint)b);
            }
        }

        /// <summary>
        /// Clear all the registered XPM images.
        /// </summary>
        public void ClearRegisteredImages() => SPerform(2408, 0, 0);

        /// <summary>
        /// Retrieve the contents of a line.
        /// </summary>
        public unsafe string GetLine(int line)
        {
            var sz = SPerform(2153, line, 0);
            var buffer = new byte[sz + 1];
            fixed (byte* b = buffer) SPerform(2153, line, (uint)b);
            return sz == 0 ? "" : Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz);
        }

        /// <summary>
        /// Select a range of text.
        /// </summary>
        public void SetSel(int start, int end) => SPerform(2160, start, end);

        /// <summary>
        /// Returns the number of selections currently active. There is always at least one selection.
        /// </summary>
        /// <returns></returns>
        public int GetSelections() => SPerform(2570, 0, 0);

        /// <summary>
        /// Set a single selection from anchor to caret as the only selection.
        /// </summary>
        /// <param name="caret"></param>
        /// <param name="anchor"></param>
        public void SetSelection(int caret, int anchor) => SPerform(2572, caret, anchor);

        /// <summary>
        /// Add a new selection from anchor to caret as the main selection retaining all other selections as additional selections.
        /// Since there is always at least one selection, to set a list of selections, the first selection should be added with SetSelection and later selections added with AddSelection
        /// </summary>
        /// <param name="caret"></param>
        /// <param name="anchor"></param>
        public void AddSelection(int caret, int anchor) => SPerform(2573, caret, anchor);

        /// <summary>
        /// Returns the start position of each already existing selection. Mostly of use to query each range for its text. The selection parameter is zero-based.
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public int GetSelectionNStart(int selection) => SPerform(2585, selection, 0);

        /// <summary>
        /// Set the start position of each already existing selection. Mostly of use to query each range for its text. The selection parameter is zero-based.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public void SetSelectionNStart(int selection, int anchor) => SPerform(2584, selection, anchor);

        /// <summary>
        /// Returns the end position of each already existing selection. Mostly of use to query each range for its text. The selection parameter is zero-based.
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public int GetSelectionNEnd(int selection) => SPerform(2587, selection, 0);

        /// <summary>
        /// Set the end position of each already existing selection. Mostly of use to query each range for its text. The selection parameter is zero-based.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public void SetSelectionNEnd(int selection, int anchor) => SPerform(2586, selection, anchor);

        /// <summary>
        /// If there are multiple selections, remove the indicated selection.
        /// If this was the main selection then make the previous selection the main and if it was the first then the last selection becomes main.
        /// If there is only one selection, or there is no selection selection, then there is no effect.
        /// </summary>
        /// <param name="selection"></param>
        public void DropSelectionN(int selection) => SPerform(2671, selection, 0);

        /// <summary>
        /// Retrieve the selected text.
        /// Return the length of the text.
        /// </summary>
        public unsafe string SelText
        {
            get
            {
                var sz = SPerform(2161, 0, 0);
                var buffer = new byte[sz + 1];
                fixed (byte* b = buffer)
                {
                    SPerform(2161, sz + 1, (uint)b);
                }
                return Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz - 1);
            }
        }

        /// <summary>
        /// Gets a range of text from the document.
        /// </summary>
        /// <param name="position">The zero-based starting byte position of the range to get.</param>
        /// <param name="end">The end byte position of the range to get.</param>
        /// <returns>A string representing the text range.</returns>
        public unsafe string GetTextRange(int position, int end)
        {
            var length = end - position;
            var bytes = new byte[length + 1];
            fixed (byte* bp = bytes)
            {
                var range = stackalloc TextRange[1];
                range->chrg.cpMin = position;
                range->chrg.cpMax = end;
                range->lpstrText = new IntPtr(bp);

                SPerform(2162 /*SCI_GETTEXTRANGE*/, 0, new IntPtr(range));
                return new string((sbyte*)bp, 0, length, Encoding);
            }
        }

        /// <summary>
        /// Draw the selection in normal style or with selection highlighted.
        /// </summary>
        public void HideSelection(bool normal) => SPerform(2163, normal ? 1 : 0, 0);

        /// <summary>
        /// Retrieve the x value of the point in the window where a position is displayed.
        /// </summary>
        public int PointXFromPosition(int pos) => SPerform(2164, 0, pos);

        /// <summary>
        /// Retrieve the y value of the point in the window where a position is displayed.
        /// </summary>
        public int PointYFromPosition(int pos) => SPerform(2165, 0, pos);

        /// <summary>
        /// Retrieve the line containing a position.
        /// </summary>
        public int LineFromPosition(int pos) => SPerform(2166, pos, 0);

        /// <summary>
        /// Retrieve the position at the start of a line.
        /// </summary>
        public int PositionFromLine(int line) => SPerform(2167, line, 0);

        /// <summary>
        /// Retrieve the text from line before position
        /// </summary>
        public string GetLineUntilPosition(int pos)
        {
            var curLine = LineFromPosition(pos);
            var curPosInLine = pos - PositionFromLine(curLine);
            var line = GetLine(curLine);
            var length = MBSafeLengthFromBytes(line, curPosInLine);
            var lineUntilPos = line.Substring(0, length);
            return lineUntilPos;
        }

        /// <summary>
        /// Scroll horizontally and vertically.
        /// </summary>
        public void LineScroll(int columns, int lines) => SPerform(2168, columns, lines);

        /// <summary>
        /// Ensure the caret is visible.
        /// </summary>
        public void ScrollCaret() => SPerform(2169, 0, 0);

        /// <summary>
        /// Replace the selected text with the argument text.
        /// </summary>
        public unsafe void ReplaceSel(string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2170, 0, (uint)b);
            }
        }

        /// <summary>
        /// Null operation.
        /// </summary>
        public void Null() => SPerform(2172, 0, 0);

        /// <summary>
        /// Delete the undo history.
        /// </summary>
        public void EmptyUndoBuffer() => SPerform(2175, 0, 0);

        /// <summary>
        /// Undo one action in the undo history.
        /// </summary>
        public void Undo() => SPerform(2176, 0, 0);

        /// <summary>
        /// Cut the selection to the clipboard.
        /// </summary>
        public void Cut() => SPerform(2177, 0, 0);

        /// <summary>
        /// Copy the selection to the clipboard.
        /// </summary>
        public void Copy()
        {
            SPerform(2178, 0, 0);
            // Invoke UI update after copy...
            UpdateUI?.Invoke(this);
        }

        /// <summary>
        /// Cut the selection to the clipboard as RTF.
        /// </summary>
        public void CutRTF()
        {
            if (SelTextSize > 0)
            {
                CopyRTF();
                Clear();
            }
        }

        /// <summary>
        /// Copy the selection to the clipboard as RTF.
        /// </summary>
        public void CopyRTF()
        {
            int start = SelectionStart;
            int end = SelectionEnd;

            if (start < end)
            {
                CopyRTF(start, end);
            }
        }

        /// <summary>
        /// Copy the text in range to the clipboard as RTF.
        /// </summary>
        public void CopyRTF(int start, int end)
        {
            var dataObject = new DataObject();
            var language = Configuration.GetLanguage(configLanguage);
            var text = RTF.GetConversion(language, this, start, end);
            dataObject.SetText(GetTextRange(start, end));
            dataObject.SetText(text, TextDataFormat.Rtf);
            Clipboard.SetDataObject(dataObject);
        }

        /// <summary>
        /// Paste the contents of the clipboard into the document replacing the selection.
        /// </summary>
        public void Paste() => SPerform(2179, 0, 0);

        /// <summary>
        /// Clear the selection.
        /// </summary>
        public void Clear() => SPerform(2180, 0, 0);

        /// <summary>
        /// Replace the contents of the document with the argument text.
        /// </summary>
        public unsafe void SetText(string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2181, 0, (uint)b);
            }
        }

        /// <summary>
        /// Retrieve all the text in the document. Returns number of characters retrieved.
        /// </summary>
        public unsafe string GetText(int length)
        {
            var sz = SPerform(2182, length, 0);
            var buffer = new byte[sz + 1];
            fixed (byte* b = buffer) SPerform(2182, length + 1, (uint)b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz - 1);
        }

        /// <summary>
        /// Replace the target text with the argument text.
        /// Text is counted so it can contain NULs.
        /// Returns the length of the replacement text.
        /// </summary>
        public unsafe int ReplaceTarget(int length, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                return SPerform(2194, length, (uint)b);
            }
        }

        /// <summary>
        /// Replace the target text with the argument text after \d processing.
        /// Text is counted so it can contain NULs.
        /// Looks for \d where d is between 1 and 9 and replaces these with the strings
        /// matched in the last search operation which were surrounded by \( and \).
        /// Returns the length of the replacement text including any change
        /// caused by processing the \d patterns.
        /// </summary>
        public unsafe int ReplaceTargetRE(int length, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                return SPerform(2195, length, (uint)b);
            }
        }

        /// <summary>
        /// Search for a counted string in the target and set the target to the found
        /// range. Text is counted so it can contain NULs.
        /// Returns length of range or -1 for failure in which case target is not moved.
        /// </summary>
        public unsafe int SearchInTarget(int length, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                return SPerform(2197, length, (uint)b);
            }
        }

        /// <summary>
        /// Show a call tip containing a definition near position pos.
        /// </summary>
        public unsafe void CallTipShow(int pos, string definition)
        {
            if (string.IsNullOrEmpty(definition)) definition = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(definition))
            {
                SPerform(2200, pos, (uint)b);
            }
        }

        /// <summary>
        /// Remove the call tip from the screen.
        /// </summary>
        public void CallTipCancel() => SPerform(2201, 0, 0);

        /// <summary>
        /// Highlight a segment of the definition.
        /// </summary>
        public void CallTipSetHlt(int start, int end) => SPerform(2204, start, end);

        /// <summary>
        /// Set the background colour for the call tip.
        /// </summary>
        public void CallTipSetBack(int color) => SPerform(2205, color, 0);

        /// <summary>
        /// Set the foreground colour for the call tip.
        /// </summary>
        public void CallTipSetFore(int color) => SPerform(2206, color, 0);

        /// <summary>
        /// Set the foreground colour for the highlighted part of the call tip.
        /// </summary>
        public void CallTipSetForeHlt(int color) => SPerform(2207, color, 0);

        /// <summary>
        /// Find the display line of a document line taking hidden lines into account.
        /// </summary>
        public int VisibleFromDocLine(int line) => SPerform(2220, line, 0);

        /// <summary>
        /// Find the document line of a display line taking hidden lines into account.
        /// </summary>
        public int DocLineFromVisible(int lineDisplay) => SPerform(2221, lineDisplay, 0);

        /// <summary>
        /// Make a range of lines visible.
        /// </summary>
        public void ShowLines(int lineStart, int lineEnd) => SPerform(2226, lineStart, lineEnd);

        /// <summary>
        /// Make a range of lines invisible.
        /// </summary>
        public void HideLines(int lineStart, int lineEnd) => SPerform(2227, lineStart, lineEnd);

        /// <summary>
        /// Switch a header line between expanded and contracted.
        /// </summary>
        public void ToggleFold(int line) => SPerform(2231, line, 0);

        /// <summary>
        /// Ensure a particular line is visible by expanding any header line hiding it.
        /// </summary>
        public void EnsureVisible(int line) => SPerform(2232, line, 0);

        /// <summary>
        /// Set some style options for folding.
        /// </summary>
        public void SetFoldFlags(int flags) => SPerform(2233, flags, 0);

        /// <summary>
        /// Ensure a particular line is visible by expanding any header line hiding it.
        /// Use the currently set visibility policy to determine which range to display.
        /// </summary>
        public void EnsureVisibleEnforcePolicy(int line) => SPerform(2234, line, 0);

        /// <summary>
        /// Get position of start of word.
        /// </summary>
        public int WordStartPosition(int pos, bool onlyWordCharacters)
        {
            return SPerform(2266, pos, onlyWordCharacters ? 1 : 0);
        }

        /// <summary>
        /// Get position of end of word.
        /// </summary>
        public int WordEndPosition(int pos, bool onlyWordCharacters)
        {
            return SPerform(2267, pos, onlyWordCharacters ? 1 : 0);
        }

        /// <summary>
        /// Measure the pixel width of some text in a particular style.
        /// NUL terminated text argument.
        /// Does not handle tab or control characters.
        /// </summary>
        public unsafe int TextWidth(int style, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                return SPerform(2276, style, (uint)b);
            }
        }

        /// <summary>
        /// Retrieve the height of a particular line of text in pixels.
        /// </summary>
        public int TextHeight(int line) => SPerform(2279, line, 0);

        /// <summary>
        /// Append a string to the end of the document without changing the selection.
        /// </summary>
        public unsafe void AppendText(int length, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2282, length, (uint)b);
            }
        }

        /// <summary>
        /// Make the target range start and end be the same as the selection range start and end.
        /// </summary>
        public void TargetFromSelection() => SPerform(2287, 0, 0);

        /// <summary>
        /// Join the lines in the target.
        /// </summary>
        public void LinesJoin() => SPerform(2288, 0, 0);

        /// <summary>
        /// Split the lines in the target into lines that are less wide than pixelWidth
        /// where possible.
        /// </summary>
        public void LinesSplit(int pixelWidth) => SPerform(2289, pixelWidth, 0);

        /// <summary>
        /// Set the colours used as a chequerboard pattern in the fold margin
        /// </summary>
        public void SetFoldMarginColour(bool useSetting, int back) => SPerform(2290, useSetting ? 1 : 0, back);

        /// <summary>
        /// Set the colours used as a chequerboard pattern in the fold margin
        /// </summary>
        public void SetFoldMarginHiColour(bool useSetting, int fore) => SPerform(2291, useSetting ? 1 : 0, fore);

        /// <summary>
        /// Move caret down one line.
        /// </summary>
        public void LineDown() => SPerform(2300, 0, 0);

        /// <summary>
        /// Move caret down one line extending selection to new caret position.
        /// </summary>
        public void LineDownExtend() => SPerform(2301, 0, 0);

        /// <summary>
        /// Move caret up one line.
        /// </summary>
        public void LineUp() => SPerform(2302, 0, 0);

        /// <summary>
        /// Move caret up one line extending selection to new caret position.
        /// </summary>
        public void LineUpExtend() => SPerform(2303, 0, 0);

        /// <summary>
        /// Move caret left one character.
        /// </summary>
        public void CharLeft() => SPerform(2304, 0, 0);

        /// <summary>
        /// Move caret left one character extending selection to new caret position.
        /// </summary>
        public void CharLeftExtend() => SPerform(2305, 0, 0);

        /// <summary>
        /// Move caret right one character.
        /// </summary>
        public void CharRight() => SPerform(2306, 0, 0);

        /// <summary>
        /// Move caret right one character extending selection to new caret position.
        /// </summary>
        public void CharRightExtend() => SPerform(2307, 0, 0);

        /// <summary>
        /// Move caret left one word.
        /// </summary>
        public void WordLeft() => SPerform(2308, 0, 0);

        /// <summary>
        /// Move caret left one word extending selection to new caret position.
        /// </summary>
        public void WordLeftExtend() => SPerform(2309, 0, 0);

        /// <summary>
        /// Move caret right one word.
        /// </summary>
        public void WordRight() => SPerform(2310, 0, 0);

        /// <summary>
        /// Move caret right one word extending selection to new caret position.
        /// </summary>
        public void WordRightExtend() => SPerform(2311, 0, 0);

        /// <summary>
        /// Move caret to first position on line.
        /// </summary>
        public void Home() => SPerform(2312, 0, 0);

        /// <summary>
        /// Move caret to first position on line extending selection to new caret position.
        /// </summary>
        public void HomeExtend() => SPerform(2313, 0, 0);

        /// <summary>
        /// Move caret to last position on line.
        /// </summary>
        public void LineEnd() => SPerform(2314, 0, 0);

        /// <summary>
        /// Move caret to last position on line extending selection to new caret position.
        /// </summary>
        public void LineEndExtend() => SPerform(2315, 0, 0);

        /// <summary>
        /// Move caret to first position in document.
        /// </summary>
        public void DocumentStart() => SPerform(2316, 0, 0);

        /// <summary>
        /// Move caret to first position in document extending selection to new caret position.
        /// </summary>
        public void DocumentStartExtend() => SPerform(2317, 0, 0);

        /// <summary>
        /// Move caret to last position in document.
        /// </summary>
        public void DocumentEnd() => SPerform(2318, 0, 0);

        /// <summary>
        /// Move caret to last position in document extending selection to new caret position.
        /// </summary>
        public void DocumentEndExtend() => SPerform(2319, 0, 0);

        /// <summary>
        /// Move caret one page up.
        /// </summary>
        public void PageUp() => SPerform(2320, 0, 0);

        /// <summary>
        /// Move caret one page up extending selection to new caret position.
        /// </summary>
        public void PageUpExtend() => SPerform(2321, 0, 0);

        /// <summary>
        /// Move caret one page down.
        /// </summary>
        public void PageDown() => SPerform(2322, 0, 0);

        /// <summary>
        /// Move caret one page down extending selection to new caret position.
        /// </summary>
        public void PageDownExtend() => SPerform(2323, 0, 0);

        /// <summary>
        /// Switch from insert to overtype mode or the reverse.
        /// </summary>
        public void EditToggleOvertype() => SPerform(2324, 0, 0);

        /// <summary>
        /// Cancel any modes such as call tip or auto-completion list display.
        /// </summary>
        public void Cancel() => SPerform(2325, 0, 0);

        /// <summary>
        /// Delete the selection or if no selection, the character before the caret.
        /// </summary>
        public void DeleteBack() => SPerform(2326, 0, 0);

        /// <summary>
        /// Delete the character after the caret.
        /// </summary>
        public void DeleteForward() => Clear();

        /// <summary>
        /// Delete a range of text in the document.
        /// </summary>
        public void DeleteRange(int start, int lengthDelete) => SPerform(2645, start, lengthDelete);

        /// <summary>
        /// If selection is empty or all on one line replace the selection with a tab character.
        /// If more than one line selected, indent the lines.
        /// </summary>
        public void Tab() => SPerform(2327, 0, 0);

        /// <summary>
        /// Dedent the selected lines.
        /// </summary>
        public void BackTab() => SPerform(2328, 0, 0);

        /// <summary>
        /// Insert a new line, may use a CRLF, CR or LF depending on EOL mode.
        /// </summary>
        public void NewLine() => SPerform(2329, 0, 0);

        /// <summary>
        /// Insert a Form Feed character.
        /// </summary>
        public void FormFeed() => SPerform(2330, 0, 0);

        /// <summary>
        /// Move caret to before first visible character on line.
        /// If already there move to first character on line.
        /// </summary>
        public void VCHome() => SPerform(2331, 0, 0);

        /// <summary>
        /// Like VCHome but extending selection to new caret position.
        /// </summary>
        public void VCHomeExtend() => SPerform(2332, 0, 0);

        /// <summary>
        /// Magnify the displayed text by increasing the sizes by 1 point.
        /// </summary>
        public void ZoomIn() => SPerform(2333, 0, 0);

        /// <summary>
        /// Make the displayed text smaller by decreasing the sizes by 1 point.
        /// </summary>
        public void ZoomOut() => SPerform(2334, 0, 0);

        /// <summary>
        /// Reset the text zooming by setting zoom level to 0.
        /// </summary>
        public void ResetZoom() => SPerform(2373, 0, 0);

        /// <summary>
        /// Delete the word to the left of the caret.
        /// </summary>
        public void DelWordLeft() => SPerform(2335, 0, 0);

        /// <summary>
        /// Delete the word to the right of the caret.
        /// </summary>
        public void DelWordRight() => SPerform(2336, 0, 0);

        /// <summary>
        /// Cut the line containing the caret.
        /// </summary>
        public void LineCut() => SPerform(2337, 0, 0);

        /// <summary>
        /// Delete the line containing the caret.
        /// </summary>
        public void LineDelete() => SPerform(2338, 0, 0);

        /// <summary>
        /// Switch the current line with the previous.
        /// </summary>
        public void LineTranspose() => SPerform(2339, 0, 0);

        /// <summary>
        /// Duplicate the current line.
        /// </summary>
        public void LineDuplicate() => SPerform(2404, 0, 0);

        /// <summary>
        /// Transform the selection to lower case.
        /// </summary>
        public void LowerCase() => SPerform(2340, 0, 0);

        /// <summary>
        /// Transform the selection to upper case.
        /// </summary>
        public void UpperCase() => SPerform(2341, 0, 0);

        /// <summary>
        /// Scroll the document down, keeping the caret visible.
        /// </summary>
        public void LineScrollDown() => SPerform(2342, 0, 0);

        /// <summary>
        /// Scroll the document up, keeping the caret visible.
        /// </summary>
        public void LineScrollUp() => SPerform(2343, 0, 0);

        /// <summary>
        /// Delete the selection or if no selection, the character before the caret.
        /// Will not delete the character before at the start of a line.
        /// </summary>
        public void DeleteBackNotLine() => SPerform(2344, 0, 0);

        /// <summary>
        /// Move caret to first position on display line.
        /// </summary>
        public void HomeDisplay() => SPerform(2345, 0, 0);

        /// <summary>
        /// Move caret to first position on display line extending selection to
        /// new caret position.
        /// </summary>
        public void HomeDisplayExtend() => SPerform(2346, 0, 0);

        /// <summary>
        /// Move caret to last position on display line.
        /// </summary>
        public void LineEndDisplay() => SPerform(2347, 0, 0);

        /// <summary>
        /// Move caret to last position on display line extending selection to new
        /// caret position.
        /// </summary>
        public void LineEndDisplayExtend() => SPerform(2348, 0, 0);

        /// <summary>
        /// </summary>
        public void HomeWrap() => SPerform(2349, 0, 0);

        /// <summary>
        /// </summary>
        public void HomeWrapExtend() => SPerform(2450, 0, 0);

        /// <summary>
        /// </summary>
        public void LineEndWrap() => SPerform(2451, 0, 0);

        /// <summary>
        /// </summary>
        public void LineEndWrapExtend() => SPerform(2452, 0, 0);

        /// <summary>
        /// </summary>
        public void VCHomeWrap() => SPerform(2453, 0, 0);

        /// <summary>
        /// </summary>
        public void VCHomeWrapExtend() => SPerform(2454, 0, 0);

        /// <summary>
        /// Copy the line containing the caret.
        /// </summary>
        public void LineCopy() => SPerform(2455, 0, 0);

        /// <summary>
        /// Move the caret inside current view if it's not there already.
        /// </summary>
        public void MoveCaretInsideView() => SPerform(2401, 0, 0);

        /// <summary>
        /// How many characters are on a line, not including end of line characters?
        /// </summary>
        public int LineLength(int line) => SPerform(2350, line, 0);

        /// <summary>
        /// Highlight the characters at two positions.
        /// </summary>
        public void BraceHighlight(int pos1, int pos2) => SPerform(2351, pos1, pos2);

        /// <summary>
        /// Highlight the character at a position indicating there is no matching brace.
        /// </summary>
        public void BraceBadLight(int pos) => SPerform(2352, pos, 0);

        /// <summary>
        /// Find the position of a matching brace or INVALID_POSITION if no match.
        /// </summary>
        public int BraceMatch(int pos) => SPerform(2353, pos, 0);

        /// <summary>
        /// Sets the current caret position to be the search anchor.
        /// </summary>
        public void SearchAnchor() => SPerform(2366, 0, 0);

        /// <summary>
        /// Find some text starting at the search anchor.
        /// Does not ensure the selection is visible.
        /// </summary>
        public unsafe int SearchNext(int flags, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                return SPerform(2367, flags, (uint)b);
            }
        }

        /// <summary>
        /// Find some text starting at the search anchor and moving backwards.
        /// Does not ensure the selection is visible.
        /// </summary>
        public unsafe int SearchPrev(int flags, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                return SPerform(2368, flags, (uint)b);
            }
        }

        /// <summary>
        /// Set whether a pop up menu is displayed automatically when the user presses
        /// the wrong mouse button.
        /// </summary>
        public void UsePopUp(bool allowPopUp) => SPerform(2371, allowPopUp ? 1 : 0, 0);

        /// <summary>
        /// Create a new document object.
        /// Starts with reference count of 1 and not selected into editor.
        /// Extend life of document.
        /// </summary>
        public void AddRefDocument(int doc) => SPerform(2376, 0, doc);

        /// <summary>
        /// Release a reference to the document, deleting document if it fades to black.
        /// </summary>
        public void ReleaseDocument(int doc) => SPerform(2377, 0, doc);

        /// <summary>
        /// Move to the previous change in capitalisation.
        /// </summary>
        public void WordPartLeft() => SPerform(2390, 0, 0);

        /// <summary>
        /// Move to the previous change in capitalisation extending selection
        /// to new caret position.
        /// </summary>
        public void WordPartLeftExtend() => SPerform(2391, 0, 0);

        /// <summary>
        /// Move to the change next in capitalisation.
        /// </summary>
        public void WordPartRight() => SPerform(2392, 0, 0);

        /// <summary>
        /// Move to the next change in capitalisation extending selection
        /// to new caret position.
        /// </summary>
        public void WordPartRightExtend() => SPerform(2393, 0, 0);

        /// <summary>
        /// Constants for use with SetVisiblePolicy, similar to SetCaretPolicy.
        /// Set the way the display area is determined when a particular line
        /// is to be moved to by Find, FindNext, GotoLine, etc.
        /// </summary>
        public void SetVisiblePolicy(int visiblePolicy, int visibleSlop) => SPerform(2394, visiblePolicy, visibleSlop);

        /// <summary>
        /// Delete back from the current position to the start of the line.
        /// </summary>
        public void DelLineLeft() => SPerform(2395, 0, 0);

        /// <summary>
        /// Delete forwards from the current position to the end of the line.
        /// </summary>
        public void DelLineRight() => SPerform(2396, 0, 0);

        /// <summary>
        /// Set the last x chosen value to be the caret x position.
        /// </summary>
        public void ChooseCaretX() => SPerform(2399, 0, 0);

        /// <summary>
        /// Set the focus to this Scintilla widget.
        /// GTK+ Specific.
        /// </summary>
        public void GrabFocus() => SPerform(2400, 0, 0);

        /// <summary>
        /// Set the way the caret is kept visible when going sideway.
        /// The exclusion zone is given in pixels.
        /// </summary>
        public void SetXCaretPolicy(int caretPolicy, int caretSlop) => SPerform(2402, caretPolicy, caretSlop);

        /// <summary>
        /// Set the way the line the caret is on is kept visible.
        /// The exclusion zone is given in lines.
        /// </summary>
        public void SetYCaretPolicy(int caretPolicy, int caretSlop) => SPerform(2403, caretPolicy, caretSlop);

        /// <summary>
        /// Move caret between paragraphs (delimited by empty lines).
        /// </summary>
        public void ParaDown() => SPerform(2413, 0, 0);

        /// <summary>
        /// Move caret between paragraphs (delimited by empty lines).
        /// </summary>
        public void ParaDownExtend() => SPerform(2414, 0, 0);

        /// <summary>
        /// Move caret between paragraphs (delimited by empty lines).
        /// </summary>
        public void ParaUp() => SPerform(2415, 0, 0);

        /// <summary>
        /// Move caret between paragraphs (delimited by empty lines).
        /// </summary>
        public void ParaUpExtend() => SPerform(2416, 0, 0);

        /// <summary>
        /// Given a valid document position, return the previous position taking code
        /// page into account. Returns 0 if passed 0.
        /// </summary>
        public int PositionBefore(int pos) => SPerform(2417, pos, 0);

        /// <summary>
        /// Given a valid document position, return the next position taking code
        /// page into account. Maximum value returned is the last position in the document.
        /// </summary>
        public int PositionAfter(int pos) => SPerform(2418, pos, 0);

        /// <summary>
        /// Copy a range of text to the clipboard. Positions are clipped into the document.
        /// </summary>
        public void CopyRange(int start, int end) => SPerform(2419, start, end);

        /// <summary>
        /// Copy argument text to the clipboard.
        /// </summary>
        public unsafe void CopyText(int length, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2420, length, (uint)b);
            }
        }

        /// <summary>
        /// Retrieve the position of the start of the selection at the given line (INVALID_POSITION if no selection on this line).
        /// </summary>
        public int GetLineSelStartPosition(int line) => SPerform(2424, line, 0);

        /// <summary>
        /// Retrieve the position of the end of the selection at the given line (INVALID_POSITION if no selection on this line).
        /// </summary>
        public int GetLineSelEndPosition(int line) => SPerform(2425, line, 0);

        /// <summary>
        /// Move caret down one line, extending rectangular selection to new caret position.
        /// </summary>
        public void LineDownRectExtend() => SPerform(2426, 0, 0);

        /// <summary>
        /// Move caret up one line, extending rectangular selection to new caret position. 
        /// </summary>
        public void LineUpRectExtend() => SPerform(2427, 0, 0);

        /// <summary>
        /// Move caret left one character, extending rectangular selection to new caret position.
        /// </summary>
        public void CharLeftRectExtend() => SPerform(2428, 0, 0);

        /// <summary>
        /// Move caret right one character, extending rectangular selection to new caret position.
        /// </summary>
        public void CharRightRectExtend() => SPerform(2429, 0, 0);

        /// <summary>
        /// Move caret to first position on line, extending rectangular selection to new caret position.
        /// </summary>
        public void HomeRectExtend() => SPerform(2430, 0, 0);

        /// <summary>
        /// Move caret to before first visible character on line.
        /// If already there move to first character on line.
        /// In either case, extend rectangular selection to new caret position.
        /// </summary>
        public void VCHomeRectExtend() => SPerform(2431, 0, 0);

        /// <summary>
        /// Move caret to last position on line, extending rectangular selection to new caret position.
        /// </summary>
        public void LineEndRectExtend() => SPerform(2432, 0, 0);

        /// <summary>
        /// Move caret one page up, extending rectangular selection to new caret position.
        /// </summary>
        public void PageUpRectExtend() => SPerform(2433, 0, 0);

        /// <summary>
        /// Move caret one page down, extending rectangular selection to new caret position.
        /// </summary>
        public void PageDownRectExtend() => SPerform(2434, 0, 0);

        /// <summary>
        /// Move caret to top of page, or one page up if already at top of page.
        /// </summary>
        public void StutteredPageUp() => SPerform(2435, 0, 0);

        /// <summary>
        /// Move caret to top of page, or one page up if already at top of page, extending selection to new caret position.
        /// </summary>
        public void StutteredPageUpExtend() => SPerform(2436, 0, 0);

        /// <summary>
        /// Move caret to bottom of page, or one page down if already at bottom of page.
        /// </summary>
        public void StutteredPageDown() => SPerform(2437, 0, 0);

        /// <summary>
        /// Move caret to bottom of page, or one page down if already at bottom of page, extending selection to new caret position.
        /// </summary>
        public void StutteredPageDownExtend() => SPerform(2438, 0, 0);

        /// <summary>
        /// Move caret left one word, position cursor at end of word.
        /// </summary>
        public void WordLeftEnd() => SPerform(2439, 0, 0);

        /// <summary>
        /// Move caret left one word, position cursor at end of word, extending selection to new caret position.
        /// </summary>
        public void WordLeftEndExtend() => SPerform(2440, 0, 0);

        /// <summary>
        /// Move caret right one word, position cursor at end of word.
        /// </summary>
        public void WordRightEnd() => SPerform(2441, 0, 0);

        /// <summary>
        /// Move caret right one word, position cursor at end of word, extending selection to new caret position.
        /// </summary>
        public void WordRightEndExtend() => SPerform(2442, 0, 0);

        /// <summary>
        /// Set the set of characters making up whitespace for when moving or selecting by word. Should be called after WordChars.
        /// </summary>
        public unsafe void WhitespaceChars(string characters)
        {
            if (string.IsNullOrEmpty(characters)) characters = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(characters))
            {
                SPerform(2443, 0, (uint)b);
            }
        }

        /// <summary>
        /// Reset the set of characters for whitespace and word characters to the defaults.
        /// </summary>
        public void SetCharsDefault() => SPerform(2444, 0, 0);

        /// <summary>
        /// Enlarge the document to a particular size of text bytes.
        /// </summary>
        public void Allocate(int bytes) => SPerform(2446, bytes, 0);

        /// <summary>
        /// Start notifying the container of all key presses and commands.
        /// </summary>
        public void StartRecord() => SPerform(3001, 0, 0);

        /// <summary>
        /// Stop notifying the container of all key presses and commands.
        /// </summary>
        public void StopRecord() => SPerform(3002, 0, 0);

        /// <summary>
        /// Colourise a segment of the document using the current lexing language.
        /// </summary>
        public void Colourise(int start, int end) => SPerform(4003, start, end);

        /// <summary>
        /// Load a lexer library (dll / so).
        /// </summary>
        public unsafe void LoadLexerLibrary(string path)
        {
            if (string.IsNullOrEmpty(path)) path = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(path))
            {
                SPerform(4007, 0, (uint)b);
            }
        }

        /// <summary>
        /// Find the position of a column on a line taking into account tabs and
        /// multi-byte characters. If beyond end of line, return line end position.
        /// </summary>
        public int FindColumn(int line, int column) => SPerform(2456, line, column);

        /// <summary>
        /// Turn a indicator on over a range.
        /// </summary>
        public void IndicatorFillRange(int position, int fillLength) => SPerform(2504, position, fillLength);

        /// <summary>
        /// Turn a indicator off over a range.
        /// </summary>
        public void IndicatorClearRange(int position, int clearLength) => SPerform(2505, position, clearLength);

        /// <summary>
        /// Are any indicators present at position?
        /// </summary>
        public int IndicatorAllOnFor(int position) => SPerform(2506, position, 0);

        /// <summary>
        /// What value does a particular indicator have at at a position?
        /// </summary>
        public int IndicatorValueAt(int indicator, int position) => SPerform(2507, indicator, position);

        /// <summary>
        /// Where does a particular indicator start?
        /// </summary>
        public int IndicatorStart(int indicator, int position) => SPerform(2508, indicator, position);

        /// <summary>
        /// Where does a particular indicator end?
        /// </summary>
        public int IndicatorEnd(int indicator, int position) => SPerform(2509, indicator, position);

        /// <summary>
        /// Copy the selection, if selection empty copy the line with the caret
        /// </summary>
        public void CopyAllowLine()
        {
            SPerform(2519, 0, 0);
            // Invoke UI update after copy...
            UpdateUI?.Invoke(this);
        }

        /// <summary>
        /// Set the alpha fill colour of the given indicator.
        /// </summary>
        public void SetIndicSetAlpha(int indicator, int alpha) => SPerform(2523, indicator, alpha);

        /// <summary>
        /// Set the alpha fill colour of the given indicator.
        /// </summary>
        public void GetIndicSetAlpha(int indicator) => SPerform(2524, indicator, 0);

        /// <summary>
        /// Which symbol was defined for markerNumber with MarkerDefine
        /// </summary>
        public int GetMarkerSymbolDefined(int markerNumber) => SPerform(2529, markerNumber, 0);

        /// <summary>
        /// Set the text in the text margin for a line
        /// </summary>
        public void SetMarginStyle(int line, int style) => SPerform(2532, line, style);

        /// <summary>
        /// Get the style number for the text margin for a line
        /// </summary>
        public int GetMarginStyle(int line) => SPerform(2533, line, 0);

        /// <summary>
        /// Clear the margin text on all lines
        /// </summary>
        public void MarginTextClearAll() => SPerform(2536, 0, 0);

        /// <summary>
        /// Find the position of a character from a point within the window.
        /// </summary>
        public int GetCharPositionFromPoint(int x, int y) => SPerform(2561, x, y);

        /// <summary>
        /// Find the position of a character from a point within the window. Return INVALID_POSITION if not close to text.
        /// </summary>
        public int GetCharPositionFromPointClose(int x, int y) => SPerform(2562, x, y);

        /// <summary>
        /// Add a container action to the undo stack
        /// </summary>
        public void AddUndoAction(int token, int flags) => SPerform(2560, token, flags);

        /// <summary>
        /// Set the style number for the annotations for a line
        /// </summary>
        public void SetAnnotationStyle(int line, int style) => SPerform(2542, line, style);

        /// <summary>
        /// Get the style number for the annotations for a line
        /// </summary>
        public int GetAnnotationStyle(int line) => SPerform(2543, line, 0);

        /// <summary>
        /// Clear the annotations from all lines
        /// </summary>
        public void AnnotatioClearAll() => SPerform(2547, 0, 0);

        /// <summary>
        /// Get the number of annotation lines for a line
        /// </summary>
        public int GetAnnotationLines(int line) => SPerform(2546, line, 0);

        /// <summary>
        /// Set the text in the text margin for a line
        /// </summary>
        public unsafe void SetMarginText(int line, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2530, line, (uint)b);
            }
        }

        /// <summary>
        /// Set the style in the text margin for a line
        /// </summary>
        public unsafe void SetMarginStyles(int line, string styles)
        {
            if (string.IsNullOrEmpty(styles)) styles = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(styles))
            {
                SPerform(2534, line, (uint)b);
            }
        }

        /// <summary>
        /// Set the annotation text for a line
        /// </summary>
        public unsafe void SetAnnotationText(int line, string text)
        {
            if (string.IsNullOrEmpty(text)) text = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(text))
            {
                SPerform(2540, line, (uint)b);
            }
        }

        /// <summary>
        /// Set the annotation styles for a line
        /// </summary>
        public unsafe void SetAnnotationStyles(int line, string styles)
        {
            if (string.IsNullOrEmpty(styles)) styles = "\0\0";
            fixed (byte* b = Encoding.GetEncoding(CodePage).GetBytes(styles))
            {
                SPerform(2544, line, (uint)b);
            }
        }

        /// <summary>
        /// Get the text in the text margin for a line
        /// </summary>
        public unsafe string GetMarginText(int line)
        {
            var sz = SPerform(2531, line, 0);
            var buffer = new byte[sz + 1];
            fixed (byte* b = buffer) SPerform(2531, line + 1, (uint)b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz - 1);
        }

        /// <summary>
        /// Get the styles in the text margin for a line
        /// </summary>
        public unsafe string GetMarginStyles(int line)
        {
            var sz = SPerform(2535, line, 0);
            var buffer = new byte[sz + 1];
            fixed (byte* b = buffer) SPerform(2535, line + 1, (uint)b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz - 1);
        }

        /// <summary>
        /// Get the annotation text for a line
        /// </summary>
        public unsafe string GetAnnotationText(int line)
        {
            var sz = SPerform(2541, line, 0);
            var buffer = new byte[sz + 1];
            fixed (byte* b = buffer) SPerform(2541, line + 1, (uint)b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz - 1);
        }

        /// <summary>
        /// Get the annotation styles for a line
        /// </summary>
        public unsafe string GetAnnotationStyles(int line)
        {
            var sz = SPerform(2545, line, 0);
            var buffer = new byte[sz + 1];
            fixed (byte* b = buffer) SPerform(2545, line + 1, (uint)b);
            return Encoding.GetEncoding(CodePage).GetString(buffer, 0, sz - 1);
        }

        /// <summary>
        /// Set caret behavior in virtual space. (1 = allow rectangular selection, 2 = allow cursor movement, 3 = both)
        /// </summary>
        public void SetVirtualSpaceOptions(int options) => SPerform(2596, options, 0);

        /// <summary>
        /// Returns caret behavior in virtual space. (1 = allow rectangular selection, 2 = allow cursor movement, 3 = both)
        /// </summary>
        public int GetVirtualSpaceOptions() => SPerform(2597, 0, 0);

        /// <summary>
        /// Set whether pasting, typing, backspace, and delete work on all lines of a multiple selection
        /// </summary>
        public void SetMultiSelectionTyping(bool flag)
        {
            int option = flag ? 1 : 0;
            SPerform(2565, option, 0);
            SPerform(2614, option, 0);
        }

        /// <summary>
        /// Returns whether pasting, typing, backspace, and delete work on all lines of a multiple selection
        /// </summary>
        public bool GetMultiSelectionTyping() => SPerform(2566, 0, 0) != 0;

        /// <summary>
        /// Find the next line at or after lineStart that is a contracted fold header line.
        /// Return -1 when no more lines.
        /// </summary>
        public int ContractedFoldNext(int lineStart) => SPerform(2618, lineStart, 0);

        #endregion

        #region Scintilla Constants

        public const int MAXDWELLTIME = 10000000;
        const int WM_NOTIFY = 0x004e;
        const int WM_SYSCHAR = 0x106;
        const int WM_COMMAND = 0x0111;
        const int WM_KEYDOWN = 0x0100;
        const int WM_SYSKEYDOWN = 0x0104;
        const int WM_DROPFILES = 0x0233;
        const uint WS_CHILD = (uint)0x40000000L;
        const uint WS_VISIBLE = (uint)0x10000000L;
        const uint WS_TABSTOP = (uint)0x00010000L;
        const uint WS_CHILD_VISIBLE_TABSTOP = WS_CHILD | WS_VISIBLE | WS_TABSTOP;
        const int PATH_LEN = 1024;

        #endregion

        #region Scintilla Shortcuts

        /// <summary>
        /// Initializes the user customizable shortcut overrides
        /// </summary>
        public static void InitShortcuts()
        {
            // reference: http://www.scintilla.org/SciTEDoc.html "Keyboard commands"
            AddShortcut("ResetZoom", Keys.Control | Keys.NumPad0, sci => sci.ResetZoom());
            AddShortcut("ZoomOut", Keys.Control | Keys.Subtract, sci => sci.ZoomOut());
            AddShortcut("ZoomIn", Keys.Control | Keys.Add, sci => sci.ZoomIn());
        }

        /// <summary>
        /// Adds a new shortcut override
        /// </summary>
        static void AddShortcut(string displayName, Keys keys, Action<ScintillaControl> action)
        {
            shortcutOverrides.Add("Scintilla." + displayName, new ShortcutOverride(keys, action));
            PluginBase.MainForm.RegisterShortcutItem("Scintilla." + displayName, keys);
        }

        /// <summary>
        /// Updates the shortcut if it changes or needs updating
        /// </summary>
        public static void UpdateShortcut(string id, Keys shortcut)
        {
            if (id.StartsWithOrdinal("Scintilla.")) shortcutOverrides[id].keys = shortcut;
        }

        /// <summary>
        /// Execute the shortcut override using reflection
        /// </summary>
        bool ExecuteShortcut(int keys)
        {
            try
            {
                foreach (var shortcut in shortcutOverrides.Values)
                {
                    if ((Keys)keys == shortcut.keys)
                    {
                        shortcut.action(this);
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Shortcut override object
        /// </summary>
        class ShortcutOverride
        {
            public Keys keys;
            public readonly Action<ScintillaControl> action;

            public ShortcutOverride(Keys keys, Action<ScintillaControl> action)
            {
                this.keys = keys;
                this.action = action;
            }
        }

        #endregion

        #region Scintilla External

        // Stops all sci events from firing...
        public bool DisableAllSciEvents = false;

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int width, int height, IntPtr hWndParent, int hMenu, IntPtr hInstance, string lpParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(HandleRef hModule, string lpProcName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int capindex);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("shell32.dll")]
        public static extern int DragQueryFileA(IntPtr hDrop, uint idx, IntPtr buff, int sz);

        [DllImport("shell32.dll")]
        public static extern int DragFinish(IntPtr hDrop);

        [DllImport("shell32.dll")]
        public static extern void DragAcceptFiles(IntPtr hwnd, int accept);

        public delegate IntPtr Perform(IntPtr sci, int iMessage, IntPtr wParam, IntPtr lParam);

        public uint SlowPerform(uint message, uint wParam, uint lParam)
        {
            return (uint)SendMessage(HandleSci, message, (int)wParam, (int)lParam);
        }

        public int SPerform(int message, int wParam, uint lParam)
        {
            if (Win32.ShouldUseWin32()) return (int)_sciFunction(directPointer, message, (IntPtr)wParam, (IntPtr)lParam);
            return Encoding.ASCII.CodePage;
        }

        public int SPerform(int message, int wParam, int lParam)
        {
            if (Win32.ShouldUseWin32()) return (int)_sciFunction(directPointer, message, (IntPtr)wParam, (IntPtr)lParam);
            return Encoding.ASCII.CodePage;
        }

        public int SPerform(int message, int wParam, IntPtr lParam)
        {
            if (Win32.ShouldUseWin32()) return (int)_sciFunction(directPointer, message, (IntPtr)wParam, lParam);
            return Encoding.ASCII.CodePage;
        }

        public override bool PreProcessMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    {
                        int keys = (int)ModifierKeys + (int)m.WParam;
                        if (!IsFocus || IgnoreAllKeys || ignoredKeys.ContainsKey(keys))
                        {
                            if (ExecuteShortcut(keys) || base.PreProcessMessage(ref m)) return true;
                        }
                        if (((ModifierKeys & Keys.Control) != 0) && ((ModifierKeys & Keys.Alt) == 0))
                        {
                            int code = (int)m.WParam;
                            if ((code >= 65) && (code <= 90)) return true; // Eat non-writable characters
                            if ((code == 9) || (code == 33) || (code == 34)) // Transmit Ctrl with Tab, PageUp/PageDown
                            {
                                return base.PreProcessMessage(ref m);
                            }
                        }
                        break;
                    }
                case WM_SYSKEYDOWN:
                    {
                        return base.PreProcessMessage(ref m);
                    }
                case WM_SYSCHAR:
                    {
                        return base.PreProcessMessage(ref m);
                    }
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COMMAND)
            {
                int message = (m.WParam.ToInt32() >> 16) & 0xffff;
                if (message == (int)Enums.Command.SetFocus || message == (int)Enums.Command.KillFocus)
                {
                    FocusChanged?.Invoke(this);
                }
            }
            else if (m.Msg == WM_NOTIFY)
            {
                SCNotification scn = (SCNotification)Marshal.PtrToStructure(m.LParam, typeof(SCNotification));
                if (scn.nmhdr.hwndFrom == HandleSci && !DisableAllSciEvents)
                {
                    switch (scn.nmhdr.code)
                    {
                        case (uint)Enums.ScintillaEvents.StyleNeeded:
                            StyleNeeded?.Invoke(this, scn.position);
                            break;

                        case (uint)Enums.ScintillaEvents.CharAdded:
                            CharAdded?.Invoke(this, scn.ch);
                            break;

                        case (uint)Enums.ScintillaEvents.SavePointReached:
                            SavePointReached?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.SavePointLeft:
                            SavePointLeft?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.ModifyAttemptRO:
                            ModifyAttemptRO?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.Key:
                            Key?.Invoke(this, scn.ch, scn.modifiers);
                            break;

                        case (uint)Enums.ScintillaEvents.DoubleClick:
                            DoubleClick?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.UpdateUI:
                            UpdateUI?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.MacroRecord:
                            MacroRecord?.Invoke(this, scn.message, scn.wParam, scn.lParam);
                            break;

                        case (uint)Enums.ScintillaEvents.MarginClick:
                            MarginClick?.Invoke(this, scn.modifiers, scn.position, scn.margin);
                            break;

                        case (uint)Enums.ScintillaEvents.NeedShown:
                            NeedShown?.Invoke(this, scn.position, scn.length);
                            break;

                        case (uint)Enums.ScintillaEvents.Painted:
                            Painted?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.UserListSelection:
                            UserListSelection?.Invoke(this, scn.listType, MarshalStr(scn.text));
                            break;

                        case (uint)Enums.ScintillaEvents.URIDropped:
                            URIDropped?.Invoke(this, MarshalStr(scn.text));
                            break;

                        case (uint)Enums.ScintillaEvents.DwellStart:
                            DwellStart?.Invoke(this, scn.position, scn.x, scn.y);
                            break;

                        case (uint)Enums.ScintillaEvents.DwellEnd:
                            DwellEnd?.Invoke(this, scn.position, scn.x, scn.y);
                            break;

                        case (uint)Enums.ScintillaEvents.Zoom:
                            Zoom?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.HotspotClick:
                            HotSpotClick?.Invoke(this, scn.modifiers, scn.position);
                            break;

                        case (uint)Enums.ScintillaEvents.HotspotDoubleClick:
                            HotSpotDoubleClick?.Invoke(this, scn.modifiers, scn.position);
                            break;

                        case (uint)Enums.ScintillaEvents.CalltipClick:
                            CallTipClick?.Invoke(this, scn.position);
                            break;

                        case (uint)Enums.ScintillaEvents.AutoCSelection:
                            AutoCSelection?.Invoke(this, MarshalStr(scn.text));
                            break;

                        case (uint)Enums.ScintillaEvents.IndicatorClick:
                            IndicatorClick?.Invoke(this, scn.position);
                            break;

                        case (uint)Enums.ScintillaEvents.IndicatorRelease:
                            IndicatorRelease?.Invoke(this, scn.position);
                            break;

                        case (uint)Enums.ScintillaEvents.AutoCCharDeleted:
                            AutoCCharDeleted?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.AutoCCancelled:
                            AutoCCancelled?.Invoke(this);
                            break;

                        case (uint)Enums.ScintillaEvents.Modified:
                            bool notify = false;
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.InsertText) > 0)
                            {
                                TextInserted?.Invoke(this, scn.position, scn.length, scn.linesAdded);
                                notify = true;
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.DeleteText) > 0)
                            {
                                TextDeleted?.Invoke(this, scn.position, scn.length, scn.linesAdded);
                                notify = true;
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.ChangeStyle) > 0)
                            {
                                StyleChanged?.Invoke(this, scn.position, scn.length);
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.ChangeFold) > 0)
                            {
                                FoldChanged?.Invoke(this, scn.line, scn.foldLevelNow, scn.foldLevelPrev);
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.UserPerformed) > 0)
                            {
                                UserPerformed?.Invoke(this);
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.UndoPerformed) > 0)
                            {
                                UndoPerformed?.Invoke(this);
                                notify = true;
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.RedoPerformed) > 0)
                            {
                                RedoPerformed?.Invoke(this);
                                notify = true;
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.LastStepInUndoRedo) > 0)
                            {
                                LastStepInUndoRedo?.Invoke(this);
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.ChangeMarker) > 0)
                            {
                                MarkerChanged?.Invoke(this, scn.line);
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.BeforeInsert) > 0)
                            {
                                BeforeInsert?.Invoke(this, scn.position, scn.length);
                                notify = false;
                            }
                            if ((scn.modificationType & (uint)Enums.ModificationFlags.BeforeDelete) > 0)
                            {
                                BeforeDelete?.Invoke(this, scn.position, scn.length);
                                notify = false;
                            }
                            if (notify && scn.text != null && Modified is { } modified)
                            {
                                try
                                {
                                    var text = MarshalStr(scn.text, scn.length);
                                    modified(this, scn.position, scn.modificationType, text, scn.length, scn.linesAdded, scn.line, scn.foldLevelNow, scn.foldLevelPrev);
                                }
                                catch { }
                            }
                            break;
                    }
                }
            }
            else if (m.Msg == WM_DROPFILES)
            {
                if (Win32.ShouldUseWin32()) HandleFileDrop(m.WParam);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        unsafe string MarshalStr(IntPtr p)
        {
            sbyte* b = (sbyte*)p;
            int len = 0;
            while (b[len] != 0) ++len;
            return new string(b, 0, len);
        }

        unsafe string MarshalStr(IntPtr p, int len)
        {
            sbyte* b = (sbyte*)p;
            return new string(b, 0, len);
        }

        #endregion

        #region Automated Features

        /// <summary>
        /// Support for selection highlighting and selection changed event
        /// </summary>
        void OnUpdateUI(ScintillaControl sci)
        {
            if (lastSelectionStart != sci.SelectionStart || lastSelectionEnd != sci.SelectionEnd || lastSelectionLength != sci.SelText.Length)
            {
                SelectionChanged?.Invoke(sci);
                switch (PluginBase.Settings.HighlightMatchingWordsMode) // Handle selection highlighting
                {
                    case Enums.HighlightMatchingWordsMode.SelectionOrPosition:
                    {
                        StartHighlightSelectionTimer();
                        break;
                    }
                    case Enums.HighlightMatchingWordsMode.SelectedWord:
                    {
                        if (sci.SelText == sci.GetWordFromPosition(sci.CurrentPos))
                        {
                            StartHighlightSelectionTimer();
                        }
                        break;
                    }
                }
            }
            lastSelectionStart = sci.SelectionStart;
            lastSelectionEnd = sci.SelectionEnd;
            lastSelectionLength = sci.SelText.Length;
        }

        /// <summary>
        /// Use timer for aggressive selection highlighting
        /// </summary>
        void StartHighlightSelectionTimer()
        {
            if (highlightDelay is null)
            {
                highlightDelay = new System.Timers.Timer(PluginBase.Settings.HighlightMatchingWordsDelay);
                highlightDelay.Elapsed += highlightDelay_Elapsed;
                highlightDelay.SynchronizingObject = this;
            }
            else highlightDelay.Stop();
            if (highlightDelay.Interval != PluginBase.Settings.HighlightMatchingWordsDelay)
            {
                highlightDelay.Interval = PluginBase.Settings.HighlightMatchingWordsDelay;
            }
            highlightDelay.Start();
        }

        void highlightDelay_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            highlightDelay.Stop();
            HighlightWordsMatchingSelected();
        }

        /// <summary>
        /// Provides basic highlighting of selected text
        /// </summary>
        void HighlightWordsMatchingSelected()
        {
            if (TextLength == 0 || TextLength > 64 * 1024) return;
            var word = GetWordFromPosition(CurrentPos);
            if (string.IsNullOrEmpty(word)) return;
            if (PositionIsOnComment(CurrentPos))
            {
                RemoveHighlights(1);
                return;
            }
            var pattern = word.Trim();
            var search = new FRSearch(pattern)
            {
                WholeWord = true,
                NoCase = !PluginBase.Settings.HighlightMatchingWordsCaseSensitive,
                Filter = SearchFilter.OutsideCodeComments | SearchFilter.OutsideStringLiterals,
                SourceFile = FileName
            };
            RemoveHighlights(1);
            var test = search.Matches(Text);
            var language = Configuration.GetLanguage(ConfigurationLanguage);
            AddHighlights(1, test, language.editorstyle.HighlightWordBackColor);
            hasHighlights = true;
        }

        /// <summary>
        /// Cancel highlights if not using aggressive highlighting
        /// </summary>
        void OnCancelHighlight(ScintillaControl sci)
        {
            if (sci.isHiliteSelected && sci.hasHighlights && sci.SelText.Length == 0
                && PluginBase.Settings.HighlightMatchingWordsMode != Enums.HighlightMatchingWordsMode.SelectionOrPosition)
            {
                sci.RemoveHighlights(1);
                sci.hasHighlights = false;
            }
        }

        /// <summary>
        /// Provides the support for code block selection
        /// </summary>
        void OnBlockSelect(ScintillaControl sci)
        {
            int position = CurrentPos - 1;
            char character = (char)CharAt(position);
            if (character != '{' && character != '(' && character != '[') return;
            if (PositionIsOnComment(position)) return;
            int bracePosStart = position;
            int bracePosEnd = BraceMatch(position);
            if (bracePosEnd != -1) SetSel(bracePosStart, bracePosEnd + 1);
        }

        /// <summary>
        /// Provides the support for brace matching
        /// </summary>
        void OnBraceMatch(ScintillaControl sci)
        {
            if (!isBraceMatching || sci.SelText.Length != 0) return;
            var position = CurrentPos - 1;
            var character = (char)CharAt(position);
            if (character != '{' && character != '}' && character != '(' && character != ')' && character != '[' && character != ']')
            {
                position = CurrentPos;
                character = (char)CharAt(position);
            }
            if (character == '{' || character == '}' || character == '(' || character == ')' || character == '[' || character == ']')
            {
                if (!PositionIsOnComment(position))
                {
                    var bracePosStart = position;
                    var bracePosEnd = BraceMatch(position);
                    if (bracePosEnd != -1) BraceHighlight(bracePosStart, bracePosEnd);
                    if (useHighlightGuides)
                    {
                        var line = LineFromPosition(position);
                        HighlightGuide = GetLineIndentation(line);
                    }
                }
                else
                {
                    BraceHighlight(-1, -1);
                    HighlightGuide = 0;
                }
            }
            else
            {
                BraceHighlight(-1, -1);
                HighlightGuide = 0;
            }
        }

        /// <summary>
        /// Provides support for smart indenting
        /// </summary>
        ///
        internal void OnSmartIndent(ScintillaControl ctrl, int ch)
        {
            char newline = (EOLMode == 1) ? '\r' : '\n';
            switch (SmartIndentType)
            {
                case Enums.SmartIndent.None:
                    return;
                case Enums.SmartIndent.Simple:
                    if (ch == newline)
                    {
                        BeginUndoAction();
                        try
                        {
                            int curLine = CurrentLine;
                            int previousIndent = GetLineIndentation(curLine - 1);
                            IndentLine(curLine, previousIndent);
                            int position = LineIndentPosition(curLine);
                            SetSel(position, position);
                        }
                        finally
                        {
                            EndUndoAction();
                        }
                    }
                    break;
                case Enums.SmartIndent.CPP:
                    if (ch == newline)
                    {
                        BeginUndoAction();
                        try
                        {
                            int curLine = CurrentLine;
                            int tempLine = curLine;
                            int previousIndent;
                            string tempText3; //line text without newline
                            string tempText2; //line text trim end
                            string tempText; //line text without comment and trim end
                            do
                            {
                                --tempLine;
                                tempText3 = GetLine(tempLine).TrimEnd('\n', '\r');
                                tempText2 = tempText3.TrimEnd();
                                tempText = tempText2;
                                if (tempText.Length == 0) previousIndent = -1;
                                else previousIndent = GetLineIndentation(tempLine);
                            }
                            while ((tempLine > 0) && (previousIndent < 0));
                            int commentIndex = tempText.IndexOfOrdinal("//");
                            if (commentIndex > 0) // remove comment at end of line
                            {
                                int slashes = MBSafeTextLength(tempText.Substring(0, commentIndex + 1));
                                if (PositionIsOnComment(PositionFromLine(tempLine) + slashes))
                                    tempText = tempText.Substring(0, commentIndex).TrimEnd();
                            }
                            if (tempText.EndsWith('{'))
                            {
                                int bracePos = CurrentPos - 2 - (tempText3.Length - tempText.Length); //CurrentPos - 1 is always ch (newline)
                                while (bracePos > 0 && CharAt(bracePos) != '{') bracePos--;
                                int style = BaseStyleAt(bracePos);
                                if (bracePos >= 0 && CharAt(bracePos) == '{' && (style == 10/*CPP*/ || style == 5/*CSS*/))
                                {
                                    previousIndent += TabWidth;
                                    if (tempText.Length == tempText2.Length) //Doesn't end with comment
                                    {
                                        if (tempText3.Length > tempText.Length) //Ends with whitespace after {
                                        {
                                            AnchorPosition = bracePos + 1;
                                            CurrentPos--; //before ch (newline)
                                            DeleteBack();
                                            AnchorPosition = bracePos + 2;
                                            CurrentPos = bracePos + 2; //same as CurrentPos++ (after ch)
                                        }
                                    }
                                }
                            }
                            // TODO: Should this test a config variable for indenting after case : statements?
                            if (Lexer == 3 && tempText.EndsWith(':') && !tempText.EndsWithOrdinal("::") && !PositionIsOnComment(PositionFromLine(tempLine)))
                            {
                                int prevLine = tempLine;
                                while (--prevLine > 0)
                                {
                                    tempText = GetLine(prevLine).Trim();
                                    if (tempText.Length != 0 && !tempText.StartsWithOrdinal("//"))
                                    {
                                        int prevIndent = GetLineIndentation(prevLine);
                                        if ((tempText.EndsWith(';') && previousIndent == prevIndent) ||
                                            (tempText.EndsWith(':') && previousIndent == prevIndent + Indent))
                                        {
                                            previousIndent -= Indent;
                                            SetLineIndentation(tempLine, previousIndent);
                                        }
                                        break;
                                    }
                                }
                                previousIndent += Indent;
                            }
                            IndentLine(curLine, previousIndent);
                            int position = LineIndentPosition(curLine);
                            SetSel(position, position);
                            if (Lexer == 3 && ModifierKeys == Keys.Shift)
                            {
                                int endPos = LineEndPosition(curLine - 1);
                                int style = BaseStyleAt(endPos - 1);
                                if (style == 12 || style == 7)
                                {
                                    string quote = GetStringType(endPos).ToString();
                                    InsertText(endPos, quote);
                                    InsertText(position + 1, "+ " + quote);
                                    GotoPos(position + 4);
                                    //if (Regex.IsMatch(GetLine(curLine - 1), "=[\\s]*" + quote))
                                    SetLineIndentation(curLine, GetLineIndentation(curLine - 1) + TabWidth);
                                }
                            }
                        }
                        finally
                        {
                            EndUndoAction();
                        }
                    }
                    else if (ch == '}')
                    {
                        BeginUndoAction();
                        try
                        {
                            int position = CurrentPos - 1;
                            int match = SafeBraceMatch(position); //SafeBraceMatch() calls Colourise(0, -1)
                            if (match != -1 && !PositionIsInString(position))
                            {
                                IndentLine(LineFromPosition(position), GetLineIndentation(LineFromPosition(match)));
                            }
                        }
                        finally
                        {
                            EndUndoAction();
                        }
                    }
                    break;
                case Enums.SmartIndent.Custom:
                    if (ch == newline)
                    {
                        SmartIndent?.Invoke(this);
                    }
                    break;
            }
        }

        /// <summary>
        /// Detects the string-literal quote style. Returns space if undefined.
        /// </summary>
        /// <param name="position">lookup position</param>
        /// <returns>' or " or Space if undefined</returns>
        public char GetStringType(int position)
        {
            int i;
            var previous = (char) CharAt(position);
            for (i = position; i > 0; i--)
            {
                var current = previous;
                previous = (char) CharAt(i - 1);
                if (current != '\'' && current != '"') continue;
                var escaped = false;
                while (previous == '\\')
                {
                    i--;
                    previous = (char) CharAt(i - 1);
                    escaped = !escaped;
                }
                if (!escaped) return current;
            }
            // for example: '${v}'
            if (i == 0 && (char) CharAt(0) is char c && (c == '\'' || c == '"')) return c;
            return ' ';
        }

        #endregion

        #region Misc Custom Stuff

        /// <summary>
        /// Gets the amount of lines visible (ie. not folded)
        /// </summary>
        int LinesVisible
        {
            get
            {
                var result = 0;
                for (int i = 0, count = LineCount; i < count; i++)
                {
                    if (GetLineVisible(i)) result++;
                }
                return result;
            }
        }

        /// <summary>
        /// Set caret to line to indent position and ensure it is visible.
        /// </summary>
        public void GotoLineIndent(int line)
        {
            var pos = LineIndentPosition(line);
            GotoPos(pos);
        }

        /// <summary>
        /// Render the contents for printing
        /// </summary>
        public int FormatRange(bool measureOnly, PrintPageEventArgs e, int charFrom, int charTo)
        {
            IntPtr hdc = e.Graphics.GetHdc();
            int wParam = (measureOnly ? 0 : 1);
            RangeToFormat frPrint = GetRangeToFormat(hdc, charFrom, charTo);
            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(frPrint));
            Marshal.StructureToPtr(frPrint, lParam, false);
            int res = SPerform(2151, wParam, lParam);
            Marshal.FreeCoTaskMem(lParam);
            e.Graphics.ReleaseHdc(hdc);
            return res;
        }

        /// <summary>
        /// Populates the RangeToFormat struct
        /// </summary>
        RangeToFormat GetRangeToFormat(IntPtr hdc, int charFrom, int charTo)
        {
            RangeToFormat frPrint;
            var pageWidth = GetDeviceCaps(hdc, 110);
            var pageHeight = GetDeviceCaps(hdc, 111);
            frPrint.hdcTarget = hdc;
            frPrint.hdc = hdc;
            frPrint.rcPage.Left = 0;
            frPrint.rcPage.Top = 0;
            frPrint.rcPage.Right = pageWidth;
            frPrint.rcPage.Bottom = pageHeight;
            frPrint.rc.Left = Convert.ToInt32(pageWidth * 0.02);
            frPrint.rc.Top = Convert.ToInt32(pageHeight * 0.03);
            frPrint.rc.Right = Convert.ToInt32(pageWidth * 0.975);
            frPrint.rc.Bottom = Convert.ToInt32(pageHeight * 0.95);
            frPrint.chrg.cpMin = charFrom;
            frPrint.chrg.cpMax = charTo;
            return frPrint;
        }

        /// <summary>
        /// Free cached data from the control after printing
        /// </summary>
        public void FormatRangeDone() => SPerform(2151, 0, 0);

        /// <summary>
        /// This holds the actual encoding of the document
        /// </summary>
        public Encoding Encoding
        {
            get => encoding;
            set
            {
                encoding = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Indicate that BOM characters should be written when saving
        /// </summary>
        public bool SaveBOM
        {
            get => saveBOM;
            set
            {
                saveBOM = value;
                UpdateSync?.Invoke(this);
            }
        }

        /// <summary>
        /// Adds a line end marker to the end of the document
        /// </summary>
        public void AddLastLineEnd()
        {
            var eolMarker = NewLineMarker;
            if (!Text.EndsWithOrdinal(eolMarker)) AppendText(eolMarker.Length, eolMarker);
        }

        /// <summary>
        /// Removes trailing spaces from each line
        /// </summary>
        public void StripTrailingSpaces() => StripTrailingSpaces(false);

        public void StripTrailingSpaces(bool keepIndentTabs)
        {
            BeginUndoAction();
            try
            {
                int maxLines = LineCount;
                for (int line = 0; line < maxLines; line++)
                {
                    int lineStart = PositionFromLine(line);
                    int lineEnd = LineEndPosition(line);
                    int i = lineEnd - 1;
                    char ch = (char)CharAt(i);
                    while ((i >= lineStart) && ((ch == ' ') || (ch == '\t')))
                    {
                        i--;
                        ch = (char)CharAt(i);
                    }
                    if (keepIndentTabs && i == lineStart - 1)
                    {
                        ch = (char)CharAt(i + 1);
                        while (i < lineEnd && ch == '\t')
                        {
                            i++;
                            ch = (char)CharAt(i + 1);
                        }
                    }
                    if (i < (lineEnd - 1))
                    {
                        TargetStart = i + 1;
                        TargetEnd = lineEnd;
                        ReplaceTarget(0, "");
                    }
                }
            }
            finally
            {
                EndUndoAction();
            }
        }

        /// <summary>
        /// Checks if a line is in preprocessor block
        /// </summary>
        public bool LineIsInPreprocessor(ScintillaControl sci, int lexerPpStyle, int line)
        {
            bool ppEnd = false;
            bool ppStart = false;
            int foldHeader = (int)Enums.FoldLevel.HeaderFlag;
            for (var i = line; i > 0; i--)
            {
                int pos = sci.PositionFromLine(i);
                int ind = sci.GetLineIndentation(i);
                int style = sci.BaseStyleAt(pos + ind);
                if (style == lexerPpStyle)
                {
                    int fold = sci.GetFoldLevel(i) & foldHeader;
                    if (fold == foldHeader) ppStart = true;
                    else
                    {
                        int foldParent = sci.FoldParent(i);
                        if (foldParent != -1)
                        {
                            pos = sci.PositionFromLine(foldParent);
                            ind = sci.GetLineIndentation(i);
                            style = sci.BaseStyleAt(pos + ind);
                            if (style == lexerPpStyle) ppStart = true;
                        }
                    }
                    break;
                }
            }
            for (var i = line; i < sci.LineCount; i++)
            {
                int pos = sci.PositionFromLine(i);
                int ind = sci.GetLineIndentation(i);
                int style = sci.BaseStyleAt(pos + ind);
                if (style == lexerPpStyle)
                {
                    int fold = sci.GetFoldLevel(i) & foldHeader;
                    if (fold != foldHeader) ppEnd = true;
                    break;
                }
            }
            return ppStart && ppEnd;
        }

        /// <summary>
        /// Checks that if the specified position is on comment.
        /// NOTE: You may need to manually update coloring: "sci.Colourise(0, -1);"
        /// </summary>
        public bool PositionIsOnComment(int position) => PositionIsOnComment(position, Lexer);

        public bool PositionIsOnComment(int position, int lexer)
        {
            int style = BaseStyleAt(position);
            if (lexer == 3 || lexer == 18 || lexer == 25 || lexer == 27)
            {
                return (    // cpp, tcl, bullant or pascal
                style == 1
                || style == 2
                || style == 3
                || style == 15
                || style == 17
                || style == 18);
            }

            if (lexer == 4 || lexer == 5)
            {
                return (    // html or xml
                    style == 9
                    || style == 20
                    || style == 29
                    || style == 30
                    || style == 42
                    || style == 43
                    || style == 44
                    || style == 57
                    || style == 58
                    || style == 59
                    || style == 72
                    || style == 82
                    || style == 92
                    || style == 107
                    || style == 124
                    || style == 125);
            }
            if (lexer == 2 || lexer == 21)
            {
                return (    // python or lisp
                    style == 1
                    || style == 12);
            }
            if (lexer == 6 || lexer == 22 || lexer == 45 || lexer == 62)
            {
                return (    // perl, bash, clarion/clw or ruby
                    style == 2);
            }
            if (lexer == 7)
            {
                return (    // sql
                    style == 1
                    || style == 2
                    || style == 3
                    || style == 13
                    || style == 15
                    || style == 17
                    || style == 18);
            }
            if (lexer == 8 || lexer == 9 || lexer == 11 || lexer == 12 || lexer == 16 || lexer == 17 || lexer == 19 || lexer == 23 || lexer == 24 || lexer == 26 || lexer == 28 || lexer == 32 || lexer == 36 || lexer == 37 || lexer == 40 || lexer == 44 || lexer == 48 || lexer == 51 || lexer == 53 || lexer == 54 || lexer == 57 || lexer == 63)
            {
                return (    // asn1, vb, diff, batch, makefile, avenue, eiffel, eiffelkw, vbscript, matlab, crontab, fortran, f77, lout, mmixal, yaml, powerbasic, erlang, octave, kix or properties
                    style == 1);
            }
            if (lexer == 14)
            {
                return (    // latex
                    style == 4);
            }
            if (lexer == 15 || lexer == 41 || lexer == 56)
            {
                return (    // lua, verilog or escript
                    style == 1
                    || style == 2
                    || style == 3);
            }
            if (lexer == 20)
            {
                return (    // ada
                    style == 10);
            }
            if (lexer == 31 || lexer == 39 || lexer == 42 || lexer == 52 || lexer == 55 || lexer == 58 || lexer == 60 || lexer == 61 || lexer == 64 || lexer == 71)
            {
                return (    // au3, apdl, baan, ps, mssql, rebol, forth, gui4cli, vhdl or pov
                    style == 1
                    || style == 2);
            }
            if (lexer == 34)
            {
                return (    // asm
                    style == 1
                    || style == 11);
            }
            if (lexer == 43)
            {
                return (    // nsis
                    style == 1
                    || style == 18);
            }
            if (lexer == 59)
            {
                return (    // specman
                    style == 2
                    || style == 3);
            }
            if (lexer == 70)
            {
                return (    // tads3
                    style == 3
                    || style == 4);
            }
            if (lexer == 74)
            {
                return (    // csound
                    style == 1
                    || style == 9);
            }
            if (lexer == 65)
            {
                return (    // caml
                    style == 12
                    || style == 13
                    || style == 14
                    || style == 15);
            }
            if (lexer == 68)
            {
                return (    // haskell
                    style == 13
                    || style == 14
                    || style == 15
                    || style == 16);
            }
            if (lexer == 73)
            {
                return (    // flagship
                    style == 1
                    || style == 2
                    || style == 3
                    || style == 4
                    || style == 5
                    || style == 6);
            }
            if (lexer == 72)
            {
                return (    // smalltalk
                    style == 3);
            }
            if (lexer == 38)
            {
                return (    // css
                    style == 9);
            }

            return false;
        }

        /// <summary>
        /// Checks that if the specified position is in string.
        /// You may need to manually update coloring: <see cref="Colourise(int, int)"/>.
        /// </summary>
        public bool PositionIsInString(int position) => PositionIsInString(position, Lexer);

        /// <summary>
        /// Checks that if the specified position is in string.
        /// You may need to manually update coloring: <see cref="Colourise(int, int)"/>.
        /// </summary>
        bool PositionIsInString(int position, int lexer)
        {
            int style = BaseStyleAt(position);
            
            switch ((Enums.Lexer) lexer)
            {
                case Enums.Lexer.CPP:
                case Enums.Lexer.BULLANT:
                case Enums.Lexer.HTML:
                case Enums.Lexer.XML:
                case Enums.Lexer.PERL:
                case Enums.Lexer.RUBY:
                case Enums.Lexer.LUA:
                case Enums.Lexer.SQL:
                case Enums.Lexer.GAP:
                case Enums.Lexer.R:
                    return style == (int) CPP.STRING || style == (int) CPP.CHARACTER;
                case Enums.Lexer.SMALLTALK:
                    return style == (int) SMALLTALK.STRING;
                case Enums.Lexer.PLM:
                    return style == (int) PLM.STRING;
                case Enums.Lexer.MAGIK:
                case Enums.Lexer.POWERSHELL:
                    return style == (int) MAGIK.STRING || style == (int) MAGIK.CHARACTER;
                // TODO: and more...
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indents the specified line
        /// </summary>
        protected void IndentLine(int line, int indent)
        {
            if (indent < 0) return;
            int selStart = SelectionStart;
            int selEnd = SelectionEnd;
            int posBefore = LineIndentPosition(line);
            SetLineIndentation(line, indent);
            int posAfter = LineIndentPosition(line);
            int posDifference = posAfter - posBefore;
            if (posAfter > posBefore)
            {
                if (selStart >= posBefore) selStart += posDifference;
                if (selEnd >= posBefore) selEnd += posDifference;
            }
            else if (posAfter < posBefore)
            {
                if (selStart >= posAfter)
                {
                    if (selStart >= posBefore) selStart += posDifference;
                    else selStart = posAfter;
                }
                if (selEnd >= posAfter)
                {
                    if (selEnd >= posBefore) selEnd += posDifference;
                    else selEnd = posAfter;
                }
            }
            SetSel(selStart, selEnd);
        }

        /// <summary>
        /// Expands all folds
        /// </summary>
        public void ExpandAllFolds()
        {
            for (int i = 0; i < LineCount; i++)
            {
                FoldExpanded(i, true);
                ShowLines(i + 1, i + 1);
            }
        }

        /// <summary>
        /// Collapses all folds
        /// </summary>
        public void CollapseAllFolds()
        {
            for (int i = 0; i < LineCount; i++)
            {
                int maxSubOrd = LastChild(i, -1);
                FoldExpanded(i, false);
                HideLines(i + 1, maxSubOrd);
            }
        }

        /// <summary>
        /// Only folds functions keeping the blocks within it open
        /// </summary>
        public void CollapseFunctions()
        {
            int lineCount = LineCount;

            for (int i = 0; i < lineCount; i++)
            {
                // Determine if function block
                string line = GetLine(i);
                if (line.Contains("function"))
                {
                    // Find the line with the closing ) of the function header
                    while (!line.Contains(")") && i < lineCount)
                    {
                        i++;
                        line = GetLine(i);
                    }

                    // Get the function closing brace
                    int maxSubOrd = LastChild(i, -1);
                    // Get brace if on the next line
                    if (maxSubOrd == i)
                    {
                        i++;
                        maxSubOrd = LastChild(i, -1);
                    }
                    FoldExpanded(i, false);
                    HideLines(i + 1, maxSubOrd);
                    i = maxSubOrd;
                }
                else
                {
                    FoldExpanded(i, true);
                    ShowLines(i + 1, i + 1);
                }
            }
        }

        /// <summary>
        /// Only folds regions and functions keeping the blocks within it open
        /// </summary>
        public void CollapseRegions()
        {
            // hide all lines inside some blocks, show lines outside
            for (int i = 0; i < LineCount; i++)
            {
                // if region/function block
                string line = GetLine(i);
                if (line.Contains("//{") || (line.Contains("function") && line.Contains("(") && line.Contains(")")))
                {
                    // Get the function closing brace
                    int maxSubOrd = LastChild(i, -1);
                    // Get brace if on the next line
                    if (maxSubOrd == i)
                    {
                        i++;
                        maxSubOrd = LastChild(i, -1);
                    }
                    // hide all lines inside
                    HideLines(i + 1, maxSubOrd);
                    i = maxSubOrd;
                }
                else
                {
                    // show lines outside
                    ShowLines(i + 1, i + 1);
                }
            }
            // collapse some block lines, expand all other type of lines
            for (int i = 0; i < LineCount; i++)
            {
                // if region/function block
                string line = GetLine(i);
                if (line.Contains("//{") || (line.Contains("function") && line.Contains("(") && line.Contains(")")))
                {
                    // Get the function closing brace
                    int maxSubOrd = LastChild(i, -1);
                    // Get brace if on the next line
                    if (maxSubOrd == i)
                    {
                        i++;
                        maxSubOrd = LastChild(i, -1);
                    }
                    // collapse some block lines
                    FoldExpanded(i, false);
                }
                else
                {
                    // expand all other type of lines
                    FoldExpanded(i, true);
                }
            }
        }

        /// <summary>
        /// Selects the specified text, starting from the caret position
        /// </summary>
        public int SelectText(string text)
        {
            int pos = Text.IndexOfOrdinal(text, MBSafeCharPosition(CurrentPos));
            if (pos >= 0) MBSafeSetSel(pos, text);
            return pos;
        }

        /// <summary>
        /// Selects the specified text, starting from the given position
        /// </summary>
        public int SelectText(string text, int startPos)
        {
            int pos = Text.IndexOfOrdinal(text, startPos);
            if (pos >= 0) MBSafeSetSel(pos, text);
            return pos;
        }

        /// <summary>
        /// Gets a word from the specified position
        /// </summary>
        public string GetWordFromPosition(int position)
        {
            try
            {
                int startPosition = MBSafeCharPosition(WordStartPosition(position, true));
                int endPosition = MBSafeCharPosition(WordEndPosition(position, true));
                string keyword = Text.Substring(startPosition, endPosition - startPosition);
                if (keyword == "" || keyword == " ") return null;
                return keyword.Trim();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Insert text with wide-char to byte position conversion
        /// </summary>
        public void MBSafeInsertText(int position, string text)
        {
            if (CodePage != 65001)
            {
                InsertText(position, text);
            }
            else
            {
                int mbpos = MBSafePosition(position);
                InsertText(mbpos, text);
            }
        }

        /// <summary>
        /// Set cursor position with wide-char to byte position conversion
        /// </summary>
        public void MBSafeGotoPos(int position)
        {
            if (CodePage != 65001)
            {
                GotoPos(position);
            }
            else
            {
                int mbpos = MBSafePosition(position);
                GotoPos(mbpos);
            }
        }

        /// <summary>
        /// Select text using wide-char to byte indexes conversion
        /// </summary>
        public void MBSafeSetSel(int start, int end)
        {
            if (CodePage != 65001)
            {
                SetSel(start, end);
            }
            else
            {
                var count = Text.Substring(start, end - start);
                start = MBSafePosition(start);
                end = start + MBSafeTextLength(count);
                SetSel(start, end);
            }
        }

        /// <summary>
        /// Select text using wide-char to byte index & text conversion
        /// </summary>
        public void MBSafeSetSel(int start, string text)
        {
            if (CodePage != 65001)
            {
                SetSel(start, start + text.Length);
            }
            else
            {
                int mbpos = MBSafePosition(start);
                SetSel(mbpos, mbpos + MBSafeTextLength(text));
            }
        }

        /// <summary>
        /// Wide-char to byte position in the editor text
        /// </summary>
        public int MBSafePosition(int position)
        {
            if (CodePage != 65001) return position;
            if (position < 0) return position;
            var count = Text.Substring(0, position);
            var result = Encoding.UTF8.GetByteCount(count);
            return result;
        }

        /// <summary>
        /// Byte to wide-char position in the editor text
        /// </summary>
        public int MBSafeCharPosition(int bytePosition)
        {
            if (CodePage != 65001) return bytePosition;
            if (bytePosition < 0) return bytePosition;
            var bytes = Encoding.UTF8.GetBytes(Text);
            var result = Encoding.UTF8.GetCharCount(bytes, 0, bytePosition);
            return result;
        }

        /// <summary>
        /// Counts byte length of wide-char text
        /// </summary>
        public int MBSafeTextLength(string text)
        {
            if (CodePage != 65001) return text.Length;
            var result = Encoding.UTF8.GetByteCount(text);
            return result;
        }

        /// <summary>
        /// Converts bytes count to text length
        /// </summary>
        /// <param name="txt">Reference text</param>
        /// <param name="bytes">Bytes count</param>
        /// <returns>Multi-byte chars length</returns>
        public int MBSafeLengthFromBytes(string txt, int bytes)
        {
            if (CodePage != 65001) return bytes;
            var raw = Encoding.UTF8.GetBytes(txt);
            return Encoding.UTF8.GetString(raw, 0, Math.Min(raw.Length, bytes)).Length;
        }

        /// <summary>
        /// Custom way to find the matching brace when BraceMatch() does not work
        /// </summary>
        public int SafeBraceMatch(int position)
        {
            int match = CharAt(position);
            int toMatch;
            int length = TextLength;
            int ch;
            int sub = 0;
            int lexer = Lexer;
            Colourise(0, -1);
            bool comment = PositionIsOnComment(position, lexer);
            switch (match)
            {
                case '{':
                    toMatch = '}';
                    goto down;
                case '(':
                    toMatch = ')';
                    goto down;
                case '[':
                    toMatch = ']';
                    goto down;
                case '}':
                    toMatch = '{';
                    goto up;
                case ')':
                    toMatch = '(';
                    goto up;
                case ']':
                    toMatch = '[';
                    goto up;
            }
            return -1;
            // search up
            up:
            while (position >= 0)
            {
                position--;
                ch = CharAt(position);
                if (ch == match)
                {
                    if (comment == PositionIsOnComment(position, lexer)) sub++;
                }
                else if (ch == toMatch && comment == PositionIsOnComment(position, lexer))
                {
                    sub--;
                    if (sub < 0) return position;
                }
            }
            return -1;
            // search down
            down:
            while (position < length)
            {
                position++;
                ch = CharAt(position);
                if (ch == match)
                {
                    if (comment == PositionIsOnComment(position, lexer)) sub++;
                }
                else if (ch == toMatch && comment == PositionIsOnComment(position, lexer))
                {
                    sub--;
                    if (sub < 0) return position;
                }
            }
            return -1;
        }

        /// <summary>
        /// File dropped on the control, fire URIDropped event
        /// </summary>
        unsafe void HandleFileDrop(IntPtr hDrop)
        {
            int nfiles = DragQueryFileA(hDrop, 0xffffffff, (IntPtr)null, 0);
            string files = "";
            byte[] buffer = new byte[PATH_LEN];
            for (uint i = 0; i < nfiles; i++)
            {
                fixed (byte* b = buffer)
                {
                    DragQueryFileA(hDrop, i, (IntPtr)b, PATH_LEN);
                    if (files.Length > 0) files += ' ';
                    files += '"' + MarshalStr((IntPtr)b) + '"';
                }
            }
            DragFinish(hDrop);
            URIDropped?.Invoke(this, files);
        }


        /// <summary>
        /// Returns the base style (without indicators) byte at the position.
        /// </summary>
        public int BaseStyleAt(int pos) => (SPerform(2010, pos, 0) & ((1 << StyleBits) - 1));

        /// <summary>
        /// Adds the specified highlight to the control
        /// </summary>
        public void AddHighlight(int indicator, int indicStyle, int highlightColor, int start, int length)
        {
            var doc = DocumentManager.FindDocument(this);
            if (doc is null) return;
            int es = EndStyled;
            int mask = (1 << StyleBits) - 1;
            // Define indics in both controls...
            doc.SplitSci1.SetIndicStyle(indicator, indicStyle);
            doc.SplitSci1.SetIndicFore(indicator, highlightColor);
            doc.SplitSci1.SetIndicSetAlpha(indicator, 40); // Improve contrast
            doc.SplitSci2.SetIndicStyle(indicator, indicStyle);
            doc.SplitSci2.SetIndicFore(indicator, highlightColor);
            doc.SplitSci2.SetIndicSetAlpha(indicator, 40); // Improve contrast
            CurrentIndicator = indicator;
            IndicatorValue = 1;
            IndicatorFillRange(start, length);
            StartStyling(es, mask);
        }
        public void AddHighlight(int indicStyle, int highlightColor, int start, int length)
        {
            AddHighlight(0, indicStyle, highlightColor, start, length);
        }
        public void AddHighlight(int highlightColor, int start, int length)
        {
            int indicStyle = (int)Enums.IndicatorStyle.RoundBox;
            AddHighlight(0, indicStyle, highlightColor, start, length);
        }

        /// <summary>
        /// Adds the specified highlights to the control
        /// </summary>
        public void AddHighlights(int indicator, int indicStyle, List<SearchMatch> matches, int highlightColor)
        {
            if (matches is null) return;
            var doc = DocumentManager.FindDocument(this);
            if (doc is null) return;
            foreach (SearchMatch match in matches)
            {
                int es = EndStyled;
                int mask = (1 << StyleBits) - 1;
                int start = MBSafePosition(match.Index);
                // Define indics in both controls...
                doc.SplitSci1.SetIndicStyle(indicator, (int)Enums.IndicatorStyle.RoundBox);
                doc.SplitSci1.SetIndicFore(indicator, highlightColor);
                doc.SplitSci1.SetIndicSetAlpha(indicator, 40); // Improve contrast
                doc.SplitSci2.SetIndicStyle(indicator, (int)Enums.IndicatorStyle.RoundBox);
                doc.SplitSci2.SetIndicFore(indicator, highlightColor);
                doc.SplitSci2.SetIndicSetAlpha(indicator, 40); // Improve contrast
                CurrentIndicator = indicator;
                IndicatorValue = 1;
                IndicatorFillRange(start, MBSafeTextLength(match.Value));
                StartStyling(es, mask);
            }
        }
        public void AddHighlights(int indicator, List<SearchMatch> matches, int highlightColor)
        {
            int indicStyle = (int)Enums.IndicatorStyle.RoundBox;
            AddHighlights(indicator, indicStyle, matches, highlightColor);
        }
        public void AddHighlights(List<SearchMatch> matches, int highlightColor)
        {
            int indicStyle = (int)Enums.IndicatorStyle.RoundBox;
            AddHighlights(0, indicStyle, matches, highlightColor);
        }

        /// <summary>
        /// Removes the specific highlight from the control
        /// </summary>
        public void RemoveHighlight(int indicator, int start, int length)
        {
            int es = EndStyled;
            int mask = (1 << StyleBits) - 1;
            CurrentIndicator = indicator;
            IndicatorClearRange(start, length);
            StartStyling(es, mask);
        }
        public void RemoveHighlight(int start, int length) => RemoveHighlight(0, start, length);

        /// <summary>
        /// Removes the specified highlights from the control
        /// </summary>
        public void RemoveHighlights(int indicator)
        {
            int es = EndStyled;
            int mask = (1 << StyleBits) - 1;
            CurrentIndicator = indicator;
            IndicatorClearRange(0, Length);
            StartStyling(es, mask);
        }
        public void RemoveHighlights() => RemoveHighlights(0);

        /// <summary>
        /// Move the current line (or selected lines) up
        /// </summary>
        public void MoveLineUp() => MoveLine(-1);

        /// <summary>
        /// Move the current line (or selected lines) down
        /// </summary>
        public void MoveLineDown() => MoveLine(1);

        /// <summary>
        /// Moves the current line(s) up or down
        /// </summary>
        public void MoveLine(int direction)
        {
            if (direction == 0) return;

            int anchorPosition = AnchorPosition;
            int currentPosition = CurrentPos;
            int selectionStart;
            int selectionEnd;
            if (anchorPosition <= currentPosition)
            {
                selectionStart = anchorPosition;
                selectionEnd = currentPosition;
            }
            else
            {
                selectionStart = currentPosition;
                selectionEnd = anchorPosition;
            }

            int startLine = LineFromPosition(selectionStart);
            int endLine = LineFromPosition(selectionEnd);
            if (startLine == endLine || selectionEnd != PositionFromLine(endLine))
            {
                // Either selection was within one line, or the selection was not made in whole lines.
                // Extend the end of the selection to the start of the next line.
                endLine++;
            }

            // Special handling for the last line case, but only if the last line doesn't end with a newline.
            string eolMarker = endLine + Math.Max(direction, 0) == LineCount ? NewLineMarker : null;

            if (direction > 0)
            {
                if (endLine + direction > LineCount) return;

                AnchorPosition = PositionFromLine(endLine);
                CurrentPos = PositionFromLine(endLine + direction);
            }
            else
            {
                if (startLine + direction < 0 || endLine > LineCount) return;

                AnchorPosition = PositionFromLine(startLine + direction);
                CurrentPos = PositionFromLine(startLine);
                startLine = endLine + direction;
            }

            string line = SelText;
            int length = line.Length;

            BeginUndoAction();
            Clear();
            if (eolMarker != null)
            {
                if (direction > 0)
                {
                    line += eolMarker;
                    length += eolMarker.Length;
                }
                else
                {
                    AppendText(eolMarker.Length, eolMarker);
                    line = line.TrimEnd(); // We don't want to decrease the length here, or the caret won't move down.
                }
            }
            InsertText(PositionFromLine(startLine), line);
            if (eolMarker != null && direction > 0) DeleteRange(TextLength - eolMarker.Length, eolMarker.Length);
            EndUndoAction();

            if (direction > 0)
            {
                AnchorPosition = anchorPosition + length;
                CurrentPos = currentPosition + length;
            }
            else
            {
                AnchorPosition = anchorPosition - length;
                CurrentPos = currentPosition - length;
            }
        }

        /// <summary>
        /// Reindents a block of pasted or moved lines to match the indentation of the destination pos
        /// </summary>
        public void ReindentLines(int startLine, int nLines)
        {
            if (nLines <= 0 || nLines > 200) return;
            string pasteStr = "";
            int commentIndent = -1;
            int pasteIndent = -1;
            int indent;
            int line;
            // find first non-comment line above the paste, so we can properly recolorize the affected area, even if it spans block comments
            for (line = startLine; line > 0;)
            {
                --line;
                if (!PositionIsOnComment(PositionFromLine(line)))
                {
                    break;
                }
            }
            Colourise(PositionFromLine(line), PositionFromLine(startLine + nLines));
            // Scan pasted lines to find their indentation
            for (line = startLine; line < startLine + nLines; ++line)
            {
                pasteStr = GetLine(line).Trim();
                if (pasteStr != "")
                {
                    indent = GetLineIndentation(line);
                    if (PositionIsOnComment(PositionFromLine(line) + indent))
                    {
                        // Indent of the first commented line
                        if (commentIndent < 0) commentIndent = indent;

                    }
                    else // We found code, so we won't be using comment-based indenting
                    {
                        commentIndent = -1;
                        pasteIndent = indent;
                        break;
                    }
                }
            }
            // Scan the destination to determine its indentation
            int destIndent = -1;
            for (line = startLine; --line >= 0;)
            {
                var destStr = GetLine(line).Trim();
                if (destStr != "")
                {
                    if (pasteIndent < 0)
                    {
                        // no code lines were found in the paste, so use the comment indentation
                        pasteIndent = commentIndent;
                        destIndent = GetLineIndentation(line);  // destination indent at any non-blank line
                        if (IsControlBlock(destStr) < 0) destIndent = GetLineIndentation(GetStartLine(line)) + Indent;
                        break;
                    }

                    if (!IsComment(destStr))
                    {
                        destIndent = GetLineIndentation(line); // destination indent at first code-line
                        if (IsControlBlock(destStr) < 0)
                        {
                            destIndent = GetLineIndentation(GetStartLine(line));
                            // Indent when we're pasting at the start of a control block (unless we're pasting an end block),
                            if (IsControlBlock(pasteStr) <= 0) destIndent += Indent;
                        }
                        else
                        {
                            // Outdent when we're pasting the end of a control block anywhere but after the start of a control block
                            if (IsControlBlock(pasteStr) > 0) destIndent -= Indent;
                        }
                        if (true) // TODO: Should test a config value for indenting after "case:" statements?
                        {
                            if (CodeEndsWith(destStr, ":") && !CodeEndsWith(FirstLine(pasteStr), ":"))
                            {
                                // If dest line ends with ":" and paste line doesn't
                                destIndent += Indent;
                            }
                            if (CodeEndsWith(FirstLine(pasteStr), ":") && CodeEndsWith(destStr, ";"))
                            {
                                // If paste line ends with ':' and dest line doesn't
                                destIndent -= Indent;
                            }
                        }
                        break;
                    }
                }
            }
            if (pasteIndent < 0) pasteIndent = 0;
            if (destIndent < 0) destIndent = 0;
            while (--nLines >= 0)
            {
                indent = GetLineIndentation(startLine);
                if (indent >= Indent || !PositionIsOnComment(PositionFromLine(startLine)))   // TODO: Are there any other lines besides comments that we want to keep in column 1? (preprocessor, ??)
                {                                                                            // Note that any changes here must also be matched when determining pasteIndent.
                    SetLineIndentation(startLine, destIndent + indent - pasteIndent);
                }
                ++startLine;
            }
        }

        /// <summary>
        /// Returns the starting line of a multi-line context (like function parameters, or long XML tags)
        /// </summary>
        public int GetStartLine(int line)
        {
            string str = GetLine(line);
            char marker = (ConfigurationLanguage == "xml" || ConfigurationLanguage == "html" || ConfigurationLanguage == "css") ? '>' : ')';
            int pos = str.LastIndexOf(marker);
            if (pos >= 0)
            {
                pos += PositionFromLine(line);
                pos = BraceMatch(pos);
                if (pos != -1 /*INVALID_POSITION*/)
                {
                    line = LineFromPosition(pos);
                }
            }
            return line;
        }

        /// <summary>
        /// Determines whether the input string starts with a comment
        /// </summary>
        public bool IsComment(string str)
        {
            string lineComment = Configuration.GetLanguage(ConfigurationLanguage).linecomment;
            string blockComment = Configuration.GetLanguage(ConfigurationLanguage).commentstart;
            var ret = ((!string.IsNullOrEmpty(lineComment) && str.StartsWithOrdinal(lineComment)) || (!string.IsNullOrEmpty(blockComment) && str.StartsWith(blockComment)));
            return ret;
        }

        /// <summary>
        /// Determines whether the input string is a start/end of a control block
        /// Returns -1:start, 1:end, 0:neither
        /// </summary>
        public int IsControlBlock(string str)
        {
            int ret = 0;
            str = str.Trim();
            if (str.Length == 0) return ret;
            // TODO: Is there a lexer test for "start/end of control block"?
            if (ConfigurationLanguage == "xml" || ConfigurationLanguage == "html" || ConfigurationLanguage == "css")
            {
                if (str.StartsWithOrdinal("</")) ret = 1;
                else if (!str.StartsWithOrdinal("<?") && !str.StartsWithOrdinal("<!") && !str.Contains("</") && !str.EndsWithOrdinal("/>") && str.EndsWith('>')) ret = -1;
            }
            else
            {
                if (str[0] == '}') ret = 1;
                else if (CodeEndsWith(str, "{")) ret = -1;
            }
            return ret;
        }

        /// <summary>
        /// Tests whether the code-portion of a string ends with a string value
        /// </summary>
        public bool CodeEndsWith(string str, string value)
        {
            bool ret = false;
            int startIndex = str.LastIndexOfOrdinal(value);
            if (startIndex >= 0)
            {
                string lineComment = Configuration.GetLanguage(ConfigurationLanguage).linecomment;
                if (!string.IsNullOrEmpty(lineComment))
                {
                    int slashIndex = str.LastIndexOfOrdinal(lineComment);
                    if (slashIndex >= startIndex) str = str.Substring(0, slashIndex);
                }
                if (str.Trim().EndsWithOrdinal(value)) ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Returns the first line of a string
        /// </summary>
        public string FirstLine(string str)
        {
            char newline = (EOLMode == 1) ? '\r' : '\n';
            int eol = str.IndexOf(newline);
            if (eol < 0) return str;
            return str.Substring(0, eol);
        }

        /// <summary>
        /// Select the word at the caret location.
        /// </summary>
        public void SelectWord()
        {
            int startPos = WordStartPosition(CurrentPos, true);
            int endPos = WordEndPosition(CurrentPos, true);
            SetSel(startPos, endPos);
        }

        /// <summary>
        /// Copy the word at the caret location.
        /// </summary>
        public void CopyWord()
        {
            SelectWord();
            Copy();
        }

        /// <summary>
        /// Replace-paste the word at the caret location.
        /// </summary>
        public void ReplaceWord()
        {
            BeginUndoAction();
            SelectWord();
            Paste();
            EndUndoAction();
        }

        /// <summary>
        /// Cut the selection, if selection empty cut the line with the caret
        /// </summary>
        public void CutAllowLine()
        {
            if (SelTextSize == 0) LineCut();
            else Cut();
        }

        /// <summary>
        /// Cut the selection, if selection empty cut the line with the caret
        /// </summary>
        public void CutAllowLineEx()
        {
            if (SelTextSize == 0 && GetLine(CurrentLine).Trim().Length > 0)
            {
                LineCut();
            }
            else Cut();
        }

        /// <summary>
        /// Copy the selection, if selection empty copy the line with the caret
        /// </summary>
        public void CopyAllowLineEx()
        {
            if (SelTextSize == 0 && GetLine(CurrentLine).Trim().Length > 0)
            {
                CopyAllowLine();
            }
            else Copy();
        }

        /// <summary>
        /// Cut the selection in RTF. If selection is empty, cut the line containing the caret.
        /// </summary>
        public void CutRTFAllowLine()
        {
            if (SelTextSize == 0)
            {
                int line = CurrentLine;
                AnchorPosition = PositionFromLine(line);
                CurrentPos = PositionFromLine(line + 1);
            }

            CutRTF();
        }

        /// <summary>
        /// Copy the selection in RTF. If selection is empty, copy the line containing the caret.
        /// </summary>
        public void CopyRTFAllowLine()
        {
            int start = SelectionStart;
            int end = SelectionEnd;

            if (start == end)
            {
                int line = CurrentLine;
                start = PositionFromLine(line);
                end = PositionFromLine(line + 1);
            }

            if (start < end)
            {
                CopyRTF(start, end);
            }
        }

        /// <summary>
        /// Cut the selection in RTF. If selection is empty and the current line is not empty, cut the line containing the caret.
        /// </summary>
        public void CutRTFAllowLineEx()
        {
            if (SelTextSize == 0 && GetLine(CurrentLine).Trim().Length > 0)
            {
                int line = CurrentLine;
                AnchorPosition = PositionFromLine(line);
                CurrentPos = PositionFromLine(line + 1);
            }

            CutRTF();
        }

        /// <summary>
        /// Copy the selection in RTF. If selection is empty and the current line is not empty, copy the line containing the caret.
        /// </summary>
        public void CopyRTFAllowLineEx()
        {
            int start = SelectionStart;
            int end = SelectionEnd;

            if (start == end && GetLine(CurrentLine).Trim().Length > 0)
            {
                int line = CurrentLine;
                start = PositionFromLine(line);
                end = PositionFromLine(line + 1);
            }

            if (start < end)
            {
                CopyRTF(start, end);
            }
        }

        /// <summary>
        /// Gets the word to the left of the cursor
        /// </summary>
        public string GetWordLeft(int position, bool skipWS)
        {
            var word = string.Empty;
            var lang = ConfigurationLanguage;
            var config = Configuration.GetLanguage(lang);
            var characterClass = config.characterclass.Characters;
            while (position >= 0)
            {
                var c = (char)CharAt(position);
                if (c <= ' ')
                {
                    if (!skipWS) break;
                }
                else if (!characterClass.Contains(c)) break;
                else
                {
                    word = c + word;
                    skipWS = false;
                }
                position--;
            }
            return word;
        }

        /// <summary>
        /// Gets the word to the right of the cursor
        /// </summary>
        public string GetWordRight(int position, bool skipWS)
        {
            var result = string.Empty;
            var characterClass = Configuration.GetLanguage(ConfigurationLanguage).characterclass.Characters;
            var endPosition = TextLength;
            while (position < endPosition)
            {
                var c = (char)CharAt(position);
                if (c <= ' ')
                {
                    if (!skipWS) break;
                }
                else if (!characterClass.Contains(c)) break;
                else
                {
                    result += c;
                    skipWS = false;
                }
                position++;
            }
            return result;
        }

        #endregion
    }
}