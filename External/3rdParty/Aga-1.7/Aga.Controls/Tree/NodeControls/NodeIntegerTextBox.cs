// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Aga.Controls.Tree.NodeControls
{

	public class NodeIntegerTextBox : NodeTextBox
	{
		private bool _allowNegativeSign = true;
		[DefaultValue(true)]
		public bool AllowNegativeSign
		{
			get { return _allowNegativeSign; }
			set { _allowNegativeSign = value; }
		}

		public NodeIntegerTextBox()
		{
		}

		protected override TextBox CreateTextBox()
		{
			NumericTextBox textBox = new NumericTextBox();
			textBox.AllowDecimalSeparator = false;
			textBox.AllowNegativeSign = AllowNegativeSign;
			return textBox;
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			SetValue(node, (editor as NumericTextBox).IntValue);
		}
	}
}
