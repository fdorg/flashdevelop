// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Aga.Controls.Tree.NodeControls
{
	public class DrawEventArgs : NodeEventArgs
	{
		private DrawContext _context;
		public DrawContext Context
		{
			get { return _context; }
		}

		private Brush _textBrush;
		[Obsolete("Use TextColor")]
		public Brush TextBrush
		{
			get { return _textBrush; }
			set { _textBrush = value; }
		}

		private Brush _backgroundBrush;
		public Brush BackgroundBrush
		{
            get { return _backgroundBrush; }
			set { _backgroundBrush = value; }
		}

		private Font _font;
		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private Color _textColor;
		public Color TextColor
		{
			get { return _textColor; }
			set { _textColor = value; }
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}


		private EditableControl _control;
		public EditableControl Control
		{
			get { return _control; }
		}

        private string _highLightToken;

        public string HighLightToken
        {
            get { return _highLightToken; }
            set { _highLightToken = value; }
        }

        private Color _highLightColor = Color.Yellow;
        public Color HighLightColor
        {
            get { return _highLightColor; }
            set { _highLightColor = value; }
        }
        

		public DrawEventArgs(TreeNodeAdv node, EditableControl control, DrawContext context, string text)
			: base(node)
		{
			_control = control;
			_context = context;
			_text = text;
		}
	}
}
