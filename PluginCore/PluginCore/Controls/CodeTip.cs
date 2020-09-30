using PluginCore.Helpers;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Configuration;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public class CodeTip
    {
        readonly ScintillaControl editor;
        readonly Panel codeTip;
        int columnWidth;
        int rowHeight;

        public CodeTip(IMainForm mainForm)
        {
            editor = new ScintillaControl {Enabled = false, IsUseTabs = false};
            codeTip = new Panel {BorderStyle = BorderStyle.FixedSingle, Visible = false};
            codeTip.Controls.Add(editor);
            mainForm.Controls.Add(codeTip);
        }

        public void Show(ScintillaControl sci, int position, string code)
        {
            ConfigureEditor(sci, code);
            ReduceIndentation();
            SizeEditor();
            PositionEditor(sci, position);

            codeTip.Visible = true;
            codeTip.BringToFront();
        }

        public void Hide() => codeTip.Visible = false;

        void ConfigureEditor(ScintillaControl sci, string code)
        {
            editor.SetMarginWidthN(1, 0);

            editor.ConfigurationLanguage = sci.ConfigurationLanguage;

            editor.Text = "  ";
            columnWidth = editor.PointXFromPosition(1) - editor.PointXFromPosition(0);
            rowHeight = editor.TextHeight(0);

            editor.TabWidth = sci.TabWidth;
            editor.Text = code;

            editor.SetProperty("lexer.cpp.track.preprocessor", "0");

            var language = GetLanguage(editor.ConfigurationLanguage);
            var defaultStyle = language?.usestyles.FirstOrDefault(useStyle => useStyle.name == "default");
            if (defaultStyle is null) return;
            codeTip.BackColor = DataConverter.BGRToColor(defaultStyle.BackgroundColor);
        }

        void ReduceIndentation()
        {
            editor.StripTrailingSpaces();

            var minIndentation = int.MaxValue;
            var lineCount = editor.LineCount;
            for (var index = 0; index < lineCount; index++)
            {
                var indentation = editor.GetLineIndentation(index);
                if (indentation == 0) continue;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            if (minIndentation < int.MaxValue)
            {
                for (var index = 0; index < lineCount; index++)
                {
                    var indentation = editor.GetLineIndentation(index);
                    if (indentation == 0) continue;
                    editor.SetLineIndentation(index, indentation - minIndentation);
                }
            }
        }

        void SizeEditor()
        {
            var lineCount = editor.LineCount;
            var targetHeight = lineCount * rowHeight;
            int targetWidth = 0;
            for (var index = 0; index < lineCount; index++)
                targetWidth = Math.Max(targetWidth, editor.LineLength(index) * columnWidth);

            var editorHeight = Math.Min(ScaleHelper.Scale(500), targetHeight);
            var editorWidth = Math.Min(ScaleHelper.Scale(800), targetWidth);

            var padding = ScaleHelper.Scale(8);
            codeTip.Height = editorHeight + (padding * 2);
            codeTip.Width = editorWidth + (padding * 2);

            editor.Top = padding;
            editor.Left = padding;
            editor.Height = editorHeight;
            editor.Width = editorWidth;

            editor.IsHScrollBar = false; // editorWidth < targetWidth;
            editor.IsVScrollBar = false; // editorHeight < targetHeight;
        }

        void PositionEditor(ScintillaControl sci, int position)
        {
            var p = new Point(sci.PointXFromPosition(position), sci.PointYFromPosition(position));
            var mainForm = ((Form)PluginBase.MainForm);
            p = mainForm.PointToClient(sci.PointToScreen(p));

            if (p.Y > codeTip.Height) codeTip.Top = p.Y - codeTip.Height;
            else codeTip.Top = p.Y + rowHeight;

            if (p.X + codeTip.Width < mainForm.ClientSize.Width) codeTip.Left = p.X;
            else codeTip.Left = mainForm.ClientSize.Width - codeTip.Width;
        }

        static Language GetLanguage(string name) => PluginBase.MainForm?.SciConfig?.GetLanguage(name);
    }
}