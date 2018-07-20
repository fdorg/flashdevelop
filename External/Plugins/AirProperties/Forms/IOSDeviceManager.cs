// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AirProperties.Controls;
using PluginCore.Localization;

namespace AirProperties.Forms
{
    public partial class IOSDeviceManager : Form
    {

        private readonly DeviceClassification[] iOSDevices;

        private string _selectedDevices;
        public string SelectedDevices { get { return _selectedDevices; } }

        public IOSDeviceManager(string[] selectedDevices)
        {
            InitializeComponent();

            this.OKButton.Text = TextHelper.GetString("Label.Ok");
            this.CancelButton1.Text = TextHelper.GetString("Label.Cancel");
            this.Text = TextHelper.GetString("Title.SelectIOSDevices");

            var iPadDevices = new DeviceClassification("iPad", "iPad", new[]
                {
                    new DeviceClassification("iPad", "iPad1", null),
                    new DeviceClassification("iPad 2/Mini", "iPad2", new[]
                        {
                            new DeviceClassification("iPad 2 (WiFi)", "iPad2,1", null),
                            new DeviceClassification("iPad 2 (GSM)", "iPad2,2", null),
                            new DeviceClassification("iPad 2 (CDMA)", "iPad2,3", null),
                            new DeviceClassification("iPad 2 (WiFi Rev A)", "iPad2,4", null),
                            new DeviceClassification("iPad Mini (WiFi)", "iPad2,5", null),
                            new DeviceClassification("iPad Mini (GSM)", "iPad2,6", null),
                            new DeviceClassification("iPad Mini (GSM+CDMA)", "iPad2,7", null)
                        }),
                    new DeviceClassification("iPad 3/4", "iPad3", new[]
                        {
                            new DeviceClassification("iPad 3 (WiFi)", "iPad3,1", null),
                            new DeviceClassification("iPad 3 (GSM+CDMA)", "iPad3,2", null),
                            new DeviceClassification("iPad 3 (GSM)", "iPad3,3", null),
                            new DeviceClassification("iPad 4 (WiFi)", "iPad3,4", null),
                            new DeviceClassification("iPad 4 (GSM)", "iPad3,5", null),
                            new DeviceClassification("iPad 4 (GSM+CDMA)", "iPad3,6", null)
                        }),
                    new DeviceClassification("iPad Air/Mini Retina", "iPad4", new[]
                        {
                            new DeviceClassification("iPad Air (WiFi)", "iPa43,1", null),
                            new DeviceClassification("iPad Air (Cellular)", "iPad4,2", null),
                            new DeviceClassification("iPad Mini 2G (WiFi)", "iPad4,4", null),
                            new DeviceClassification("iPad Mini 2G (Cellular)", "iPad4,5", null)
                        })
                });
            var iPodDevices = new DeviceClassification("iPod", "iPod", new[]
                {
                    new DeviceClassification("iPod Touch 4th Generation", "iPod4,1", null),
                    new DeviceClassification("iPod Touch 5th Generation", "iPod5,1", null)
                });
            var iPhoneDevices = new DeviceClassification("iPhone", "iPhone", new[]
                {
                    new DeviceClassification("iPhone 3GS ", "iPhone2,1", null),
                    new DeviceClassification("iPhone 4", "iPhone3", new[]
                        {
                            new DeviceClassification("iPhone 4", "iPhone3,1", null),
                            new DeviceClassification("iPhone 4 (Rev A)", "iPhone3,2", null),
                            new DeviceClassification("iPhone 4 (CDMA)", "iPhone3,3", null)
                        }),
                    new DeviceClassification("iPhone 4S", "iPhone4", null),
                    new DeviceClassification("iPhone 5/5C", "iPhone5", new[]
                        {
                            new DeviceClassification("iPhone 5 (GSM)", "iPhone5,1", null),
                            new DeviceClassification("iPhone 5 (GSM+CDMA)", "iPhone5,2", null),
                            new DeviceClassification("iPhone 5C (GSM)", "iPhone5,3", null),
                            new DeviceClassification("iPhone 5C (GSM+CDMA)", "iPhone5,4", null)
                        }),
                    new DeviceClassification("iPhone 5S", "iPhone6", new[]
                        {
                            new DeviceClassification("iPhone 5s (GSM)", "iPhone6,1", null),
                            new DeviceClassification("iPhone 5s (GSM+CDMA)", "iPhone6,2", null),
                        })
                });

            iOSDevices = new[] { iPadDevices, iPhoneDevices, iPodDevices };

            IOSDevicesTree.BeginUpdate();
            FillTree(iOSDevices, selectedDevices, null);
            IOSDevicesTree.EndUpdate();
        }

        private void FillTree(DeviceClassification[] devices, string[] selected, TreeNode parent)
        {
            var selectedNodes = new List<TreeNode>(devices.Length);
            foreach (var device in devices)
            {
                TreeNode newNode;
                if (parent != null)
                {
                    newNode = parent.Nodes.Add(device.HardwareId, device.Name);
                }
                else
                {
                    newNode = IOSDevicesTree.Nodes.Add(device.HardwareId, device.Name);
                }

                // Since we're still populating the tree, we cannot check here.
                // We could make SelectedNodes a read/write property, and find the nodes by key after in the setter, but it just came out this way
                if (selected != null && Array.IndexOf(selected, device.HardwareId) > -1) selectedNodes.Add(newNode);

                if (device.SubClassifications != null && device.SubClassifications.Length > 0)
                    FillTree(device.SubClassifications, selected, newNode);
            }

            foreach (var node in selectedNodes) node.Checked = true;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            StringBuilder deviceBuilder = new StringBuilder();

            foreach (TreeNode child in IOSDevicesTree.Nodes)
                GetSelectedDevices(child, deviceBuilder);

            _selectedDevices = deviceBuilder.ToString();
        }

        private void GetSelectedDevices(TreeNode node, StringBuilder builder)
        {
            if (node.StateImageIndex == (int) TriStateTreeView.CheckedState.Checked)
            {
                if (builder.Length > 0) builder.Append(' ');
                builder.Append(node.Name);
            }
            else if (node.StateImageIndex == (int) TriStateTreeView.CheckedState.Mixed)
            {
                foreach (TreeNode child in node.Nodes) GetSelectedDevices(child, builder);
            }
        }

        public class DeviceClassification
        {
            public String Name { get; set; }

            public String HardwareId { get; set; }

            public DeviceClassification[] SubClassifications { get; set; }

            public DeviceClassification(string name, string hardwareId, DeviceClassification[] subClassifications)
            {
                Name = name;
                HardwareId = hardwareId;
                SubClassifications = subClassifications;
            }
        }

    }
}
