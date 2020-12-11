// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Managers
{
    /// <summary>
    /// Provides methods to manage an instance of <see cref="System.Windows.Forms.ImageList"/> object.
    /// </summary>
    public class ImageListManager : IDisposable
    {
        static readonly List<WeakReference> instances = new List<WeakReference>();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListManager"/> class with default values for <see cref="ColorDepth"/>, <see cref="ImageSize"/>, and <see cref="TransparentColor"/>.
        /// </summary>
        public ImageListManager()
        {
            ImageList = new ImageList();
            AddInstance(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListManager"/> class, associating its <see cref="ImageList"/> with a container.
        /// </summary>
        /// <param name="container">An object implementing <see cref="IContainer"/> to associate with the <see cref="ImageList"/> of this instance of <see cref="ImageListManager"/>.</param>
        public ImageListManager(IContainer container)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));
            ImageList = new ImageList(container);
            AddInstance(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListManager"/> class with an <see cref="System.Windows.Forms.ImageList"/> object.
        /// </summary>
        /// <param name="target">An <see cref="System.Windows.Forms.ImageList"/> object to associate with this instance of <see cref="ImageListManager"/>.</param>
        public ImageListManager(ImageList target)
        {
            ImageList = target ?? throw new ArgumentNullException(nameof(target));
            AddInstance(this);
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Returns the <see cref="ImageList"/> of the specified <see cref="ImageListManager"/> instance.
        /// </summary>
        /// <param name="obj">An <see cref="ImageListManager"/> object.</param>
        public static implicit operator ImageList(ImageListManager obj) => obj.ImageList;

        /// <summary>
        /// Returns the <see cref="ImageListManager"/> instance associated with the specified <see cref="System.Windows.Forms.ImageList"/> object.
        /// </summary>
        /// <param name="obj">An <see cref="System.Windows.Forms.ImageList"/> object.</param>
        public static explicit operator ImageListManager(ImageList obj)
        {
            for (int i = 0, length = instances.Count; i < length; i++)
            {
                if (instances[i].Target is ImageListManager imageListManager)
                {
                    if (imageListManager.ImageList == obj) return imageListManager;
                }
                else
                {
                    // Free up spaces whenever we are looping through the list lol
                    instances.RemoveAt(i--);
                    length--;
                }
            }
            throw new InvalidCastException($"Unable to cast object of type '{obj.GetType()}' to type '{typeof(ImageListManager)}'.");
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the images in the <see cref="ImageList"/> should be populated.
        /// </summary>
        public event EventHandler Populate;

        /// <summary>
        /// Occurs when the <see cref="ImageList"/> is disposed by a call to the <see cref="Component.Dispose"/> method.
        /// </summary>
        public event EventHandler Disposed
        {
            add => ImageList.Disposed += value;
            remove => ImageList.Disposed -= value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="System.Windows.Forms.ImageList"/> for this instance of <see cref="ImageListManager"/>.
        /// </summary>
        public ImageList ImageList { get; }

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Forms.ColorDepth"/> of the <see cref="ImageList"/>.
        /// </summary>
        public ColorDepth ColorDepth
        {
            get => ImageList.ColorDepth;
            set => ImageList.ColorDepth = value;
        }

        /// <summary>
        /// Gets the <see cref="ImageList.ImageCollection"/> for the <see cref="ImageList"/>.
        /// </summary>
        public ImageList.ImageCollection Images => ImageList.Images;

        /// <summary>
        /// Gets or sets the <see cref="Size"/> of the images in the <see cref="ImageList"/>.
        /// </summary>
        public Size ImageSize
        {
            get => ImageList.ImageSize;
            set => ImageList.ImageSize = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> to treat as transparent.
        /// </summary>
        public Color TransparentColor
        {
            get => ImageList.TransparentColor;
            set => ImageList.TransparentColor = value;
        }

        /// <summary>
        /// Gets or sets an <see cref="object"/> that contains additional data about the <see cref="ImageList"/>.
        /// </summary>
        public object Tag
        {
            get => ImageList.Tag;
            set => ImageList.Tag = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes all instances of <see cref="ImageListManager"/>.
        /// </summary>
        public static void RefreshAll()
        {
            for (int i = 0, length = instances.Count; i < length; i++)
            {
                if (instances[i].Target is ImageListManager instance) instance.Refresh();
                else
                {
                    instances.RemoveAt(i--);
                    length--;
                }
            }
        }

        /// <summary>
        /// Adds a weak reference of an instance to the <see cref="instances"/> list.
        /// </summary>
        /// <param name="instance">An instance of <see cref="ImageListManager"/>.</param>
        static void AddInstance(ImageListManager instance) => instances.Add(new WeakReference(instance));

        /// <summary>
        /// Removes a weak reference to an instance from the instances list.
        /// </summary>
        /// <param name="instance">An instance of <see cref="ImageListManager"/>.</param>
        static void RemoveInstance(ImageListManager instance)
        {
            for (int i = 0, length = instances.Count; i < length; i++)
            {
                if (instances[i].Target is ImageListManager imageListManager)
                {
                    if (imageListManager != instance) continue;
                    instances.RemoveAt(i);
                    break;
                }
                instances.RemoveAt(i--);
                length--;
            }
        }

        /// <summary>
        /// Refreshes all images contained by the <see cref="ImageList"/>.
        /// </summary>
        public void Refresh() => OnRefresh();

        /// <summary>
        /// Initializes the <see cref="ImageList"/> by raising the <see cref="Populate"/> event.
        /// </summary>
        public void Initialize() => Populate?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Assigns the specified <see cref="EventHandler"/> to <see cref="Populate"/> and initializes the <see cref="ImageList"/> by calling <see cref="Initialize"/>.
        /// </summary>
        /// <param name="populate">An <see cref="EventHandler"/> to assign to <see cref="Populate"/>.</param>
        public void Initialize(EventHandler populate)
        {
            Populate = populate;
            Initialize();
        }

        /// <summary>
        /// Calls <see cref="Component.Dispose"/> on <see cref="ImageList"/>.
        /// </summary>
        public void Dispose()
        {
            ImageList.Dispose();
            RemoveInstance(this);
        }

        /// <summary>
        /// Clears <see cref="Images"/> and raises <see cref="Populate"/>. Override this method to provide custom refreshing.
        /// </summary>
        protected virtual void OnRefresh()
        {
            ImageList.Images.Clear();
            Populate?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
