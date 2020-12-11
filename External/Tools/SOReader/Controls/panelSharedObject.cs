// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SOReader.Sol.AMF.DataType;
using SOReader.Sol;

namespace SharedObjectReader.Controls
{
    public partial class panelSharedObject : UserControl, IAMFDisplayPanel
    {
        public panelSharedObject()
        {
            InitializeComponent();
        }

        #region IAMFDisplayPanel Members

        public void Populate(string name, SOReader.Sol.AMF.DataType.IAMFBase element)
        {
        }

        public void Populate(string name, SharedObject so)
        {
            this.so_name.Text = so.Name;
            this.so_size.Text = so.FileSize.ToString();
            this.so_encoding.Text = so.AMFEncoding.ToString();
            this.rawView.Text = so.ToString();
        }

        #endregion
    }
}
