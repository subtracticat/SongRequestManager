using System.Collections.Generic;
using System.IO;

namespace SongRequestManager.Config
{
    public class SongModerationSettingsData
    {
        public Dictionary<string, string> Remaps = new Dictionary<string, string>();
        public List<string> Bans = new List<string>();
    }

    public class SongModerationSettings : PersistedStateManager<SongModerationSettingsData>
    {
        protected override string FileName => "SRMSongSettings.json";

        private static SongModerationSettings _instance = null;
        public static SongModerationSettings Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SongModerationSettings();
                }

                return _instance;
            }
        }
    }
}
