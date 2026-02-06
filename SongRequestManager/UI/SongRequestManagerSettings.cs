using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Util;
using SongRequestManager.Config;

namespace SongRequestManager.UI
{
    public class SongRequestManagerSettings : PersistentSingleton<SongRequestManagerSettings>
    {
        //[UIValue("autopick-first-song")]
        //public bool AutopickFirstSong
        //{
        //    get => RequestBotSettings.Current.Data.AutopickFirstSong;
        //    set => RequestBotSettings.Current.Data.AutopickFirstSong = value;
        //}

        [UIValue("lowest-allowed-rating")]
        public float LowestAllowedRating
        {
            get => RequestBotSettings.Current.Data.LowestAllowedRating;
            set => RequestBotSettings.Current.Data.LowestAllowedRating = value;
        }

        [UIValue("maximum-song-length")]
        public int MaximumSongLength
        {
            get => (int) RequestBotSettings.Current.Data.MaximumSongLength;
            set => RequestBotSettings.Current.Data.MaximumSongLength = value;
        }

        //[UIValue("minimum-njs")]
        //public int MinimumNJS
        //{
        //    get => (int) RequestBotSettings.Current.Data.MinimumNJS;
        //    set => RequestBotSettings.Current.Data.MinimumNJS = value;
        //}

        //[UIValue("automap")]
        //public bool Automap
        //{
        //    get => RequestBotSettings.Current.Data.Automap;
        //    set => RequestBotSettings.Current.Data.Automap = value;
        //}

        //[UIValue("tts-support")]
        //public bool TtsSupport
        //{
        //    get => RequestBotSettings.Current.Data.BotPrefix != "";
        //    set => RequestBotSettings.Current.Data.BotPrefix = value ? "! " : "";
        //}

        //[UIValue("user-request-limit")]
        //public int UserRequestLimit
        //{
        //    get => RequestBotSettings.Current.Data.UserRequestLimit;
        //    set => RequestBotSettings.Current.Data.UserRequestLimit = value;
        //}

        //[UIValue("sub-request-limit")]
        //public int SubRequestLimit
        //{
        //    get => RequestBotSettings.Current.Data.SubRequestLimit;
        //    set => RequestBotSettings.Current.Data.SubRequestLimit = value;
        //}

        //[UIValue("mod-request-limit")]
        //public int ModRequestLimit
        //{
        //    get => RequestBotSettings.Current.Data.ModRequestLimit;
        //    set => RequestBotSettings.Current.Data.ModRequestLimit = value;
        //}

        //[UIValue("vip-bonus-requests")]
        //public int VipBonusRequests
        //{
        //    get => RequestBotSettings.Current.Data.VipBonusRequests;
        //    set => RequestBotSettings.Current.Data.VipBonusRequests = value;
        //}

        
        //[UIValue("request-time-censor")]
        //public int minimumUploadTimeCensor
        //{
        //    get => RequestBotSettings.Current.Data.minimumUploadTimeCensor;
        //    set => RequestBotSettings.Current.Data.minimumUploadTimeCensor = value;
        //}
            
        //[UIValue("websocket-url")]
        //public string WebsocketURL {
        //    get => RequestBotSettings.Current.Data.WebsocketURL;
        //    set => RequestBotSettings.Current.Data.WebsocketURL = value;
        //}

        //[UIValue("websocket-enable")]
        //public bool WebsocketEnabled
        //{
        //    get => RequestBotSettings.Current.Data.WebsocketEnabled;
        //    set => RequestBotSettings.Current.Data.WebsocketEnabled = value;
        //}
        
        [UIAction("connect-click")]
        private void ConnectClick()
        {
            //ChatHandler.WebsocketHandlerConnect();
            //modal.HandleBlockerButtonClicked();
        }
        
        //[UIValue("disable-chatcore")]
        //public bool DisableChatcore
        //{
        //    get => RequestBotSettings.Current.Data.DisableChatcore;
        //    set => RequestBotSettings.Current.Data.DisableChatcore = value;
        //}
            
        //[UIValue("mod-full-rights")]
        //public bool ModFullRights
        //{
        //    get => RequestBotSettings.Current.Data.ModFullRights;
        //    set => RequestBotSettings.Current.Data.ModFullRights = value;
        //}
        
        //[UIValue("requestui-enable")]
        //public bool BeatsaverRequestUIEnabled
        //{
        //    get => RequestBotSettings.Current.Data.BeatsaverRequestUIEnabled;
        //    set => RequestBotSettings.Current.Data.BeatsaverRequestUIEnabled = value;
        //}
        
        //[UIValue("requestui-id")]
        //public string BeatsaverRequestUIId {
        //    get => RequestBotSettings.Current.Data.BeatsaverRequestUIId;
        //    set => RequestBotSettings.Current.Data.BeatsaverRequestUIId = value;
        //}
        
        //[UIValue("requestui-url")]
        //public string BeatsaverRequestUIurl {
        //    get => RequestBotSettings.Current.Data.BeatsaverRequestUIurl;
        //    set => RequestBotSettings.Current.Data.BeatsaverRequestUIurl = value;
        //}
        
        [UIAction("rqui-connect-click")]
        private void RequestUIConnectClick()
        {
            //ChatHandler.BeatsaberRequestUiHandlerConnect();
            //modal.HandleBlockerButtonClicked();
        }

    }
}
