﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;

namespace FlashDebugger.Helpers
{

    /// <summary>
    /// A simple static factory to get IDataTreeExporter implementations
    /// </summary>
    public static class DataTreeExporterFactory
    {

        private readonly static IDictionary<string, IDataTreeExporter> exporters;
        public static IDictionary<string, IDataTreeExporter> Exporters
        {
            get { return exporters; }
        }

        static DataTreeExporterFactory()
        {
            exporters = new Dictionary<string, IDataTreeExporter>();
            exporters[""] = new DefaultDataTreeExporter();
        }

    }
}
