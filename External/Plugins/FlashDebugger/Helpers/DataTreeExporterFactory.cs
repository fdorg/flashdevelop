using System.Collections.Generic;

namespace FlashDebugger.Helpers
{

    /// <summary>
    /// A simple static factory to get IDataTreeExporter implementations
    /// </summary>
    public static class DataTreeExporterFactory
    {
        public static IDictionary<string, IDataTreeExporter> Exporters { get; } = new Dictionary<string, IDataTreeExporter>
        {
            [""] = new DefaultDataTreeExporter()
        };

        static DataTreeExporterFactory()
        {
        }
    }
}