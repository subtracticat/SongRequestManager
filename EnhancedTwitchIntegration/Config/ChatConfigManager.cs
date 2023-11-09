using System.IO;

namespace SongRequestManager.Config
{
    public class ChatConfig
    {
        public string ChatUsername = "";
        public string ChatToken = "";
        public string ChatChannel = "";
    }

    public class ChatConfigManager : ConfigBase<ChatConfig>
    {
        protected override string FilePath => Path.Combine(Plugin.DataPath, "SRMChatConfig.json");
        protected override string LegacyFilePath => Path.Combine(Plugin.DataPath, "SRMChatConfig.ini");

        private static ChatConfigManager _instance = null;
        public static ChatConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChatConfigManager();
                }

                return _instance;
            }
        }
    }
}
