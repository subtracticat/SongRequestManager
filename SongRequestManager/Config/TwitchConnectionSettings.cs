using System.IO;

namespace SongRequestManager.Config
{
    public class TwitchConnectionSettingsData
    {
        public string ChatUsername = "";
        public string ChatToken = "";
        public string ChatChannel = "";
    }

    public class TwitchConnectionSettings : StateManagerBase<TwitchConnectionSettingsData>
    {
        protected override string FileName => "SRMTwitchSettings.json";
        protected override string LegacyFileName => "SRMChatConfig.ini";

        private static TwitchConnectionSettings _instance = null;
        public static TwitchConnectionSettings Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TwitchConnectionSettings();
                }

                return _instance;
            }
        }
    }
}
