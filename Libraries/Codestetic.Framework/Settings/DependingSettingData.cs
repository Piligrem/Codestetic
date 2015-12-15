using System.Collections.Generic;

namespace Codestetic.Web.Framework.Settings
{
    public class DependingSettingData
    {
        public DependingSettingData()
        {
            OverrideSettingKeys = new List<string>();
        }

        public List<string> OverrideSettingKeys { get; set; }
        public string RootSettingClass { get; set; }
    }
}
