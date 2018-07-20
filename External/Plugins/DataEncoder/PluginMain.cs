using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private String pluginName = "DataEncoder";
        private String pluginGuid = "ca182923-bcdc-46bf-905c-aaa0bf64eebd";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Converts the file data for specific files to view them properly in FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private List<TypeData> objectTypes = new List<TypeData>();
        private String oldFileName = String.Empty;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return this.pluginGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return null; }
        }
        
        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.AddEventHandlers();
            this.pluginDesc = TextHelper.GetString("Info.Description");
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
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.FileEncode :
                    DataEvent fe = (DataEvent)e;
                    String ext = Path.GetExtension(fe.Action);
                    if (ext == ".fdb" || ext == ".fda" || ext == ".fdm")
                    {
                        this.SaveBinaryFile(fe.Action, fe.Data as String);
                        fe.Handled = true;
                    }
                    break;

                case EventType.FileDecode:
                    DataEvent fd = (DataEvent)e;
                    String ext1 = Path.GetExtension(fd.Action);
                    if (ext1 == ".fdb" || ext1 == ".fda" || ext1 == ".fdm")
                    {
                        String text = this.LoadBinaryFile(fd.Action);
                        if (text != null)
                        {
                            fd.Data = text;
                            fd.Handled = true;
                        }
                    }
                    break;

                case EventType.FileSaving:
                    TextEvent se = (TextEvent)e;
                    if (this.IsFileOpen(se.Value))
                    {
                        if (!this.IsXmlSaveable(se.Value))
                        {
                            se.Handled = true;
                        }
                    }
                    this.oldFileName = String.Empty;
                    break;

                case EventType.FileRenaming:
                    TextEvent re = (TextEvent)e;
                    String[] files = re.Value.Split(';');
                    this.oldFileName = files[0]; // Save for later..
                    if (this.IsFileOpen(this.oldFileName))
                    {
                        foreach (TypeData objType in this.objectTypes)
                        {
                            if (objType.File == this.oldFileName)
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
        private readonly String CANT_SAVE_FILE = TextHelper.GetString("Info.CantSaveFile");

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventType events = EventType.FileSaving | EventType.FileEncode | EventType.FileDecode | EventType.FileRenaming;
            EventManager.AddEventHandler(this, events);
        }

        /// <summary>
        /// Loads the serialized binary file
        /// </summary>
        public String LoadBinaryFile(String file)
        {
            try
            {
                Object settings = new Object();
                MemoryStream stream = new MemoryStream();
                settings = ObjectSerializer.Deserialize(file, settings, false);
                XmlSerializer xs = XmlSerializer.FromTypes(new[]{settings.GetType()})[0];
                xs.Serialize(stream, settings); // Obj -> XML
                XmlTextWriter xw = new XmlTextWriter(stream, Encoding.UTF8);
                xw.Formatting = Formatting.Indented; stream.Close();
                this.objectTypes.Add(new TypeData(file, settings.GetType()));
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
        public void SaveBinaryFile(String file, String text)
        {
            try
            {
                Object settings = new Object();
                Byte[] buffer = Encoding.UTF8.GetBytes(text);
                MemoryStream stream = new MemoryStream(buffer);
                TypeData typeData = this.GetFileObjectType(file);
                XmlSerializer xs = XmlSerializer.FromTypes(new[]{typeData.Type})[0];
                settings = xs.Deserialize(stream); // XML -> Obj
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
        private Boolean IsXmlSaveable(String file)
        {
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.IsEditable && document.FileName == file || document.FileName == this.oldFileName)
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(document.SciControl.Text);
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
        private Boolean IsFileOpen(String file)
        {
            foreach (TypeData objType in this.objectTypes)
            {
                if (file == objType.File) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the file type for file
        /// </summary>
        public TypeData GetFileObjectType(String file)
        {
            foreach (TypeData objType in this.objectTypes)
            {
                if (file == objType.File) return objType;
            }
            return null;
        }

        #endregion

    }

    #region Structures

    public class TypeData
    {
        public Type Type;
        public String File;

        public TypeData(String file, Type type)
        {
            this.File = file;
            this.Type = type;
        }
    }

    #endregion

}
