using BeatSaberMarkupLanguage.Attributes;
using SongRequestManager.Config;

namespace SongRequestManager.UI
{
    public class SongRequestManagerSettings : PersistentSingleton<SongRequestManagerSettings>
    {
        [UIValue("autopick-first-song")]
        public bool AutopickFirstSong
        {
            get => QueueConfig.Instance.AutopickFirstSong;
            set => QueueConfig.Instance.AutopickFirstSong = value;
        }

        [UIValue("lowest-allowed-rating")]
        public float LowestAllowedRating
        {
            get => QueueConfig.Instance.LowestAllowedRating;
            set => QueueConfig.Instance.LowestAllowedRating = value;
        }

        [UIValue("maximum-song-length")]
        public int MaximumSongLength
        {
            get => (int) QueueConfig.Instance.MaximumSongLength;
            set => QueueConfig.Instance.MaximumSongLength = value;
        }

        [UIValue("minimum-njs")]
        public int MinimumNJS
        {
            get => (int) QueueConfig.Instance.MinimumNJS;
            set => QueueConfig.Instance.MinimumNJS = value;
        }

        [UIValue("automap")]
        public bool Automap
        {
            get => QueueConfig.Instance.Automap;
            set => QueueConfig.Instance.Automap = value;
        }

        [UIValue("tts-support")]
        public bool TtsSupport
        {
            get => QueueConfig.Instance.BotPrefix != "";
            set => QueueConfig.Instance.BotPrefix = value ? "! " : "";
        }

        [UIValue("user-request-limit")]
        public int UserRequestLimit
        {
            get => QueueConfig.Instance.UserRequestLimit;
            set => QueueConfig.Instance.UserRequestLimit = value;
        }

        [UIValue("sub-request-limit")]
        public int SubRequestLimit
        {
            get => QueueConfig.Instance.SubRequestLimit;
            set => QueueConfig.Instance.SubRequestLimit = value;
        }

        [UIValue("mod-request-limit")]
        public int ModRequestLimit
        {
            get => QueueConfig.Instance.ModRequestLimit;
            set => QueueConfig.Instance.ModRequestLimit = value;
        }

        [UIValue("vip-bonus-requests")]
        public int VipBonusRequests
        {
            get => QueueConfig.Instance.VipBonusRequests;
            set => QueueConfig.Instance.VipBonusRequests = value;
        }

        
        [UIValue("request-time-censor")]
        public int minimumUploadTimeCensor
        {
            get => QueueConfig.Instance.minimumUploadTimeCensor;
            set => QueueConfig.Instance.minimumUploadTimeCensor = value;
        }
            
        [UIValue("websocket-url")]
        public string WebsocketURL {
            get => QueueConfig.Instance.WebsocketURL;
            set => QueueConfig.Instance.WebsocketURL = value;
        }

        [UIValue("websocket-enable")]
        public bool WebsocketEnabled
        {
            get => QueueConfig.Instance.WebsocketEnabled;
            set => QueueConfig.Instance.WebsocketEnabled = value;
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
            get => QueueConfig.Instance.DisableChatcore;
            set => QueueConfig.Instance.DisableChatcore = value;
        }
            
        [UIValue("mod-full-rights")]
        public bool ModFullRights
        {
            get => QueueConfig.Instance.ModFullRights;
            set => QueueConfig.Instance.ModFullRights = value;
        }
        
        [UIValue("requestui-enable")]
        public bool BeatsaverRequestUIEnabled
        {
            get => QueueConfig.Instance.BeatsaverRequestUIEnabled;
            set => QueueConfig.Instance.BeatsaverRequestUIEnabled = value;
        }
        
        [UIValue("requestui-id")]
        public string BeatsaverRequestUIId {
            get => QueueConfig.Instance.BeatsaverRequestUIId;
            set => QueueConfig.Instance.BeatsaverRequestUIId = value;
        }
        
        [UIValue("requestui-url")]
        public string BeatsaverRequestUIurl {
            get => QueueConfig.Instance.BeatsaverRequestUIurl;
            set => QueueConfig.Instance.BeatsaverRequestUIurl = value;
        }
        
        [UIAction("rqui-connect-click")]
        private void RequestUIConnectClick()
        {
            //ChatHandler.BeatsaberRequestUiHandlerConnect();
            //modal.HandleBlockerButtonClicked();
        }

    }
}
