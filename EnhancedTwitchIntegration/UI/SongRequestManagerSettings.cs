using BeatSaberMarkupLanguage.Attributes;
using SongRequestManager.Config;

namespace SongRequestManager.UI
{
    public class SongRequestManagerSettings : PersistentSingleton<SongRequestManagerSettings>
    {
        [UIValue("autopick-first-song")]
        public bool AutopickFirstSong
        {
            get => QueueConfigManager.Instance.Config.AutopickFirstSong;
            set => QueueConfigManager.Instance.Config.AutopickFirstSong = value;
        }

        [UIValue("lowest-allowed-rating")]
        public float LowestAllowedRating
        {
            get => QueueConfigManager.Instance.Config.LowestAllowedRating;
            set => QueueConfigManager.Instance.Config.LowestAllowedRating = value;
        }

        [UIValue("maximum-song-length")]
        public int MaximumSongLength
        {
            get => (int) QueueConfigManager.Instance.Config.MaximumSongLength;
            set => QueueConfigManager.Instance.Config.MaximumSongLength = value;
        }

        [UIValue("minimum-njs")]
        public int MinimumNJS
        {
            get => (int) QueueConfigManager.Instance.Config.MinimumNJS;
            set => QueueConfigManager.Instance.Config.MinimumNJS = value;
        }

        [UIValue("automap")]
        public bool Automap
        {
            get => QueueConfigManager.Instance.Config.Automap;
            set => QueueConfigManager.Instance.Config.Automap = value;
        }

        [UIValue("tts-support")]
        public bool TtsSupport
        {
            get => QueueConfigManager.Instance.Config.BotPrefix != "";
            set => QueueConfigManager.Instance.Config.BotPrefix = value ? "! " : "";
        }

        [UIValue("user-request-limit")]
        public int UserRequestLimit
        {
            get => QueueConfigManager.Instance.Config.UserRequestLimit;
            set => QueueConfigManager.Instance.Config.UserRequestLimit = value;
        }

        [UIValue("sub-request-limit")]
        public int SubRequestLimit
        {
            get => QueueConfigManager.Instance.Config.SubRequestLimit;
            set => QueueConfigManager.Instance.Config.SubRequestLimit = value;
        }

        [UIValue("mod-request-limit")]
        public int ModRequestLimit
        {
            get => QueueConfigManager.Instance.Config.ModRequestLimit;
            set => QueueConfigManager.Instance.Config.ModRequestLimit = value;
        }

        [UIValue("vip-bonus-requests")]
        public int VipBonusRequests
        {
            get => QueueConfigManager.Instance.Config.VipBonusRequests;
            set => QueueConfigManager.Instance.Config.VipBonusRequests = value;
        }

        
        [UIValue("request-time-censor")]
        public int minimumUploadTimeCensor
        {
            get => QueueConfigManager.Instance.Config.minimumUploadTimeCensor;
            set => QueueConfigManager.Instance.Config.minimumUploadTimeCensor = value;
        }
            
        [UIValue("websocket-url")]
        public string WebsocketURL {
            get => QueueConfigManager.Instance.Config.WebsocketURL;
            set => QueueConfigManager.Instance.Config.WebsocketURL = value;
        }

        [UIValue("websocket-enable")]
        public bool WebsocketEnabled
        {
            get => QueueConfigManager.Instance.Config.WebsocketEnabled;
            set => QueueConfigManager.Instance.Config.WebsocketEnabled = value;
        }
        
        [UIAction("connect-click")]
        private void ConnectClick()
        {
            //ChatHandler.WebsocketHandlerConnect();
            //modal.HandleBlockerButtonClicked();
        }
        
        [UIValue("disable-chatcore")]
        public bool DisableChatcore
        {
            get => QueueConfigManager.Instance.Config.DisableChatcore;
            set => QueueConfigManager.Instance.Config.DisableChatcore = value;
        }
            
        [UIValue("mod-full-rights")]
        public bool ModFullRights
        {
            get => QueueConfigManager.Instance.Config.ModFullRights;
            set => QueueConfigManager.Instance.Config.ModFullRights = value;
        }
        
        [UIValue("requestui-enable")]
        public bool BeatsaverRequestUIEnabled
        {
            get => QueueConfigManager.Instance.Config.BeatsaverRequestUIEnabled;
            set => QueueConfigManager.Instance.Config.BeatsaverRequestUIEnabled = value;
        }
        
        [UIValue("requestui-id")]
        public string BeatsaverRequestUIId {
            get => QueueConfigManager.Instance.Config.BeatsaverRequestUIId;
            set => QueueConfigManager.Instance.Config.BeatsaverRequestUIId = value;
        }
        
        [UIValue("requestui-url")]
        public string BeatsaverRequestUIurl {
            get => QueueConfigManager.Instance.Config.BeatsaverRequestUIurl;
            set => QueueConfigManager.Instance.Config.BeatsaverRequestUIurl = value;
        }
        
        [UIAction("rqui-connect-click")]
        private void RequestUIConnectClick()
        {
            //ChatHandler.BeatsaberRequestUiHandlerConnect();
            //modal.HandleBlockerButtonClicked();
        }

    }
}
