using System.Drawing;
using System.Drawing.Printing;
using PluginCore;
using PluginCore.Localization;

namespace FlashDevelop.Managers
{
    class PrintingManager
    {
        public static int PrintPageNumber;
        public static int PrintPageLastChar;
        public static PrinterSettings PrinterSettings;

        /// <summary>
        /// Gets the global printer settings
        /// </summary> 
        public static PrinterSettings GetPrinterSettings() => PrinterSettings ??= new PrinterSettings();

        /// <summary>
        /// Creates a new print document
        /// </summary> 
        public static PrintDocument CreatePrintDocument()
        {
            PrintPageNumber = 0;
            PrintPageLastChar = 0;
            PrintDocument printDocument = new PrintDocument();
            printDocument.DocumentName = PluginBase.MainForm.CurrentDocument.Text;
            printDocument.PrintPage += OnPrintDocumentPrintPage;
            return printDocument;
        }

        /// <summary>
        /// Handles the PrintPage event
        /// </summary>
        public static void OnPrintDocumentPrintPage(object sender, PrintPageEventArgs e)
        {
            PrintPageNumber++;
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var page = TextHelper.GetString("Info.PrintFooterPage");
            var footer = page + PrintPageNumber + " - " + sci.FileName;
            PrintPageLastChar = sci.FormatRange(false, e, PrintPageLastChar, sci.Length);
            e.Graphics.DrawLine(new Pen(Color.Black), 35, e.PageBounds.Height - 60, e.PageBounds.Width - 35, e.PageBounds.Height - 60);
            e.Graphics.DrawString(footer, new Font("Arial", 7), Brushes.Black, 35, e.PageBounds.Height - 55, StringFormat.GenericDefault);
            if (PrintPageLastChar < sci.Length) e.HasMorePages = true;
            else
            {
                e.HasMorePages = false;
                sci.FormatRangeDone();
                PrintPageLastChar = 0;
                PrintPageNumber = 0;
            }
        }
    }
}