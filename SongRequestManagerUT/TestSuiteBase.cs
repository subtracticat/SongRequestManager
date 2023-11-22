using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongRequestManager.Chat;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Service;
using SongRequestManagerUT.Mocks;

namespace SongRequestManagerUT
{
    public abstract class TestSuiteBase
    {
        protected IStateProvider<RequestBotSettingsData> MockBotSettings { get; set; }
        protected IStateProvider<SongModerationSettingsData> MockSongModerationSettings { get; set; }
        protected IStateProvider<TwitchConnectionSettingsData> MockTwitchSettings { get; set; }
        protected IStateProvider<QueueData> MockRequestQueue { get; set; }
        protected IStateProvider<PriorityTrackerData> MockPriorityTracker { get; set; }

        protected MockChatProvider MockChat { get; set; }

        [TestInitialize()]
        public virtual void BeforeEach()
        {
            this.MockBotSettings = new MockStateProvider<RequestBotSettingsData>();
            RequestBotSettings.Current.Initialize(this.MockBotSettings);

            this.MockSongModerationSettings = new MockStateProvider<SongModerationSettingsData>();
            SongModerationSettings.Current.Initialize(this.MockSongModerationSettings);

            this.MockTwitchSettings = new MockStateProvider<TwitchConnectionSettingsData>();
            TwitchConnectionSettings.Current.Initialize(this.MockTwitchSettings);

            this.MockRequestQueue = new MockStateProvider<QueueData>();
            RequestQueue.Current.Initialize(this.MockRequestQueue);

            this.MockPriorityTracker = new MockStateProvider<PriorityTrackerData>();
            PriorityTracker.Current.Initialize(this.MockPriorityTracker);

            this.MockChat = new MockChatProvider();
            ChatHandler.Initialize(this.MockChat);

            BeatSaverService.Instance.SetFetchOverride(MockBeatSaverUtils.MockFetchSongMetadataAsync);
        }

        [TestCleanup()]
        public void AfterEach()
        {
            BeatSaverService.Instance.SetFetchOverride(null);
        }
    }
}
