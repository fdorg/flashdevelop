using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace FlashDevelop.Controls
{
    /// <summary>
    /// This class overrides the standard PropertyGrid provided by Microsoft.
    /// It also allows to hide (or filter) the properties of the SelectedObject displayed by the PropertyGrid.
    /// </summary>
    public class FilteredGrid : PropertyGrid
    {
        /// <summary>
        /// Contain a reference to the collection of properties to show in the parent PropertyGrid.
        /// </summary>
        List<PropertyDescriptor> m_PropertyDescriptors = new List<PropertyDescriptor>();

        /// <summary>
        /// Contain a reference to the array of properties to display in the PropertyGrid.
        /// </summary>
        private AttributeCollection m_HiddenAttributes = null, m_BrowsableAttributes = null;

        /// <summary>
        /// Contain references to the arrays of properties or categories to hide.
        /// </summary>
        private string[] m_BrowsableProperties = null, m_HiddenProperties = null;

        /// <summary>
        /// Contain a reference to the wrapper that contains the object to be displayed into the PropertyGrid.
        /// </summary>
        private ObjectWrapper m_Wrapper = null;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public FilteredGrid() 
        {
            base.SelectedObject = m_Wrapper;
        }

        /// <summary>
        /// 
        /// </summary>
        public new AttributeCollection BrowsableAttributes 
        {
            get { return m_BrowsableAttributes; }
            set 
            {
                if (m_BrowsableAttributes != value) 
                {
                    m_HiddenAttributes = null;
                    m_BrowsableAttributes = value;
                    RefreshProperties();
                }
            }
        }

        /// <summary>
        /// Get or set the categories to hide.
        /// </summary>
        public AttributeCollection HiddenAttributes 
        {
            get { return m_HiddenAttributes; }
            set 
            {
                if (value != m_HiddenAttributes) 
                {
                    m_HiddenAttributes = value;
                    m_BrowsableAttributes = null;
                    RefreshProperties();
                }
            }
        }

        /// <summary>
        /// Get or set the properties to show.
        /// </summary>
        public string[] BrowsableProperties 
        {
            get { return m_BrowsableProperties; }
            set 
            {
                if (value != m_BrowsableProperties) 
                {
                    m_BrowsableProperties = value;
                    RefreshProperties();
                }
            }
        }

        /// <summary>
        /// Get or set the properties to hide.
        /// </summary>
        public string[] HiddenProperties 
        {
            get { return m_HiddenProperties; }
            set 
            {
                if (value != m_HiddenProperties) 
                {
                    m_HiddenProperties = value;
                    RefreshProperties();
                } 
            }
        }

        /// <summary>
        /// Overwrite the PropertyGrid.SelectedObject property.
        /// </summary>
        public new object SelectedObject 
        {
            get { return m_Wrapper != null ? ((ObjectWrapper)base.SelectedObject).SelectedObject : null; }
            set 
            {
                if (value != null)
                {
                    if (m_Wrapper == null) 
                    {
                        m_Wrapper = new ObjectWrapper(value);
                        RefreshProperties();
                    }
                    else if (m_Wrapper.SelectedObject != value) 
                    {
                        bool needrefresh;
                        needrefresh = (value.GetType() != m_Wrapper.SelectedObject.GetType());
                        m_Wrapper.SelectedObject = value;
                        if (needrefresh) RefreshProperties();
                    }
                    // Set the list of properties to the wrapper.
                    m_Wrapper.PropertyDescriptors = m_PropertyDescriptors;
                    // Link the wrapper to the parent PropertyGrid.
                    base.SelectedObject = m_Wrapper;
                }
                else
                {
                    m_Wrapper = null;
                    base.SelectedObject = null;
                }
            }
        }

        /// <summary>
        /// Called when the browsable properties have changed.
        /// </summary>
        private void OnBrowsablePropertiesChanged() 
        {
            if(m_Wrapper == null) return;
        }

        /// <summary>
        /// Build the list of the properties to be displayed in the PropertyGrid, following the filters defined the Browsable and Hidden properties.
        /// </summary>
        private void RefreshProperties() 
        {
            if (m_Wrapper == null) return;
            // Clear the list of properties to be displayed.
            m_PropertyDescriptors.Clear();
            // Check whether the list is filtered 
            if (m_BrowsableAttributes != null && m_BrowsableAttributes.Count > 0) 
            {
                // Add to the list the attributes that need to be displayed.
                foreach(Attribute attribute in m_BrowsableAttributes) ShowAttribute(attribute);
            } 
            else 
            {
                // Fill the collection with all the properties.
                PropertyDescriptorCollection originalpropertydescriptors = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject);
                foreach(PropertyDescriptor propertydescriptor in originalpropertydescriptors) m_PropertyDescriptors.Add(propertydescriptor);
                // Remove from the list the attributes that mustn't be displayed.
                if(m_HiddenAttributes != null) foreach(Attribute attribute in m_HiddenAttributes) HideAttribute(attribute);
            }
            // Get all the properties of the SelectedObject
            PropertyDescriptorCollection allproperties = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject);
            // Hide if necessary, some properties
            if (m_HiddenProperties != null && m_HiddenProperties.Length > 0) 
            {
                // Remove from the list the properties that mustn't be displayed.
                foreach(string propertyname in m_HiddenProperties) 
                {
                    try 
                    {
                        PropertyDescriptor property = allproperties[propertyname];
                        // Remove from the list the property
                        HideProperty(property);
                    } 
                    catch(Exception ex) 
                    {
                        throw new ArgumentException(ex.Message);
                    }
                }
            }
            if (m_BrowsableProperties != null && m_BrowsableProperties.Length > 0) 
            {
                // Clear properties to filter the list from scratch BY IAP
                m_PropertyDescriptors.Clear();
                foreach(string propertyname in m_BrowsableProperties) 
                {
                    try 
                    {
                        ShowProperty(allproperties[propertyname]);
                    } 
                    catch (Exception)
                    {
                        throw new ArgumentException("Property not found.", propertyname);
                    }
                }
            }
            m_PropertyDescriptors.Sort(CompareDescriptors);
        }

        /// <summary>
        /// Compare two property descriptors.
        /// </summary>
        private static int CompareDescriptors(PropertyDescriptor a, PropertyDescriptor b)
        {
            if (a == null) return b == null ? 0 : -1;
            else if (b == null) return 1;
            int value = string.Compare(a.Category, b.Category);
            return value == 0 ? string.Compare(a.DisplayName, b.DisplayName) : value;
        }

        /// <summary>
        /// Allows to hide a set of properties to the parent PropertyGrid.
        /// </summary>
        private void HideAttribute(Attribute attribute) 
        {
            PropertyDescriptorCollection filteredoriginalpropertydescriptors = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject, new Attribute[] { attribute });
            if(filteredoriginalpropertydescriptors == null || filteredoriginalpropertydescriptors.Count == 0) throw new ArgumentException("Attribute not found", attribute.ToString());
            foreach(PropertyDescriptor propertydescriptor in filteredoriginalpropertydescriptors) HideProperty(propertydescriptor);
        }

        /// <summary>
        /// Add all the properties that match an attribute to the list of properties to be displayed in the PropertyGrid.
        /// </summary>
        private void ShowAttribute(Attribute attribute) 
        {
            PropertyDescriptorCollection filteredoriginalpropertydescriptors = TypeDescriptor.GetProperties(m_Wrapper.SelectedObject,new Attribute[] { attribute });
            if (filteredoriginalpropertydescriptors == null || filteredoriginalpropertydescriptors.Count == 0) throw new ArgumentException("Attribute not found", attribute.ToString());
            foreach(PropertyDescriptor propertydescriptor in filteredoriginalpropertydescriptors) ShowProperty(propertydescriptor);
        }

        /// <summary>
        /// Add a property to the list of properties to be displayed in the PropertyGrid.
        /// </summary>
        private void ShowProperty(PropertyDescriptor property) 
        {
            if (!m_PropertyDescriptors.Contains(property)) m_PropertyDescriptors.Add(property);
        }

        /// <summary>
        /// Allows to hide a property to the parent PropertyGrid.
        /// </summary>
        private void HideProperty(PropertyDescriptor property) 
        {
            if (m_PropertyDescriptors.Contains(property)) m_PropertyDescriptors.Remove(property);
        }

    }

    #region Internal Classes

    /// <summary>
    /// This class is a wrapper. It contains the object the propertyGrid has to display.
    /// </summary>
    internal class ObjectWrapper : ICustomTypeDescriptor
    {
        /// <summary>
        /// Contain a reference to the selected objet that will linked to the parent PropertyGrid.
        /// </summary>
        private object m_SelectedObject = null;

        /// <summary>
        /// Contain a reference to the collection of properties to show in the parent PropertyGrid.
        /// </summary>
        List<PropertyDescriptor> m_PropertyDescriptors = new List<PropertyDescriptor>();

        /// <summary>
        /// Simple constructor.
        /// </summary>
        /// <param name="obj">A reference to the selected object that will linked to the parent PropertyGrid.</param>
        internal ObjectWrapper(object obj)
        {
            m_SelectedObject = obj;
        }

        /// <summary>
        /// Get or set a reference to the selected objet that will linked to the parent PropertyGrid.
        /// </summary>
        public object SelectedObject
        {
            get { return m_SelectedObject; }
            set { if (m_SelectedObject != value) m_SelectedObject = value; }
        }

        /// <summary>
        /// Get or set a reference to the collection of properties to show in the parent PropertyGrid.
        /// </summary>
        public List<PropertyDescriptor> PropertyDescriptors
        {
            get { return m_PropertyDescriptors; }
            set { m_PropertyDescriptors = value; }
        }

        #region ICustomTypeDescriptor Members

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(m_PropertyDescriptors.ToArray(), true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(m_SelectedObject, true);
        }

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(m_SelectedObject, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(m_SelectedObject, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(m_SelectedObject, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(m_SelectedObject, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(m_SelectedObject, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(m_SelectedObject, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(m_SelectedObject, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return m_SelectedObject;
        }

        #endregion

    }

    #endregion

}