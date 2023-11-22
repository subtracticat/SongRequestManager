using System.IO;

namespace SongRequestManager.Config
{
    public class RequestBotSettingsData
    {
        public bool RequestQueueOpen = true;
        public int SessionResetAfterXHours = 6; // Number of hours before persistent session properties are reset (ie: Queue, Played , Duplicate List)

        public float LowestAllowedRating = 0; // Lowest allowed song rating to be played 0-100 *IMPLEMENTED*, needs UI
        public float MaximumSongLength = 180; // Maximum song length in minutes
        public bool SendNextSongBeingPlayedtoChat = true; // Enable chat message when you hit play
        public bool UpdateQueueStatusFiles = true; // Create and update queue list and open/close status files for OBS *IMPLEMENTED*, needs UI
        
        public float MinimumPriorityRequestValue = 4.99f;
        public int PriorityExpirationDays = 2;
        public bool EnableAutoPrio = true;
    }

    public class RequestBotSettings : StateManagerBase<RequestBotSettingsData>
    {
        protected override string FileName => "SRMBotSettings.json";
        protected override string LegacyFileName => "RequestBotSettings.ini";

        private static RequestBotSettings _instance = null;
        public static RequestBotSettings Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RequestBotSettings();
                }

                return _instance;
            }
        }
    }
}
