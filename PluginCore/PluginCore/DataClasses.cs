using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using Ookii.Dialogs;

namespace PluginCore
{
    #region Data Objects

    /// <summary>
    /// Menus items
    /// </summary>
    public class ItemData
    {
        public String Id = String.Empty;
        public String Tag = String.Empty;
        public String Flags = String.Empty;

        public ItemData(String id, String tag, String flags)
        {
            if (id != null) this.Id = id;
            if (tag != null) this.Tag = tag;
            if (flags != null) this.Flags = flags;
        }

    }

    /// <summary>
    /// User custom arguments
    /// </summary>
    [Serializable]
    public class Argument
    {
        private String key = String.Empty;
        private String value = String.Empty;

        public Argument() { }
        public Argument(String key, String value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets and sets the key
        /// </summary> 
        public String Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// Gets and sets the value
        /// </summary> 
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(this.key) ? "New argument" : "$(" + this.key + ")";
        }
    }

    #endregion

    #region SDK Management

    /// <summary>
    /// A SDK should be associated to an object able to provide custom validation
    /// </summary>
    public interface InstalledSDKOwner
    {
        /// <summary>
        /// Control that sdk.Path points to a valid path location
        /// and update sdk.Name and sdk.Version accordingly
        /// (invoked when creating a new InstalledSDK object)
        /// </summary>
        bool ValidateSDK(InstalledSDK sdk);
    }

    /// <summary>
    /// Holds compiler location/version information
    /// </summary>
    [Serializable]
    public class InstalledSDK
    {
        static public readonly InstalledSDK INVALID_SDK = new InstalledSDK(null);

        private String path;
        private String name;
        private String version;
        private bool isValid;

        [field: NonSerialized]
        private InstalledSDKOwner owner;

        [Category("Location")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public String Path
        {
            get { return this.path; }
            set
            {
                this.path = value;
                Validate();
            }
        }

        [Category("Properties")]
        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [Category("Properties")]
        public String Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (!isValid) return false;
                try
                {
                    if (System.IO.Path.IsPathRooted(path) 
                        && !Directory.Exists(path) && !File.Exists(path))
                        return false;
                }
                catch { return false; }
                return true;
            }
        }

        [Browsable(false)]
        public InstalledSDKOwner Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public InstalledSDK(InstalledSDKOwner owner) { this.owner = owner; }
        public InstalledSDK() { this.owner = InstalledSDKContext.Current; }

        public int Compare(InstalledSDK sdk)
        {
            return 0;
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(this.name) ? "New SDK" : this.name;
        }

        public string ToPreferredSDK()
        {
            if (!String.IsNullOrEmpty(this.version))
                return (this.name ?? "") + ";" + (this.version ?? "") + ";";
            else
                return (this.name ?? "") + ";" + (this.version ?? "") + ";" + (this.path ?? "");
        }

        public void Validate()
        {
            if (owner != null)
            {
                InstalledSDKOwner o = owner;
                owner = null;
                isValid = o.ValidateSDK(this);
                owner = o;
            }
            else isValid = true;
        }
    }

    /// <summary>
    /// Set InstalledSDK default owner temporarily
    /// 
    /// Usage: 
    ///   using(new InstalledSDKContext(owner)) { ... }
    /// or
    ///   InstalledSDKContext isc = new InstalledSDKContext(owner);
    ///   ...
    ///   isc.Dispose();
    /// </summary>
    public class InstalledSDKContext: IDisposable
    {
        static public InstalledSDKOwner Current;

        public InstalledSDKOwner owner;

        public InstalledSDKContext(InstalledSDKOwner owner)
        {
            this.owner = owner;
            Current = owner;
        }

        public void Dispose()
        {
            if (Current == owner) Current = null;
        }
    }

    #endregion

}
