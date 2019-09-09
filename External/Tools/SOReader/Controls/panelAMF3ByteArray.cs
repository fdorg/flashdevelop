// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SOReader.Sol.AMF.DataType;

namespace SharedObjectReader.Controls
{
    public partial class panelAMF3ByteArray : UserControl, IAMFDisplayPanel
    {
        public panelAMF3ByteArray()
        {
            InitializeComponent();
        }

        #region IAMFDisplayPanel Members

        public void Populate(string name, SOReader.Sol.AMF.DataType.IAMFBase element)
        {
            prop_name.Text = name;
            prop_value.Text = BitConverter.ToString((byte[])element.Source);
        }

        #endregion
    }
}
