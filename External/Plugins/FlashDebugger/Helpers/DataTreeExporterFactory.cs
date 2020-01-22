// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;

namespace FlashDebugger.Helpers
{

    /// <summary>
    /// A simple static factory to get IDataTreeExporter implementations
    /// </summary>
    public static class DataTreeExporterFactory
    {
        public static IDictionary<string, IDataTreeExporter> Exporters { get; }

        static DataTreeExporterFactory()
        {
            Exporters = new Dictionary<string, IDataTreeExporter> {[""] = new DefaultDataTreeExporter()};
        }
    }
}