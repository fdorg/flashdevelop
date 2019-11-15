// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore.Localization;
using System.Drawing;

namespace AS3Context.Controls
{
    class ProfilerMemView
    {
        readonly ToolStripLabel memLabel;
        readonly Label statsLabel;
        readonly ComboBox scaleCombo;

        public MemGraph Graph { get; }

        public ProfilerMemView(ToolStripLabel label, Label stats, ComboBox scale, Control memoryPage)
        {
            Graph = new MemGraph {Dock = DockStyle.Fill};
            memoryPage.Controls.Add(Graph);
            Graph.BringToFront();

            memLabel = label;
            statsLabel = stats;

            scaleCombo = scale;
            scaleCombo.SelectedIndex = scaleCombo.Items.Count - 1;
            scaleCombo.SelectedIndexChanged += scaleCombo_SelectedIndexChanged;
            Graph.TimeScale = scaleCombo.SelectedIndex + 1;

            Clear();
        }

        void scaleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Graph.TimeScale = scaleCombo.SelectedIndex + 1;
            Graph.Invalidate();
        }

        public void Clear()
        {
            Graph.Values = new List<float>();
            Graph.MaxValue = 1;
            memLabel.Text = string.Format(TextHelper.GetString("Label.MemoryDisplay"), FormatMemory(0), FormatMemory(0));
            statsLabel.Text = string.Format(TextHelper.GetString("Label.MemoryStats"), "\n", FormatMemory(0), FormatMemory(0));
        }

        /// <summary>
        /// Memory stats display
        /// </summary>
        /// <param name="info"></param>
        public void UpdateStats(string[] info)
        {
            int.TryParse(info[1], out var mem);
            Graph.Values.Add(mem);
            if (mem > Graph.MaxValue) Graph.MaxValue = mem;
            string raw = TextHelper.GetString("Label.MemoryDisplay");
            memLabel.Text = string.Format(raw, FormatMemory(mem), FormatMemory((int)Graph.MaxValue));
            raw = TextHelper.GetString("Label.MemoryStats");
            statsLabel.Text = string.Format(raw, "\n", FormatMemory(mem), FormatMemory((int)Graph.MaxValue));
            Graph.Invalidate();
        }

        string FormatMemory(int mem)
        {
            double m = mem / 1024.0;
            return (Math.Round(m * 10.0) / 10.0).ToString("N0");
        }
    }

    class MemGraph : Control
    {
        public List<float> Values = new List<float>();
        public float MaxValue = 1;
        public int TimeScale = 4;
        Color back;
        Color rect;
        Color norm;
        Color peak;
        Color cur;

        public MemGraph()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateColors();
        }

        public void UpdateColors()
        {
            rect = PluginCore.PluginBase.MainForm.GetThemeColor("MemGraph.ForeColor", Color.Gray);
            back = PluginCore.PluginBase.MainForm.GetThemeColor("MemGraph.BackColor", Color.White);
            norm = PluginCore.PluginBase.MainForm.GetThemeColor("MemGraph.NormalColor", Color.LightGray);
            peak = PluginCore.PluginBase.MainForm.GetThemeColor("MemGraph.PeakColor", Color.Red);
            cur = PluginCore.PluginBase.MainForm.GetThemeColor("MemGraph.CurrentColor", Color.Blue);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            Rectangle r = pe.ClipRectangle;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(new SolidBrush(back), r);
            g.DrawRectangle(new Pen(rect), 0, 0, Width - 1, Height - 1);

            if (Width < 8 || Height < 8) return;

            int n = Values.Count;
            if (n < 2) 
                return;

            int diff = Math.Min(Width / TimeScale, n);
            int x0 = Width - diff * TimeScale;
            int i = Math.Max(0, n - diff);
            float h = Height;

            // peak
            var line = new Pen(norm, 1);
            float step = 25000000f;
            while (step * 4 < MaxValue) step *= 2;
            float top = step;
            float y;
            while (top < MaxValue)
            {
                y = (float)Math.Round(h * (1f - 0.9f * top / MaxValue));
                g.DrawLine(line, 1f, y, Width - 2, y);
                top += step;
            }
            line = new Pen(peak, 1);
            y = (float)Math.Round(h * 0.1f);
            g.DrawLine(line, 0f, y, Width - 1, y);

            // graph
            var points = new List<PointF>();
            while (i < n)
            {
                points.Add(new PointF(x0, h * (1f - 0.9f * Values[i] / MaxValue)));
                i++;
                x0 += TimeScale;
            }
            line = new Pen(cur, 2) {LineJoin = System.Drawing.Drawing2D.LineJoin.Round};
            g.DrawLines(line, points.ToArray());
        }

    }

}
