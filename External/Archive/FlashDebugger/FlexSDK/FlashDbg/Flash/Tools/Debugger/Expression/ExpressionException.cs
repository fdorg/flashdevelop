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
