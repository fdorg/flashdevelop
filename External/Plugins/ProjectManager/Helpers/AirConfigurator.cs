using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PluginCore.Helpers;

namespace ProjectManager.Helpers
{
    public class AirConfigurator
    {

        private static Regex BatchParamRegEx = new Regex("(^\\s*set\\s+)(\\w+)(\\s*=\\s*)(.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline |
                                                             RegexOptions.Compiled);

        public const string DescriptorPath = "APP_XML";
        public const string PackageDir = "APP_DIR";
        public const string AndroidCert = "AND_CERT_FILE";
        public const string AppleDistCert = "IOS_DIST_CERT_FILE";
        public const string AppleDevCert = "IOS_DEV_CERT_FILE";
        public const string AppleProvision = "IOS_PROVISION";

        public string SdkSetupBatch { get; set; }
        public string ApplicationSetupBatch { get; set; }

        public Dictionary<string, string> ApplicationSetupParams { get; private set; }

        public AirConfigurator()
        {
            ApplicationSetupParams = new Dictionary<string, string>();
        }

        public void SetUp()
        {
            if (!string.IsNullOrEmpty(ApplicationSetupBatch) && ApplicationSetupParams.Count > 0)
            {
                var fileInfo = FileHelper.GetEncodingFileInfo(ApplicationSetupBatch);
                string contents = BatchParamRegEx.Replace(fileInfo.Contents, ParseApplicationSetupBatchParams);
                FileHelper.WriteFile(ApplicationSetupBatch, contents, Encoding.GetEncoding(fileInfo.CodePage), fileInfo.ContainsBOM);
            }

        }

        private string ParseApplicationSetupBatchParams(Match m)
        {
            string paramValue;
            if (ApplicationSetupParams.TryGetValue(m.Groups[2].Value, out paramValue))
                return m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value + paramValue;

            return m.Value;
        }
    }
}

