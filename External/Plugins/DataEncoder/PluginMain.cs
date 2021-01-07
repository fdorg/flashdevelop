using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace DataEncoder
{
    public class PluginMain : IPlugin
    {
        readonly List<TypeData> objectTypes = new List<TypeData>();
        string oldFileName = string.Empty;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(DataEncoder);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "ca182923-bcdc-46bf-905c-aaa0bf64eebd";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Converts the file data for specific files to view them properly in FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => null;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            AddEventHandlers();
            Description = TextHelper.GetString("Info.Description");
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            // Nothing here..
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.FileEncode :
                    var fe = (DataEvent)e;
                    var ext = Path.GetExtension(fe.Action);
                    if (ext == ".fdb" || ext == ".fda" || ext == ".fdm")
                    {
                        SaveBinaryFile(fe.Action, fe.Data as string);
                        fe.Handled = true;
                    }
                    break;

                case EventType.FileDecode:
                    var fd = (DataEvent)e;
                    var ext1 = Path.GetExtension(fd.Action);
                    if (ext1 == ".fdb" || ext1 == ".fda" || ext1 == ".fdm")
                    {
                        var text = LoadBinaryFile(fd.Action);
                        if (text != null)
                        {
                            fd.Data = text;
                            fd.Handled = true;
                        }
                    }
                    break;

                case EventType.FileSaving:
                    var se = (TextEvent)e;
                    if (IsFileOpen(se.Value) && !IsXmlSaveable(se.Value))
                    {
                        se.Handled = true;
                    }
                    oldFileName = string.Empty;
                    break;

                case EventType.FileRenaming:
                    var te = (TextEvent)e;
                    var files = te.Value.Split(';');
                    oldFileName = files[0]; // Save for later..
                    if (IsFileOpen(oldFileName))
                    {
                        foreach (var objType in objectTypes)
                        {
                            if (objType.File == oldFileName)
                            {
                                objType.File = files[1];
                                break;
                            }
                        }
                    }
                    break;
            }
        }
        
        #endregion

        #region Custom Methods

        /**
        * Information messages.
        */
        readonly string CANT_SAVE_FILE = TextHelper.GetString("Info.CantSaveFile");

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.FileSaving | EventType.FileEncode | EventType.FileDecode | EventType.FileRenaming);

        /// <summary>
        /// Loads the serialized binary file
        /// </summary>
        string LoadBinaryFile(string file)
        {
            try
            {
                var settings = new object();
                var stream = new MemoryStream();
                settings = ObjectSerializer.Deserialize(file, settings, false);
                var xs = XmlSerializer.FromTypes(new[]{settings.GetType()})[0];
                xs.Serialize(stream, settings); // Obj -> XML
                var xw = new XmlTextWriter(stream, Encoding.UTF8);
                xw.Formatting = Formatting.Indented;
                stream.Close();
                objectTypes.Add(new TypeData(file, settings.GetType()));
                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Saves the serialized binary file
        /// </summary>
        void SaveBinaryFile(string file, string text)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(text);
                var stream = new MemoryStream(buffer);
                var typeData = GetFileObjectType(file);
                var xs = XmlSerializer.FromTypes(new[]{typeData.Type})[0];
                var settings = xs.Deserialize(stream); // XML -> Obj
                ObjectSerializer.Serialize(file, settings);
                stream.Close();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Checks if the syntax is ok to save
        /// </summary>
        bool IsXmlSaveable(string file)
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (document.SciControl is { } sci && (sci.FileName == file || sci.FileName == oldFileName))
                {
                    try
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(sci.Text);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowWarning(CANT_SAVE_FILE, ex);
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a file is open already
        /// </summary>
        /// <returns></returns>
        bool IsFileOpen(string file) => objectTypes.Any(it => it.File == file);

        /// <summary>
        /// Gets the file type for file
        /// </summary>
        TypeData GetFileObjectType(string file) => objectTypes.FirstOrDefault(it => it.File == file);

        #endregion

    }

    #region Structures

    internal class TypeData
    {
        public Type Type;
        public string File;

        public TypeData(string file, Type type)
        {
            File = file;
            Type = type;
        }
    }

    #endregion
}