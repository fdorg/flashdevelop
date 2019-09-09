// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockContentEventArgs : EventArgs
    {
        private readonly IDockContent m_content;

        public DockContentEventArgs(IDockContent content)
        {
            m_content = content;
        }

        public IDockContent Content => m_content;
    }
}
