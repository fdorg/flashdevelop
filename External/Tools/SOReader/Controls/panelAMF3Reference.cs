using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SOReader.Sol.AMF.DataType;
using System.Diagnostics;
using SOReader.Sol;

namespace SharedObjectReader.Controls
{
    public partial class panelAMF3Reference : UserControl, IAMFDisplayPanel
    {
        public event EventHandler<EventArgs> Reference;

        private int objectReference;
        private IAMFBase element;

        public IAMFBase Element
        {
            get { return this.element; }
        }

        public int ObjectReference
        {
            get { return this.objectReference; }
        }

        public panelAMF3Reference()
        {
            InitializeComponent();
            this.linkLabel1.Click += new EventHandler(linkLabel1_Click);
        }

        void linkLabel1_Click(object sender, EventArgs e)
        {
            EventHandler<EventArgs> evt = Reference;
            if (evt != null)
            {
                evt(this, new EventArgs());
            }
        }

        #region IAMFDisplayPanel Members

        public void Populate(string name, SOReader.Sol.AMF.DataType.IAMFBase item)
        {
            prop_name.Text = name;
            if (item.AmfType == AMFType.AMF3_REFERENCE)
            {
                linkLabel1.Text = "Object reference #" + ((AMF3Reference)item).Reference;
                linkLabel1.Tag = ((AMF3Reference)item).Reference;
                objectReference = ((AMF3Reference)item).Reference;
            }
            else if (item.AmfType == AMFType.AMF0_REFERENCE)
            {
                linkLabel1.Tag = ((AMF0Reference)item).Reference;
                linkLabel1.Text = "Object reference #" + ((AMF0Reference)item).Reference;
                objectReference = ((AMF0Reference)item).Reference;
            }
            else if (item.AmfType == AMFType.AMF0_LOST_REFERENCE)
            {
                linkLabel1.Enabled = false;
                linkLabel1.Text = "Invalid reference";
                linkLabel1.Tag = -1;
                objectReference = -1; 
            }
            element = item;
        }

        #endregion
    }
}
