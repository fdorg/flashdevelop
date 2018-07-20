using System;
using System.Drawing;
using System.Drawing.Printing;
using PluginCore;
using PluginCore.Localization;

namespace FlashDevelop.Managers
{
    class PrintingManager
    {
        public static Int32 PrintPageNumber = 0;
        public static Int32 PrintPageLastChar = 0;
        public static PrinterSettings PrinterSettings = null;

        /// <summary>
        /// Gets the global printer settings
        /// </summary> 
        public static PrinterSettings GetPrinterSettings()
        {
            if (PrinterSettings == null)
            {
                PrinterSettings = new PrinterSettings();
            }
            return PrinterSettings;
        }

        /// <summary>
        /// Creates a new print document
        /// </summary> 
        public static PrintDocument CreatePrintDocument()
        {
            PrintPageNumber = 0;
            PrintPageLastChar = 0;
            PrintDocument printDocument = new PrintDocument();
            printDocument.DocumentName = Globals.CurrentDocument.Text;
            printDocument.PrintPage += new PrintPageEventHandler(OnPrintDocumentPrintPage);
            return printDocument;
        }

        /// <summary>
        /// Handles the PrintPage event
        /// </summary>
        public static void OnPrintDocumentPrintPage(Object sender, PrintPageEventArgs e)
        {
            PrintPageNumber++;
            ITabbedDocument document = Globals.CurrentDocument;
            String page = TextHelper.GetString("Info.PrintFooterPage");
            String footer = page + PrintPageNumber + " - " + document.FileName;
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
