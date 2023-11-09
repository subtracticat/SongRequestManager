using System.Collections.Generic;
using System.IO;

namespace SongRequestManager.Config
{
    public class RemapConfigManager : ConfigBase<Dictionary<string, string>>
    {
        protected override string FilePath => Path.Combine(Plugin.DataPath, "SRMRemapConfig.json");

        private static RemapConfigManager _instance = null;
        public static RemapConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RemapConfigManager();
                }

                return _instance;
            }
        }
    }
}
