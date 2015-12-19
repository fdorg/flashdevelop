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

        #region Events

        /// <summary>
        /// Raised when the images in the <see cref="ImageList"/> should be initialized.
        /// </summary>
        public event EventHandler OnInitialize;

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
        /// Gets or sets the color depth of the <see cref="ImageList"/>.
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
        /// Gets or sets the size of the images in the <see cref="ImageList"/>.
        /// </summary>
        public Size ImageSize
        {
            get { return imageList.ImageSize; }
            set { imageList.ImageSize = value; }
        }

        /// <summary>
        /// Gets or sets the color to treat as transparent
        /// </summary>
        public Color TransparentColor
        {
            get { return imageList.TransparentColor; }
            set { imageList.TransparentColor = value; }
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

        private static void RemoveInstance(ImageListManager instance)
        {
            for (int i = 0, length = instances.Count; i < length; i++)
            {
                if (instances[i].Target == instance)
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
        /// Initializes the <see cref="ImageList"/> by raising the <see cref="OnInitialize"/> event.
        /// </summary>
        public void Initialize()
        {
            if (OnInitialize != null) OnInitialize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Assigns the specified <see cref="EventHandler"/> to <see cref="OnInitialize"/> and initializes the <see cref="ImageList"/> by raising the event.
        /// </summary>
        /// <param name="initialization">An <see cref="EventHandler"/> to assign to <see cref="OnInitialize"/>.</param>
        public void Initialize(EventHandler initialization)
        {
            OnInitialize = initialization;
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
        /// Clears <see cref="Images"/> and calls <see cref="Initialize"/>. Override this method to provide custom initialization.
        /// </summary>
        protected virtual void OnRefresh()
        {
            imageList.Images.Clear();
            Initialize();
        }

        #endregion
    }
}
