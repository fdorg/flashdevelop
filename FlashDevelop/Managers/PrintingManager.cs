// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            printDocument.DocumentName = Globals.CurrentDocument.Text;
            printDocument.PrintPage += OnPrintDocumentPrintPage;
            return printDocument;
        }

        /// <summary>
        /// Handles the PrintPage event
        /// </summary>
        public static void OnPrintDocumentPrintPage(object sender, PrintPageEventArgs e)
        {
            PrintPageNumber++;
            ITabbedDocument document = Globals.CurrentDocument;
            string page = TextHelper.GetString("Info.PrintFooterPage");
            string footer = page + PrintPageNumber + " - " + document.FileName;
            PrintPageLastChar = document.SciControl.FormatRange(false, e, PrintPageLastChar, document.SciControl.Length);
            e.Graphics.DrawLine(new Pen(Color.Black), 35, e.PageBounds.Height - 60, e.PageBounds.Width - 35, e.PageBounds.Height - 60);
            e.Graphics.DrawString(footer, new Font("Arial", 7), Brushes.Black, 35, e.PageBounds.Height - 55, StringFormat.GenericDefault);
            if (PrintPageLastChar < document.SciControl.Length) e.HasMorePages = true;
            else
            {
                e.HasMorePages = false;
                document.SciControl.FormatRangeDone();
                PrintPageLastChar = 0;
                PrintPageNumber = 0;
            }
        }

    }

}
