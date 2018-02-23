using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ScintillaNet;

namespace HaXeContext.Completion
{
    class CodeComplete : ASComplete
    {
        public override bool IsRegexStyle(ScintillaControl sci, int position)
        {
            var result = base.IsRegexStyle(sci, position);
            if (result) return true;
            return sci.BaseStyleAt(position) == 10 && sci.CharAt(position) == '~' && sci.CharAt(position + 1) == '/';
        }

        /// <summary>
        /// Returns whether or not position is inside of an expression block in String interpolation ('${expr}')
        /// </summary>
        public override bool IsStringInterpolationStyle(ScintillaControl sci, int position)
        {
            if (!ASContext.Context.Features.hasStringInterpolation) return false;
            var stringChar = sci.GetStringType(position - 1);
            if (ASContext.Context.Features.stringInterpolationQuotes.Contains(stringChar))
            {
                char current = (char)sci.CharAt(position);

                for (int i = position - 1; i >= 0; i--)
                {
                    var next = current;
                    current = (char)sci.CharAt(i);

                    if (current == stringChar)
                    {
                        if (!IsEscapedCharacter(sci, i)) break;
                    }
                    else if (current == '$')
                    {
                        if (next == '{' && !IsEscapedCharacter(sci, i, '$')) return true;
                    }
                    else if (current == '}')
                    {
                        i = sci.BraceMatch(i);
                        current = (char)sci.CharAt(i);
                        if (i > 0 && current == '{' && sci.CharAt(i - 1) == '$') break;
                    }
                }
            }
            return false;
        }

        /// <inheritdoc />
        protected override bool HandleWhiteSpaceCompletion(ScintillaControl sci, int position, string wordLeft, bool autoHide)
        {
            var currentClass = ASContext.Context.CurrentClass;
            if (currentClass.Flags.HasFlag(FlagType.Abstract))
            {
                switch (wordLeft)
                {
                    case "from":
                    case "to":
                        return PositionIsOutsideBody(sci, position, currentClass) && HandleNewCompletion(sci, string.Empty, autoHide, wordLeft);
                }
            }
            return base.HandleWhiteSpaceCompletion(sci, position, wordLeft, autoHide);
        }

        /// <summary>
        /// Returns true if position is outside body of class or member
        /// </summary>
        internal bool PositionIsOutsideBody(ScintillaControl sci, int position, MemberModel member)
        {
            if (member == ClassModel.VoidClass) return false;
            if (sci.LineEndPosition(member.LineTo) <= position) return false;
            var groupCount = 0;
            var positionFrom = sci.PositionFromLine(member.LineFrom);
            for (var i = positionFrom; i < position; i++)
            {
                if (sci.PositionIsOnComment(position)) continue;
                var c = (char)sci.CharAt(i);
                if (c == '(' || c == '<') groupCount++;
                else if (c == ')' || (c == '>' && sci.CharAt(i - 1) != '-')) groupCount--;
                else if (c == '{' && groupCount == 0)
                {
                    if (i <= position) return false;
                }
            }
            return true;
        }
    }
}
