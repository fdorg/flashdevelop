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
    public partial class panelAMF0Boolean : UserControl, IAMFDisplayPanel
    {
        public panelAMF0Boolean()
        {
            InitializeComponent();
        }

        #region IAMFDisplayPanel Members

        public void Populate(string name, SOReader.Sol.AMF.DataType.IAMFBase element)
        {
            prop_name.Text = name;
            if ((bool)element.Source == true)
            {
                radioButton1.Checked = true;
            }
            else
            {
                radioButton2.Checked = true;
            }
        }

        #endregion
    }
}
