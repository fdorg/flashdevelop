using System;
using System.Collections.Generic;
using System.Text;

namespace FlashDebugger {
	public static class Strings {


		public static string Before(string text, string find, int startAt = 0, bool returnAll = false, bool forward = true) {
			if (text == null) { return returnAll ? text : ""; }
			int idx;
			if (forward) {
				idx = text.IndexOf(find, startAt, StringComparison.Ordinal);
			} else {
				idx = text.LastIndexOf(find, text.Length - startAt, StringComparison.Ordinal);
			}
			if (idx == -1) { return returnAll ? text : ""; }
			return text.Substring(0, idx);

		}
		public static string After(string text, string find, int startAt = 0, bool returnAll = false, bool forward = true) {
			if (text == null) { return returnAll ? text : ""; }
			int idx;
			if (!forward) {
				idx = text.LastIndexOf(find, text.Length - startAt, StringComparison.Ordinal);
			} else {
				idx = text.IndexOf(find, startAt, StringComparison.Ordinal);
			}
			if (idx == -1) { return returnAll ? text : ""; }
			idx += find.Length;
			return text.Substring(idx);
		}
		public static string AfterLast(string text, string find, bool returnAll = false) {
			int idx = text.LastIndexOf(find, StringComparison.Ordinal);
			if (idx == -1) {
				return returnAll ? text : "";
			}
			idx += find.Length;
			return text.Substring(idx);
		}

	}
}
