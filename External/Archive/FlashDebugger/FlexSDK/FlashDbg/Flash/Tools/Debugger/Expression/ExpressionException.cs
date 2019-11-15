﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Tools.Debugger.Expression
{
	public abstract class ExpressionException : Exception
	{
		public abstract String getLocalizedMessage();

		public ExpressionException()
			: base()
		{
		}

        public ExpressionException(String s)
			: base(s)
		{
		}

		public ExpressionException(System.Object o)
			: base(o.ToString())
		{
		}
	}
}
