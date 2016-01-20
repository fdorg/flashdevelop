using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Completion;
using PluginCore;
using PluginCore.Controls;

namespace ASCompletion.Helpers
{

    /**
     * Helper class to overcome some ASComplete design choices:
     *  - Some methods that may be helpful are either private or directly call the default CompletionList instead of returning results.
     *  - Being a fully static class complicates some reuse.
     *  - It needs a Scintilla control to work.
     *  
     * Making the current ASComplete.CompletionList to be interchangeable is the fastest and easiest way to achieve what we want.
     * 
     * Another rather particular case is that of Haxe. To use the compiler output or the completion server we use a temporal file.
     */
    public class ASCompletionListBackend : IDisposable, IEventHandler
    {

        // Current code file state
        private ScintillaNet.ScintillaControl sci;
        private string currentLine;
        private BackendFileInfo lastFileInfo;

        // ASCompletion state
        private ASCompletion.Context.IASContext context;
        private string contextFile;
        private int contextLine;
        private int memberPosition;

        // Haxe backend file
        private string haxeTmpFile;

        private CompletionListControl completionList;
        private IBackendFileGetter backendFileGetter;

        public ASCompletionListBackend(CompletionListControl completionList, IBackendFileGetter backendFileGetter)
        {
            if (completionList == null)
                throw new ArgumentNullException("completionList");

            if (backendFileGetter == null)
                throw new ArgumentNullException("backendFileGetter");

            this.completionList = completionList;
            this.backendFileGetter = backendFileGetter;

            this.completionList.CallTip.OnShowing += CallTip_OnShowing;
            this.completionList.CallTip.OnUpdateCallTip += CallTip_OnUpdateCallTip;
            this.completionList.Host.KeyPress += CompletionListHost_KeyPress;
        }

        public void ShowAutoCompletionList()
        {
            if (!UpdateCompletionBackend())
            {
                CleanBackend();
                return;
            }

            if (haxeTmpFile != null)
                PluginCore.Helpers.FileHelper.WriteFile(haxeTmpFile, sci.Text, sci.Encoding, sci.SaveBOM);

            ASComplete.OnChar(sci, '.', true);
        }

        public void ShowFunctionDetails()
        {
            if (!UpdateCompletionBackend())
            {
                CleanBackend();
                return;
            }

            if (haxeTmpFile != null)
                PluginCore.Helpers.FileHelper.WriteFile(haxeTmpFile, sci.Text, sci.Encoding, sci.SaveBOM);

            ASComplete.HandleFunctionCompletion(sci, true);
        }

        public bool SetCompletionBackend(BackendFileInfo file, string expression)
        {
            var info = backendFileGetter.GetFileContent(file);
            if (info == null || info.CodePage == -1)
                return false;

            if (sci != null && sci.ConfigurationLanguage == "haxe" && haxeTmpFile != null)
            {
                PluginCore.Managers.EventManager.RemoveEventHandler(this);
                if (File.Exists(haxeTmpFile))
                    File.Delete(haxeTmpFile);
                haxeTmpFile = null;
            }

            sci = new ScintillaNet.ScintillaControl();
            sci.Text = info.Contents;
            sci.CodePage = info.CodePage;
            sci.Encoding = Encoding.GetEncoding(info.CodePage);
            sci.ConfigurationLanguage = PluginBase.CurrentProject.Language;

            if (sci.ConfigurationLanguage == "haxe")
            {
                string tmpFile = Path.Combine(Path.GetDirectoryName(file.File),
                                              Path.GetFileNameWithoutExtension(file.File) + "___" +
                                              Path.GetExtension(file.File));
                if (!File.Exists(tmpFile))
                {
                    haxeTmpFile = tmpFile;
                    sci.FileName = haxeTmpFile;
                    PluginCore.Managers.EventManager.AddEventHandler(this, EventType.FileSaving);
                    ASCompletion.Context.ASContext.CurSciControl = sci;
                }
            }
            
            currentLine = sci.GetLine(file.Line);
            sci.CurrentPos = sci.PositionFromLine(file.Line);
            sci.SetSel(sci.CurrentPos, sci.CurrentPos);
            sci.ReplaceSel(expression);

            context = ASCompletion.Context.ASContext.Context;
            contextFile = context.CurrentFile;
            contextLine = context.CurrentLine;
            var newContext = ASCompletion.Context.ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            newContext.CurrentFile = file.File;
            newContext.CurrentLine = sci.CurrentLine;
            ASCompletion.Context.ASContext.Context = newContext;

            var language = ScintillaNet.ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage);
            if (language != null)   // Should we provide some custom string otherwise?
                completionList.CharacterClass = language.characterclass.Characters;

            completionList.Host.LostFocus += CompletionListHost_LostFocus;
            completionList.OnHidden += CompletionList_Hidden;

            ASComplete.CompletionList = completionList;

            lastFileInfo = file;

            return true;
        }

        public bool UpdateCompletionBackend()
        {
            BackendFileInfo newFileInfo;
            string expression;

            if (!backendFileGetter.GetFileInfo(out newFileInfo))
                return false;

            expression = backendFileGetter.GetExpression();
            if (expression == null)
                return false;

            if (sci != null && lastFileInfo == newFileInfo)
            {
                sci.CurrentPos = sci.PositionFromLine(lastFileInfo.Line);
                sci.SetSel(sci.CurrentPos, sci.CurrentPos + sci.LineLength(lastFileInfo.Line));
                sci.ReplaceSel(currentLine);
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                sci.ReplaceSel(expression);
                
                return true;
            }

            return SetCompletionBackend(newFileInfo, expression);
        }

        private void CompletionList_Hidden(object sender, EventArgs e)
        {
            if (completionList.Host.Owner.ContainsFocus)
                return;

            CleanBackend();
        }

        private void CompletionListHost_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (char.IsControl(c))
                return;

            BackendFileInfo file;
            if (!backendFileGetter.GetFileInfo(out file))
                return;

            string expression = backendFileGetter.GetExpression();

            if (expression == null)
                return;

            bool active = completionList.Active;
            // Hacky... the current CompletionListControl implementation relies on the OnChar Scintilla event, which happens after the KeyPress event
            // We either create an OnChar event in ICompletionListHost and implement it, or change how CompletionListControl works
            e.Handled = true;
            completionList.Host.SelectedText = new string(e.KeyChar, 1);
            completionList.Host.SelectionStart++;
            bool charHandled = active && completionList.OnChar(c);
            if (!UpdateCompletionBackend())
            {
                CleanBackend();
                return;
            }

            if (!charHandled)
            {
                if (".,()".IndexOf(c) > -1 && haxeTmpFile != null)
                    PluginCore.Helpers.FileHelper.WriteFile(haxeTmpFile, sci.Text, sci.Encoding, sci.SaveBOM);
                ASComplete.OnChar(sci, c, true);
            }
        }

        private void CompletionListHost_LostFocus(object sender, EventArgs e)
        {
            if (completionList.Active)
                return;

            CleanBackend();
        }

        private void CallTip_OnShowing(object sender, CancelEventArgs e)
        {
            var callTip = (MethodCallTip)sender;

            if (callTip.MemberPosition == memberPosition) return;

            e.Cancel = true;

            memberPosition = completionList.Host.SelectionStart - (sci.CurrentPos - callTip.MemberPosition);

            completionList.Host.Owner.BeginInvoke(new Action(() =>
            {
                int hlsStart = completionList.CallTip.CurrentHLStart;
                int hlsEnd = completionList.CallTip.CurrentHLEnd;
                callTip.CallTipShow(memberPosition, callTip.RawText);
                callTip.CallTipSetHlt(hlsStart, hlsEnd);
            }));
        }

        private void CallTip_OnUpdateCallTip(Control owner, int position)
        {
            if (!UpdateCompletionBackend())
            {
                completionList.CallTip.Hide();
                CleanBackend();

                return;
            }

            if (!ASComplete.HandleFunctionCompletion(sci, false, true))
                completionList.CallTip.Hide();
            else if (completionList.CallTip.MemberPosition != memberPosition)
            {
                int hlsStart = completionList.CallTip.CurrentHLStart;
                int hlsEnd = completionList.CallTip.CurrentHLEnd;
                memberPosition = completionList.Host.SelectionStart - (sci.CurrentPos - completionList.CallTip.MemberPosition);
                completionList.CallTip.CallTipShow(memberPosition, completionList.CallTip.RawText);
                completionList.CallTip.CallTipSetHlt(hlsStart, hlsEnd, true);
            }
        }

        private void CleanBackend()
        {
            completionList.Host.LostFocus -= CompletionListHost_LostFocus;
            completionList.OnHidden -= CompletionList_Hidden;
            if (context != null)
            {
                ASCompletion.Context.ASContext.Context = context;
                context.CurrentFile = contextFile;
                context.CurrentLine = contextLine;
                context = null;
            }
            ASComplete.CompletionList = UITools.CompletionList;
            lastFileInfo = default(BackendFileInfo);
            if (sci != null)
            {
                sci.Dispose();
                sci = null;
            }
            if (haxeTmpFile != null)
            {
                PluginCore.Managers.EventManager.RemoveEventHandler(this);
                if (File.Exists(haxeTmpFile))
                    File.Delete(haxeTmpFile);
                haxeTmpFile = null;
                ASCompletion.Context.ASContext.CurSciControl = null;
            }
        }

        public void Dispose()
        {
            if (sci != null)
            {
                sci.Dispose();
                sci = null;
            }

            if (context != null)
            {
                ASCompletion.Context.ASContext.Context = context;
                context.CurrentFile = contextFile;
                context.CurrentLine = contextLine;
                context = null;
            }

            if (completionList != null)
            {
                completionList.CallTip.OnShowing -= CallTip_OnShowing;
                completionList.CallTip.OnUpdateCallTip -= CallTip_OnUpdateCallTip;
                completionList.Host.KeyPress -= CompletionListHost_KeyPress;
                completionList.Host.LostFocus -= CompletionListHost_LostFocus;
                completionList.OnHidden -= CompletionList_Hidden;

                completionList.Hide();
                completionList = null;

                ASComplete.CompletionList = UITools.CompletionList;
            }

            if (haxeTmpFile != null)
            {
                PluginCore.Managers.EventManager.RemoveEventHandler(this);
                if (File.Exists(haxeTmpFile))
                    File.Delete(haxeTmpFile);
                haxeTmpFile = null;
                ASCompletion.Context.ASContext.CurSciControl = null;
            }

            backendFileGetter = null;
        }

        void IEventHandler.HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            // Note: With this we avoid some unwanted saves on Haxe projects, but we also disable wanted ones through Ctrl+S if we're
            //both "running" this class, and placed in MainForm.
            TextEvent se = (TextEvent)e;
            if (se.Value != lastFileInfo.File) return;
            se.Handled = true;
            try
            {
                sci.FileName = haxeTmpFile;
                PluginCore.Helpers.FileHelper.WriteFile(haxeTmpFile, sci.Text, sci.Encoding, sci.SaveBOM);
            }
            catch (Exception)
            {
            }
        }

        #region Used Types

        public struct BackendFileInfo
        {
            public string File { get; set; }
            public int Line { get; set; }
            public DateTime LastUpdate { get; set; }

            public static bool operator ==(BackendFileInfo a, BackendFileInfo b)
            {
                return a.File == b.File && a.Line == b.Line && a.LastUpdate == b.LastUpdate;
            }

            public static bool operator !=(BackendFileInfo a, BackendFileInfo b)
            {
                return a.File != b.File || a.Line != b.Line || a.LastUpdate != b.LastUpdate;
            }

            public bool Equals(BackendFileInfo other)
            {
                return this == other;
            }

            public override bool Equals(object obj)
            {
                return obj is BackendFileInfo && this == (BackendFileInfo)obj;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (File != null ? File.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Line;
                    hashCode = (hashCode * 397) ^ LastUpdate.GetHashCode();
                    return hashCode;
                }
            }

        }

        public interface IBackendFileGetter
        {

            bool GetFileInfo(out BackendFileInfo file);
            string GetExpression();
            PluginCore.Helpers.EncodingFileInfo GetFileContent(BackendFileInfo file);

        }

    }

    #endregion
}
