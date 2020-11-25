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
        public string Id = string.Empty;
        public string Tag = string.Empty;
        public string Flags = string.Empty;

        public ItemData(string id, string tag, string flags)
        {
            if (id != null) Id = id;
            if (tag != null) Tag = tag;
            if (flags != null) Flags = flags;
        }

    }

    /// <summary>
    /// User custom arguments
    /// </summary>
    [Serializable]
    public class Argument
    {
        string key = string.Empty;
        string value = string.Empty;

        public Argument() { }
        public Argument(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets and sets the key
        /// </summary> 
        public string Key
        {
            get => key;
            set => key = value;
        }

        /// <summary>
        /// Gets and sets the value
        /// </summary> 
        public string Value
        {
            get => value;
            set => this.value = value;
        }

        public override string ToString() => string.IsNullOrEmpty(key) ? "New argument" : "$(" + key + ")";
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
        public static readonly InstalledSDK INVALID_SDK = new InstalledSDK(null);

        string path;
        string name;
        string version;
        string classPath;
        bool isValid;

        [field: NonSerialized] InstalledSDKOwner owner;

        [Category("Location")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string Path
        {
            get => path;
            set
            {
                path = value;
                Validate();
            }
        }

        [Category("Properties")]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [Category("Properties")]
        public string Version
        {
            get => version;
            set => version = value;
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
        public bool IsHaxeShim => classPath != null;

        [Browsable(false)]
        public string ClassPath
        {
            get => classPath;
            set => classPath = value;
        }

        [Browsable(false)]
        public InstalledSDKOwner Owner
        {
            get => owner;
            set => owner = value;
        }

        public InstalledSDK(InstalledSDKOwner owner) { this.owner = owner; }
        public InstalledSDK() { owner = InstalledSDKContext.Current; }

        public int Compare(InstalledSDK sdk) => 0;

        public override string ToString() => string.IsNullOrEmpty(name) ? "New SDK" : name;

        public string ToPreferredSDK()
        {
            if (!string.IsNullOrEmpty(version)) return $"{name ?? string.Empty};{version};";
            return $"{name ?? string.Empty};{version ?? string.Empty};{path ?? string.Empty}";
        }

        public void Validate()
        {
            if (owner != null)
            {
                var o = owner;
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
        public static InstalledSDKOwner Current;

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