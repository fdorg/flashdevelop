/*
 * Control created to replace NodeTextBox to overcome some limitations:
 *   - There is no proper event to know when the editor is created, although there are some limited workarounds
 *   - There is no way to prevent Enter and Escape behaviour in a satisfactory way. TreeViewAdv should override some of the Key methods, and 
 *       check with the current EditableControl what to do. This would allow us to use Handled or IsInputKey. DGV works this way basically.
 *   - Due to how CallTip works, a simple TextBox isn't enough.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using PluginCore.Controls;

namespace FlashDebugger.Controls.DataTree.NodeControls
{
    public class NodeTextBoxEx : BaseTextControl
    {
        private const int MinTextBoxWidth = 30;

        protected override Size CalculateEditorSize(EditorContext context)
        {
            if (Parent.UseColumns)
                return context.Bounds.Size;
            else
            {
                Size size = GetLabelSize(context.CurrentNode, context.DrawContext, _label);
                int width = Math.Max(size.Width + Font.Height, MinTextBoxWidth); // reserve a place for new typed character
                return new Size(width, size.Height);
            }
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if (args.KeyCode == Keys.F2 && Parent.CurrentNode != null && EditEnabled)
            {
                args.Handled = true;
                BeginEdit();
            }
        }

        protected override Control CreateEditor(TreeNodeAdv node)
        {
            var textBox = CreateTextBox();
            OnEditorCreated(new ControlEventArgs(textBox));
            textBox.TextAlign = TextAlign;
            textBox.Text = GetLabel(node);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.TextChanged += EditorTextChanged;
            _label = textBox.Text;
            SetEditControlProperties(textBox, node);
            textBox.PreviewKeyDown += new PreviewKeyDownEventHandler(textBox_PreviewKeyDown);
            textBox.KeyPosted += textBox_KeyPosted;
            return textBox;
        }

        void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Escape:
                    e.IsInputKey = true;
                    break;
            }
        }

        void textBox_KeyPosted(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                EndEdit(false);
            else if (e.KeyCode == Keys.Enter)
                EndEdit(true);
        }

        protected override void DisposeEditor(Control editor)
        {
            var textBox = editor as TextBoxEx;
            textBox.TextChanged -= EditorTextChanged;
            textBox.KeyPosted -= textBox_KeyPosted;
            textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
        }

        private string _label;
        private void EditorTextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            _label = textBox.Text;
            Parent.UpdateEditorBounds();
        }

        protected virtual TextBoxEx CreateTextBox()
        {
            return new TextBoxEx();
        }

        protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
        {
            var label = (editor as TextBox).Text;
            string oldLabel = GetLabel(node);
            if (oldLabel != label)
            {
                SetLabel(node, label);
                OnLabelChanged(node.Tag, oldLabel, label);
            }
        }

        public override void Cut(Control control)
        {
            (control as TextBox).Cut();
        }

        public override void Copy(Control control)
        {
            (control as TextBox).Copy();
        }

        public override void Paste(Control control)
        {
            (control as TextBox).Paste();
        }

        public override void Delete(Control control)
        {
            var textBox = control as TextBox;
            int len = Math.Max(textBox.SelectionLength, 1);
            if (textBox.SelectionStart < textBox.Text.Length)
            {
                int start = textBox.SelectionStart;
                textBox.Text = textBox.Text.Remove(textBox.SelectionStart, len);
                textBox.SelectionStart = start;
            }
        }

        public event EventHandler<LabelEventArgs> LabelChanged;
        protected void OnLabelChanged(object subject, string oldLabel, string newLabel)
        {
            if (LabelChanged != null)
                LabelChanged(this, new LabelEventArgs(subject, oldLabel, newLabel));
        }

        public event ControlEventHandler EditorCreated;
        protected virtual void OnEditorCreated(ControlEventArgs e)
        {
            if (EditorCreated != null)
                EditorCreated(this, e);
        }
    }
}
