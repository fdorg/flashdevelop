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
        private static readonly List<WeakReference> instances;
        private readonly ImageList imageList;

        #region Constructors

        /// <summary>
        /// The <see cref="ImageListManager"/> static constructor.
        /// </summary>
        static ImageListManager()
        {
            instances = new List<WeakReference>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListManager"/> class with default values for <see cref="ColorDepth"/>, <see cref="ImageSize"/>, and <see cref="TransparentColor"/>.
        /// </summary>
        public ImageListManager()
        {
            imageList = new ImageList();
            AddInstance(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListManager"/> class, associating its <see cref="ImageList"/> with a container.
        /// </summary>
        /// <param name="container">An object implementing <see cref="IContainer"/> to associate with the <see cref="ImageList"/> of this instance of <see cref="ImageListManager"/>.</param>
        public ImageListManager(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            imageList = new ImageList(container);
            AddInstance(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListManager"/> class with an <see cref="System.Windows.Forms.ImageList"/> object.
        /// </summary>
        /// <param name="target">An <see cref="System.Windows.Forms.ImageList"/> object to associate with this instance of <see cref="ImageListManager"/>.</param>
        public ImageListManager(ImageList target)
        {
            if (target == null) throw new ArgumentNullException("target");
            imageList = target;
            AddInstance(this);
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Returns the <see cref="ImageList"/> of the specified <see cref="ImageListManager"/> instance.
        /// </summary>
        /// <param name="obj">An <see cref="ImageListManager"/> object.</param>
        public static implicit operator ImageList(ImageListManager obj)
        {
            return obj.imageList;
        }

        /// <summary>
        /// Returns the <see cref="ImageListManager"/> instance associated with the specified <see cref="System.Windows.Forms.ImageList"/> object.
        /// </summary>
        /// <param name="obj">An <see cref="System.Windows.Forms.ImageList"/> object.</param>
        public static explicit operator ImageListManager(ImageList obj)
        {
            for (int i = 0, length = instances.Count; i < length; i++)
            {
                var imageListManager = instances[i].Target as ImageListManager;
                if (imageListManager == null)
                {
                    // Free up spaces whenever we are looping through the list lol
                    instances.RemoveAt(i--);
                    length--;
                }
                else if (imageListManager.imageList == obj) return imageListManager;
            }
            throw new InvalidCastException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", obj.GetType(), typeof(ImageListManager)));
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
            add { imageList.Disposed += value; }
            remove { imageList.Disposed -= value; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="System.Windows.Forms.ImageList"/> for this instance of <see cref="ImageListManager"/>.
        /// </summary>
        public ImageList ImageList
        {
            get { return imageList; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Forms.ColorDepth"/> of the <see cref="ImageList"/>.
        /// </summary>
        public ColorDepth ColorDepth
        {
            get { return imageList.ColorDepth; }
            set { imageList.ColorDepth = value; }
        }

        /// <summary>
        /// Gets the <see cref="ImageList.ImageCollection"/> for the <see cref="ImageList"/>.
        /// </summary>
        public ImageList.ImageCollection Images
        {
            get { return imageList.Images; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Size"/> of the images in the <see cref="ImageList"/>.
        /// </summary>
        public Size ImageSize
        {
            get { return imageList.ImageSize; }
            set { imageList.ImageSize = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> to treat as transparent.
        /// </summary>
        public Color TransparentColor
        {
            get { return imageList.TransparentColor; }
            set { imageList.TransparentColor = value; }
        }

        /// <summary>
        /// Gets or sets an <see cref="object"/> that contains additional data about the <see cref="ImageList"/>.
        /// </summary>
        public object Tag
        {
            get { return imageList.Tag; }
            set { imageList.Tag = value; }
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
                var instance = instances[i].Target as ImageListManager;
                if (instance == null)
                {
                    instances.RemoveAt(i--);
                    length--;
                }
                else instance.Refresh();
            }
        }

        /// <summary>
        /// Adds a weak reference of an instance to the <see cref="instances"/> list.
        /// </summary>
        /// <param name="instance">An instance of <see cref="ImageListManager"/>.</param>
        private static void AddInstance(ImageListManager instance)
        {
            instances.Add(new WeakReference(instance));
        }

        /// <summary>
        /// Removes a weak reference to an instance from the instances list.
        /// </summary>
        /// <param name="instance">An instance of <see cref="ImageListManager"/>.</param>
        private static void RemoveInstance(ImageListManager instance)
        {
            for (int i = 0, length = instances.Count; i < length; i++)
            {
                var imageListManager = instances[i].Target as ImageListManager;
                if (imageListManager == null)
                {
                    instances.RemoveAt(i--);
                    length--;
                }
                else if (imageListManager == instance)
                {
                    instances.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Refreshes all images contained by the <see cref="ImageList"/>.
        /// </summary>
        public void Refresh()
        {
            OnRefresh();
        }

        /// <summary>
        /// Initializes the <see cref="ImageList"/> by raising the <see cref="Populate"/> event.
        /// </summary>
        public void Initialize()
        {
            if (Populate != null) Populate(this, EventArgs.Empty);
        }

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
            imageList.Dispose();
            RemoveInstance(this);
        }

        /// <summary>
        /// Clears <see cref="Images"/> and raises <see cref="Populate"/>. Override this method to provide custom refreshing.
        /// </summary>
        protected virtual void OnRefresh()
        {
            imageList.Images.Clear();
            if (Populate != null) Populate(this, EventArgs.Empty);
        }

        #endregion
    }
}
