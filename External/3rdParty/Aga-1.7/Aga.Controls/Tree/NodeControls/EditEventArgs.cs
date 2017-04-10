﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Aga.Controls.Tree.NodeControls
{
	public class EditEventArgs : NodeEventArgs
	{
		private Control _control;
		public Control Control
		{
			get { return _control; }
		}

		public EditEventArgs(TreeNodeAdv node, Control control)
			: base(node)
		{
			_control = control;
		}
	}
}
