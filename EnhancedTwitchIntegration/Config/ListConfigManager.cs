using System.Collections.Generic;
using System.IO;

namespace SongRequestManager.Config
{
    public class ListConfig
    {
        public Dictionary<string, string> Remaps = new Dictionary<string, string>();
        public List<string> Bans = new List<string>();
    }

    public class ListConfigManager : ConfigBase<ListConfig>
    {
        protected override string FilePath => Path.Combine(Plugin.DataPath, "SRMListConfig.json");

        private static ListConfigManager _instance = null;
        public static ListConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ListConfigManager();
                }

                return _instance;
            }
        }
    }
}
